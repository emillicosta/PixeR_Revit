using Autodesk.Revit.DB;

namespace Form2
{
    public class Triangle : MyObject
    {
        public bool backface_cull;
        public XYZ v0;
        public XYZ v1;
        public XYZ v2;
        public Cube bbox;
        readonly double error = 0.00001;
        public Triangle(MyMaterial m_, XYZ v0_, XYZ v1_, XYZ v2_, bool backface_cull_ = false)
        {
            origin = new XYZ(0, 0, 0);
            material = m_;
            v0 = v0_;
            v1 = v1_;
            v2 = v2_;
            backface_cull = backface_cull_;
            bbox = new Cube(material, MinVector(v0, MinVector(v1, v2)), MaxVector(v0, MaxVector(v1, v2)));
        }

        public override bool Hit(ref Ray r_, double t_min_, double t_max_, ref HitRecord  ht_ )
        {
            XYZ e1, e2, h, s, q;
            double det, inv_det, u, v;
            e1 = (v1 - v0);
            e2 = (v2 - v0);
            h = r_.GetDirection().CrossProduct(e2);
            det = e1.DotProduct(h);

            if (backface_cull)
            {
                if (det < error)
                    return false;
                s = r_.GetOrigin() - v0;
                u = s.DotProduct(h);
                if (u < 0.0 || u > det)
                    return false;
                q = s.CrossProduct(e1);
                v = r_.GetDirection().DotProduct(q);
                if (v < 0.0 || u + v > det)
                    return false;
                inv_det = 1.0 / det;
                double t = inv_det * e2.DotProduct(q);
                t *= inv_det;
                if (t > error && t <= t_max_) // ray intersection
                {
                    ht_.t = t;
                    ht_.p = r_.PointAt(t);
                    ht_.normal = e1.CrossProduct(e2).Normalize();
                    ht_.mat = material;
                    return true;
                }
                else // This means that there is a line intersection but not a ray intersection.
                    return false;
            }
            else
            {
                if (det > -error && det < error)
                    return false;
                inv_det = 1.0 / det;
                s = r_.GetOrigin() - v0;
                u = inv_det * s.DotProduct(h);
                if (u < 0.0 || u > 1.0)
                    return false;
                q = s.CrossProduct(e1);
                v = inv_det * r_.GetDirection().DotProduct(q);
                if (v < 0.0 || u + v > 1.0)
                    return false;
                double t = inv_det * e2.DotProduct(q);
                if (t > error && t <= t_max_) // ray intersection
                {
                    ht_.p = r_.PointAt(t);
                    ht_.normal = e1.CrossProduct(e2).Normalize();
                    ht_.mat = material;
                    //Autodesk.Revit.UI.TaskDialog.Show("Valor de T do triangulo",t.ToString());
                    ht_.t = t;
                    return true;
                }
                else // This means that there is a line intersection but not a ray intersection.
                    return false;
            }

        }


        public XYZ MinVector(XYZ v, XYZ u)
        {
            double x, y, z;
            if (v.X < u.X)
                x = v.X;
            else
                x = u.X;

            if (v.Y < u.Y)
                y = v.Y;
            else
                y = u.Y;

            if (v.Z < u.Z)
                z = v.Z;
            else
                z = u.Z;

            XYZ aux = new XYZ(x, y, z);
            return aux;
        }

        public XYZ MaxVector(XYZ v, XYZ u)
        {
            double x, y, z;
            if (v.X > u.X)
                x = v.X;
            else
                x = u.X;

            if (v.Y > u.Y)
                y = v.Y;
            else
                y = u.Y;

            if (v.Z > u.Z)
                z = v.Z;
            else
                z = u.Z;

            XYZ aux = new XYZ(x, y, z);

            return aux;
        }

        public XYZ get_midpoint()
        {
            double xB = (v0.X + v1.X + v2.X) / 3;
            double yB = (v0.Y + v1.Y + v2.Y) / 3;
            double zB = (v0.Z + v1.Z + v2.Z) / 3;

            return new XYZ(xB, yB, zB);
        }
    //
}
}