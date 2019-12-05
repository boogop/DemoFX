using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class Bobs : Base.BaseGraphics, Base.iDemo
    {
        #region mad props

        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        int dlen, dwid;
        int[,] d;
        int blen, bwid;
        int[,] b;
        int centerx, centery;
        double[] ypath;
        double[] xpath;
        double count = 0;

        public struct bl
        {
            public double x, y;
        }

        private bl[] balls;

        const int NUMBER_OF_BOBS = 14;

        struct cols
        {
            public int r, g, b;
        }

        private cols[] colors;

        private const double Whorls = 16.0;
        private const double rad = 3.14159 / 180;
        double mult1;
        int l, m, n, o;

        #endregion

        public void init()
        {
            loadTexture(DemoFX.Properties.Resources.ball4, out dlen, out dwid, out d);
            loadTexture(DemoFX.Properties.Resources.check, out blen, out bwid, out b);

            mult1 = .06;
            centerx = theWidth >> 1;
            centery = theHeight >> 1;

            double rad;

            xpath = new double[512];
            ypath = new double[512];

            for (int i = 0; i < 512; i++)
            {
                rad = (i * 0.703125) * 0.0174532;
                xpath[i] = Math.Sin(rad) * centerx + centerx;
                xpath[i] = Math.Cos(rad * 2) * centerx / 2 + centerx / 2 + 120;
                ypath[i] = (centery) - (int)((Math.Cos(rad * 2) * Math.Sin(rad)) * 120) - 20;
            }


            balls = new bl[NUMBER_OF_BOBS];

            l = m = n = o = 0;
            for (int index = 0; index < NUMBER_OF_BOBS; index++)
            {
                balls[index].x = xpath[l & 511];
                balls[index].y = xpath[m & 511];
                l += 20;
                m += 20;
            }


            colors = new cols[256];

            for (int i = 0; i < 256; i++)
            {
                double red = 1 + Math.Cos(i * Math.PI / 128);
                double grn = 1 + Math.Cos((i - 85) * Math.PI / 128);
                double blu = 1 + Math.Cos((i + 85) * Math.PI / 128);
                colors[i].r = (int)(red * 127) % 256;
                colors[i].g = (int)(grn * 127) % 256;
                colors[i].b = (int)(blu * 127) % 256;
            }

        }


        public void doIt(string msg)
        {
            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();


            #region ripples

            for (int X = 0; X < theWidth; X++)
            {
                double xm = X * mult1;
                double yAdj = Math.Sin(xm + count) * 5;

                for (int Y = 0; Y < theHeight; Y++)
                {
                    double ym = Y * mult1 + count;
                    double xAdj = Math.Sin(ym) * 5;
                    double xval = Math.Cos(xm + ym) * 5;

                    int sx = (int)(X + xval - xAdj);
                    int sy = (int)(Y + xval - yAdj);

                    if (sx > theWidth || sy > theHeight) continue;

                    Color c = Color.FromArgb(X ^ Y);

                    updateIt(sx, sy, c);
                }
            }

            count += 0.3;

            #endregion


            #region bobs

            l = n;
            m = o;
            double scale = .66;
            for (int k = 0; k < NUMBER_OF_BOBS; k++)
            {
                balls[k].x = xpath[l & 511];
                balls[k].y = ypath[m & 511];
                drawBall(scale, balls[k].x, balls[k].y);
                scale += .01;
                l += 20;
                m += 20;
            }

            n += 1;
            o += 2;
            n &= 511;
            o &= 511;

            #endregion


            endIt();
            drawIt();

        }


        private void drawBall(double scale, double ix, double iy)
        {
            for (int i = 0; i < dlen; i++)
            {
                for (int j = 0; j < dwid; j++)
                {
                    int xx = d[i, j];
                    if (xx == 999) continue;

                    updateIt((int)((i + ix) * scale), (int)((j + iy) * scale), Color.FromArgb(xx));

                }
            }
        }

    }
}
