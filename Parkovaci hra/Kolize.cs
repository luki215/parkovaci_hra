using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;



/* TODO:
    Nějak zrefaktorovat - stačilo by najednou načíst jak hranice pro kontrolu cíle tak i kolizí... Z Formu by to mělo jít na jediné volání (I tak se tam používá to samé => sjednotit všechny funkce na checkování 
    Potřeba načíst barvu cíle a předat sem

    */








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
            static PotrebnyPixel[] hranice_auta;
            static int sirka_obrazku;

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

                obrazek = new PotrebnyPixel[rgbaValues.Length/4];
                sirka_obrazku = auto.Width;
               
                
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
                        pixel.pixel = new byte[4];
                        pixel.pixel[0] = rgbaValues[counter];
                        pixel.pixel[1] = rgbaValues[counter + 1];
                        pixel.pixel[2] = rgbaValues[counter + 2];
                        pixel.pixel[3] = rgbaValues[counter + 3];
                        pixel.potrebny = false;                 
                    }
                    else
                    {
                        pixel.pixel = new byte[4];
                        pixel.pixel[0] = rgbaValues[counter];
                        pixel.pixel[1] = rgbaValues[counter + 1];
                        pixel.pixel[2] = rgbaValues[counter + 2];
                        pixel.pixel[3] = rgbaValues[counter + 3];
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
                        //pokud se zmenil (tedy kolize)
                        
                        if(pixel.pixel[0] != rgbaValues[counter] ||
                           pixel.pixel[1] != rgbaValues[counter+1] ||
                           pixel.pixel[2] != rgbaValues[counter + 2])
                        {

                            //rgbaValues[counter] = 0;
                            //rgbaValues[counter+1] = 0;
                            //rgbaValues[counter+2] = 255;

                            vyrez_auta.UnlockBits(bmpData);
                            return true;
                        }

                    }
                }

                // Unlock the bits.
                //Marshal.Copy(rgbaValues, 0, ptr, numBytes);
                vyrez_auta.UnlockBits(bmpData);
                return false;
            }


            public static void NactiPotrebneBodyProOvereniCile()
            {
                hranice_auta = new PotrebnyPixel[obrazek.Length ];              
                for (int i = 0; i < obrazek.Length; i++)
                { 
                    //1px okraj obrázku => ignorovat
                    if (!(i % sirka_obrazku == 0 || i % sirka_obrazku == 1 || i % sirka_obrazku == 2|| i < sirka_obrazku*2 || ( i >= (obrazek.Length - sirka_obrazku * 2) -2) ))
                    {
                        if ( obrazek[i].potrebny == false && obrazek[i].pixel[3] == 0)
                        {
                            //je v okoli nejaky pixel z auta => chceme jej jako pixel k overeni jestli jsme v cili

                            for (int z = -2; z <= 2; z++)
                                for (int y = -2; y <= 2; y++)
                                    if (obrazek[i + z + y * sirka_obrazku].potrebny == true )
                                    {
                                        hranice_auta[i] = new PotrebnyPixel();
                                        hranice_auta[i].potrebny = true;
                                        hranice_auta[i].pixel = obrazek[i].pixel;
                                    }
                        }
                    }

                }
            }

            public static bool JsmeVCili(ref Bitmap vyrez_auta)
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
                bool jeVCili = true;
                for (int counter = 0; counter < hranice_auta.Length; counter ++)
                {
                    PotrebnyPixel pixel = hranice_auta[counter];

                    //pokud to byl pixel auta
                    if (pixel!= null && pixel.potrebny)
                    {
                        //pokud je barva cile

                        if (rgbaValues[counter*4] != 40 || rgbaValues[counter*4+1] != 173 || rgbaValues[counter*4+2] != 40 )
                        {
                            rgbaValues[counter * 4] = 40;
                            rgbaValues[counter * 4+1] = 173;
                            rgbaValues[counter * 4+2] = 40;
                            jeVCili = false;                            
                        }

                    }
                }
                // Unlock the bits.
                Marshal.Copy(rgbaValues, 0, ptr, numBytes);
                vyrez_auta.UnlockBits(bmpData);

                return (jeVCili) ? true : false;
                return false;
            }
        }
    }
}
