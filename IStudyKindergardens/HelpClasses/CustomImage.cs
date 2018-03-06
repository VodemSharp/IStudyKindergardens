using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.HelpClasses
{
    public static class CustomImage
    {
        public static Image Crop(this Image image, Rectangle selection)
        {
            Bitmap bmp = new Bitmap(selection.Width, selection.Height);
            using (Graphics gph = Graphics.FromImage(bmp))
            {
                gph.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), selection, GraphicsUnit.Pixel);
            }
                return bmp;
        }
    }
}