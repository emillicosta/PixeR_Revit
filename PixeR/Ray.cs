using Autodesk.Revit.DB;

namespace Form2
{
    public class Ray
    {
        private XYZ o;
        private XYZ d;

        public Ray(XYZ o, XYZ d)
        {
            this.o = o;
            this.d = d;
        }
        public Ray() { }
        public XYZ GetDirection()
        {
            return d;
        }
        public XYZ GetOrigin( )
        {
            return o;
        }
        public XYZ PointAt(double t_) 
        {
            return o + t_* d; // parametric equation of the ray.
        }
    }
}