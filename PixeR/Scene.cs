using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Form2
{
    public class Scene
    {
        public List<MyObject> list;
        public int list_size;
        public List<MyLight> lum;
        public int lum_size;
        public BackGround bg;
        public XYZ ambientLight;

        public Scene(List<MyObject> list, List<MyLight> lum, BackGround bg, XYZ ambientLight)
        {
            this.list = list;
            this.list_size = list.Count;
            this.lum = lum;
            this.lum_size = lum.Count;
            this.bg = bg;
            this.ambientLight = ambientLight;
        }
    }
}