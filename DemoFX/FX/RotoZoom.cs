using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    public class RotoZoom : Base.BaseGraphics,Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        public int inDemo = 1;

        Color[,] c;
        Color[,] b;

        int clen, cwid;
        int blen, bwid;
        double[] roto;
        double[] roto2;
        int path = 0, zpath = 0;
        double turn = 0;
        double sr1, cr1, zpos;


        public void init()
        {
            roto = new double[256];
            roto2 = new double[256];
          
            loadTexture(DemoFX.Properties.Resources.phar, out clen, out cwid, out c);
            loadTexture(DemoFX.Properties.Resources.ZPLAS, out blen, out bwid, out b);            

            for (int i = 0; i < 256; i++)
            {
                double rad = i * 1.41176 * 0.0174532;
                double cx = Math.Sin(rad);// +Math.Cos(rad);
                double cy = Math.Cos(rad);
                roto[i] = (cx + 0.8) * 4096.0;
                roto2[i] = (2.0 * cx) * 4096.0;
            }


        }

        public void doIt(string msg)
        {
            int x, y, i, j, xd, yd, a, bb, sx, sy;
            turn += .01;
            zpos = -1;
            
            sr1 = Math.Sin(turn);
            cr1 = Math.Cos(turn);

            int cl = blen / 2;
            int cw = bwid / 2;

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();
           
            int stepx = (int)roto[path];
            int stepy = (int)roto[(path + 128) & 255];
            int zoom = (int)roto2[path];
           
            path = (path - 1) & 255;
            zpath = (zpath + 1) & 255;

            sx = sy = 0;
            xd = (stepx * zoom) >> 12;
            yd = (stepy * zoom) >> 12;

            for (j = 0; j < theHeight; j++)
            {
                x = sx; y = sy;
                for (i = 0; i < theWidth; i++)
                {
                    a = x >> 12 & (clen - 1);
                    bb = y >> 12 & (cwid - 1);

                    int xx = i, xy = j;

                    Color co = c[bb, a];

                   // if (xx > theMarginWidth && xx < theWidth - theMarginWidth && xy > theMarginHeight && xy < theHeight - theMarginHeight)
                        if (bb < clen && a < cwid)
                            updateIt(xx, xy, co);

                    x += xd; y += yd;
                }

                sx -= yd; sy += xd;
            }
           
            int cx, cy;
            cx = 0;
            for (int ii = -cl; ii < cl; ii++)
            {
                cy = 0;
                for (int ij = -cw; ij < cw; ij++)
                {
                    double ix = (ii / zpos);
                    double iy = (ij / zpos);

                    calc_3d((int)ix, (int)iy, (int)zpos, theWidth >>1, theHeight >>1, cr1, sr1, out sx, out sy, 600, -500);

                    Color co = c[cx, cy];

                  //  if (sx > theMarginWidth && sx < theWidth - theMarginWidth && sy > theMarginHeight && sy < theHeight - theMarginHeight)
                        updateIt(sx, sy, co);

                    cy++;
                }
                cx++;
            }


            endIt();
            drawIt();

        }
    }
}
