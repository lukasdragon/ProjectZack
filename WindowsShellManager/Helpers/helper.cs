using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsShellManager.Helpers
{
    class helper
    {
        internal static byte[] CombineByte(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        internal static string ImageToBase64(Image image)
        {
            try
            {
                var imageStream = new MemoryStream();
                image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Bmp);
                imageStream.Position = 0;
                var imageBytes = imageStream.ToArray();
                var ImageBase64 = Convert.ToBase64String(imageBytes);
                return ImageBase64;
            }
            catch (Exception ex)
            {
                return "Error converting image to base64!";
            }
        }

    }
}
