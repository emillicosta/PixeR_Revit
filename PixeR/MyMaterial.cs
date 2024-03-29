﻿using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Form2
{
    public abstract class MyMaterial
    {
        public Texture albedo;
        public XYZ ks;
        public XYZ ka = new XYZ(0.1,0.1,0.1);
        public XYZ km;
        public List<XYZ> gradient;
        public double alpha;
        public List<double> angles;
        public double m;
        public double ref_idx;

        abstract public bool Scatter(ref Ray r, ref HitRecord ht, ref XYZ attenuation, ref Ray scatterd );
            
        abstract public XYZ Emitted(double u, double v,  ref XYZ p);
    }
}