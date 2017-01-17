using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace JatanWebApp.Helpers
{
    /// <summary>
    /// Class to help with image manipulation.
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// Resizes an image and returns the resulting Bitmap object.
        /// </summary>
        /// <param name="inputImageStream">The stream containing the raw image data.</param>
        /// <param name="width">The new width of the image.</param>
        /// <param name="height">The new height of the image.</param>
        /// <param name="preserveAspectRatio">If true, retain the original aspect ratio of the image.</param>
        /// <param name="allowClipping">If true, the output width/height of the image will be exactly what was specified, but the image may be clipped.
        /// Otherwise, the image will be scaled so none of the image is clipped. However, the output width/height may be smaller than what was specified.
        /// This parameter is only used when preserveAspectRatio is true.</param>
        /// <returns>Returns a resized Bitmap object.</returns>
        public static Bitmap ResizeImage(Stream inputImageStream, int width, int height, bool preserveAspectRatio = false, bool allowClipping = false)
        {
            Image originalImage = Image.FromStream(inputImageStream);

            int newWidth;
            int newHeight;
            Rectangle clippingRect;

            if (preserveAspectRatio)
            {
                int originalWidth = originalImage.Width;
                int originalHeight = originalImage.Height;

                float aspectRatio = (float)originalWidth / originalHeight;

                if (allowClipping)
                {
                    if (originalHeight > originalWidth)
                    {
                        clippingRect = new Rectangle(0, 0, width, (int)(width / aspectRatio));
                    }
                    else if (originalWidth > originalHeight)
                    {
                        clippingRect = new Rectangle(0, 0, (int)(height * aspectRatio), height);
                    }
                    else
                    {
                        clippingRect = new Rectangle(0, 0, width, height);
                    }
                    newWidth = width;
                    newHeight = height;
                }
                else
                {
                    if (originalHeight > originalWidth)
                    {
                        newHeight = height;
                        newWidth = (int)(height * aspectRatio);
                    }
                    else if (originalWidth > originalHeight)
                    {
                        newHeight = (int)(width / aspectRatio);
                        newWidth = width;
                    }
                    else
                    {
                        newWidth = width;
                        newHeight = height;
                    }
                    clippingRect = new Rectangle(0, 0, newWidth, newHeight);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
                clippingRect = new Rectangle(0, 0, newWidth, newHeight);
            }

            Bitmap resizedBitmap = new Bitmap(newWidth, newHeight);

            Graphics gfx = Graphics.FromImage(resizedBitmap);
            gfx.CompositingQuality = CompositingQuality.HighQuality;
            gfx.SmoothingMode = SmoothingMode.HighQuality;
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            gfx.DrawImage(originalImage, clippingRect);

            originalImage.Dispose();
            gfx.Dispose();
            inputImageStream.Close();
            inputImageStream.Dispose();

            return resizedBitmap;
        }
    }
}