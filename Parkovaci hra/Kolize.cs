using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Parkovaci_hra
{
    namespace Kolize
    {

        class PotrebnyPixel
        {
            public byte []pixel;
            public bool potrebny;
        }

        class Kolize
        {
            static PotrebnyPixel []obrazek;
            static bool nainicializovany_obrazek = false;

            public static void NactiAuto(Bitmap auto)
            {
                // Specify a pixel format.
                PixelFormat pxf = PixelFormat.Format32bppArgb;

                // Lock the bitmap's bits.
                Rectangle rect = new Rectangle(0, 0, auto.Width, auto.Height);
                BitmapData bmpData =
                auto.LockBits(rect, ImageLockMode.ReadWrite,
                             pxf);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap. 
                // int numBytes = bmp.Width * bmp.Height * 3; 
                int numBytes = bmpData.Stride * auto.Height;
                byte[] rgbaValues = new byte[numBytes];

                if (!nainicializovany_obrazek)
                {
                    obrazek = new PotrebnyPixel[rgbaValues.Length];

                    nainicializovany_obrazek = true;
                }
                
                // Copy the RGB values into the array.
                Marshal.Copy(ptr, rgbaValues, 0, numBytes);

                /* prochazime byty pixelů 
                    0. = Modrá
                    1. = Zelená
                    2. = Červená
                    3. = alpha
                */    
                for (int counter = 0; counter < rgbaValues.Length; counter += 4)
                {
                    PotrebnyPixel pixel = new PotrebnyPixel();

                    //pokud je pruhledny, jedna se o okoli auta => nepotrebny
                    if (rgbaValues[counter + 3] != 255)
                    {
                        pixel.potrebny = false;                 
                    }
                    else
                    {
                        pixel.pixel = new byte[3];
                        pixel.pixel[0] = rgbaValues[counter];
                        pixel.pixel[1] = rgbaValues[counter + 1];
                        pixel.pixel[2] = rgbaValues[counter + 2];
                        pixel.potrebny = true;
                    }
                    obrazek[counter / 4 ] = pixel;
                }                    
               
                // Unlock the bits.
                auto.UnlockBits(bmpData);
                
            }


            public static bool BylaKolize(ref Bitmap vyrez_auta)
            {

                // Specify a pixel format.
                PixelFormat pxf = PixelFormat.Format32bppArgb;

                // Lock the bitmap's bits.
                Rectangle rect = new Rectangle(0, 0, vyrez_auta.Width, vyrez_auta.Height);
                BitmapData bmpData =
                vyrez_auta.LockBits(rect, ImageLockMode.ReadWrite,
                             pxf);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap. 
                // int numBytes = bmp.Width * bmp.Height * 3; 
                int numBytes = bmpData.Stride * vyrez_auta.Height;
                byte[] rgbaValues = new byte[numBytes];

                

                // Copy the RGB values into the array.
                Marshal.Copy(ptr, rgbaValues, 0, numBytes);

                /* prochazime byty pixelů 
                    0. = Modrá
                    1. = Zelená
                    2. = Červená
                    3. = alpha
                */
                for (int counter = 0; counter < rgbaValues.Length; counter += 4)
                {
                    PotrebnyPixel pixel = obrazek[counter/4];

                       
                       
                    
                    //pokud to byl pixel auta
                    if (pixel.potrebny)
                    {

                        PotrebnyPixel[] tolerancni_pole = new PotrebnyPixel[9];
                        /* tolerancni pole je okoli 1 bodu pixelu, resp:
                        
                            aaa
                            axa
                            aaa

                            kde x je chteny pixel
                       */
                        int z = 0;
                        for (int i = -1; i <= 1; i++)
                            for (int y = -1; y <= 1; y++)
                            {
                                int index = (counter / 4 + i * vyrez_auta.Width + y);
                                if (!(index < 0 || index > obrazek.Length))
                                {
                                    if (obrazek[index].potrebny &&(
                                         pixel.pixel[0] == obrazek[index].pixel[0] &&
                                         pixel.pixel[1] == obrazek[index].pixel[1] &&
                                         pixel.pixel[2] == obrazek[index].pixel[2]))
                                    {
                                        vyrez_auta.UnlockBits(bmpData);
                                        return false;

                                       // rgbaValues[counter] = 0;
                                       // rgbaValues[counter + 1] = 0;
                                       // rgbaValues[counter + 2] = 255;
                                    }





                                }
                                z++;
                            }
                                            
                    }
                }

                // Unlock the bits.
                //Marshal.Copy(rgbaValues, 0, ptr, numBytes);
                vyrez_auta.UnlockBits(bmpData);
                return true;
            }

        }
    }
}
