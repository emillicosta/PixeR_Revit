using Autodesk.Revit.DB;
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

        public override XYZ color(Ray r_, double t_min, double t_max, int depth_)
        {
            HitRecord ht = null;
            // If the ray hitted anything
            if (Shader.hit_anything(r_, t_min, t_max, ht))
            {
                Ray scattered_ray = r_;
                Ray scattered_ray2 = new Ray(new XYZ(0, 0, 0), new XYZ(0, 0, 0));
                double reflect_prob = 0.0;
                XYZ attenuation = new XYZ(1, 1, 1);
                XYZ emitted = ht.mat.emitted(0, 0, ht.p);
                if (depth_ > 0)
                {
                    if (ht.mat.scatter(r_, ht, attenuation, scattered_ray, reflect_prob, scattered_ray2))
                        return emitted + Multiplicacao(attenuation, ((reflect_prob * color(scattered_ray, t_min, t_max, depth_ - 1)) + ((1 - reflect_prob) * color(scattered_ray2, t_min, t_max, depth_ - 1))));
                    else
                        return emitted + Multiplicacao(attenuation, color(scattered_ray, t_min, t_max, depth_ - 1));
                }

                return emitted;
            }
            // Else, dye the pixel with the background color
            // return Shader.interpolation_biline(r_);
            //XYZ colorido = Shader.vertical_interpolation(r_, Shader.world.bg.lower_left, Shader.world.bg.top_left);
            XYZ ray = r_.GetDirection().Normalize();
            double ray_y = ray.Y;    
            double t = (1 + ray_y) * 0.5;   
            XYZ result = Shader.world.bg.lower_left * (1 - t) + Shader.world.bg.top_left * (t);
            return result;

        }

        public static XYZ  Multiplicacao(XYZ v1, XYZ v2)
            {
                return new XYZ(v1.X * v2.X,
                    v1.Y * v2.Y,
                    v1.Z * v2.Z);
            }
        }
}