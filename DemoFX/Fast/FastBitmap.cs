using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

/// <summary>
/// FASTBITMAP class is by Matt Dibb, based on a MSDN article
/// by Eric Gunnerson where he showed how the “unsafe” 
/// technique can be used to make an image greyscale. 
/// The author modified his code into a general 
/// purpose class, so we can just drop this class in and 
/// use its faster GetPixel and SetPixel methods the way we 
/// would with the Bitmap class (which is way too slow).
/// If Microsoft intended us to write all this crap ourselves,
/// they wouldn't have provided such spiffy examples!
/// </summary>

namespace DemoFX
{

  public unsafe class FastBitmap
  {
    public struct PixelData
    {
      public byte blue;
      public byte green;
      public byte red;
    }

    Bitmap Subject;
    int SubjectWidth;
    BitmapData bitmapData = null;
    Byte* pBase = null;

    public FastBitmap(Bitmap SubjectBitmap)
    {
      this.Subject = SubjectBitmap;
      try
      {
        LockBitmap();
      }
      catch (Exception ex)
      { throw ex; }
    }

    public void Release()
    {
      try
      {
        UnlockBitmap();
      }
      catch
      {

      }
    }

    public Bitmap Bitmap
    {
      get
      {
        return Subject;
      }
    }

    public void SetPixel(int X, int Y, Color Colour)
    {
      try
      {
        PixelData* p = PixelAt(X, Y);
        p->red = Colour.R;
        p->green = Colour.G;
        p->blue = Colour.B;
      }
      catch (AccessViolationException)
      {
        //throw ; 
      }
      catch (Exception)
      {
        // throw; 
      }
    }

    public Color GetPixel(int X, int Y)
    {
      try
      {
        PixelData* p = PixelAt(X, Y);
        return Color.FromArgb((int)p->red, (int)p->green, (int)p->blue);
      }
      catch (AccessViolationException ave)
      {
        throw (ave);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private void LockBitmap()
    {
      try
      {
        GraphicsUnit unit = GraphicsUnit.Pixel;
        RectangleF boundsF = Subject.GetBounds(ref unit);
        Rectangle bounds = new Rectangle((int)boundsF.X, (int)boundsF.Y, (int)boundsF.Width, (int)boundsF.Height);
        SubjectWidth = (int)boundsF.Width * sizeof(PixelData);
        if (SubjectWidth % 4 != 0)
        {
          SubjectWidth = 4 * (SubjectWidth / 4 + 1);
        }
        bitmapData = Subject.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
        pBase = (Byte*)bitmapData.Scan0.ToPointer();
      }
      catch (Exception)
      {

        throw;
      }
    }

    private PixelData* PixelAt(int x, int y)
    {
      return (PixelData*)(pBase + y * SubjectWidth + x * sizeof(PixelData));
    }

    private void UnlockBitmap()
    {
      Subject.UnlockBits(bitmapData);
      bitmapData = null;
      pBase = null;
    }
  }
}


