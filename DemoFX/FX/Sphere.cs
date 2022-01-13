using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class _Sphere : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;


        double turn1 = 0;
        private const int Distance = -500;
        private const int CameraPosition = 300;
        private Random RandomClass;
        private int midWidth, midHeight;

        public void init()
        {
            midWidth = theWidth >> 1;
            midHeight = theHeight >> 1;

            RandomClass = new Random();
            turn1 = -6.3;
        }
        public void doIt(string msg)
        {
            int sx, sy;
            double cr1, sr1;
            int radius = 50;

            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            turn1 += 0.001;

            cr1 = Math.Cos(turn1);
            sr1 = Math.Sin(turn1);

            for (int latitude = -90; latitude < 90; latitude += 5)
            {
                double current_radius = Math.Cos(Utils.Calc3D.d2r(latitude)) * radius;
                double z = Math.Sin(Utils.Calc3D.d2r(latitude)) * radius;

                // Every 10 degrees of latitude, draw a longitude line.
                // Otherwise, draw a point every 10 degrees of longitude.
                int increment = 5;
                if (latitude % 10 == 0)
                    increment = 10;               

                for (int longitude = 0; longitude < 360; longitude += increment)
                {
                    double x = Math.Cos(Utils.Calc3D.d2r(longitude)) * current_radius;
                    double y = Math.Sin(Utils.Calc3D.d2r(longitude)) * current_radius;

                    Utils.Calc3D.calc_3d((int)x, (int)y, (int)z, midWidth, midHeight, cr1, sr1, out sx, out sy, CameraPosition, Distance);
                    Color c = Color.FromArgb(sx % 255, sy % 255, longitude % 255);
                    
                    updateIt(sx, sy, c);//RGBHSL.SetBrightness(Color.White,1));
                }
            }

            endIt();
            drawIt();
        }


    }
}
