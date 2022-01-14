using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DemoFX.FX
{
    class _Scroller : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;
        struct Point3D
        {
            public double[] Coord;
            public double[] Trans;
            public Color c;

        }
        Point3D[,] Points;

        double EyeX;
        double EyeY;
        double EyeZ;

        double xmove;
        double xmover;

        double shapemove;
        double shapemover;
        private colr[] colors;

        double PI = 3.14159;
        double Dtheta = .001;
        double Dphi;
        double theta;
        double phi;
        double r1;
        double r2;
        int midWidth, midHeight;
        double hh;
        double hw;

        string message_db = "SCROLLERS!! lol A tribute to the old demos ... Amiga demos rock! ... done in C# with 100% getpixel/putpixel routines (except for " +
          "the scrolling text and title, those are bmp.drawstring!)... no openGL, no directX ... getpixel routines are freaking slow but bounds checks on multidimensional " +
          "arrays are slower ... greets to everyone at dbfinteractive ... pouet is another source but I never found it very easy to use ";
         int maxX;
         int minY;
        // double txtXPos;
         double txtXMover;
        struct stext
        {
            public double x, y, t;
            public string c;
        }
        stext[] scroller;
        char[] scroll;
        int charSpacing = 25;
        bool flip;
        static LinearGradientBrush LGBrush;
        static Font myFont;

        public void init()
        {
            // grid stuff
            Points = new Point3D[40, 40];

            colors = new colr[256];
            standardPalette(ref colors);

            EyeX = 40;
            EyeY = 20;
            EyeZ = 30;

            Dphi = PI / 8;
            theta = Atan(d2r(EyeX), d2r(EyeY));
            r1 = Math.Sqrt(EyeX * EyeX + EyeY * EyeY);
            r2 = Math.Sqrt(EyeX * EyeX + EyeY * EyeY + EyeZ * EyeZ);
            phi = Atan(r1, EyeZ);

            midHeight = theHeight >> 1;
            midWidth = theWidth >> 1;

            hh = (double)midHeight;
            hw = (double)midWidth;

            xmove = .1;
            xmover = .001;
            shapemove = 6;
            shapemover = .06;

            double count = 0;

            for (int i = 0; i < 40; i++)
            {
                count += .2;

                for (int j = 0; j < 40; j++)
                {
                    Points[i, j].Coord = new double[5];
                    Points[i, j].Coord[0] = i - 20;
                    Points[i, j].Coord[1] = j - 20;
                    Points[i, j].Coord[3] = .5;

                    double r1 = Math.Sqrt((i - 10) * (i - 10) + (j - 10) * (j - 10));

                    double Y = hh + (50 * Math.Sin(d2r(r1)));
                    double X = hw + (50 * Math.Cos(d2r(r1)));

                    double r2 = Math.Sqrt((X - 5) * (X - 5) + (Y - 5) * (Y - 5));
                    Points[i, j].Coord[2] = (float)Math.Sin(r2) * count;
                    Points[i, j].Coord[4] = Points[i, j].Coord[2];
                }
            }

            // set up the scroller
            minY = theHeight - theMarginHeight - 25;
            maxX = theWidth -theMarginWidth-10;
            txtXMover = 1;

            scroll = message_db.ToCharArray();

            scroller = new stext[scroll.Length];
            for (int i = 0; i < scroll.Length; i++)
            {
                scroller[i].c = scroll[i].ToString();
            }

            scroller[0].x = maxX;
            scroller[0].y = 0;
            double k = maxX;
            for (int i = 1; i < scroller.Length; i++)
            {
                scroller[i].x = k + charSpacing;
                scroller[i].y = 0;
                k = scroller[i].x;
            }

            myFont = new Font("Arial", 18, FontStyle.Bold);

        }

        public void doIt(string msg)
        {

            theta = theta - Dtheta;

            shapemove += shapemover;
            if (shapemove < 5)
            {
                shapemove = 5;
                shapemover = -shapemover;
            }
            if (shapemove > 30)
            {
                shapemove = 30;
                shapemover = -shapemover;
            }

            double count = 0;

            for (int i = 0; i < 40; i++)
            {
                count += .2;
                for (int j = 0; j < 40; j++)
                {
                    double xxx = Math.Sqrt((i - shapemove) * (i - shapemove) + (j - shapemove) * (j - shapemove));

                    double Y = hh + (50 * Math.Sin(d2r(xxx)));// -(50 * Math.Cos(d2r(xxx)));
                    double X = hw + (50 * Math.Cos(d2r(xxx)));

                    double yyy = Math.Sqrt((X - 5) * (X - 5) + (Y - 5) * (Y - 5));
                    Points[i, j].Coord[2] = (float)Math.Sin(yyy) * count;
                    Points[i, j].Coord[4] = Points[i, j].Coord[2];
                }
            }
            ///////////////////////////////////////////////////

            EyeX = (float)(r1 * Math.Cos(theta));
            EyeY = (float)(r1 * Math.Sin(theta));
            EyeZ = (float)(r2 * Math.Sin(phi));

            double[,] T = new double[4, 4];

            CalculateTransformation(out T);

            int sx, sy;

            xmove += xmover;
            if (xmove < -1)
            {
                xmove = -1;
                xmover = -xmover;
            }
            if (xmove > 1.8)
            {
                xmove = 1.8;
                xmover = -xmover;
            }

            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    Points[i, j].Coord[2] = Points[i, j].Coord[4] * xmove;
                    VectorMatrixMult(out Points[i, j].Trans, Points[i, j].Coord, T);
                }
            }
            double CurrentX, CurrentY;
            int distance = 12;

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            for (int i = 0; i < 40; i++)
            {
                int b = 255 - (80 + i * 3);
                Color c = Color.FromArgb(colors[b].r, colors[b].g, colors[b].b);

                for (int j = 1; j < 40; j++)
                {
                    CurrentX = distance * Points[i, j].Trans[0] + hw + xmove;
                    CurrentY = distance * Points[i, j].Trans[1] + hh;

                    sx = (int)CurrentX;
                    sy = (int)CurrentY;

                    updateIt(sx, sy, c);

                }
            }


            endIt();
            drawScrollerText();
            drawIt();

        }

        private void drawScrollerText()
        {
            int x;
            int count = 0;

            for (int i = 0; i < scroller.Length; i++)
            {
                scroller[i].x -= txtXMover;
                x = (int)scroller[i].x;
                if (x < theMarginWidth)
                {
                    // as each letter goes off the screen we increment a counter and don't try to draw it
                    count++;
                    continue;
                }
                // don't draw any letters beyond the left border
                if (x > maxX) continue;

                int y;
                double xrad = x * 0.0174532;
                if (flip)
                    y = minY - (int)((Math.Sin(xrad) * Math.Cos(xrad)) * 20) - 20;
                else
                    y = minY - (int)(Math.Sin(xrad) * 20) - 20;

                int gW = (int)(scroller.Length * 24);
                // it's a sign of how fast processors have gotten that we used to have to do this in assembler and now GDI works just fine
                LGBrush = new LinearGradientBrush(new Rectangle(x, y, gW, 24), Color.Red, Color.Yellow, LinearGradientMode.Vertical);
                
                theGBMP.DrawString(scroller[i].c, myFont, LGBrush, new Point(x, y));
               
            }

            if (count == scroller.Length)
            {
                // if all letters have gone off the screen, reset the whole thing
                scroller[0].x = maxX;
                scroller[0].y = 0;
                double k = maxX;
                for (int i = 1; i < scroller.Length; i++)
                {
                    scroller[i].x = k + charSpacing;
                    scroller[i].y = 0;
                    k = scroller[i].x;
                }
                count = 0;
                flip = !flip;
            }
        }


        private double Atan(double x, double y)
        {
            double angle;

            if (x == 0)
                angle = 0;
            else
            {
                angle = Math.Atan(y / x);
                if (x < 0) angle = PI + angle;
            }

            return angle;
        }

        private void MakeIdentity(out double[,] M)
        {
            M = new double[4, 4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (i == j)
                        M[i, j] = 1;
                    else
                        M[i, j] = 0;

                }
            }
        }

        private void MatrixMatrixMult(out double[,] R, double[,] A, double[,] B)
        {
            double val;
            R = new double[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    val = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        val += (A[i, k] * B[k, j]);
                    }
                    R[i, j] = val;
                }
            }
        }

        private void CalculateTransformation(out double[,] T)
        {
            double[,] T1;
            double[,] T2;

            double r1 = (float)Math.Sqrt((EyeX * EyeX + EyeY * EyeY));
            double stheta = EyeX / r1;
            double ctheta = EyeY / r1;
            MakeIdentity(out T1);
            T1[0, 0] = ctheta;
            T1[0, 1] = stheta;
            T1[1, 0] = -stheta;
            T1[1, 1] = ctheta;

            double r2 = (float)Math.Sqrt((EyeX * EyeX + EyeY * EyeY + EyeZ * EyeZ));
            double sphi = -r1 / r2;
            double cphi = -EyeZ / r2;
            MakeIdentity(out T2);
            T2[1, 1] = cphi;
            T2[1, 2] = sphi;
            T2[2, 1] = -sphi;
            T2[2, 2] = cphi;

            MatrixMatrixMult(out T, T1, T2);
        }

        private void VectorMatrixMult(out double[] Rpt, double[] Ppt, double[,] A)
        {
            double val = 0;
            Rpt = new double[4];

            // grid
            for (int i = 0; i < 4; i++)
            {
                val = 0;
                for (int j = 0; j < 4; j++)
                {
                    val += Ppt[j] * A[j, i];
                }
                Rpt[i] += val;
            }

            Rpt[0] = Rpt[0] * val;
            Rpt[1] = Rpt[1] * val;
            Rpt[2] = Rpt[2] * val;
            Rpt[3] = Rpt[3] * val;
        }

        new private double d2r(double degrees)
        {
            double conversion = 3.1416 / 180.0;
            return degrees * conversion;
        }


    }
}
