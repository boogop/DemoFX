using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class _ParallaxStars : Base.BaseGraphics, Base.iDemo
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
        private const int MAX_STARS = 150;
        private double StarSpeed1 = .1;
        private double StarSpeed2 = .3;
        private double StarSpeed3 = .5;

        struct pstar
        {
            public int y, layer;
            public double x;
            public Color color;
        }

        pstar[] pStars;

        public void init()
        {
            pStars = new pstar[MAX_STARS];
            RandomClass = new Random();

            // Initialize our array of stars
            for (int x = 0; x < MAX_STARS; x++)
            {
                // we'll place them at random on the screen
                int RandomNumber = RandomClass.Next(0, theWidth);

                pStars[x].x = RandomNumber;

                // we'll set some different colors for the
                // different layers just for kicks
                pStars[x].color = Color.LightGray;

                // set a random height
                RandomNumber = RandomClass.Next(0, theHeight);

                pStars[x].y = RandomNumber;

                // and assign each to layer 1
                pStars[x].layer = 1;

            }

            // Now we'll take half of them and assign
            // them to layers two and three
            int g = MAX_STARS >> 1;

            // so increment our counter by two each time,
            // assign the first pixel to layer two and the
            // one next to it to layer 3. We'll give the layers
            // different colors.
            for (int x = 0; x < g; x += 2)
            {
                pStars[x].layer = 2;
                pStars[x].color = Color.LightGray;
                pStars[x + 1].layer = 3;
                pStars[x + 1].color = Color.White;
            }
        }

        public void doIt(string msg)
        {

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();
            // Now we just increment each star horizontally
            // depending on which speed we assigned to the
            // StarSpeed variables
            for (int c = 0; c < MAX_STARS; c++)
            {
                switch (pStars[c].layer)
                {
                    case 1:
                        pStars[c].x += StarSpeed1;
                        break;
                    case 2:
                        pStars[c].x += StarSpeed2;
                        break;
                    case 3:
                        pStars[c].x += StarSpeed3;
                        break;
                    default:
                        pStars[c].x += StarSpeed1;
                        break;
                }

                // If the star has gone off the edge of the
                // screen, put it back at the other edge
                if (pStars[c].x > theWidth)
                {
                    pStars[c].x = 0;
                }


                // We're doing a conversion here because integer
                // speed values makes the whole thing move too fast
                int x = (int)pStars[c].x;
                int y = (int)pStars[c].y;


                // set the pixels
                // if (x > 0 && x < theWidth && y > 0 && y < theHeight)
                updateIt(x, y, pStars[c].color);
            }


            endIt();
            drawIt();

            // reset the y positions when x goes off the screen so it doesn't
            // look so freaking static
            for (int x = 0; x < MAX_STARS; x++)
            {
                int RandomNumber = RandomClass.Next(0, theHeight);
                if (pStars[x].x == 0)
                    pStars[x].y = RandomNumber;
            }
        }


    }
}
