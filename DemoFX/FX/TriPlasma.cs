using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class TriPlasma : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        private colr[] colors;

        double[] aSin, aCos;
        int pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0, tpos1, tpos2, tpos3, tpos4;


        public void init()
        {
            aSin = new double[512];
            aCos = new double[512];
            colors = new colr[256];

            int i;
            double rad;

            for (i = 0; i < 512; i++)
            {
                rad = (i * 0.703125) * 0.0174532;
                aSin[i] = Math.Sin(rad) * 3024;
                aCos[i] = Math.Cos(rad * 2) * 1024;
            }

            for (i = 0; i < 256; i++)
            {
                colors[i].r = 0;
                colors[i].g = 0;
                colors[i].b = 0;
            }

            for (int ch = 0; ch < 64; ch++)
            {
                colors[ch + 128].r = (ch * 3);
                colors[255 - ch].r = (ch * 3);
            }
        }

        public void doIt(string msg)
        {
            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            tpos4 = pos4;
            tpos3 = pos3;

            #region red

            for (int j = 0; j < theWidth; j++)
            {
                tpos3 &= 511;
                tpos4 &= 511;

                tpos1 = pos1 + 5;
                tpos2 = pos2 + 4;
                for (int i = 0; i < theHeight; i++)
                {
                    tpos1 &= 511;
                    tpos2 &= 511;
                    int x = (int)(aSin[tpos1] + aCos[tpos2] + aCos[tpos4]);

                    int index = (x >> 3);
                    int c = Math.Abs(index) & 255;

                    if (colors[c].r > 0)
                    {
                        Color co = Color.FromArgb(colors[c].r, 0, 0);
                        updateIt(j, i, co);
                    }

                    tpos1 += 1;
                    tpos2 -= 2;
                }

                tpos4 += 1;
                tpos3 -= 2;
            }

            #endregion

            pos1 += 1;
            pos3 -= 2;

            #region purple

            for (int j = 0; j < theWidth; j++)
            {
                tpos3 &= 511;
                tpos4 &= 511;

                tpos1 = pos1 - 1;
                tpos2 = pos2 + 2;
                for (int i = 0; i < theHeight; i++)
                {
                    tpos1 &= 511;
                    tpos2 &= 511;

                    int x = (int)(aSin[tpos4] + aCos[tpos2] + aCos[tpos3]);

                    int index = (x >> 3);
                    int c = Math.Abs(index) & 255;

                    if (colors[c].r > 0)
                    {
                        Color co = Color.FromArgb(colors[c].r, 0, colors[c].r);
                        updateIt(j, i, co);
                    }

                    tpos1 -= 1;
                    tpos2 += 1;
                }

                tpos4 -= 1;
                tpos3 += 1;
            }

            #endregion

            pos1 -= 1;
            pos3 += 1;

            #region green

            for (int i = 0; i < theHeight; i++)
            {
                tpos3 &= 511;
                tpos4 &= 511;

                tpos1 = pos1 - 5;
                tpos2 = pos2 + 4;
                for (int j = 0; j < theWidth; j++)
                {
                    tpos1 &= 511;
                    tpos2 &= 511;
                    int x = (int)(aSin[tpos4] + aCos[tpos1] + aCos[tpos2]);

                    int index = (x >> 3);
                    int c = Math.Abs(index) & 255;

                    if (colors[c].r > 50)
                    {
                        Color co = Color.FromArgb(0, colors[c].r, 0);
                        updateIt(j, i, co);
                    }

                    tpos1 += 1;
                    tpos2 -= 2;
                }

                tpos4 += 1;
                tpos3 -= 2;
            }

            #endregion

            pos1 += 1;
            pos3 -= 2;


            endIt();
            drawIt();


        }

    }
}
