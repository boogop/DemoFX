using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    public class Boxen : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        private int Distance = -500;
        private const int CameraPosition = 300;
        private const double Pi_Squared = 9.86958;
        private const int PixelSpacing = 60;
    
        double turn, cr1, sr1;
        int midWidth, midHeight;

        colr[] colors;

        int[,] c, b, d;

        int clen, cwid;
        int blen, bwid;
        int dlen, dwid;
        int centerx, centery;
        int x1, y1, z1;
        int x2, y2, z2;
        int x3, y3, z3;
        int x4, y4, z4;
        int x5, y5, z5;
        int x6, y6, z6;
        int x7, y7, z7;
        int x8, y8, z8;
        double sr2 = 0;

        public void init()
        {
            midWidth = Convert.ToInt32(theWidth / 2);
            midHeight = Convert.ToInt32(theHeight / 2);

            turn = 0;

            colors = new colr[256];

            for (int i = 0; i < 256; i++)
            {
                double red = 1 + Math.Cos(i * Math.PI / 128);
                double grn = 1 + Math.Cos((i - 85) * Math.PI / 128);
                double blu = 1 + Math.Cos((i + 85) * Math.PI / 128);
                colors[i].r = (int)(red * 127) % 256;
                colors[i].g = (int)(grn * 127) % 256;
                colors[i].b = (int)(blu * 127) % 256;
            }

            centerx = theWidth >> 1;
            centery = theHeight >> 1;


            x1 = -30;
            y1 = -30;
            z1 = -30;
            x2 = -30;
            y2 = -30;
            z2 = 30;
            x3 = 30;
            y3 = -30;
            z3 = 30;
            x4 = 30;
            y4 = -30;
            z4 = -30;
            x5 = 30;
            y5 = 30;
            z5 = 30;
            x6 = 30;
            y6 = 30;
            z6 = -30;
            x7 = -30;
            y7 = 30;
            z7 = -30;
            x8 = -30;
            y8 = 30;
            z8 = 30;

            loadTexture(DemoFX.Properties.Resources.check, out blen, out bwid, out b);
            loadTexture(DemoFX.Properties.Resources.Planets1, out clen, out cwid, out c);
            loadTexture(DemoFX.Properties.Resources.ball4, out dlen, out dwid, out d);
        }

        public void doIt(string msg)
        {
            double scale = .6;

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();
            int i, j;

            for (i = 0; i < clen; i++)
            {
                for (j = 0; j < cwid; j++)
                {
                    int xx = c[i, j];
                    if (xx == 999) continue;
                    updateIt(i + centerx, j + 50, Color.FromArgb(xx));

                }
            }
            

            for (i = 0; i < blen; i++)
            {
                for (j = 0; j < bwid; j++)
                {
                    int xx = b[i, j];
                    if (xx == 999) continue;

                    updateIt(i, j + 210, Color.FromArgb(xx));
                }
            }

            turn += 0.015;

            cr1 = Math.Cos(turn);
            sr1 = Math.Sin(turn);

            sr2 = sr1;

            double zz1, zz2, zz3, zz4, zz5, zz6, zz7, zz8;
            int sx1, sy1, sx2, sy2, sx3, sy3, sx4, sy4;
            int sx5, sy5, sx6, sy6, sx7, sy7, sx8, sy8;

            calc_3d(x1, y1, z1, midWidth, midHeight, cr1, sr1, out sx1, out sy1, CameraPosition, Distance, out zz1);
            calc_3d(x2, y2, z2, midWidth, midHeight, cr1, sr1, out sx2, out sy2, CameraPosition, Distance, out zz2);
            calc_3d(x3, y3, z3, midWidth, midHeight, cr1, sr1, out sx3, out sy3, CameraPosition, Distance, out zz3);
            calc_3d(x4, y4, z4, midWidth, midHeight, cr1, sr1, out sx4, out sy4, CameraPosition, Distance, out zz4);
            calc_3d(x5, y5, z5, midWidth, midHeight, cr1, sr1, out sx5, out sy5, CameraPosition, Distance, out zz5);
            calc_3d(x6, y6, z6, midWidth, midHeight, cr1, sr1, out sx6, out sy6, CameraPosition, Distance, out zz6);
            calc_3d(x7, y7, z7, midWidth, midHeight, cr1, sr1, out sx7, out sy7, CameraPosition, Distance, out zz7);
            calc_3d(x8, y8, z8, midWidth, midHeight, cr1, sr1, out sx8, out sy8, CameraPosition, Distance, out zz8);

            ArrayList ar = new ArrayList();
            ar.Add(zz1);
            ar.Add(zz2);
            ar.Add(zz3);
            ar.Add(zz4);
            ar.Add(zz5);
            ar.Add(zz6);
            ar.Add(zz7);
            ar.Add(zz8);
            ar.Sort();

            for (i = 0; i < ar.Count; i++)
            {
                if ((double)ar[i] == zz1)
                    drawBall(scale, sx1, sy1);
                if ((double)ar[i] == zz2)
                    drawBall(scale, sx2, sy2);
                if ((double)ar[i] == zz3)
                    drawBall(scale, sx3, sy3);
                if ((double)ar[i] == zz4)
                    drawBall(scale, sx4, sy4);
                if ((double)ar[i] == zz5)
                    drawBall(scale, sx5, sy5);
                if ((double)ar[i] == zz6)
                    drawBall(scale, sx6, sy6);
                if ((double)ar[i] == zz7)
                    drawBall(scale, sx7, sy7);
                if ((double)ar[i] == zz8)
                    drawBall(scale, sx8, sy8);
            }


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

                    updateIt((int)((i + ix) * scale) + 50, (int)((j + iy) * scale) + 50, Color.FromArgb(xx));

                }
            }
        }

      

        // cycling rotation
        private void calc_3d(int x, int y, int z, int midx, int midy, double cr1,
             double sr1, out int sx, out int sy, int CameraPosition, int Distance, out double zz)
        {
            double scale = 1.5;
            double x1, z1, y1, x2, y2, z2;

            x1 = cr1 * x + sr1 * z;
            y1 = cr1 * y + sr1 * x1;
            z1 = sr1 * x - cr1 * z;

            x2 = cr1 * x1 - sr1 * y;
            y2 = sr1 * z1 - cr1 * y1;
            z2 = cr1 * z1 + sr1 * y1;

            z2 -= CameraPosition;
            zz = z2;

            sx = (int)(scale * (x2 * Math.Cos(turn) - y2 * Math.Sin(turn))) + midx;
            sy = (int)(scale * (x2 * Math.Sin(turn) + y2 * Math.Cos(turn))) + midy;


        }

       

    }
}
