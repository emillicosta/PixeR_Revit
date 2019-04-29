using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.DB.Lighting;
using Autodesk.Revit.DB.DirectContext3D;
using Form2;

namespace PixeR
{
    public class Ribbon : IExternalApplication
    {
        public Result OnStartup (UIControlledApplication application)
        {
            // Add a new ribbon panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Renderizar");

            // Create a push button to trigger a command add it to the ribbon panel.
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButtonData buttonData = new PushButtonData("cmdPixeR", "PixeR", thisAssemblyPath, "PixeR.Main");

            PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;

            // Optionally, other properties may be assigned to the button
            // a) tool-tip
            pushButton.ToolTip = "Say hello to the entire world.";

            // b) large bitmap
            Uri uriImage = new Uri(@"C:\Tcc\PixeR_Revit\PixeR\chaleira.png");
            BitmapImage largeImage = new BitmapImage(uriImage);
            pushButton.LargeImage = largeImage;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // nothing to clean up in this simple case
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]

    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                Document doc = uiapp.ActiveUIDocument.Document;

                /*using (Transaction transaction = new Transaction(doc))
                {
                    Selection sel = uiapp.ActiveUIDocument.Selection;

                    XYZ eye = sel.PickPoint("Por favor, escolha a posição da câmera");
                    TaskDialog.Show("Ponto selecionado", eye.ToString());

                }*/

                List<Element> elem = GetAllModelElements(doc);

                List<List<Face>> allFaces = new List<List<Face>>();
                foreach (Element e in elem)
                {
                    allFaces.Add(GetFaces(e));
                }

                List<List<Mesh>> allMesh = new List<List<Mesh>>();
                foreach (List<Face> f in allFaces)
                {
                    allMesh.Add(GetMesh(f));
                }

                List<List<Material>> allMaterial = new List<List<Material>>();
                foreach (List<Face> f in allFaces)
                {
                    allMaterial.Add(GetMaterialFace(f, doc));
                }

                List<LightType> lights = GetLightsData(doc);

                List<Objeto> objects = GetObjects( elem,  doc);


                Form1.FormsImage wf = new Form1.FormsImage();
                wf.ShowDialog();
                Double altura = wf.GetZ() * 0.0328125;//nº 30 foi tentativa e erro

                List<XYZ> cam = AddView3D(uiapp, doc, altura);

                List<Element> elem_light = GetElementLight(doc);
                Form2.FormRender fr = new Form2.FormRender(commandData, elem_light, objects, cam, doc);
                fr.ShowDialog();
                
