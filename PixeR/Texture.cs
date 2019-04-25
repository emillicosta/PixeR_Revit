using Autodesk.Revit.DB;

namespace Form2
{
    public abstract class Texture
    {
        abstract public XYZ value(double u, double v, XYZ p);
    }
}