using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.Base
{
    public abstract class BaseGraphics
    {

        private int _marginWidth;

        public int theMarginWidth
        {
            get { return _marginWidth; }
            set { _marginWidth = value; }
        }

        private int _marginHeight;

        public int theMarginHeight
        {
            get { return _marginHeight; }
            set { _marginHeight = value; }
        }

        private FastBitmap _fb;

        public FastBitmap theFB
        {
            get { return _fb; }
            set { _fb = value; }
        }

        private Bitmap _bmp;

        public Bitmap theBMP
        {
            get { return _bmp; }
            set { _bmp = value; }
        }

        private Graphics _gBmp;

        public Graphics theGBMP
        {
            get { return _gBmp; }
            set { _gBmp = value; }
        }

        private Graphics gForm;

        public Graphics theGForm
        {
            get { return gForm; }
            set { gForm = value; }
        }

        private int _width;

        public int theWidth
        {
            get { return _width; }
            set { _width = value; }
        }
        private int _height;

        public int theHeight
        {
            get { return _height; }
            set { _height = value; }
        }

        private bool _fadeOut;

        public bool theFadeOut
        {
            get { return _fadeOut; }
            set { _fadeOut = value; }
        }


        public bool fadeIn = true;
        public double fival = 0;
        public double fimover = .05;


        public struct colr
        {
            public int r;
            public int g;
            public int b;
        }
        
        public void shadePalette(ref colr[] colors)
        {
            for (int i = 0; i < 64; i++)
            {
                colors[i].r = 0;
                colors[i].g = 0;
                colors[i].b = 4 * i;

                colors[64 + i].r = 0;
                colors[64 + i].g = i << 2;
                colors[64 + i].b = 128 - (i << 1);
            }

            for (int i = 128; i < 256; i++)
            {
                colors[i].r = i;
                colors[i].g = i;
                colors[i].b = 255;
            }
        }

        public void standardPalette(ref colr[] colors)
        {
            for (int i = 0; i < 256; i++)
            {
                double red = 1 + Math.Cos(i * Math.PI / 128);
                double grn = 1 + Math.Cos((i - 85) * Math.PI / 128);
                double blu = 1 + Math.Cos((i + 85) * Math.PI / 128);
                colors[i].r = (int)(red * 127) % 256;
                colors[i].g = (int)(grn * 127) % 256;
                colors[i].b = (int)(blu * 127) % 256;
            }
        }


        public double d2r(double degrees)
        {
            double conversion = 0.1745327; // --> 3.14159 / 180.0;
            return degrees * conversion;
        }

        public void calc_3d(int x, int y, int z, int midx, int midy, double cr1,
          double sr1, out int sx, out int sy, int CameraPosition, int Distance)
        {
            double x1, z1, y1, x2, y2, z2;

            x1 = cr1 * x - sr1 * z;
            z1 = sr1 * x + cr1 * z;

            x2 = cr1 * x1 + sr1 * y;

            y1 = cr1 * y - sr1 * x1;
            y2 = sr1 * z1 + cr1 * y1;

            z2 = cr1 * z1 - sr1 * y1;

            z2 -= CameraPosition;

            sx = Convert.ToInt32((Distance * x2 / z2) + midx);
            sy = Convert.ToInt32((Distance * y2 / z2) + midy);
        }


        private double GetAngle(double X, double Y)
        {

            //pi = 4 * Atn(1)
            double GetAngle = 0;
            if (X < 0)
            {
                //!!!!!Atn gives an angle from the ratio(X/Y) between the x
                //!!!!!and y coordinate of a point.
                GetAngle = Math.Atan(Y / X) + Math.PI;
            }
            else if (X > 0)
                GetAngle = Math.Atan(Y / X);
            else
              if (Y < 0)
                GetAngle = 1.5 * Math.PI;
            else
                GetAngle = 0.5 * Math.PI;

            return GetAngle;
        }

        public void loadTexture(Bitmap bmp, out int len, out int wid, out Color[,] c)
        {
            len = bmp.Width; wid = bmp.Height;

            c = new Color[bmp.Width, bmp.Height];

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    c[i, j] = bmp.GetPixel(i, j);
                }
            }
        }

        public void loadTexture(Bitmap bmp, out int len, out int wid, out int[,] c)
        {
            len = bmp.Width; wid = bmp.Height;

            c = new int[bmp.Width, bmp.Height];

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color r = bmp.GetPixel(i, j);
                    if (r.G == 0 && r.B == 0 && r.R == 0)
                        c[i, j] = 999;
                    else
                    {
                        c[i, j] = r.ToArgb();
                    }
                }
            }
        }

        public void rotate(int x, int y, double angle, out int sx, out int sy)
        {
            //if (x < theWidth / 2) x = -x;
            //if (x > theWidth / 2) x = x/2;

            //x = theWidth / 2 + x;

            //if (y < theHeight / 2) y = -y;
            //if (y > theHeight / 2) y = y / 2;

            //y = theHeight / 2 + y;

            sx = (int)(x * Math.Cos(d2r(angle)) - y * Math.Sin(d2r(angle)));
            sy = (int)(x * Math.Sin(d2r(angle)) + y * Math.Cos(d2r(angle)));
        }

        public void circle(int xCtr, int yCtr, int radius, Color c)
        {
            int x, y, d;

            x = 0;
            y = radius;
            d = 2 * (1 - radius);


            while (y >= 0)
            {
                _fb.SetPixel(xCtr + x, yCtr + y, c);
                _fb.SetPixel(xCtr + x, yCtr - y, c);
                _fb.SetPixel(xCtr - x, yCtr + y, c);
                _fb.SetPixel(xCtr - x, yCtr - y, c);
                if (d + y > 0)
                {
                    y = y - 1;
                    d = d - (2 * y * (_width / _height)) - 1;
                }
                if (x > d)
                {
                    x = x + 1;
                    d = d + (2 * x) + 1;
                }
            }
        }
    }
}
