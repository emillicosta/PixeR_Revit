using Autodesk.Revit.DB;
using System;

namespace Form2
{
    public class PerspectiveCamera
    {
        public XYZ lower_left_corner; // lower left corner of the view plane.
        public XYZ horizontal; // Horizontal dimension of the view plane.
        public XYZ vertical; // Vertical dimension of the view plane.
        public XYZ origin;

        public double lens_radius;
        public XYZ u, v, w;

        public PerspectiveCamera(XYZ e_, XYZ d_, XYZ vup_, double fov_, double aspect_, double aperture_, double focus_dist_)
        {
            origin = e_;
            w = (e_ - d_);
            w = w.Normalize();
            u = vup_.CrossProduct(w);
            u = u.Normalize();
            v = w.CrossProduct(u);

            lens_radius = aperture_ / 2;
            double theta = fov_ * (Math.PI/180); // Radian transformation
            double half_height = Math.Tan(theta / 2);
            double half_width = aspect_ * half_height;
            lower_left_corner = e_ - half_width * focus_dist_ * u - half_height * focus_dist_ * v - w * focus_dist_;
            horizontal = 2 * half_width * focus_dist_ * u;
            vertical = 2 * half_height * focus_dist_ * v;
        }

        public XYZ random_in_unit_sphere()
        {
            XYZ p;
            Random rnd = new Random();
            do {
                p = 2.0* new XYZ(rnd.NextDouble(), rnd.NextDouble(),0) - new XYZ(1,1,0);
            }while(p.DotProduct(p) >= 1.0);
            return p;
        }
        public Ray get_ray(double u_, double v_) 
        {
            XYZ rd = lens_radius * random_in_unit_sphere();
            XYZ offset = u * rd.X + v * rd.Y;
            XYZ end_point = lower_left_corner + u_ * horizontal + v_ * vertical;
            Ray r = new Ray(origin + offset, end_point - origin - offset );
            return r;
        }
    }
}