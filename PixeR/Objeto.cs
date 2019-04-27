using Autodesk.Revit.DB;

namespace Form2
{
    
    public abstract class Objeto
    {
        public XYZ origin;
        public MyMaterial material;

        public abstract bool Hit(Ray ray, double t_min, double t_max, HitRecord ht);
    }
}