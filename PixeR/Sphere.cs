using Autodesk.Revit.DB;
using System;

namespace Form2
{
    public class Sphere : Objeto
    {
        private double radius;

        public Sphere(MyMaterial mat, XYZ center, double radius)
        {
            material = mat;
            origin = center;
            this.radius = radius;
        }

        public override bool Hit(ref Ray r_, double t_min_, double t_max_, ref HitRecord ht_)
        {
            // First, manipulate the 1), 2) and 3) line to create the 4) equation   
            // 1) (point - center) * (point - center) - radius² = 0     
            // 2) point == ray  // 3) ray(t) = origin + direction*t     
            // 4) d*d*t² + 2(origin - center)*d*t + (origin - center)*(origin - center) - radius² = 0   

            // Second, calculate the a, b and c from the equation 4) (a*t² + b*t + c = 0)   
            XYZ oc = r_.GetOrigin() - origin; // (origin - center)    

            double a = r_.GetDirection().DotProduct(r_.GetDirection()); // d*d  
            double b = 2 * oc.DotProduct(r_.GetDirection());             // 2(origin-center)*d
            double c = oc.DotProduct(oc) - (radius * radius);               // (origin-center)*(origin-center) - radius²
                                                                             // Third, calculate the delta (b² - 4ac)    
            double delta = b * b - 4 * a * c;
            // Last, return the t component if the ray hit the sphere 
            if (delta >= 0)
            {
                double t = (-b - Math.Sqrt(delta)) / (2 * a);
                double x = r_.GetOrigin().X + t * r_.GetDirection().X;
                double y = r_.GetOrigin().Y + t * r_.GetDirection().Y;
                if (t < t_max_ && t > t_min_)
                {
                    ht_.t = t;
                    ht_.p = r_.PointAt(t);
                    ht_.normal = (ht_.p - origin) / radius;
                    ht_.mat = material;
                    //std::cout << *ht_.mat->gradient[0];
                    return true;
                }
            }
            return false;
        }
    }
}