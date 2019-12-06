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
        
        int[] clr;
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
            horiz = scale * elevation;
            floorStart = midHeight + 20;
            skipcolor = Color.FromArgb(0, 0, 0).ToArgb();

            clr = new int[128];

            for (int i = 0; i < 128; i++)
                clr[i] = Color.FromArgb(255, 255, 255).ToArgb();

            for (int i = 0; i < 64; i++)
                clr[i] = Color.FromArgb(0, 0, 0).ToArgb();
           
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
            int uu,u1;
            turnF += 2.8;

            for (int y = floorStart; y < theHeight; y++)
            {
                double fvs = (horiz / (y - midHeight)) / scale;

                for (int x = 0; x < theWidth; x += 2)
                {
                    double u = (x - midWidth) * fvs + turnF;

                    u1 = (int)u;
                    uu = Math.Abs(u1 % 128);

                    int fcol = clr[uu];
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
