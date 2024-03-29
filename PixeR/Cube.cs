﻿using Autodesk.Revit.DB;
using System;

namespace Form2
{
    public class Cube : MyObject
    {
        public XYZ maxi, mini;
        public Cube(MyMaterial m_, XYZ min_, XYZ max_)
        {
            origin = new XYZ(0, 0, 0);
            material = m_;

            double x = max_.X, y = max_.Y, z=max_.Z;
            if (min_.X == max_.X)
                x += 0.1;
            if (min_.Y == max_.Y)
                y += 0.1;
            if (min_.Z == max_.Z)
                z += 0.1;

            mini = min_;
            maxi = new XYZ(x,y,z);
        }
        public Cube() { }
        public override bool Hit(ref Ray r_, double t_min, double t_max, ref HitRecord ht_)
        {
            double tmin = double.NegativeInfinity, tmax = double.PositiveInfinity;

            //testa pro x
            double t1 = (mini.X - r_.GetOrigin().X) / r_.GetDirection().X;
            double t2 = (maxi.X - r_.GetOrigin().X) / r_.GetDirection().X;
            tmin = Math.Max(tmin, Math.Min(t1, t2));
            tmax = Math.Min(tmax, Math.Max(t1, t2));

            //testa pro y
            t1 = (mini.Y - r_.GetOrigin().Y) / r_.GetDirection().Y;
            t2 = (maxi.Y - r_.GetOrigin().Y) / r_.GetDirection().Y;
            tmin = Math.Max(tmin, Math.Min(t1, t2));
            tmax = Math.Min(tmax, Math.Max(t1, t2));

            //testa pro Z
            t1 = (mini.Z - r_.GetOrigin().Z) / r_.GetDirection().Z;
            t2 = (maxi.Z - r_.GetOrigin().Z) / r_.GetDirection().Z;
            tmin = Math.Max(tmin, Math.Min(t1, t2));
            tmax = Math.Min(tmax, Math.Max(t1, t2));

            if (tmax > Math.Max(tmin, 0.0))
            {
                return true;                
            }

            return false;
        }

        public Cube Wrap(ref Cube node, ref Cube bbox_tri)
        {
            XYZ mini_ = MinVector(node.mini, bbox_tri.mini);
            XYZ maxi_1 = MaxVector(node.maxi, bbox_tri.maxi);
            Cube aux = new Cube(material, mini_, maxi_1);
            return aux;
        }

        public XYZ MinVector(XYZ v,  XYZ u)
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

        public XYZ MaxVector(XYZ v, XYZ u)
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