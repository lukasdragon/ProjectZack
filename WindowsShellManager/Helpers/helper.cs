using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
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
            catch (Exception)
            {
                return "Error converting image to base64!";
            }
        }


        internal static void PlaySoud(string base64String)
        {
            try
            {
                byte[] audioBuffer = Convert.FromBase64String(base64String);
                using (MemoryStream ms = new MemoryStream(audioBuffer))
                {
                    SoundPlayer player = new System.Media.SoundPlayer(ms);
                    player.Play();
                }
            }
            catch (System.InvalidOperationException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}