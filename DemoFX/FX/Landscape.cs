using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    public class Landscape : Base.BaseGraphics, Base.iDemo
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

        struct cols
        {
            public int r, g, b;
        }

        Point3D[,] Points;
      
        private cols[] colors;

        double EyeX;
        double EyeY;
        double EyeZ;


        struct pos
        {
            public double x, y, z;
        }

        pos[] Point3d;
        double[,] heightmap;
        int mapdim = 650;
        private const int PixelSpacing = 1;
        Random rand;
        double countx = 0; double county = 0;
        double xcounter, ycounter;
        double xcountermover, ycountermover;
        int count = 0;
        int centerx, centery;
        

        public void init()
        {
            centerx = theWidth >> 1;
            centery = theHeight >> 1;

            EyeX = 30;
            EyeY = 60;
            EyeZ = 30;

            colors = new cols[256];
            double red;
            double grn;
            double blu;
            for (int i = 0; i < 256; i++)
            {
                red = 1 + Math.Cos(i * Math.PI / 128);
                grn = 1 + Math.Cos((i - 85) * Math.PI / 128);
                blu = 1 + Math.Cos((i + 85) * Math.PI / 128);
                colors[i].r = (int)(red * 127) % 256;
                colors[i].g = (int)(grn * 127) % 256;
                colors[i].b = (int)(blu * 127) % 256;
            }

            #region height

            heightmap = new double[mapdim * 3, mapdim * 3];
            rand = new Random();
            int d = ((mapdim * 2) / PixelSpacing) + 1;
            Point3d = new pos[d * d];

            for (int x = 0; x < mapdim * 2; x++)
                for (int y = 0; y < mapdim * 2; y++)
                    heightmap[x, y] = rand.NextDouble() * 50;


            for (int idx = 0; idx < 2; idx++)
                for (int x = 1; x < mapdim * 2 - 1; x++)
                    for (int y = 1; y < mapdim * 2 - 1; y++)
                        heightmap[x, y] = (heightmap[x - 1, y - 1] + heightmap[x - 1, y + 1] + heightmap[x + 1, y - 1] +
                                           heightmap[x + 1, y + 1] + heightmap[x, y - 1] + heightmap[x, y + 1] +
                                           heightmap[x - 1, y] + heightmap[x + 1, y]) / 5.2;


            #endregion

            #region point3d

            for (int i = 0; i < Point3d.Length; i++)
            {
                Point3d[i].x = 0;
                Point3d[i].y = 100;
                Point3d[i].z = 0;
            }

            int cc = 0;
            countx = 0; county = 0;
           
            countx = 20;
            for (int x = -mapdim; x <= mapdim; x += PixelSpacing)
            {
                county = 20;
                for (int z = -mapdim; z <= mapdim; z += PixelSpacing)
                {
                    double y = heightmap[(int)countx, (int)county];

                    Point3d[cc].x = x * 3;
                    Point3d[cc].y = y;
                    Point3d[cc].z = z;                   

                    cc++;
                    county++;
                }
                countx++;
            }

            #endregion


            Points = new Point3D[40, 40];

            countx = 30;

            for (int i = 0; i < 40; i++)
            {
                countx++;
                county = 30;
                for (int j = 0; j < 40; j++)
                {
                    Points[i, j].Coord = new double[5];
                    Points[i, j].Coord[0] = i - 20;
                    Points[i, j].Coord[1] = j - 20;
                    Points[i, j].Coord[3] = .8;

                    double y = heightmap[(int)countx, (int)county];
                    Points[i, j].c = Color.LimeGreen;
                   
                    Points[i, j].Coord[2] = y * .06;// (float)Math.Sin(r1);
                    Points[i, j].Coord[4] = Points[i, j].Coord[2];

                    county++;
                }
            }


            countx = county = 31;
            xcounter = ycounter = 1;
            xcountermover = .4;
            ycountermover = .4;

        }

        public void doIt(string msg)
        {
            double PI = 3.14159F;
            double Dtheta = .005;// rotation speed
            double Dphi = PI / 8;

            double theta;
            double phi;
            double r1;
            double r2;

            count++;

            theta = Atan(d2r(EyeX), d2r(EyeY));
            r1 = Math.Sqrt(EyeX * EyeX + EyeY * EyeY);
            r2 = Math.Sqrt(EyeX * EyeX + EyeY * EyeY + EyeZ * EyeZ);

            phi = Atan(r1, EyeZ);

            theta = theta - Dtheta;

            EyeX = (float)(r1 * Math.Cos(theta));
            EyeY = (float)(r1 * Math.Sin(theta));          

            double[,] T = new double[4, 4];

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            CalculateTransformation(out T);

            if (count % 5 == 0)
            {
                countx += xcountermover;
                county += ycountermover;
            }

            if (countx >= mapdim - 145)
            {
                xcountermover = -xcountermover;
                countx = mapdim - 145;
            }

            if (countx <= 30)
            {
                xcountermover = -xcountermover;
                countx = 30;
            }

            xcounter = countx;

            for (int i = 0; i < 40; i++)
            {
                xcounter += .5;

                if (county >= mapdim - 145)
                {
                    ycountermover = -ycountermover;
                    county = mapdim - 145;
                }

                if (county <= 30)
                {
                    ycountermover = -ycountermover;
                    county = 30;
                }
                               
                ycounter = county;

                for (int j = 0; j < 40; j++)
                {
                    double y = heightmap[(int)xcounter, (int)ycounter];

                    Points[i, j].c = Color.LimeGreen;
                    if (y < 250) Points[i, j].c = Color.Orange;
                    if (y > 290) Points[i, j].c = Color.Blue; // 280

                    Points[i, j].Coord[2] = y * .06;
                    Points[i, j].Coord[4] = Points[i, j].Coord[2];

                    ycounter++;
                }
            }         

            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    VectorMatrixMult(out Points[i, j].Trans, Points[i, j].Coord, T);
                }
            }

            double CurrentX, CurrentY;
            int distance = 12;

            for (int i = 10; i < 30; i++)
            {
                for (int j = 10; j < 30; j++)
                {
                    Color c = Points[i, j].c;

                    CurrentX = distance * Points[i, j].Trans[0] + centerx * .7;
                    CurrentY = distance * Points[i, j].Trans[1] + theHeight * .5;

                    int ix = (int)(CurrentX * 1.5);
                    int iy = (int)CurrentY - 120;

                    if (ix > theMarginWidth && ix < theWidth - theMarginWidth && iy > theMarginHeight && iy < theHeight - theMarginHeight)
                    {
                        updateIt(ix, iy, c); updateIt(ix + 1, iy, c); updateIt(ix, iy + 1, c);
                    }
                }
            }

            endIt();
            drawIt();

        }


        private double Atan(double x, double y)
        {
            double PI = 3.14159;

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


        private void VectorMatrixMult(out double[] Rpt, double[] Ppt, double[,] A)
        {
            double val = 0;
            Rpt = new double[4];
          
            for (int i = 0; i < 4; i++)
            {
                val = 0;
                for (int j = 0; j < 4; j++)
                {
                    val += Ppt[j] * A[j, i];
                }
                Rpt[i] = val;
            }           

            Rpt[0] = Rpt[0] * val;
            Rpt[1] = Rpt[1] * val;
            Rpt[2] = Rpt[2] * val;
            Rpt[3] = 1;// +Math.Cos(hslMover);
        }

        private void CalculateTransformation(out double[,] T)
        {
            double[,] T1 = new double[4, 4];
            double[,] T2 = new double[4, 4];

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


    }
}
