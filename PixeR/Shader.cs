using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Form2
{
    public abstract class Shader
    {
        protected static Scene world;
        public abstract XYZ color(ref Ray r, double t_min, double t_max, int depth_);

        public static bool hit_anything(ref Ray r_, double t_min_, double t_max_, ref HitRecord ht_)
        {
            HitRecord temp_ht = new HitRecord();
            bool hit_anything = false;
            double closest_so_far = t_max_;
            for (int i = 0; i < world.list_size; i++)
            {
                if (world.list[i].Hit(ref r_, t_min_, closest_so_far, ref temp_ht))
                {
                    hit_anything = true;
                    closest_so_far = temp_ht.t;
                    ht_ = temp_ht;
                }
            }
            return hit_anything;
        }

       public static XYZ vertical_interpolation(Ray r_, XYZ bottom_, XYZ top_)
        {
            // Make the ray a vector in the same direction.     
            XYZ ray = r_.GetDirection().Normalize();
            // Take only the vertical component, since the lerp has to interpolate colors verticaly     
            double ray_y = ray.Y; // this component might assume values ranging from -1 to 1     
                                // Normalize the ray's vertical component to the range [0;1]     
            double t = (1 + ray_y) * 0.5;
            // Use linear interpolation (lerp) between the colors that compose the background     
            XYZ result = bottom_ * (1 - t) + top_ * (t);
            return result;
        }
        
        /*
         * Calculates the horizontal interpolation between 2 colors
         * Only works with the aspect ratio [2:1]
         */
       public static XYZ horizontal_interpolation(Ray r_, XYZ left_, XYZ right_)
        {
            // Make the ray a vector in the same direction.     
            XYZ ray = r_.GetDirection().Normalize();
            // Take only the horizontal component     
            double ray_x = ray.X; // this component might assume values ranging from -2 to 2     
                                // Normalize the ray's horizontal component to the range [0;1]     
            double t = (2 + ray_x) * 0.25;
            // Use linear interpolation (lerp) between the colors that compose the background     
            XYZ result = left_ * (1 - t) + right_ * (t);
            return result;
        }
        
        public static XYZ interpolation_biline(Ray r_)
        {

            XYZ unit_ray = r_.GetDirection().Normalize();
            double unit_ray_y = unit_ray.Y;
            double unit_ray_x = unit_ray.X;
            double a = (0.5 * unit_ray_y) + 0.5;
            double t = Math.Max(0.0, a);
            double b = (0.25 * unit_ray_x) + 0.5;
            double u = Math.Max(0.0, b);
            XYZ result = (  world.bg.lower_left * (1 - t) * (1 - u) +
                            world.bg.top_left * t * (1 - u) +
                            world.bg.lower_right * (1 - t) * (u) +
                            world.bg.top_right * (t) * (u)
                        );
            return result;
        }
    }
}