using Autodesk.Revit.DB;

namespace Form2
{
    public class Triangle : Objeto
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
            bbox = new Cube(m_, min_vector(v0, min_vector(v1, v2)), max_vector(v0, max_vector(v1, v2)));
        }

        public override bool Hit( Ray r_, double t_min_, double t_max_, HitRecord  ht_ )
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
                u *= inv_det;
                v *= inv_det;
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
                v = inv_det * r_.GetDirection().DotProduct( q);
                if (v < 0.0 || u + v > 1.0)
                    return false;
                double t = inv_det * e2.DotProduct(q);
                if (t > error && t <= t_max_) // ray intersection
                {
                    ht_.t = t;
                    ht_.p = r_.PointAt(t);
                    ht_.normal = e1.CrossProduct( e2).Normalize();
                    ht_.mat = material;
                    return true;
                }
                else // This means that there is a line intersection but not a ray intersection.
                    return false;
            }


            return true;
        }


        public XYZ min_vector(XYZ v, XYZ u)
        {
            XYZ aux = new XYZ(0, 0, 0);
            if (v.X < u.X)
            {
                aux = new XYZ(v.X, aux.Y, aux.Z);
            }
            else
            {
                aux = new XYZ(u.X, aux.Y, aux.Z);
            }
            if (v.Y < u.Y)
            {
                aux = new XYZ(aux.X, v.Y, aux.Z);
            }
            else
            {
                aux = new XYZ(aux.X, u.Y, aux.Z);
            }
            if (v.Z < u.Z)
            {
                aux = new XYZ(aux.X, aux.Y, v.Z);
            }
            else
            {
                aux = new XYZ(aux.X, aux.Y, u.Z);
            }
            return aux;
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