using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class _Fire : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        private Random RandomClass;
        public void init()
        {
            RandomClass = new Random();
        }
        public void doIt(string msg)
        {
            // bool inWidth = x > theMarginWidth && x < theWidth - theMarginWidth;
            // bool inHeight = y > theMarginHeight && y < theHeight - theMarginHeight;

          //  theGBMP.DrawLine(Pens.Black, theMarginWidth, theHeight - theMarginHeight - 5, theWidth - theMarginWidth - 1, theHeight - theMarginHeight - 5);
           // theGBMP.DrawLine(Pens.Black, theMarginWidth, theHeight - theMarginHeight - 6, theWidth - theMarginWidth - 1, theHeight - theMarginHeight - 6);
            
            theFB = startIt();

            for (int x = theMarginWidth; x < theWidth - theMarginWidth - 1; x += 45)
            {
                int RandomNumber = RandomClass.Next(theMarginWidth, theWidth - theMarginWidth - 1);
                theFB.SetPixel(RandomNumber, theHeight - theMarginHeight - 5, Color.Yellow);
                theFB.SetPixel(RandomNumber + 1, theHeight - theMarginHeight - 5, Color.Red);
                theFB.SetPixel(RandomNumber, theHeight - theMarginHeight - 6, Color.Yellow);
                theFB.SetPixel(RandomNumber + 1, theHeight - theMarginHeight - 6, Color.Red);
            }

            // Get and set each pixel on the screen.
            for (int y = theHeight - theMarginHeight - 5; y > theMarginHeight; y--)
            {
                for (int x = theMarginWidth; x < theWidth - theMarginWidth - 1; x++)
                {
                    // Get the color of the pixels. We'll grab
                    // four at a time and average them. Our fire
                    // will look different depending on how many
                    // we're averaging at a time.
                    Color c = theFB.GetPixel(x, y);
                    Color d = theFB.GetPixel(x, y - 1);
                    Color e = theFB.GetPixel(x - 1, y);
                    Color r = theFB.GetPixel(x + 1, y - 1);

                    // Add 'em and divide by four. Our RGB is
                    // a set of integer values we'll use in the
                    // FromArgb call below.
                    int rR = ((c.R + d.R + e.R + r.R) / 5);
                    int rG = ((c.G + d.G + e.G + r.G) / 4);
                    int rB = ((c.B + d.B + e.B + r.B) / 4);

                    // Now put them back one row up from where we got them!
                    theFB.SetPixel(x, y - 1, Color.FromArgb(rR, rG, rB));
                }
            }

            endIt();
            drawIt();
        }
    }
}
