using System.Collections.Generic;
using System.Linq;

namespace Form2
{
    public class KDNode
    {
        public Cube bbox;
        KDNode left;
        KDNode right;
        List<Triangle> triangles;

        public KDNode() { }

        public KDNode Build(ref List<Triangle> tris, int depth)
        {
            KDNode node = new KDNode
            {
                triangles = tris,
                left = null,
                right = null,
                bbox = null
            };

            if (tris.Count == 0) {
                return node;
            }
            if (tris.Count == 1)
            {
                node.bbox =  tris[0].bbox;
                node.left = new KDNode();
                node.right = new KDNode();
                node.left.triangles = new List<Triangle>();
                node.right.triangles = new List<Triangle>();
                return node;
            }

            List<Triangle> left_tris = new List<Triangle>();
            List<Triangle> right_tris = new List<Triangle>();

            int axis = depth % 3;
            IEnumerable<Triangle> query = new List<Triangle>();
            switch (axis)
            {
                case 0:
                    query = tris.OrderBy(triangle => triangle.get_midpoint().X);
                    break;
                case 1:
                    query = tris.OrderBy(triangle => triangle.get_midpoint().Y);
                    break;
                case 2:
                    query = tris.OrderBy(triangle => triangle.get_midpoint().Z);
                    break;
            }
            node.bbox = query.First().bbox;
            foreach(Triangle t in query)
            {
                node.bbox = node.bbox.Wrap(ref node.bbox, ref t.bbox);
            }

            int contAux = query.Count() / 2;
            int aux = 0;
            foreach (Triangle t in query)
            {
                if(aux < contAux)
                    left_tris.Add(t);
                else
                    right_tris.Add(t);
                aux++;
            }


            node.left = Build(ref left_tris, depth + 1);
            node.right = Build(ref right_tris, depth + 1);

            return node;
        }

        public bool Hit(ref KDNode node, ref Ray r_, double t_min_, double t_max_, ref HitRecord ht_)
        {
            if (node.bbox.Hit(ref r_, t_min_, t_max_, ref ht_))
            {
                ht_ = new HitRecord();
                HitRecord left_ht = new HitRecord(), right_ht = new HitRecord();

                if (node.left.triangles.Count > 0 || node.right.triangles.Count > 0)
                {
                    bool hitleft = Hit(ref node.left, ref r_, t_min_, t_max_, ref left_ht);
                    bool hitright = Hit(ref node.right,ref r_, t_min_, t_max_, ref right_ht);
                    if (hitleft && hitright)
                    {
                        if (left_ht.t < right_ht.t)
                            ht_ = left_ht;
                        else
                            ht_ = right_ht;
                        return true;
                    }
                    else if (hitleft)
                    {
                        ht_ = left_ht;
                        return true;
                    }
                    else if (hitright)
                    {
                        ht_ = right_ht;
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    for (int i = 0; i < node.triangles.Count; ++i)
                    {
                        if (node.triangles[i].Hit(ref r_, t_min_, t_max_, ref ht_))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            else
                return false;
        }
    }
}