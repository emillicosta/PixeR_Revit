using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Drawing;

namespace Form2
{
    public class Raytrace
    {
        public Bitmap Render(PerspectiveCamera cam, Scene world, Shader shader, int n_cols, int n_rows, int n_samples, int ray_depth, double t_min, double t_max)
        {
            Bitmap image = new Bitmap(n_cols, n_rows);
            Random rnd = new Random();
            for (int row = n_rows - 1; row >= 0; --row) // Y
            {
                for (int col = 0; col < n_cols; col++) // X
                {
                    XYZ hue = new XYZ(0,0,0);
                    //antialiasing
                    for (int s = 0; s < n_samples; ++s)
                    {
                        // Determine how much we have 'walked' on the image: in [0,1]
                        double u = (col + rnd.NextDouble()) / n_cols; // walked u% of the horizontal dimension of the view plane.
                        double v = (row + rnd.NextDouble()) / n_rows; // walked v% of the vertical dimension of the view plane.

                        Ray r = cam.get_ray(u, v);
                        hue += shader.color(r, t_min, t_max, ray_depth);
                    }

                    hue /= Convert.ToDouble(n_samples);
                   
                    hue = new XYZ(Math.Sqrt(hue.X), Math.Sqrt(hue.Y), Math.Sqrt(hue.Z));
                    int ir = Convert.ToInt32(255 * hue.X);
                    int ig = Convert.ToInt32(255 * hue.Y);
                    int ib = Convert.ToInt32(255 * hue.Z);

                    

                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(ir, ig, ib);

                    image.SetPixel(col, row, newColor);
                }
            }
            return image;
        }
    }
}