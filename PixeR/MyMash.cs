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

        public override bool Hit( Ray r_, double t_min_, double t_max_, HitRecord ht_ )
        {
    
            if(bbox.Hit(r_, t_min_, t_max_, ht_)){
                return node.Hit(node, r_, t_min_, t_max_, ht_);
            }
            return false;
        }
    }
}