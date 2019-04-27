using Autodesk.Revit.DB;
using System;

namespace Form2
{
    public class Cube : Objeto
    {
        public XYZ maxi, mini;
        public Cube(MyMaterial m_, XYZ min_, XYZ max_)
        {
            origin = new XYZ(0, 0, 0);
            material = m_;
            mini = min_;
            maxi = max_;
        }
        public override bool Hit(Ray r_, double t_min, double t_max, HitRecord ht_)
        {
            double tymin = (mini.Y - r_.GetOrigin().Y) / r_.GetDirection().Y;
            double tymax = (maxi.Y - r_.GetOrigin().Y) / r_.GetDirection().Y;
            double tmin = double.NegativeInfinity, tmax = double.PositiveInfinity;
            for (int i = 0; i < 3; ++i)
            {
                double t1 = (mini[i] - r_.GetOrigin()[i]) / r_.GetDirection()[i];
                double t2 = (maxi[i] - r_.GetOrigin()[i]) / r_.GetDirection()[i];

                tmin = Math.Max(tmin, Math.Min(t1, t2));
                tmax = Math.Min(tmax, Math.Max(t1, t2));
            }
            if (tmax > Math.Max(tmin, 0.0))
            {
                double t = tmin;
                ht_.t = tmin;
                ht_.p = r_.PointAt(t);
                XYZ c = (mini + maxi) * 0.5;
                XYZ p = ht_.p - c;
                XYZ d = (mini - maxi) * 0.5;
                double bias = 1.000001;
                XYZ normal = new XYZ(
                        p.X / Math.Abs(d.X) * bias,
                        p.Y / Math.Abs(d.Y) * bias,
                        p.Z / Math.Abs(d.Z) * bias
                       );
                ht_.normal = normal.Normalize();
                ht_.mat = material;
                return true;
            }

            return false;
        }

        public Cube wrap(Cube node, Cube bbox_tri)
        {
            XYZ mini_ = min_vector(node.mini, bbox_tri.mini);
            XYZ maxi_1 = max_vector(node.maxi, bbox_tri.maxi);
            Cube aux = new Cube(material, mini_, maxi_1);
            return aux;
        }

        public XYZ min_vector(XYZ v,  XYZ u)
        {
            XYZ aux = new XYZ(0, 0, 0);
            if (v.X < u.X)
            {
                aux = new XYZ(v.X, aux.Y, aux.Z);
            }else{
                aux = new XYZ(u.X, aux.Y, aux.Z);
            }
            if (v.Y < u.Y)
            {
                aux = new XYZ(aux.X, v.Y, aux.Z);
            }
            else{
                aux = new XYZ(aux.X, u.Y, aux.Z);
            }
            if (v.Z < u.Z)
            {
                aux = new XYZ(aux.X, aux.Y, v.Z);
            }
            else{
                aux = new XYZ(aux.X, aux.Y, u.Z);
            }
            return aux ;
        }

        public XYZ max_vector(XYZ v, XYZ u)
        {
            XYZ aux = new XYZ(0, 0, 0);
            if (v.X > u.X)
            {
                aux = new XYZ(v.X, aux.Y, aux.Z);
            }
            else
            {
                aux = new XYZ(u.X, aux.Y, aux.Z);
            }
            if (v.Y > u.Y)
            {
                aux = new XYZ(aux.X, v.Y, aux.Z);
            }
            else
            {
                aux = new XYZ(aux.X, u.Y, aux.Z);
            }
            if (v.Z > u.Z)
            {
                aux = new XYZ(aux.X, aux.Y, v.Z);
            }
            else
            {
                aux = new XYZ(aux.X, aux.Y, u.Z);
            }
            return aux;
        }
    }
}