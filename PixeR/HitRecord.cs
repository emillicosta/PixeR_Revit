using Autodesk.Revit.DB;

namespace Form2
{
    public class HitRecord
    {
        public double t;
        public XYZ p;
        public XYZ normal;
        public MyMaterial mat;
    }
}