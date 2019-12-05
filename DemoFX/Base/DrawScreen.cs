using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace DemoFX.Base
{
    public class DrawScreen: BaseGraphics
    {

        private int _viewerWidth;

        public int theViewerWidth
        {
            get { return _viewerWidth; }
            set { _viewerWidth = value; }
        }

        private int _viewerHeight;

        public int theViewerHeight
        {
            get { return _viewerHeight; }
            set { _viewerHeight = value; }
        }

      
        FastBitmap fb;
       

        public DrawScreen()
        {
            FX.RotoZoom.drawIt += new FX.RotoZoom.drawHandler(drawScreen);
            FX.RotoZoom.startIt += new FX.RotoZoom.startHandler(startFB);
            FX.RotoZoom.endIt += new FX.RotoZoom.endHandler(stopFB);
            FX.RotoZoom.updateIt += new FX.RotoZoom.updateHandler(drawFB);

            FX.XOR.drawIt += new FX.XOR.drawHandler(drawScreen);
            FX.XOR.startIt += new FX.XOR.startHandler(startFB);
            FX.XOR.endIt += new FX.XOR.endHandler(stopFB);
            FX.XOR.updateIt += new FX.XOR.updateHandler(drawFB);

            FX.Landscape.drawIt += new FX.Landscape.drawHandler(drawScreen);
            FX.Landscape.startIt += new FX.Landscape.startHandler(startFB);
            FX.Landscape.endIt += new FX.Landscape.endHandler(stopFB);
            FX.Landscape.updateIt += new FX.Landscape.updateHandler(drawFB);

            FX.Tunnel.drawIt += new FX.Tunnel.drawHandler(drawScreen);
            FX.Tunnel.startIt += new FX.Tunnel.startHandler(startFB);
            FX.Tunnel.endIt += new FX.Tunnel.endHandler(stopFB);
            FX.Tunnel.updateIt += new FX.Tunnel.updateHandler(drawFB);

            FX.Boxen.drawIt += new FX.Boxen.drawHandler(drawScreen);
            FX.Boxen.startIt += new FX.Boxen.startHandler(startFB);
            FX.Boxen.endIt += new FX.Boxen.endHandler(stopFB);
            FX.Boxen.updateIt += new FX.Boxen.updateHandler(drawFB);

            FX.Clouds.drawIt += new FX.Clouds.drawHandler(drawScreen);
            FX.Clouds.startIt += new FX.Clouds.startHandler(startFB);
            FX.Clouds.endIt += new FX.Clouds.endHandler(stopFB);
            FX.Clouds.updateIt += new FX.Clouds.updateHandler(drawFB);

            FX.Fractal.drawIt += new FX.Fractal.drawHandler(drawScreen);
            FX.Fractal.startIt += new FX.Fractal.startHandler(startFB);
            FX.Fractal.endIt += new FX.Fractal.endHandler(stopFB);
            FX.Fractal.updateIt += new FX.Fractal.updateHandler(drawFB);

            FX.ShadeBobs.drawIt += new FX.ShadeBobs.drawHandler(drawScreen);
            FX.ShadeBobs.startIt += new FX.ShadeBobs.startHandler(startFB);
            FX.ShadeBobs.endIt += new FX.ShadeBobs.endHandler(stopFB);
            FX.ShadeBobs.updateIt += new FX.ShadeBobs.updateHandler(drawFB);

            FX.TriPlasma.drawIt += new FX.TriPlasma.drawHandler(drawScreen);
            FX.TriPlasma.startIt += new FX.TriPlasma.startHandler(startFB);
            FX.TriPlasma.endIt += new FX.TriPlasma.endHandler(stopFB);
            FX.TriPlasma.updateIt += new FX.TriPlasma.updateHandler(drawFB);

            FX.Bobs.drawIt += new FX.Bobs.drawHandler(drawScreen);
            FX.Bobs.startIt += new FX.Bobs.startHandler(startFB);
            FX.Bobs.endIt += new FX.Bobs.endHandler(stopFB);
            FX.Bobs.updateIt += new FX.Bobs.updateHandler(drawFB);

            FX.Voxels.drawIt += new FX.Voxels.drawHandler(drawScreen);
            FX.Voxels.startIt += new FX.Voxels.startHandler(startFB);
            FX.Voxels.endIt += new FX.Voxels.endHandler(stopFB);
            FX.Voxels.updateIt += new FX.Voxels.updateHandler(drawFB);

            FX.Floor.drawIt += new FX.Floor.drawHandler(drawScreen);
            FX.Floor.startIt += new FX.Floor.startHandler(startFB);
            FX.Floor.endIt += new FX.Floor.endHandler(stopFB);
            FX.Floor.updateIt += new FX.Floor.updateHandler(drawFB);
        }

        public FastBitmap startFB()
        {
            fb = new FastBitmap(theBMP);
            return fb;
        }

        public void stopFB()
        {
            fb.Release();
        }

        public void drawFB(int x, int y, Color co)
        {
            bool inWidth = x > theMarginWidth && x < theWidth - theMarginWidth;
            bool inHeight = y > theMarginHeight && y < theHeight - theMarginHeight;

            if (inWidth && inHeight)
                fb.SetPixel(x, y, co);
        }

        public void drawScreen()
        {
            try
            {
                Pen thePen = new Pen(Color.White);

                int x1 = theMarginWidth;
                int x2 = theWidth - theMarginWidth;
                int y1 = theMarginHeight;
                int y2 = theHeight - theMarginHeight;

                theGBMP.DrawLine(thePen, x1, y1, x2, y1);
                theGBMP.DrawLine(thePen, x1, y2, x2, y2);
                theGBMP.DrawLine(thePen, x1, y1, x1, y2);
                theGBMP.DrawLine(thePen, x2, y1, x2, y2);              

                theGForm.DrawImage(theBMP, 0, 0, theWidth, theHeight);
            }
            catch (Exception)
            {
                //throw;
            }
        }

    }
}
