using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class XOR : Base.BaseGraphics, Base.iDemo
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

        double turn = 0;
        int midWidth, midHeight;
        int w, h;
        Random rand;
        double[] path;
        int count = 0;

        public void init()
        {
            midWidth = theWidth / 2;
            midHeight = theHeight / 3;

            w = theWidth;// - theMarginWidth;
            h = theHeight;// - theMarginHeight;

            rand = new Random();

            path = new double[1024];
            for (int i = 0; i < 1024; i++)
                path[i] = ((i * 0.3515625) * 0.0174532) * 10;

        }

        public void doIt(string msg)
        {
            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            if (count > 1023) count = 0;
            count++;

            turn += .08;
            double turn1 = turn * .1;
            double cr1 = Math.Sin(turn);
            double sr1 = Math.Cos(turn);
            double sr2 = Math.Cos(turn1) + Math.Sin(turn1);
         

            for (int i = 0; i < theHeight; i++)
            {
                for (int j = 0; j < theWidth; j++)
                {
                    double a = j - w * sr1;
                    double b = i - h * cr1;

                    double x = 128 + (128 * Math.Sin(Math.Sqrt(a * a + b * b) * .2));

                    a = j - w / 3;
                    b = i - h / 3;

                    double y = 128 + (128 * Math.Sin(Math.Sqrt(a * a + b * b) * .2));

                    int ix, iy;
                    ix = j; iy = i;
                   
                    int d = (int)x ^ (int)y;
                    Color co = Color.FromArgb(d << 16);

                   // if (ix > theMarginWidth && ix < theWidth - theMarginWidth && iy > theMarginHeight && iy < theHeight - theMarginHeight)
                        updateIt(ix, iy, co);
                }
            }  

            endIt();
            drawIt();
        }

    }
}
