using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.HelpClasses
{
    public static class CustomImage
    {
        public static void Crop(Image image, Rectangle selection, string fileName, HttpServerUtilityBase server)
        {
            Image newImage = CropInner(image, selection);
            newImage.Save(server.MapPath("~/Images/Uploaded/Source/" + fileName));
            image.Dispose();
        }

        private static Image CropInner(this Image image, Rectangle selection)
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