using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace JatanWebApp.Helpers
{
    public class ImageHelper
    {
        public static Bitmap ResizeImage(Stream inputImageStream, int width, int height, bool preserveAspectRatio)
        {
            Image originalImage = Image.FromStream(inputImageStream);

            int newWidth;
            int newHeight;

            if (preserveAspectRatio)
            {
                int originalWidth = originalImage.Width;
                int originalHeight = originalImage.Height;
                float percentWidth = (float)width / (float)originalWidth;
                float percentHeight = (float)height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                //Rounding to get the set width to the exact pixel
                newWidth = (float)originalWidth * percent < (float)width ? (int)(Math.Ceiling((float)originalWidth * percent)) : (int)((float)originalWidth * percent);
                //Rounding to get the set height to the exact pixel
                newHeight = (float)originalHeight * percent < (float)height ? (int)(Math.Ceiling((float)originalHeight * percent)) : (int)((float)originalHeight * percent);
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }

            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);

            Graphics gfx = Graphics.FromImage(resizedBitmap);
            gfx.CompositingQuality = CompositingQuality.HighQuality;
            gfx.SmoothingMode = SmoothingMode.HighQuality;
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle imageRectangle = new Rectangle(0, 0, width, height);
            gfx.DrawImage(originalImage, imageRectangle);

            originalImage.Dispose();
            gfx.Dispose();
            inputImageStream.Close();
            inputImageStream.Dispose();

            return resizedBitmap;
        }
    }
}