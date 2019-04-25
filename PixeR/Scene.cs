using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Form2
{
    public class Scene
    {
        public List<Objeto> list;
        public int list_size;
        public List<Luzes> lum;
        public int lum_size;
        public PlanoFundo bg;
        public XYZ ambientLight;

        public Scene(List<Objeto> list, List<Luzes> lum, PlanoFundo bg, XYZ ambientLight)
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