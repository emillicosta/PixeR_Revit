using Autodesk.Revit.DB;

namespace Form2
{
    public abstract class Luzes
    {
        public XYZ direction;
        public static XYZ intensity;

        abstract public XYZ GetDirection(XYZ point);
        abstract public XYZ GetIntensity();
    }
}