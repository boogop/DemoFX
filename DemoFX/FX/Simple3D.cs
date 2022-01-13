using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    // This many nested loops is a really bad idea
    class _Simple3D : Base.BaseGraphics, Base.iDemo
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
        //private const double Pi_Squared = 9.86958;
        private const int PixelSpacing = 7;
        int distanceMover = 1;
        double turn;

        public void init()
        {
            turn = 0;

        }
        public void doIt(string msg)
        {

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            int x, y, z, sx, sy;
            double cr1, sr1;
            int midWidth, midHeight;

            // variables that tell us where the middle of the drawing
            // screen is
            midWidth = theWidth >> 1;
            midHeight = theHeight >> 1;

            Distance -= distanceMover;
            if (Distance < -1500 || Distance > -500)
                distanceMover = -distanceMover;

            // Our cube completes an entire rotation in approximately
            // 2 * Pi so we can approximate that using a fudge factor.
            // Anytime you can get rid of a calculation your code has to
            // do inside a loop, you should!
            if (turn < 6.3)
            {
                // increment turn. Larger values will make
                // things rotate faster, which isn't a good thing.
                turn += 0.001;

                // each time the while loop, we need a sin and cos
                // of our turn variable. 'Turn' is our angle of rotation,
                // and taking the sin and cos gives us a circular movement.
                cr1 = Math.Cos(turn);
                sr1 = Math.Sin(turn);

                // draw one set of sides for an open-ended box. I dunno,
                // cyan and violet. Whattaya think? Too metro?
                // This x-loop, you'll notice, executes exactly twice. The
                // y and z loops draw a set of pixels at--amazingly--two
                // sets of x-coordinates, so we get two flat "panels"
                for (x = -30; x <= 30; x += 60)
                {
                    for (y = -30; y <= 30; y += PixelSpacing)
                    {
                        for (z = -30; z <= 30; z += PixelSpacing)
                        {
                            // calculate rotation before we draw. Note the 'out'
                            // parameter. We want values for those returned.
                            Utils.Calc3D.calc_3d(x, y, z, midWidth, midHeight, cr1, sr1, out sx, out sy, CameraPosition, Distance);

                            // ...and draw the pixels using sx and sy our helper function
                            // above returned

                            updateIt(sx, sy, Color.White);

                        }
                    }
                }

                // draw the bottom of the box
                for (y = -30; y <= 30; y += PixelSpacing)
                {
                    // we just want one side here so we won't 
                    // go through the z loop. We could add the z loop
                    // if we wanted to close the box but it looks a little
                    // weird
                    z = -30;

                    for (x = -30; x <= 30; x += PixelSpacing)
                    {
                        Utils.Calc3D.calc_3d(x, y, z, midWidth, midHeight, cr1, sr1, out sx, out sy, CameraPosition, Distance);

                        updateIt(sx, sy, Color.White);

                    }
                }

                // draw another set of sides the way we did above,
                // but this time y is the 'controlling' loop
                for (x = -30; x <= 30; x += PixelSpacing)
                {
                    for (y = -30; y <= 30; y += 60)
                    {
                        for (z = -30; z <= 30; z += PixelSpacing)
                        {
                            Utils.Calc3D.calc_3d(x, y, z, midWidth, midHeight, cr1, sr1, out sx, out sy, CameraPosition, Distance);
                            updateIt(sx, sy, Color.White);

                        }
                    }
                }
            }
            else
            {
                // reset our turn variable to zero
                turn = 0;
            }


            endIt();
            drawIt();
        }

    }
}
