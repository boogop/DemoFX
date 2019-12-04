using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class ShadeBobs : Base.BaseGraphics, Base.iDemo
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
        int midHeight = 0;
        int midWidth = 0;
        const int bobWidth = 25;
        const int numAngles = 256;
        int[,] shade_array;
        int[] anglearrayX, anglearrayY;
        int angleCounter = 0;
        int current_angle = 0;
        const double radius = 120;
        double rads;
        Color[] colArray;
        double patternSwitch = 0;

        bool clear_once = false;

        public void init()
        {
          
            colors = new colr[256];
            colArray = new Color[256];
            shadePalette(ref colors);

            midWidth = theHeight >> 1;
            midHeight = theWidth >> 1;
            shade_array = new int[theWidth, theHeight];

            // optimization. If we use a color array we can skip doing the conversion
            // in the loop and trade three bounds checks for one
            for (int i = 0; i < 256; i++)
            {
                Color c = Color.FromArgb(colors[i].r, colors[i].g, colors[i].b);
                colArray[i] = c;
            }

            // numAngles is 256 instead of 360 so I can trade an if for a bitwise &. Increasing or 
            // decreasing the value of angle makes it run either faster, and look worse, or
            // slower and look better
            rads = 0.0249;

            anglearrayX = new int[numAngles];
            anglearrayY = new int[numAngles];

            resetAngleArray(6, 5);
            initShadeArray();

        }

        private void resetAngleArray(int cosMult, int sinMult)
        {
            current_angle = 0;
            for (int i = 0; i < numAngles; i++)
            {
                double angle = current_angle * rads;
                anglearrayX[i] = midWidth + (int)((radius) * Math.Cos(cosMult * angle));
                anglearrayY[i] = midHeight + (int)((radius) * Math.Sin(sinMult * angle)) - 25;
                current_angle++;
            }
        }

        private void initShadeArray()
        {
            int x, y;
            for (y = 0; y < theHeight; y++)
            {
                for (x = 0; x < theWidth; x++)
                {
                    shade_array[x, y] = 0;
                }
            }
        }

        public void doIt(string msg)
        {
            if (!clear_once)
            {
                // clear the last effect, we don't need this otherwise for this effect unless
                // we're switching patterns
                theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                clear_once = true;
            }

            patternSwitch+=0.5;
            if (patternSwitch > 2100)
                patternSwitch = 200;
            switchBob();

            theFB = startIt();
           
            move_bob();
            draw_shadebob();

            endIt();
            drawIt();

        }

        private void switchBob()
        {
            // I used to see two ways of doing shadebobs, leave one pattern to run or
            // switch up patterns. To me the second was more interesting
            switch ((int)patternSwitch)
            {
                case 200:
                    resetAngleArray(4, 5);
                    initShadeArray();
                    theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                    break;
                case 400:
                    resetAngleArray(3, 6);
                    initShadeArray();
                    theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                    break;
                case 600:
                    resetAngleArray(4, 6);
                    initShadeArray();
                    theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                    break;
                case 800:
                    resetAngleArray(4, 3);
                    initShadeArray();
                    theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                    break;
                case 1000:
                    resetAngleArray(5, 3);
                    initShadeArray();
                    theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                    break;
                case 1200:
                    resetAngleArray(1, 3);
                    initShadeArray();
                    theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                    break;
                case 1400:
                    resetAngleArray(4, 3);
                    initShadeArray();
                    theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                    break;
                case 1600:
                    resetAngleArray(8, 6);
                    initShadeArray();
                    theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                    break;
                case 1800:
                    resetAngleArray(1, 3);
                    initShadeArray();
                    theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
                    break;
                default:
                    break;
            }
        }

        void incrementBob(int x, int y)
        {
            int xa, ya;
            for (ya = 0; ya < bobWidth; ya++)
            {
                for (xa = 0; xa < bobWidth; xa++)
                {
                    shade_array[x + xa, y + ya] += 30;
                }
            }
        }

        void move_bob()
        {
            int x = anglearrayX[angleCounter];
            int y = anglearrayY[angleCounter];

            incrementBob(x, y);

            angleCounter++;
            angleCounter &= 255;
        }

        void draw_shadebob()
        {
            int x, y;

            for (y = 0; y < theHeight; y++)
            {
                for (x = 0; x < theWidth; x++)
                {
                    if (shade_array[x, y] == 0) continue; // skip black colors
                    int cx = shade_array[x, y];
                    if (cx > 255)
                        cx = 255;

                    updateIt(x, y, colArray[cx]);

                }
            }
        }

    }
}
