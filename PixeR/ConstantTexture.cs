using Autodesk.Revit.DB;

namespace Form2
{
    public class ConstantTexture : Texture
    {
        private readonly XYZ kd;

        public ConstantTexture(XYZ kd)
        {
            this.kd = kd;
        }

        public override XYZ Value(double u, double v, XYZ p)
        {
            return kd;
        }
    }
}