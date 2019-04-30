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
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.DB.Lighting;

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

                List<Element> elementsDoc = GetAllModelElements(doc);

                List<List<Mesh>> allMesh = new List<List<Mesh>>();
                foreach (Element e in elementsDoc)
                {
                    allMesh.Add(GetMesh(e));
                }

                List<List<Material>> allMaterial = new List<List<Material>>();
                foreach (Element e in elementsDoc)
                {
                    allMaterial.Add(GetMaterial(e, doc));
                }

                List<LightType> lights = GetLightsData(doc);


                Form1.FormsImage wf = new Form1.FormsImage();
                wf.ShowDialog();
                Double altura = wf.GetZ() * 0.0328125;//nº 30 foi tentativa e erro

                List<XYZ> cam = AddView3D(uiapp, doc, altura);

                List<Element> elem_light = GetElementLight(doc);
                Form2.FormRender fr = new Form2.FormRender(commandData, elem_light, allMesh, allMaterial, cam, doc);
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

        private List<Mesh> GetMesh(Element element)
        {
            List<Mesh> meshes = new List<Mesh>();

            Options opt = new Options();
            GeometryElement geomElem = element.get_Geometry(opt);
            foreach (GeometryObject geomObj in geomElem)
            {
                Solid geomSolid = geomObj as Solid;
                if (null != geomSolid)
                {
                    foreach (Face geomFace in geomSolid.Faces)
                    {
                        Mesh mesh = geomFace.Triangulate();
                        meshes.Add(mesh);
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
                                Transform instTransform = geoInstance.Transform;
                                foreach (Face geomFace in s.Faces)
                                {
                                    Mesh mesh = geomFace.Triangulate();
                                    mesh = mesh.get_Transformed(instTransform);
                                    meshes.Add(mesh);
                                }
                            }
                        }
                    }
                }
            }            
            return meshes;
        }

        private List<Material> GetMaterial(Element element, Document doc)
        {
            List<Material> materiais = new List<Material>();
            Options opt = new Options();
            GeometryElement geomElem = element.get_Geometry(opt);
            foreach (GeometryObject geomObj in geomElem)
            {
                Solid geomSolid = geomObj as Solid;
                if (null != geomSolid)
                {
                    foreach (Face geomFace in geomSolid.Faces)
                    {
                        ElementId elementId = geomFace.MaterialElementId;
                        Material m = doc.GetElement(elementId) as Material;
                        materiais.Add(m);
                    }
                }
                else
                {
                    GeometryInstance geoInstance = geomObj as GeometryInstance;
                    if (null != geoInstance)
                    {
                        GeometryElement symbolGeo = geoInstance.SymbolGeometry;

                        foreach (GeometryObject geometryObject in symbolGeo)
                        {
                            Solid solid = geometryObject as Solid;
                            if (null != solid)
                            {
                                
                                foreach (Face geomFace in solid.Faces)
                                {
                                    ElementId elementId = geomFace.MaterialElementId;
                                    Material m = doc.GetElement(elementId) as Material;
                                    materiais.Add(m);
                                }
                            }
                        }
                    }
                }
            }

            return materiais;
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
