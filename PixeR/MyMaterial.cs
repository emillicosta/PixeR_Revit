using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Form2
{
    public abstract class MyMaterial
    {
        public Texture albedo;
        public XYZ ks;
        public XYZ ka;
        public XYZ km;
        public List<XYZ> gradient;
        public double alpha;
        public List<double> angles;
        public double m;
        public double ref_idx;

        abstract public bool scatter(Ray r, HitRecord ht, XYZ attenuation, Ray scatterd, double reflect_prob , Ray scatterd2 );
            
        abstract public XYZ emitted(double u, double v,  XYZ p);
    }
}