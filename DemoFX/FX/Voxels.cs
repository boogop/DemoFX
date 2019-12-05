using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    public class Voxels : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

       
        int xc = 180, yc = 225;
        int xh = 64, yh = 64;
        int xhc = 32, yhc = 32;
        int dar = 30;
        colr[] colors;
        int[,] land;
        double[] sine;
        double[] cosine;
        int[] rotate_c;
        int[] rotate_s;

        int[,] xtable;
        int[,] ytoptable;
        double amover = 2;
        int ybottom = 210;

        public void init()
        {
            land = new int[64, 64];
            sine = new double[64];
            cosine = new double[64];
            rotate_c = new int[512];
            rotate_s = new int[512];

            xtable = new int[73, 64];
            ytoptable = new int[73, 255];

            precalc();

            colors = new colr[256];
            voxelPalette(ref colors);
        }

        void precalc()
        {
            double deg2rad = 3.14159 / 32.0;
            double scale = 512;

            for (int i = 0; i < 63; i++)
            {
                sine[i] = (int)(Math.Sin(3.14 * i / 2 * deg2rad) * 40) + 40;
                cosine[i] = (int)(Math.Cos(3.14 * i / 2 * deg2rad) * 40) + 40;
            }

            for (int theta = 0; theta < 512; theta++)
            {
                rotate_s[theta] = (int)(Math.Sin(theta * deg2rad) * scale);
                rotate_c[theta] = (int)(Math.Cos(theta * deg2rad) * scale);
            }

            for (int j = 0; j < yh - 1; j++)
            {
                for (int i = 0; i < xh - 1; i++)
                {
                    land[i, j] = (int)(sine[j] + cosine[i] + 1);
                }
            }

            int count1 = 0;
            for (int z = 1; z < 73; z++)
            {
                int count = 0;
                for (int x3d = -32; x3d < 31; x3d++)
                {
                    xtable[count1, count] = (int)(xc * x3d / z + xc);
                    count++;
                }
                count1++;
            }

            count1 = 0;
            for (int z = 1; z < 73; z++)
            {
                for (int y3d = 0; y3d < 255; y3d++)
                {
                    ytoptable[count1, y3d] = (int)(yc - dar * y3d / z);
                }
                count1++;
            }
        }

        public void doIt(string msg)
        {
            int y3d, ytop;

            amover += 1;
            int m = (int)amover;
            m &= 511;

            double Ca = rotate_c[m];
            double Sa = rotate_s[m];

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);

            theFB = startIt();

            // move division out of the loops
            double sa2 = Sa / 256;
            double ca2 = Ca / 256;
            double sa1 = Sa / 1024;
            double ca1 = Ca / 1024;

            for (int z3d = 63; z3d > 0; z3d--)
            {
                double z = z3d + 10;
                double yo = z3d - 31;
                double yrot2 = (yo * sa2);
                double yrot1 = (yo * ca2);

                for (int j = 0; j < 64; j++)
                {
                    int xr = (int)(j * ca1 + yrot2 + xhc);
                    int yr = (int)(yrot1 + yhc - j * sa1);

                    if (xr < 0 || xr > xh - 1 || yr < 0 || yr > yh - 1)
                    {
                        break;
                    }

                    y3d = land[xr, yr];

                    int b = y3d;// %255;
                    ytop = ytoptable[(int)z, y3d];
                    int x = xtable[(int)z, j];

                    Color c = Color.FromArgb(colors[b].r, colors[b].g, colors[b].b);

                    for (int t = 0; t < 10; t++)
                    {
                        drawLine(x + t, ytop, x + t, ybottom, c);
                        drawLine(x - t, ytop, x - t, ybottom, c);
                    }

                }
            }


            endIt();
            drawIt();  

        }      


        void drawLine(int x0, int y0, int x1, int y1, Color pixel)
        {
            int i;
            double x = x1 - x0;
            double y = y1 - y0;
            double length = Math.Sqrt(x * x + y * y);
            double addx = x / length;
            double addy = y / length;
            x = x0;
            y = y0;

            for (i = 0; i < length; i += 1)
            {
                updateIt((int)x, (int)y, pixel);
                x += addx;
                y += addy;
            }
        }



    }
}
