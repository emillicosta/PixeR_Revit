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

        public KDNode build(List<Triangle> tris, int depth)
        {
            KDNode node = new KDNode();
            node.triangles = tris;
            node.left = null;
            node.right = null;
            Cube cube_;
            node.bbox = null;

            if (tris.Count == 0)
                return node;
            if (tris.Count == 1)
            {
                node.bbox = tris[0].bbox;
                node.left = new KDNode();
                node.right = new KDNode();
                node.left.triangles = new List<Triangle>();
                node.right.triangles = new List<Triangle>();
                return node;
            }

            List<Triangle> left_tris = new List<Triangle>();
            List<Triangle> right_tris = new List<Triangle>();

            int axis = depth % 3;
            switch (axis)
            {
                case 0:
                    ordenarX(tris.First(), tris.Last());
                    break;
                case 1:
                    ordenarY(tris.First(), tris.Last());
                    break;
                case 2:
                    ordenarZ(tris.First(), tris.Last());
                    break;
            }
            node.bbox = tris[0].bbox;
            for (int i = 1; i < tris.Count; ++i)
            {
                node.bbox = node.bbox.wrap(node.bbox, tris[i].bbox);
            }

            int contAux = tris.Count / 2;
            for (int i = 0; i < contAux; ++i)
            {
                left_tris.Add(tris[i]);
            }
            for (int i = contAux; i < tris.Count; ++i)
            {
                right_tris.Add(tris[i]);
            }

            node.left = build(left_tris, depth + 1);
            node.right = build(right_tris, depth + 1);

            return node;
        }

        public bool ordenarX(Triangle i, Triangle j)
        {
            return i.get_midpoint().X < j.get_midpoint().Y;
        }
        bool ordenarY(Triangle i, Triangle j)
        {
            return i.get_midpoint().Y < j.get_midpoint().Y;
        }
        bool ordenarZ(Triangle i, Triangle j)
        {
            return i.get_midpoint().Z < j.get_midpoint().Z;
        }

        public bool Hit(KDNode node, Ray r_, double t_min_, double t_max_, HitRecord ht_)
        {
            if (node.bbox.Hit(r_, t_min_, t_max_, ht_))
            {
                HitRecord left_ht = null, right_ht = null;
                if (node.left.triangles.Count > 0 || node.right.triangles.Count > 0)
                {
                    bool hitleft = Hit(node.left, r_, t_min_, t_max_, left_ht);
                    bool hitright = Hit(node.right, r_, t_min_, t_max_, right_ht);
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
                        if (node.triangles[i].Hit(r_, t_min_, t_max_, ht_))
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