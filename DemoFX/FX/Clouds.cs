using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    public class Clouds : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        Random rand;

        struct cols
        {
            public int r, g, b;
        }

        private cols[] colors;

        private int[,] scn;
        private int[] scnx;

        double[] aSin, aCos;//[512];

        private const double Whorls = 16.0;
        private const double rad = 3.14159 / 180;

        int iterate = 0;
        int w = 0;
        int h = 0;

        int xadj, yadj;

        public void init()
        {           
            rand = new Random();
            aSin = new double[512];
            aCos = new double[512];
            colors = new cols[256];

            w = theWidth;// - (theMarginWidth);
            h = theHeight;// - (theMarginHeight);

            xadj = w / 2;
            yadj = h / 2;

            scn = new int[theWidth, theHeight];
            scnx = new int[(theWidth * theHeight)];

            double rad;

            for (int i = 0; i < 512; i++)
            {
                rad = (i * 0.703125) * 0.0174532;
                aSin[i] = Math.Sin(rad) * 2024;
                aCos[i] = Math.Cos(rad) * 1024;
            }

            for (int i = 0; i < 256; i++)
            {
                double red = 1 + Math.Cos(i * Math.PI / 128);
                double grn = 1 + Math.Cos((i - 85) * Math.PI / 128);
                double blu = 1 + Math.Cos((i + 85) * Math.PI / 128);
                colors[i].r = (int)(red * 127) % 256;
                colors[i].g = (int)(grn * 127) % 256;
                colors[i].b = (int)(blu * 127) % 256;
            }

         
            for (int i = 1; i < theWidth; i++)
                for (int j = 1; j < theHeight; j++)
                    scn[i, j] = 0;

            for (int i = 0; i < theWidth * theHeight; i++)
                scnx[i] = 0;

        }


        public void doIt(string msg)
        {
           
            theGBMP.FillRectangle(Brushes.Black, 0, 0, theWidth, theHeight);
            theFB = startIt();

            if (iterate == 0)
                drawMap(0, 0, w-2, h-2);

            iterate++;

            for (int i = 0; i < theHeight; i++)
            {
                for (int j = 0; j < theWidth; j++)
                {
                    int x = (int)scn[j, i] & 255;
                    updateIt(j, i, Color.FromArgb(colors[x].r, colors[x].g, colors[x].b));

                }
            }            

            endIt();
            drawIt();

            int r = colors[0].r;
            int g = colors[0].g;
            int b = colors[0].b;
            for (int i = 1; i < 256; i++)
            {
                colors[i - 1].r = colors[i].r;
                colors[i - 1].g = colors[i].g;
                colors[i - 1].b = colors[i].b;
            }
            colors[255].r = r;
            colors[255].g = g;
            colors[255].b = b;
        }


        Color getNewColor(int c1, int c2, int dist)
        {
            int this_height;
            int random_displacement;
            random_displacement = (rand.Next(dist));// - dist) / 2);

            this_height = (c1 + c2 + random_displacement);

            this_height = this_height >> 1;

            return Color.FromArgb(this_height);

        }

        Color getNewColor4(int c1, int c2, int c3, int c4, int dist)
        {
            int this_height;
            int random_displacement;
            random_displacement = (rand.Next(dist));// - dist);// / 2);

            this_height = (c1 + c2 + c3 + c4 + random_displacement);

            this_height = this_height >> 2;

            return Color.FromArgb(this_height);

        }

        void drawMap(int x1, int y1, int x2, int y2)
        {
            int midx = (x1 + x2) >> 1;
            int midy = (y1 + y2) >> 1;

            Color c = getNewColor(getColor1(x1, y1), getColor1(x2, y1), x2 - x1);
            scn[midx, y1] = c.ToArgb();            
            updateIt(midx, y1, c);

            c = getNewColor(getColor1(x2, y1), getColor1(x2, y2), y2 - y1);
            scn[x2, midy] = c.ToArgb();           
            updateIt(x2, midy, c);

            c = getNewColor(getColor1(x1, y2), getColor1(x2, y2), x2 - x1);
            scn[midx, y2] = c.ToArgb();            
            updateIt(midx, y2, c);

            c = getNewColor(getColor1(x1, y1), getColor1(x1, y2), y2 - y1);           
            scn[x1, midy] = c.ToArgb();            
            updateIt(x1, midy, c);

            c = getNewColor4(getColor1(x1, midy), getColor1(x2, midy), getColor1(midx, y1), getColor1(midx, y2), y2 - y1);
            scn[midx, midy] = c.ToArgb();           
            updateIt(midx, midy, c);

            if (x2 > x1 + 1 || y2 > y1 + 1)
            {
                drawMap(x1, y1, midx, midy);
                drawMap(midx, y1, x2, midy);
                drawMap(midx, midy, x2, y2);
                drawMap(x1, midy, midx, y2);
            }
        }

        private int getColor1(int x, int y)
        {
            Color c = theFB.GetPixel(x, y);
            return c.ToArgb();// c.R + c.G + c.B;
        }
    }
}
