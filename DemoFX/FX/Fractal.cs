using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    public class Fractal : Base.BaseGraphics, Base.iDemo
    {
        /*
     * Dive down Scepter Valley
     * 
     * Scepter Valley wants to spend more time rendering the black areas. Those
     * will always run to max iterations so if k reaches bail, we can exit. We do
     * period checking to see if we're in the period-2 or cardiod bulb.
     * Other assumptions:
     * - must have a doevents for the app to behave while rendering
     * - escape time is the fastest algorithm
     * - add is faster than mult (old assumption that may no longer be valid)
     * - goto will emit as jump command 
     * - switch should be faster than if
     * - C# is optimized for doubles
     * - using pointers in the loop would probably be slower (haven't tried though)
     * 
     * MSIL for iteration loop     
     * 
     * By unrolling the loop we only execute LDARG and BLT every 4th time, so we trade 4 LDARGs and BLTs
     * for 4 BNE.
     * IL_01fa:  ldarg.s    iterations
     * IL_01fc:  blt        IL_00ec
     * Better or worse? Dunno!
     
          for (k = 0; k < iterations; k += 4) --> this is tested at the end in MSIL
          {            
            switch (k)
            {
              case bail:
                black = true;
                goto endit;
              default:
                z1 = dx * dx;
                z2 = dy * dy;
                if (z1 + z2 > 2.0)
                  goto endit; 
                dydx = dy * dx;
                dy = dydx + dydx + ci; 
                dx = z1 - z2 + cr;
                break;
            }

        IL_00e4:  ldc.i4.0
        IL_00e5:  stloc.s    k
        IL_00e7:  br         IL_01f8
        IL_00ec:  ldloc.s    k
        IL_00ee:  stloc.s    CS$0$0000    // first switch statement
        IL_00f0:  ldloc.s    CS$0$0000
        IL_00f2:  ldc.i4     0xc8         // 200
        IL_00f7:  bne.un.s   IL_0101      // compare k to 200, break not equal
        IL_00f9:  ldc.i4.1
        IL_00fa:  stloc.s    black        // black = true
        IL_00fc:  br         IL_0201      // goto endit
        IL_0101:  ldloc.2
        IL_0102:  ldloc.2
        IL_0103:  mul                     // dx * dx
        IL_0104:  stloc.0                 // not sure what this is doing, stack pointer?
        IL_0105:  ldloc.3
        IL_0106:  ldloc.3
        IL_0107:  mul                     // dy * dy
        IL_0108:  stloc.1
        IL_0109:  ldloc.0
        IL_010a:  ldloc.1
        IL_010b:  add                     // z1 + z2
        IL_010c:  ldc.r8     2.
        IL_0115:  bgt        IL_0201      // break greater than: if (z1 + z2 > 2.0)
        IL_011a:  ldloc.3
        IL_011b:  ldloc.2
        IL_011c:  mul                     // dy * dx
        IL_011d:  stloc.s    dydx
        IL_011f:  ldloc.s    dydx
        IL_0121:  ldloc.s    dydx
        IL_0123:  add                     // dydx + dydx
        IL_0124:  ldloc.s    ci
        IL_0126:  add                     // + ci
        IL_0127:  stloc.3
        IL_0128:  ldloc.0
        IL_0129:  ldloc.1
        IL_012a:  sub                     // z1 - z2
        IL_012b:  ldloc.s    cr
        IL_012d:  add                     // + cr
       
     * 
     */

        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;
        private static colr[] colors;

        int w, h, w2, h2;

        double a1 = -2.5;
        double b1 = -1.6;
        double ax1 = -2.5;
        double ax2 = 1.5;
        double bx1 = -1.6;
        double bx2 = 1.6;

        double ww;
        double hh;

        double a1w, b1h;

        double zoom = 1;

        const int iterations = 400;
        const int bail = 350;
        double aw;
        double bh;
        Color[] colArray;

        public void init()
        {
            colArray = new Color[iterations];

            w = theWidth;
            h = theHeight;
            w2 = w >> 1;
            h2 = h >> 1;

            colors = new colr[256];
            standardPalette(ref colors);

            a1 = 153.41801;
            b1 = h2 - 8.650006;

            ww = ax2 - ax1;
            hh = bx2 - bx1;

            a1w = a1 * ww;
            b1h = b1 * hh;

            aw = a1w / w;
            bh = b1h / h;

            for (int k = 0; k < iterations; k++)
            {
                int x = k % 128 * 2;
                Color c = Color.FromArgb(colors[x].g, colors[x].r, colors[x].b);
                colArray[k] = c;
            }


        }

        public void doIt(string msg)
        {
            theFB = startIt();

            DrawMandelbrot();

            endIt();
            drawIt();
        }

        private void DrawMandelbrot()
        {

            zoom += (zoom * .05);
            double d = zoom + zoom; // change mult to add

            if (d > 103260650)
                return;

            double wax = ww / d;
            double wah = hh / d;


            double newleft = aw + ax1 - wax;
            double newright = aw + ax1 + wax;
            double newtop = bh + bx1 - wah;
            double newbottom = bh + bx1 + wah;

            CreateMandelbrot(newleft, newright, newtop, newbottom, iterations);

        }

        private void CreateMandelbrot(double a1, double a2, double b1, double b2, int iterations)
        {           

            // declaring these at form level is significantly slower
            // ????
            double z1, z2, dx, dy, crx, yy, cr, ci, dydx;
            int k = 0;

            double dr = (a2 - a1) / w;
            double di = (b2 - b1) / h;

            for (int j = h; j > 0; j--)
            {
                ci = b1 + j * di;

              
                bool black = false;
               
                for (int i = 0; i < w; i++)
                {
                    cr = a1 + i * dr;

                    dx = 0.0;
                    dy = 0.0;

                    #region periods

                    // if we can tell we're inside the cardiod or period-2 bulb we can skip the iterations.
                    // period checks make the routine blaze until we're past them but the iteration loop is
                    // the big problem
                    crx = cr + 1.0;
                    yy = ci * ci;

                    if ((crx * crx) + yy < .0625)
                    {
                        // checks for a point inside the period-2 bulb
                        // but once you're past that it does nothing
                        updateIt(i, j, Color.Black);
                        continue;
                    }

                    // check for cardiod bulb
                    double temp = cr - .25;
                    double q = temp * temp + yy;
                    double a = q * (q + temp);
                    double b = .25 * yy;
                    if (a < b)
                    {
                        updateIt(i, j, Color.Black);
                        continue;
                    }


                    #endregion

                    
                    for (k = 0; k < iterations; k ++)
                    {
                        // the black areas will run to max iterations (inside the mandlebrot), so figure out if we're in
                        // a black area, set the color and break
                        // switch is usually faster than if due to jumptable opcodes          

                       
                        switch (k)
                        {
                            case bail:
                             
                                black = true;
                                goto endit;
                            default:
                                black = false;
                                z1 = dx * dx;
                                z2 = dy * dy;
                                if (z1 + z2 > 2.0) // no way around this comparison
                                {
                                    goto endit; // goto emits as jump
                                }
                                dydx = dy * dx;
                                dy = dydx + dydx + ci; // change (2.0 * dy * dx) to an add
                                dx = z1 - z2 + cr;
                               
                                break;
                        }
                    }

                    endit:

                    if (black)
                        updateIt(i, j, Color.Black);
                    else
                        updateIt(i, j, colArray[k]);


                }
            }

        }


    }
}
