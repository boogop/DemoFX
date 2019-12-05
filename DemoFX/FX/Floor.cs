using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    public class Floor : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        int[,] colors;
        double turnF = 0;
        double scale = 250;
        double elevation = -40;
        double horiz;
        int skipcolor, floorStart;
        int midWidth, midHeight;

        public void init()
        {
            midWidth = theWidth >> 1;
            midHeight = theHeight >> 1;
            colors = new int[128, 128];
            horiz = scale * elevation;
            floorStart = midHeight + 20;
            skipcolor = Color.FromArgb(0, 0, 0).ToArgb();


            for (int i = 0; i < 128; i++)
                for (int j = 0; j < 128; j++)
                    colors[i, j] = Color.FromArgb(255, 255, 255).ToArgb();

            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    colors[i, j] = Color.FromArgb(0, 0, 0).ToArgb();
                    colors[i + 64, j + 64] = Color.FromArgb(0, 0, 0).ToArgb();
                }
            }

        }

        public void doIt(string msg)
        {
            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            doFloor();

            endIt();
            drawIt();
        }


        private void doFloor()
        {
            int uu;
            turnF += 2.8;

            for (int y = floorStart; y < theHeight; y++)
            {
                double fvs = (horiz / (y - midHeight)) / scale;

                for (int x = 0; x < theWidth; x += 2)
                {
                    double u = (x - midWidth) * fvs + turnF;

                    int u1 = (int)u;
                    uu = ((u1 % 128) + 128) % 128;

                    int fcol = colors[uu, 0];
                    if (fcol > skipcolor)
                    {
                        updateIt(x, y, Color.White);
                        updateIt(x + 1, y, Color.White);
                    }
                    else
                    {
                        updateIt(x, y, Color.Red);
                        updateIt(x + 1, y, Color.Red);
                    }
                }
            }
        }




    }
}
