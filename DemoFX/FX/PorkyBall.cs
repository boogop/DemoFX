using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class PorkyBall : Base.BaseGraphics, Base.iDemo
    {

        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        double turn;
        colr[] colors;
        int midw, midh;

        public void init()
        {
            colors = new colr[256];
            standardPalette(ref colors);
            midw = theWidth >> 1;
            midh = theHeight >> 1;
        }

        public void doIt(string msg)
        {

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            doPorkySphere();
            // drawLine(115, 135, 220, 220, Color.White);


            endIt();
            drawIt();
        }

        private void drawLine(int x0, int y0, int x1, int y1, Color c)
        {
            // I was translating a porkyball routine I'd done years ago in freebasic (which has a line function) when I
            // realized I had no idea how to manually do it. Thanks stackoverflow!
            // https://stackoverflow.com/questions/5186939/algorithm-for-drawing-a-4-connected-line

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sgnX = x0 < x1 ? 1 : -1;
            int sgnY = y0 < y1 ? 1 : -1;
            int e = 0;
            for (int i = 0; i < dx + dy; i++)
            {
                updateIt(x0, y0, c);
                int e1 = e + dy;
                int e2 = e - dx;
                if (Math.Abs(e1) < Math.Abs(e2))
                {
                    x0 += sgnX;
                    e = e1;
                }
                else
                {
                    y0 += sgnY;
                    e = e2;
                }
            }
        }



        private void doPorkySphere()
        {
            // just draw a sphere and shoot some lines through it
            int rx = 30;
            double cr1, sr1, x, y, z, x1, z1, y1, x2, y2, z2, lr, lrx, current_radius;
            int sx, sy, cnt, isx, isy;
            double radians = 3.14 / 180;

            turn += 0.05;

            cr1 = Math.Cos(turn);
            sr1 = Math.Sin(turn);

            Color n = Color.FromArgb(255, 128, 255);
            cnt = 64;

            for (int latitude = -90; latitude < 90; latitude++)
            {
                lr = latitude * radians;
                current_radius = Math.Cos(lr) * rx;
                z = Math.Sin(lr) * rx;

                cnt++;
                int cc = cnt % 256;
                Color c = Color.FromArgb(colors[cc].r, colors[cc].g, colors[cc].b);

                for (int longitude = 0; longitude < 360; longitude++)
                {
                    lrx = longitude * radians;
                    x = Math.Sin(lrx) * current_radius;
                    y = Math.Cos(lrx) * current_radius;

                    x1 = cr1 * x - sr1 * z;
                    z1 = sr1 * x + cr1 * z;
                    x2 = cr1 * x1 + sr1 * y;
                    y1 = cr1 * y - sr1 * x1;
                    y2 = sr1 * z1 + cr1 * y1;
                    z2 = cr1 * z1 - sr1 * y1;
                    z2 -= 300;

                    double xz = x2 / z2;
                    double yz = y2 / z2;

                    sx = (int)((-500 * xz) + midw);
                    sy = (int)((-500 * yz) + midh);

                    // these determine how many spines you get
                    if (latitude % 50 == 0)
                    {
                        if (longitude % 50 == 0)
                        {
                            // the sphere coords are sx & sy. Back them off a little and
                            // draw a line between them.
                            isx = (int)((-900 * xz) + midw);
                            isy = (int)((-900 * yz) + midh);

                            int lx1, lx2, ly1, ly2;
                            lx1 = sx;
                            lx2 = isx;
                            ly1 = sy;
                            ly2 = isy;

                            drawLine(lx1, ly1, lx2, ly2, n);
                        }
                    }

                    updateIt(sx, sy, c);

                }
            }

        }


    }
}
