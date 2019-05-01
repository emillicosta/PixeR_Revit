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
        protected XYZ Random_in_unit_sphere()
        {
            Random rnd = new Random();
            XYZ p;
            do
            {
                p = 2.0 * new XYZ(rnd.NextDouble() , rnd.NextDouble(), rnd.NextDouble()) - new XYZ(1, 1, 1);
            } while (p.DotProduct(p) >= 1.0);
            return p;
        }

        public override XYZ Color(ref Ray r_, double t_min, double t_max, int depth_)
        {
            HitRecord ht = new HitRecord();

            // If the ray hitted anything
            if (Shader.HitAnything( r_, t_min, t_max, ref ht))
            {
                Ray scattered_ray = r_;
                XYZ attenuation = new XYZ(1, 1, 1);
                XYZ emitted = ht.mat.Emitted(0, 0, ref ht.p);

                if (depth_ > 0 && ht.mat.Scatter(ref r_, ref ht, ref attenuation, ref scattered_ray) )
                        return emitted + Multiplication(attenuation, 
                             Color(ref scattered_ray, t_min, t_max, depth_ - 1) );
                
                return emitted;


            }
            // Else, dye the pixel with the background color
            return Shader.VerticalInterpolation(r_, Shader.world.bg.lower_left, Shader.world.bg.top_left);

        }

        public static XYZ Multiplication(XYZ v1, XYZ v2)
            {
                return new XYZ(v1.X * v2.X,
                    v1.Y * v2.Y,
                    v1.Z * v2.Z);
            }
        }
}