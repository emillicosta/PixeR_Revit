using Autodesk.Revit.DB;

namespace Form2
{
    internal class PointLight : MyLight
    {
        public XYZ origin;
        public PointLight(XYZ origin, XYZ intensity)
        {
            this.origin = origin;
            MyLight.intensity = intensity;
        }
        public override XYZ GetDirection(XYZ point)
        {
            return origin - point;
        }
        public override XYZ GetIntensity()
        {
            return MyLight.intensity;
        }
    }
}