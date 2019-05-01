using Autodesk.Revit.DB;
using System;

namespace Form2
{
    public class Lambertian : MyMaterial
    {

        public Lambertian(Texture albedo_)
        {
            albedo = albedo_;
            ks = new XYZ();
            ka = new XYZ();
            alpha = 0;
        }

        public override XYZ Emitted(double u, double v, ref XYZ p)
        {
            return new XYZ();
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

        public override bool Scatter(ref Ray r, ref HitRecord ht_, ref XYZ attenuation_, ref Ray scattered_ray)
        {
            XYZ p_ = Random_in_unit_sphere();
            XYZ target = ht_.p + ht_.normal + p_;
            scattered_ray = new Ray(ht_.p, ht_.p - ht_.normal + p_);//new Ray(ht_.p, ht_.p - target);
            attenuation_ = albedo.Value(0, 0, ht_.p);

            return true;
        }
    }
}