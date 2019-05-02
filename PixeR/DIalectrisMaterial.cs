using Autodesk.Revit.DB;
using System;

namespace Form2
{
    internal class DIalectrisMaterial : MyMaterial
    {
        private double ref_idx;

        public DIalectrisMaterial(ConstantTexture constantTexture, double ri)
        {
            albedo = constantTexture;
            this.ref_idx = ri;
        }

        public override XYZ Emitted(double u, double v, ref XYZ p)
        {
            return new XYZ();
        }

        public override bool Scatter(ref Ray r_, ref HitRecord ht_, ref XYZ attenuation_, ref Ray scattered_ray)
        {
            XYZ outward_normal, refracted = new XYZ();
            double ni_over_nt, reflect_prob, cosine;

            XYZ reflected = Reflect(r_.GetDirection(), ht_.normal);
            attenuation_ = albedo.Value(0, 0, new XYZ(0, 0, 0));

            double d = r_.GetDirection().DotProduct(ht_.normal);

            if (r_.GetDirection().DotProduct(ht_.normal) > 0)
            {
                outward_normal = -ht_.normal;
                ni_over_nt = ref_idx;
                // cosine = (d / r_.get_direction().length());
                // cosine = std::sqrt(1 - ((ref_idx *ref_idx) * (1 - (cosine * cosine))));
                cosine = ref_idx * r_.GetDirection().DotProduct(ht_.normal) / r_.GetDirection().GetLength();
            }
            else
            {
                outward_normal = ht_.normal;
                ni_over_nt = 1.0 / ref_idx;
                cosine = -r_.GetDirection().DotProduct(ht_.normal) / r_.GetDirection().GetLength();
            }

            if (Refract(r_.GetDirection(), outward_normal, ni_over_nt, ref refracted))
            {
                reflect_prob = Schlick(cosine, ni_over_nt);
            }
            else
            {
                reflect_prob = 1.0;
            }

            Random rnd = new Random();
            if (rnd.NextDouble() < reflect_prob)
                scattered_ray = new Ray(ht_.p, reflected);
            else
                scattered_ray = new Ray(ht_.p, refracted);

            return true;
        }

        protected XYZ Random_in_unit_sphere()
        {
            Random rnd = new Random();
            XYZ p;
            do
            {
                p = 2.0 * new XYZ(rnd.NextDouble(), rnd.NextDouble(), rnd.NextDouble()) - new XYZ(1, 1, 1);
            } while (p.DotProduct(p) >= 1.0);
            return p;
        }

        protected XYZ Reflect(XYZ v, XYZ n)
        {
            double cosI = -n.DotProduct(v);
            return v + 2 * cosI * n;
        }

        protected bool Refract(XYZ v, XYZ n, double ni_over_nt, ref XYZ refracted)
        {
            XYZ uv = v.Normalize();
            double dt = uv.DotProduct(n);
            double discriminant = 1.0 - (ni_over_nt * ni_over_nt * (1 - dt*dt));
            if (discriminant > 0.0)
            {
                refracted = ni_over_nt * (uv - n * dt) - n * Math.Sqrt(discriminant);
                return true;
            }
            else
                return false;
        }

        protected double Schlick(double cosine, double ref_idx)
        {
            double r = (1.0 - ref_idx) / (1.0 + ref_idx);
            r *= r;
            return (r + (1.0 - r) * Math.Pow((1.0 - cosine), 5.0));
        }

    }
}