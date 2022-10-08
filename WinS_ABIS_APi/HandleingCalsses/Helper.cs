using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinS_ABIS_APi.HandleingCalsses
{
   internal class Helper
    {
        /// <summary>
        ///   Convert RAW 8-bit byte array to Image type
        /// </summary>
        /// <param name = "byteArray">RAW image in byte array format</param>
        /// <param name = "width">width of the raw image</param>
        /// <param name = "height">height of the raw image</param>
        /// <returns>converted image</returns>
        public static Image RawToBitmap(byte[] RawData, int nWidth, int nHeight, PixelFormat format)
        {
            try
            {

                Bitmap BitmapImage = new Bitmap(nWidth, nHeight, format);
                BitmapData BitmapImageData = BitmapImage.LockBits(new Rectangle(new Point(), BitmapImage.Size), ImageLockMode.WriteOnly, format);

                Marshal.Copy(RawData, 0, BitmapImageData.Scan0, RawData.Length);


                if (format == PixelFormat.Format8bppIndexed)
                {
                    ColorPalette palette = BitmapImage.Palette;

                    for (int i = 0; i < 256; i++)
                        palette.Entries[i] = Color.FromArgb(i, i, i);

                    BitmapImage.Palette = palette;
                }

                BitmapImage.UnlockBits(BitmapImageData);


                return BitmapImage;
            }
            catch (Exception)
            {
                Console.WriteLine("-----------------------------------------------------------------------Exception");
                return null;
            }
        }

        /// <summary>
        ///   Save a byte array to a file
        /// </summary>
        /// <param name = "directoryName">Name of the directory to save</param>
        /// <param name = "fileName">Name of file to save</param>
        /// <param name = "data"></param>
        /// <returns></returns>
        public static bool ByteArrayToFile(string directoryName, string fileName, byte[] data)
        {
            try
            {
                string completePath = Path.Combine(directoryName, fileName);

                //Create directory if not present
                Directory.CreateDirectory(directoryName);
                using (
                    FileStream fileStream = new FileStream(completePath, FileMode.Create,
                                                           FileAccess.Write))
                {
                    fileStream.Write(data, 0, data.Length);
                    fileStream.Close();
                }

                return true;
            }
            catch (Exception exception)
            {
                var ErrorReulstMessage = @"Failed to save file:" + fileName + Environment.NewLine + exception.Message + Constants.TITLE;
            }
            return false;
        }
    }
}
