using Autodesk.Revit.DB;
using System;

namespace Form2
{
    internal class MetalMaterial : MyMaterial
    {
        private ConstantTexture kd;
        private double fuzz;

        public MetalMaterial(ConstantTexture kd, double fuzz)
        {
            this.kd = kd;
            this.fuzz = fuzz;
        }

        public override XYZ Emitted(double u, double v, ref XYZ p)
        {
            return new XYZ();
        }

        public override bool Scatter(ref Ray r, ref HitRecord ht, ref XYZ attenuation, ref Ray scatterd)
        {
            XYZ reflected = Reflect(r.GetDirection().Normalize(), ht.normal);
            scatterd = new Ray(ht.p, reflected + fuzz*Random_in_unit_sphere());
            attenuation = albedo.Value(0, 0, new XYZ());

            return scatterd.GetDirection().DotProduct(ht.normal) > 0;
        }

        protected XYZ Random_in_unit_sphere()
        {
            Random rnd = new Random();
            XYZ p;
            do
            {
                p = 2.0 * new XYZ(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble()) - new XYZ(1, 1, 1);
            } while (p.DotProduct(p) >= 1.0);
            return p;
        }

        protected XYZ Reflect(XYZ v, XYZ n)
        {
            return v - 2 * v.DotProduct(n) * n;
        }
    }
}