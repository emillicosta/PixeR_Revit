using Autodesk.Revit.DB;

namespace Form2
{
    public class DiffuseLight : MyMaterial
    {
        public Texture emit;

        public DiffuseLight(Texture emit_)
        {
            this.emit = emit_;
        }

        public override XYZ emitted(double u, double v, ref XYZ p)
        {
            return emit.value(u, v, p);
        }

        public override bool scatter(ref Ray r, ref HitRecord ht, ref XYZ attenuation, ref Ray scatterd)
        {
            return false;
        }
    }
}