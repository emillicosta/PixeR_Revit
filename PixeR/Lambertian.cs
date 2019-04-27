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

        public override XYZ emitted(double u, double v, XYZ p)
        {
            return new XYZ();
        }

        protected XYZ random_in_unit_sphere()
        {
            Random rnd = new Random();
            XYZ p;
            do
            {
                p = 2.0 * new XYZ(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble()) - new XYZ(1, 1, 1);
            } while (p.DotProduct(p) >= 1.0);
            return p;
        }

        public override bool scatter(Ray r, HitRecord ht_, XYZ attenuation_, Ray scattered_ray, double reflect_prob, Ray scatterd2)
        {
            XYZ p_ = random_in_unit_sphere();
            XYZ target = ht_.p + ht_.normal + p_;
            scattered_ray = new Ray(ht_.p, target - ht_.p);
            attenuation_ = albedo.value(0, 0, ht_.p);

            return true;
        }
    }
}