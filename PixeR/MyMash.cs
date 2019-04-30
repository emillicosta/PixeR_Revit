using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace Form2
{
    internal class MyMash : MyObject
    {
        public List<Triangle> triangles;
        public KDNode node = new KDNode();

        public MyMash(List<Triangle> triangles)
        {
            this.triangles = triangles;
            node = node.Build(ref triangles, 0);
        }

        public override bool Hit(ref Ray r_, double t_min_, double t_max_, ref HitRecord ht_ )
        {
            return node.Hit(ref node, ref r_, t_min_, t_max_, ref ht_);
        }
    }
}