using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    public class Tunnel : Base.BaseGraphics, Base.iDemo
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

        colr[] colors;

        const double PI = 3.14159f;

        bool doMovement = false;

        struct star
        {
            public double xpos, ypos, speed, zpos;
            public double origX, origY, origZ, pathvarx, pathvary;
            public Color color;
        }

        private star[,] circles;

        private int _pointsPerCircle = 36;
        private int _numCircles = 25;
        private double zDistance = 5;


        double maxZ = 0;
        double diam = 50;
        double camZmover, camYmover, camXmover, camZinc, camXinc, camYinc;
        double zposmult;
        double increment;
        double startPath;
        double turn;

        public void init()
        {
            theMarginWidth = theMarginHeight = 5;

            camZinc = -.0001;
            camXinc = -.03;
            camYinc = .03;
            camZmover = -10;
            camYmover = -70;
            camXmover = -27;

            zposmult = 1;

            circles = new star[_numCircles, _pointsPerCircle];

            #region init

            int angle = 0;
            double zPos = 1;
            int angleAdder = (int)(360 / _pointsPerCircle);
            double zPosAdder = 2.06;
            zPosAdder = zDistance / (double)_numCircles;

            increment = 2 / (double)_numCircles;
            startPath = -1.0;

            double midWidth = theWidth / 2;
            double midHeight = theHeight / 2;

            for (int j = 0; j < _numCircles; j++)
            {

                startPath += increment;
                double adderX = midWidth * Math.Sin(startPath);
                double adderY = midHeight * Math.Sin(startPath);


                for (int i = 0; i < _pointsPerCircle; i++)
                {
                    double Y = .8 * diam * Math.Sin(d2r(angle));
                    double X = 2 * diam * Math.Cos(d2r(angle));
                    circles[j, i].xpos = X;// +30;
                    circles[j, i].ypos = Y;// -10;
                    circles[j, i].speed = .006;
                    circles[j, i].zpos = zPos;
                    circles[j, i].origZ = zPos;
                    circles[j, i].origX = X;
                    circles[j, i].origY = Y;
                    circles[j, i].pathvarx = adderX;
                    circles[j, i].pathvary = adderY;

                    circles[j, i].color = Color.White;
                    angle += angleAdder;

                }

                zPos += zPosAdder;

            }

            startPath = -1.0;
            maxZ = 0;

            for (int j = 0; j < _numCircles; j++)
            {
                for (int i = 0; i < _pointsPerCircle; i++)
                {
                    if (circles[j, i].origZ > maxZ)
                        maxZ = circles[j, i].origZ;
                }
            }

            colors = new colr[256];

            for (int i = 0; i < 256; i++)
            {
                double red = 1 + Math.Cos(i * Math.PI / 128);
                double grn = 1 + Math.Cos((i - 85) * Math.PI / 128);
                double blu = 1 + Math.Cos((i + 85) * Math.PI / 128);
                colors[i].r = (int)(red * 127) % 256;
                colors[i].g = (int)(grn * 127) % 256;
                colors[i].b = (int)(blu * 127) % 256;
            }

            #endregion

           

        }


        public void doIt(string msg)
        {
            doMovement = true;

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            turn += .002;
            double sr1 = Math.Sin(turn);
            double cr1 = Math.Cos(turn);

            #region movement
            if (doMovement)
            {
                camZmover += camZinc;
                camYmover += camYinc;
                camXmover += camXinc;
            }
            if (camXmover > -7)
            {
                camXmover = -7;
                camXinc = -camXinc;
            }
            if (camXmover < -132)
            {
                camXmover = -132;
                camXinc = -camXinc;
            }
            if (camYmover > 12)
            {
                camYmover = 12;
                camYinc = -camYinc;
            }
            if (camYmover < -60)
            {
                camYmover = -60;
                camYinc = -camYinc;
            }
            if (camZmover > .46)
            {
                camZmover = .46;
                camZinc = -camZinc;
            }
            if (camZmover < -.47)
            {
                camZmover = -.47;
                camZinc = -camZinc;
            }
            #endregion

            Color co = Color.Black;
            for (int i = 0; i < _numCircles; i++)
            {
                double px;
                double py;

                #region draw a circle

                for (int j = 0; j < _pointsPerCircle; j++)
                {
                    circles[i, j].zpos -= circles[i, j].speed;

                    if (circles[i, j].zpos < 1.5)
                    {
                        circles[i, j].xpos = circles[i, j].origX;
                        circles[i, j].ypos = circles[i, j].origY;
                        circles[i, j].zpos = maxZ;
                    }

                    double cc = 1 - Math.Abs(circles[i, 0].zpos / (maxZ * .8));
                    if (cc < 0) cc = 0;
                    if (cc > 1) cc = 1;
                    int c = (int)(300 * cc);
                    if (c > 255) c = 255;
                    co = Color.FromArgb(0, c, c);


                    double xrad = camZmover * .4;
                    double ymov = (Math.Sin(xrad) * Math.Cos(xrad));

                    double x = circles[i, j].xpos - camXmover;
                    double y = circles[i, j].ypos - camYmover;
                    double z = circles[i, j].zpos - 1.5;

                    px = 80 * sr1 * Math.Sin(circles[i, j].zpos * zposmult);
                    py = 50 * cr1 * Math.Cos(circles[i, j].zpos * zposmult) + 50;

                    double sx = (x / z) + px + theMarginWidth;
                    double sy = (y / z) + py + theMarginHeight;

                    int ix = (int)(sx);
                    int iy = (int)(sy);

                  
                    if (ix + 1 > theMarginWidth && ix + 1 < theWidth - theMarginWidth && iy + 1 > theMarginHeight && iy + 1 < theHeight - theMarginHeight - 1)
                    {
                        updateIt(ix, iy, co);
                        updateIt(ix + 1, iy, co);
                        updateIt(ix - 1, iy, co);
                        updateIt(ix, iy + 1, co);
                        updateIt(ix, iy - 1, co);

                    }

                    #endregion


                }
            }

            endIt();
            drawIt();


        }      


    }
}
