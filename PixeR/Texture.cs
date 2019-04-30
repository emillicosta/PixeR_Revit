using Autodesk.Revit.DB;

namespace Form2
{
    public abstract class Texture
    {
        public abstract XYZ Value(double u, double v, XYZ p);
    }
}