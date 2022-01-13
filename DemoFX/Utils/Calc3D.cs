using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoFX.Utils
{
    // I got this from somewhere in the 90's, possibly on a BBS. Thus it's
    // a very old way to do 3D rotation yourself
    public static class Calc3D
    {

        public static double d2r(double degrees)
        {
            double conversion = 3.1416 / 180.0;
            return degrees * conversion;
        }

        public static void calc_3d(int x, int y, int z, int midx, int midy, double cr1,
           double sr1, out int sx, out int sy, int CameraPosition, int Distance)
        {
            // In 2D math, you can rotate an x,y point around some
            // central point like so:
            //
            // xnew = cos(turn) * x - sin(turn) * y 
            // ynew = sin(turn) * x + cos(turn) * y
            //
            // But now we've added a third dimension, z. So we
            // take our basic 2D rotation algorithm and apply
            // the same idea with a 'z' point. It makes things
            // a little harder to read but if you study the 2D
            // algorithm above you can see that we've just messed
            // with it a little bit in the code below.

            double x1, z1, y1, x2, y2, z2;

            // first, modify the basic 2D formula to account for z
            x1 = cr1 * x - sr1 * z;
            z1 = sr1 * x + cr1 * z;

            // Now do it again and plug in our new value for x and y.
            //
            // Here's the x. Using the x and y coordinates the way we are
            // gives us a certain kind of rotation. You can get other effects
            // by switching these around.
            x2 = cr1 * x1 + sr1 * y;

            // and here's the y. You can see we're using the z1 variable
            // we came up with in our calculations for x and y
            y1 = cr1 * y - sr1 * x1;
            y2 = sr1 * z1 + cr1 * y1;

            // Do it again for z.
            z2 = cr1 * z1 - sr1 * y1;

            // we need to futz with z2 a little to get a good camera position, ie
            // to ensure our rotating box isn't rotating off the screen somewhere
            z2 -= CameraPosition;

            // now we have x, y and z. Add the point around which we want to rotate
            // and calculate a distance
            sx = (int)(Distance * x2 / z2) + midx;
            sy = (int)(Distance * y2 / z2) + midy;
        }

    }
}
