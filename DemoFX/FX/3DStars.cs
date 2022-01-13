using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class _3DStars : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;
        struct star
        {
            public float xpos, ypos;
            public int zpos, speed;
            //Uint8 color;
            //public Fix.FInt x, y, z;
            //public int ox, oy;
            public Color color;
            public double bright;
        }

        star[] myStars;
        private Random RandomClass;
        private const int MAX_STARS = 150;
        int centerx, centery;

        public void init()
        {
            RandomClass = new Random();

            myStars = new star[MAX_STARS];

            centerx = theWidth >> 1;
            centery = theHeight >> 1;

            for (int i = 0; i < MAX_STARS; i++)
                init_star(out myStars[i]);
        }

        void init_star(out star star)
        {
            int sx = (int)(theWidth * .7);
            int sy = (int)(theHeight * .7);

            float x = (float)(RandomClass.Next(sx));
            float y = (float)(RandomClass.Next(sy));

            star.xpos = (float)(Math.Cos(x) * y * 2500);
            star.ypos = (float)(Math.Sin(x) * y * 2500);

            star.zpos = RandomClass.Next(80, 160) << 6;
            star.speed = RandomClass.Next(5, 15);
            star.color = Color.FromArgb(RandomClass.Next(255), RandomClass.Next(255), 255);
            star.bright = .001;
        }


        public void doIt(string msg)
        {

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            for (int i = 0; i < MAX_STARS; i++)
            {
                myStars[i].zpos -= myStars[i].speed;

                if (myStars[i].zpos <= 0)
                    init_star(out myStars[i]);

                /*compute 3D position*/
                double ix = (myStars[i].xpos / myStars[i].zpos) + (centerx);
                double iy = (myStars[i].ypos / myStars[i].zpos) + (centery);
               
                myStars[i].bright += .00095;
                myStars[i].color = Utils.RGBHSL.SetBrightness(myStars[i].color, myStars[i].bright);

                // the draw class checks for pixel off screen but we need it here to tell us
                // to reinitialize the star
                if (ix > 0 && ix < theWidth && iy > 0 && iy < theHeight)
                    updateIt((int)ix, (int)iy, myStars[i].color);
                else
                    init_star(out myStars[i]);
            }


            endIt();
            drawIt();

           
        }
    }
}
