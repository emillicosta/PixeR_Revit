using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace Form2
{
    internal class MyMash : Objeto
    {
        public List<Triangle> triangles;
        public Cube bbox;
        public KDNode node = new KDNode();

        public MyMash(MyMaterial mat, List<Triangle> triangles, Cube cube)
        {
            material = mat;
            this.triangles = triangles;
            this.bbox = cube;
            node = node.build(triangles, 0);
        }

        public override bool Hit(ref Ray r_, double t_min_, double t_max_, ref HitRecord ht_ )
        {
            if(bbox.Hit(ref r_, t_min_, t_max_, ref ht_)){
                return node.Hit(node, ref r_, t_min_, t_max_, ref ht_);
            }
            return false;
        }
    }
}