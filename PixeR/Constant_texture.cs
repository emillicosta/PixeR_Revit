using Autodesk.Revit.DB;

namespace Form2
{
    public class Constant_texture : Texture
    {
        private XYZ kd;

        public Constant_texture(XYZ kd)
        {
            this.kd = kd;
        }

        public override XYZ value(double u, double v, XYZ p)
        {
            return kd;
        }
    }
}