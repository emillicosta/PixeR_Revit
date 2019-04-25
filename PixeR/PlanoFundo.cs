using Autodesk.Revit.DB;

namespace Form2
{
    public class PlanoFundo
    {
        public XYZ lower_left;
        public XYZ lower_right;
        public XYZ top_left;
        public XYZ top_right;

        public PlanoFundo(XYZ lower_left, XYZ lower_right, XYZ top_left, XYZ top_right)
        {
            this.lower_left = lower_left;
            this.lower_right = lower_right;
            this.top_left = top_left;
            this.top_right = top_right;
        }
    }
}