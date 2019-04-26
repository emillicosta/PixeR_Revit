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

                IList<Element> elem = GetAllModelElements(doc);

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

                //APAGAR- exibir a malha e o material da malha todos
                if (allMaterial.Count == elem.Count)
                {
                    String mensagem = "";
                    for (int i = 0; i < 0; i++)
                    {
                        mensagem += elem[i].Name + " \n";
                        for (int j = 0; j < allMaterial[i].Count; j++)
                        {
                            for (int k = 0; k < allMesh[i][j].NumTriangles; k++)
                            {
                                MeshTriangle triangle = allMesh[i][j].get_Triangle(k);

                                XYZ vertex1 = triangle.get_Vertex(0);

                                XYZ vertex2 = triangle.get_Vertex(1);

                                XYZ vertex3 = triangle.get_Vertex(2);

                                mensagem += (j+1).ToString() + "- " + vertex1.ToString() + vertex2.ToString() + vertex3.ToString() + " " + allMaterial[i][j].Name + "\n";
                            }
                        }
                        //TaskDialog.Show("Elementos", mensagem);
                        mensagem = "";
                    }
                    
                }

                Form1.FormsImage wf = new Form1.FormsImage();
                wf.ShowDialog();
                Double altura = wf.GetZ() / 30;//nº 30 foi tentativa e erro

                List<XYZ> cam = AddView3D(uiapp, doc, altura);

                List<Element> elem_light = GetElementLight(doc);
                Form2.FormRender fr = new Form2.FormRender(commandData, elem_light, allFaces, cam);
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

                        //view3D.DisplayStyle = DisplayStyle.Realistic;
                    }

                    transaction.Commit();
                    uiapp.ActiveUIDocument.RequestViewChange(view3D);

                }
            

            }
            return cam;
        }

        private IList<Element> GetAllModelElements(Document doc)
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


    }
    //fim da classe main
}
