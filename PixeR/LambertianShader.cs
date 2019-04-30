using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Form2
{
    public class LambertianShader : Shader
    {
        public LambertianShader(Scene world)
        {
            Shader.world = world;
        }
        protected XYZ random_in_unit_sphere()
        {
            Random rnd = new Random();
            XYZ p;
            do
            {
                p = 2.0 * new XYZ(rnd.NextDouble() , rnd.NextDouble(), rnd.NextDouble()) - new XYZ(1, 1, 1);
            } while (p.DotProduct(p) >= 1.0);
            return p;
        }

        public override XYZ color(ref Ray r_, double t_min, double t_max, int depth_)
        {
            HitRecord ht = new HitRecord();
            HitRecord ht_s = new HitRecord();

            // If the ray hitted anything
            if (Shader.hit_anything( r_, t_min, t_max, ref ht))
            {
                Ray scattered_ray = r_;
                Ray scattered_ray2 = new Ray(new XYZ(0, 0, 0), new XYZ(0, 0, 0));
                double reflect_prob = 0.0;
                XYZ attenuation = new XYZ(1, 1, 1);
                XYZ emitted = ht.mat.emitted(0, 0, ref ht.p);

                XYZ p = r_.GetOrigin() + ht.t * r_.GetDirection();

                XYZ ambient = Multiplicacao(ht.mat.ka, world.ambientLight);

                for (int i = 0; i < world.lum_size; i++)
                {
                    Ray r_light = new Ray(p, world.lum[i].GetDirection(p));
                    if (!Shader.hit_anything(r_light, t_min, t_max, ref ht_s))
                    {
                        if (depth_ > 0)
                        {
                            if (ht.mat.scatter(ref r_, ref ht, ref attenuation, ref scattered_ray, ref reflect_prob, ref scattered_ray2))
                                return emitted + Multiplicacao(attenuation, ((reflect_prob * color(ref scattered_ray, t_min, t_max, depth_ - 1)) + ((1 - reflect_prob) * color(ref scattered_ray2, t_min, t_max, depth_ - 1))));
                            else
                                return emitted + Multiplicacao(attenuation, color(ref scattered_ray, t_min, t_max, depth_ - 1));
                        }
                        return emitted;
                    }
                }
                return ambient;



            }
            // Else, dye the pixel with the background color
            return Shader.vertical_interpolation(r_, Shader.world.bg.lower_left, Shader.world.bg.top_left);

        }

        public static XYZ  Multiplicacao(XYZ v1, XYZ v2)
            {
                return new XYZ(v1.X * v2.X,
                    v1.Y * v2.Y,
                    v1.Z * v2.Z);
            }
        }
}