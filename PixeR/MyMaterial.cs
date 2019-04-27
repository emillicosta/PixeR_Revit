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

        abstract public bool scatter(ref Ray r, ref HitRecord ht, ref XYZ attenuation, ref Ray scatterd, ref double reflect_prob , ref Ray scatterd2 );
            
        abstract public XYZ emitted(double u, double v,  ref XYZ p);
    }
}