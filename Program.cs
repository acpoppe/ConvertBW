using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace ConvertBW
{
    class Program
    {
        //Constants for Luminosity black and white algorithm
        private static double RWeight = 0.21;
        private static double GWeight = 0.72;
        private static double BWeight = 0.07;

        static void Main(string[] args)
        {
            Parallel.ForEach(args, MakeBlackAndWhite);
        }

        private static void MakeBlackAndWhite(string filePath)
        {
            Console.WriteLine("Working");
            using (Bitmap startingImage = Bitmap.FromFile(filePath, true) as Bitmap)
            {
                using (Bitmap outputImage = new Bitmap(startingImage.Width, startingImage.Height))
                {
                    // Lock the bits of the starting image
                    Rectangle rect = new Rectangle(0, 0, startingImage.Width, startingImage.Height);
                    System.Drawing.Imaging.BitmapData bmpData = startingImage.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                    // Get RGB values into an array
                    // Find memory address of first pixel data
                    IntPtr pointer = bmpData.Scan0;
                    // Declare the array to hold the RGB values
                    int numOfBytes = Math.Abs(bmpData.Stride) * startingImage.Height;
                    byte[] rgbValues = new byte[numOfBytes];
                    // Copy values into the array
                    Marshal.Copy(pointer, rgbValues, 0, numOfBytes);

                    for (int y = 0; y < startingImage.Height; y++)
                    {
                        for (int x = 0; x < startingImage.Width; x++)
                        {
                            //int XPixelLocation = x * 3;
                            //int YPixelLocation = y * 3;
                            int pixelLocation = y * startingImage.Width * 3 + x * 3;
                            int newPixel = (int)(rgbValues[pixelLocation] * RWeight + rgbValues[pixelLocation + 1] * GWeight + rgbValues[pixelLocation + 2] * BWeight);
                            outputImage.SetPixel(x, y, Color.FromArgb(newPixel, newPixel, newPixel));
                        }
                    }

                    outputImage.Save(string.Format("{0}/{1}-grayscale{2}", Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath), Path.GetExtension(filePath)), startingImage.RawFormat);
                }
            }
            Console.WriteLine("Completed");
            Console.ReadLine();
        }
    }
}