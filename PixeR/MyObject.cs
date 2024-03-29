﻿using Autodesk.Revit.DB;

namespace Form2
{
    
    public abstract class MyObject
    {
        public XYZ origin;
        public MyMaterial material;

        public abstract bool Hit(ref Ray ray, double t_min, double t_max, ref HitRecord ht);
    }
}