                return Result.Succeeded;
                
            }
            catch (Exception e)
            {
                message = e.Message;
                return Result.Failed;
            }
        }

        //Fim Main

        private List<XYZ> AddView3D(UIApplication uiapp, Document doc, Double altura)
        {
            List<XYZ> cam = new List<XYZ>();
            using (Transaction transaction = new Transaction(doc))
            {
                if (transaction.Start("Create model curves") == TransactionStatus.Started)
                {
                    // Find a 3D view type
                    IEnumerable<ViewFamilyType> viewFamilyTypes = from elem in new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType))
                                                                  let type = elem as ViewFamilyType
                                                                  where type.ViewFamily == ViewFamily.ThreeDimensional
                                                                  select type;
                    // Create a new Perspective View3D

                    View3D view3D = View3D.CreatePerspective(doc, viewFamilyTypes.First().Id);

                    if (view3D != null)
                    {
                        Selection sel = uiapp.ActiveUIDocument.Selection;

                        XYZ eye = sel.PickPoint("Por favor, escolha a posição da câmera");
                        eye = new XYZ(eye.X, eye.Y, altura + eye.Z);
                        //TaskDialog.Show("eye", eye.ToString());
                        cam.Add(eye);

                        XYZ up = new XYZ(0, 0, 1);

                        XYZ lookAt = sel.PickPoint("Por favor, escolha a direção da câmera");
                        //TaskDialog.Show("lookat", lookAt.ToString());
                        cam.Add(lookAt);
                        XYZ forward = new XYZ(lookAt.X - eye.X, lookAt.Y - eye.Y, 0);

                        view3D.SetOrientation(new ViewOrientation3D(eye, up, forward));
                        

                        //turn off the far clip plane with standard parameter API
                        Parameter farClip = view3D.LookupParameter("Recorte afastado ativo");
                        farClip.Set(0);

                        view3D.DisplayStyle = DisplayStyle.Realistic;
                    }

                    transaction.Commit();
                    uiapp.ActiveUIDocument.RequestViewChange(view3D);

                }
            

            }
            return cam;
        }

        private List<Element> GetAllModelElements(Document doc)
        {
            List<Element> elements = new List<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();

            foreach (Element e in collector)
            {
                if (null != e.Category && e.Category.HasMaterialQuantities)
                {
                    elements.Add(e);
                }
            }
            return elements;
        }

        private List<Mesh> GetMesh(List<Face> faces)
        {
            List<Mesh> meshes = new List<Mesh>();
            foreach (Face geomFace in faces)
            {
                Mesh mesh = geomFace.Triangulate();
                meshes.Add(mesh);
            }
            
            return meshes;
        }

        private List<Face> GetFaces(Element element)
        {
            List<Face> faces = new List<Face>();
            Options opt = new Options();
            GeometryElement geomElem = element.get_Geometry(opt);
            foreach (GeometryObject geomObj in geomElem)
            {
                Solid geomSolid = geomObj as Solid;
                if (null != geomSolid)
                {
                    foreach (Face geomFace in geomSolid.Faces)
                    {
                        faces.Add(geomFace);
                    }
                }
                else
                {
                    GeometryInstance geoInstance = geomObj as GeometryInstance;
                    if (null != geoInstance)
                    {
                        GeometryElement symbolGeo = geoInstance.SymbolGeometry;

                        foreach (GeometryObject o2 in symbolGeo)
                        {
                            Solid s = o2 as Solid;
                            if (null != s)
                            {
                                foreach (Face geomFace in s.Faces)
                                {
                                    faces.Add(geomFace);
                                }
                            }
                        }
                    }
                }
            }

            //TaskDialog.Show("Revit", faces.Size.ToString());
            return faces;
        }
        
        private List<Material> GetMaterialFace(List<Face> faces, Document doc)
        {
            List<Material> materiais = new List<Material>();

            foreach (Face face in faces)
            {
                ElementId elementId = face.MaterialElementId;
                Material m = doc.GetElement(elementId) as Material;
                materiais.Add(m);
            }

            return materiais;
            //TaskDialog.Show("Revit", faceInfo);
        }

        public List<LightType> GetLightsData(Document document)
        {
            List<LightType> lightTypes = new List<LightType>();
            // This code demonstrates how to get light information from project and family document
            
            if (document.IsFamilyDocument)
            {
                // In family document, get LightType via LightFamily.GetLightType(int) method. 
                LightFamily lightFamily = LightFamily.GetLightFamily(document);
                for (int index = 0; index < lightFamily.GetNumberOfLightTypes(); index++)
                {
                    lightTypes.Add(lightFamily.GetLightType(index));
                }
            }
            else
            {
                // In family document, get LightType via GetLightTypeFromInstance or GetLightType method.
                // In order to get the light information, please get a light fixture instance first
                FilteredElementCollector collector = new FilteredElementCollector(document);
                collector.OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_LightingFixtures);

                IEnumerable<FamilyInstance> familyInstances = collector.Cast<FamilyInstance>();

                foreach (FamilyInstance lightFixture in familyInstances)
                {
                    lightTypes.Add(LightType.GetLightTypeFromInstance(document, lightFixture.Id));
                }
            }

            return lightTypes;
        }

        public List<Element> GetElementLight(Document doc)
        {
            List<Element> elem_light = new List<Element>();
            FilteredElementCollector collector = new FilteredElementCollector(doc).WhereElementIsNotElementType();
            collector.OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_LightingFixtures);
            foreach (Element e in collector)
            {
                elem_light.Add(e);
            }
            return elem_light;
        }

        public List<Objeto> GetObjects( List<Element> elements, Document doc)
        {
            List<Objeto> objetos = new List<Objeto>();
            //String mensagem = "";

            foreach (Element element in elements)
            {
                //mensagem = element.Name + "\n";
                Options opt = new Options();
                GeometryElement geomElem = element.get_Geometry(opt);
                foreach (GeometryObject geoObject in geomElem)
                {
                    //get walls and floor
                    Solid geomSolid = geoObject as Solid;
                    if (null != geomSolid)
                    {
                        foreach (Face geomFace in geomSolid.Faces)
                        {

                            List<Triangle> triangles = new List<Triangle>();
                            double xmin = double.PositiveInfinity, xmax = double.NegativeInfinity;
                            double ymin = double.PositiveInfinity, ymax = double.NegativeInfinity;
                            double zmin = double.PositiveInfinity, zmax = double.NegativeInfinity;

                            MyMaterial mat = new Lambertian(new Constant_texture(new XYZ()));

                            foreach (Face face in geomSolid.Faces)
                            {
                                //get material of face
                                ElementId elementId = face.MaterialElementId;
                                XYZ kd = new XYZ();
                                Material m = doc.GetElement(elementId) as Material;
                                if (m != null)
                                {
                                    double red = Convert.ToDouble(m.Color.Red) / 255;
                                    double green = Convert.ToDouble(m.Color.Green) / 255;
                                    double blue = Convert.ToDouble(m.Color.Blue) / 255;
                                    kd = new XYZ(red, green, blue);
                                }
                                mat = new Lambertian(new Constant_texture(kd));

                                //get point of object
                                Mesh mesh = face.Triangulate();
                                for (int k = 0; k < mesh.NumTriangles; k++)
                                {
                                    MeshTriangle triangle = mesh.get_Triangle(k);

                                    XYZ v1 = new XYZ(triangle.get_Vertex(0).X * -1, triangle.get_Vertex(0).Z, triangle.get_Vertex(0).Y * -1);
                                    xmin = Math.Min(v1.X, xmin);
                                    ymin = Math.Min(v1.Y, ymin);
                                    zmin = Math.Min(v1.Z, zmin);
                                    xmax = Math.Max(v1.X, xmax);
                                    ymax = Math.Max(v1.Y, ymax);
                                    zmax = Math.Max(v1.Z, zmax);

                                    XYZ v2 = new XYZ(triangle.get_Vertex(1).X * -1, triangle.get_Vertex(1).Z, triangle.get_Vertex(1).Y * -1);
                                    xmin = Math.Min(v2.X, xmin);
                                    ymin = Math.Min(v2.Y, ymin);
                                    zmin = Math.Min(v2.Z, zmin);
                                    xmax = Math.Max(v2.X, xmax);
                                    ymax = Math.Max(v2.Y, ymax);
                                    zmax = Math.Max(v2.Z, zmax);

                                    XYZ v3 = new XYZ(triangle.get_Vertex(2).X * -1, triangle.get_Vertex(2).Z, triangle.get_Vertex(2).Y * -1);
                                    xmin = Math.Min(v3.X, xmin);
                                    ymin = Math.Min(v3.Y, ymin);
                                    zmin = Math.Min(v3.Z, zmin);
                                    xmax = Math.Max(v3.X, xmax);
                                    ymax = Math.Max(v3.Y, ymax);
                                    zmax = Math.Max(v3.Z, zmax);

                                    triangles.Add(new Triangle(mat, v1, v2, v3));
                                    //mensagem +=  v1.ToString() + v2.ToString() + v3.ToString() + " COR:" + m.Color.Red.ToString() + ";"+ m.Color.Green.ToString()+";"+ m.Color.Blue.ToString()+"\n";
                                }
                                //mensagem += "\n\n";
                            }

                            XYZ mini = new XYZ(xmin, ymin, zmin);
                            XYZ maxi = new XYZ(xmax, ymax, zmax);

                            objetos.Add(new MyMash(null, triangles, new Cube(null, mini, maxi)));
                        }
                    }

                    //get the components
                    GeometryInstance instance = geoObject as GeometryInstance;
                    if (instance != null)
                    {
                        foreach (GeometryObject instObj in instance.SymbolGeometry)
                        {
                            Solid solid = instObj as Solid;
                            if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size)
                            {
                                continue;
                            }
                            Transform instTransform = instance.Transform;

                            List<Triangle> triangles = new List<Triangle>();
                            double xmin = double.PositiveInfinity, xmax = double.NegativeInfinity;
                            double ymin = double.PositiveInfinity, ymax = double.NegativeInfinity;
                            double zmin = double.PositiveInfinity, zmax = double.NegativeInfinity;

                            MyMaterial mat = new Lambertian(new Constant_texture(new XYZ()));

                            foreach (Face face in solid.Faces)
                            {
                                //get material of face
                                ElementId elementId = face.MaterialElementId;
                                XYZ kd = new XYZ();
                                Material m = doc.GetElement(elementId) as Material;
                                if (m != null)
                                {
                                    double red = Convert.ToDouble(m.Color.Red) / 255;
                                    double green = Convert.ToDouble(m.Color.Green) / 255;
                                    double blue = Convert.ToDouble(m.Color.Blue) / 255;
                                    kd = new XYZ(red, green, blue);
                                }
                                mat = new Lambertian(new Constant_texture(kd));

                                //get point of object
                                Mesh mesh = face.Triangulate();
                                for (int k = 0; k < mesh.NumTriangles; k++)
                                {
                                    MeshTriangle triangle = mesh.get_Triangle(k);

                                    XYZ transV1 = instTransform.OfPoint(triangle.get_Vertex(0));
                                    XYZ v1 = new XYZ(transV1.X * -1, transV1.Z, transV1.Y * -1);
                                    xmin = Math.Min(v1.X, xmin);
                                    ymin = Math.Min(v1.Y, ymin);
                                    zmin = Math.Min(v1.Z, zmin);
                                    xmax = Math.Max(v1.X, xmax);
                                    ymax = Math.Max(v1.Y, ymax);
                                    zmax = Math.Max(v1.Z, zmax);

                                    XYZ transV2 = instTransform.OfPoint(triangle.get_Vertex(1));
                                    XYZ v2 = new XYZ(transV2.X * -1, transV2.Z, transV2.Y * -1);
                                    xmin = Math.Min(v2.X, xmin);
                                    ymin = Math.Min(v2.Y, ymin);
                                    zmin = Math.Min(v2.Z, zmin);
                                    xmax = Math.Max(v2.X, xmax);
                                    ymax = Math.Max(v2.Y, ymax);
                                    zmax = Math.Max(v2.Z, zmax);

                                    XYZ transV3 = instTransform.OfPoint(triangle.get_Vertex(2));
                                    XYZ v3 = new XYZ(transV3.X * -1, transV3.Z, transV3.Y * -1);
                                    xmin = Math.Min(v3.X, xmin);
                                    ymin = Math.Min(v3.Y, ymin);
                                    zmin = Math.Min(v3.Z, zmin);
                                    xmax = Math.Max(v3.X, xmax);
                                    ymax = Math.Max(v3.Y, ymax);
                                    zmax = Math.Max(v3.Z, zmax);

                                    triangles.Add(new Triangle(mat, v1, v2, v3));
                                    //mensagem +=  v1.ToString() + v2.ToString() + v3.ToString() + " COR:" + m.Color.Red.ToString() + ";"+ m.Color.Green.ToString()+";"+ m.Color.Blue.ToString()+"\n";
                                }
                                //mensagem += "\n\n";
                            }

                            XYZ mini = new XYZ(xmin, ymin, zmin);
                            XYZ maxi = new XYZ(xmax, ymax, zmax);

                            objetos.Add(new MyMash(null, triangles, new Cube(null, mini, maxi)));

                            //TaskDialog.Show("Malha do objetos", mensagem);
                        }
                    }
                }
            }

            return objetos;
        }



    }
    //fim da classe main
}
