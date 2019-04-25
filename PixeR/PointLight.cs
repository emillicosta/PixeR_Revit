using Autodesk.Revit.DB;

namespace Form2
{
    internal class PointLight : Luzes
    {
        public XYZ origin;
        public PointLight(XYZ origin, XYZ intensity)
        {
            this.origin = origin;
            Luzes.intensity = intensity;
        }
        public override XYZ GetDirection(XYZ point)
        {
            return origin - point;
        }
        public override XYZ GetIntensity()
        {
            return Luzes.intensity;
        }
    }
}