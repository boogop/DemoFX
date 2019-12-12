using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.FX
{
    class Rain : Base.BaseGraphics, Base.iDemo
    {
        public delegate FastBitmap startHandler();
        public static event startHandler startIt;

        public delegate void endHandler();
        public static event endHandler endIt;

        public delegate void updateHandler(int x, int y, Color c);
        public static event updateHandler updateIt;

        public delegate void drawHandler();
        public static event drawHandler drawIt;

        private bool _ripplesPresent;
        private Random RandomClass;
        private int[,,] waveMap;
        private int[,] rawPic;
        private int _raindropHeight, _raindropWidth;
        private int RaindropCount = 0;
        private int radius = 60;
        private double damp = .09;
        int blen, bwid;
        int dropCount = 0;


        public void init()
        {
            RandomClass = new Random();
            loadTexture(DemoFX.Properties.Resources.zowee, out blen, out bwid, out rawPic);
            _raindropWidth = theWidth;
            _raindropHeight = theHeight;
            waveMap = new int[_raindropWidth, _raindropHeight, 2];

            StartRaindrop();
        }

        public void doIt(string msg)
        {
            dropCount++;
            if (dropCount % 20 == 0)
                StartRaindrop();

            theFB = startIt();

            ProcessWaves();
            DrawRain();

            endIt();
            drawIt();

        }

        private void StartRaindrop()
        {
            int RandomX = RandomClass.Next(10, _raindropWidth - 10);
            int RandomY = RandomClass.Next(10, _raindropHeight - 10);
            int RandomHeight = RandomClass.Next(200, 600);

            PutDrop(RandomX, RandomY, RandomHeight);
        }


        private void PutDrop(int x, int y, int height)
        {
            // calculate the initial settings for a raindrop
            _ripplesPresent = true;          
            double dist;
            int tmpX;
            int tmpY;
            int foo = _raindropWidth - 1;
            bool okayX;

            for (int i = -radius; i < radius; i++)
            {
                tmpX = x + i;
                okayX = tmpX >= 0 && tmpX < foo;

                for (int j = -radius; j < radius; j++)
                {
                    tmpY = y + j;

                    // make sure we're not putting a drop off the edge of the bitmap
                    if (okayX)
                    {
                        if (tmpY >= 0 && tmpY < foo)
                        {
                            // pythagorean formula for longest side of a triangle so we can calculate a series of rings around a central point(x,y)
                            dist = Math.Sqrt(i * i + j * j);

                            // this equation can take Sin or Cos to produce a similar circular effect.                        
                            int xpos = x + i;
                            int ypos = y + j;
                            if (xpos < _raindropWidth && ypos < _raindropHeight)
                            {
                                if (dist < radius)
                                    waveMap[x + i, y + j, RaindropCount] = (int)(Math.Cos(dist * Math.PI / radius) * height);
                            }
                        }
                    }
                }

            }
        }

        private void DrawRain()
        {
            int x, y, xOffset, yOffset, waveX, waveY;

            if (_ripplesPresent)
            {
                for (x = 1; x < _raindropWidth - 1; x++)
                {
                    waveX = x;

                    for (y = 1; y < _raindropHeight - 1; y++)
                    {
                        // take the circular data in waveMap and expand it outward
                        waveY = y;
                        xOffset = (waveMap[waveX - 1, waveY, RaindropCount] - waveMap[waveX + 1, waveY, RaindropCount]) >> 3;
                        yOffset = (waveMap[waveX, waveY - 1, RaindropCount] - waveMap[waveX, waveY + 1, RaindropCount]) >> 3;

                        int xpos = Math.Abs(x + xOffset);
                        int ypos = Math.Abs(y + yOffset);

                        if (xpos > blen - 1) continue;
                        if (ypos > bwid - 1) continue;


                        updateIt(x, y, Color.FromArgb(rawPic[xpos, ypos]));

                    }
                }
            }
        }

        private void ProcessWaves()
        {
            // each time through we calculate an increasing radius and stick the new coordinates in the raindrop array
            bool wavesFound = false;
            int x, x1, x2;
            int y, y1, y2;
            int newBuffer = 0;

            if (RaindropCount == 0)
                newBuffer = 1;
            else
                newBuffer = 0;

            for (x = 1; x < _raindropWidth - 1; x++)
            {
                x1 = x - 1;
                x2 = x + 1;

                for (y = 1; y < _raindropHeight - 1; y++)
                {
                    y1 = y - 1;
                    y2 = y + 1;

                    // the heart of what this thing does. To produce magnification effects you sample 8 pixels surrounding the index pixel color, the index being 
                    // wherever the leading edge of our ripple is. There are a few different ways to get a similar effect but the 8-pixel method looks best
                    waveMap[x, y, newBuffer] = (int)((waveMap[x1, y1, RaindropCount] + waveMap[x, y1, RaindropCount] +
                    waveMap[x2, y1, RaindropCount] + waveMap[x1, y, RaindropCount] + waveMap[x2, y, RaindropCount] +
                    waveMap[x1, y2, RaindropCount] + waveMap[x, y2, RaindropCount] + waveMap[x2, y2, RaindropCount] >> 2) -
                    waveMap[x, y, newBuffer]);

                    if (waveMap[x, y, newBuffer] != 0)
                    {
                        // if you don't shift by the dampening factor the ripple will never decrease in height                        
                        waveMap[x, y, newBuffer] -= (int)(waveMap[x, y, newBuffer] * damp);

                        wavesFound = true;
                    }
                }
            }

            _ripplesPresent = wavesFound;
            RaindropCount = newBuffer;

        }


    }
}
