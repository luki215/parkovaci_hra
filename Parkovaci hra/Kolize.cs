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

        class PraceSObrazkem
        {
            Bitmap obrazek;
            BitmapData bmpData;
            IntPtr ptr;
            int numBytes;

            public byte[] ZalockujObrazekAVratPoleBytu(ref Bitmap obrazek)
            {
                this.obrazek = obrazek;
                // Specify a pixel format.
                PixelFormat pxf = PixelFormat.Format32bppArgb;
                
                // Lock the bitmap's bits.
                Rectangle rect = new Rectangle(0, 0, obrazek.Width, obrazek.Height);
                bmpData =
                obrazek.LockBits(rect, ImageLockMode.ReadWrite,
                             pxf);

                // Get the address of the first line.
               ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap. 
                // int numBytes = bmp.Width * bmp.Height * 3; 
                numBytes = bmpData.Stride * obrazek.Height;
                byte[] rgbaValues = new byte[numBytes];

                // Copy the RGB values into the array.
                Marshal.Copy(ptr, rgbaValues, 0, numBytes);

                return rgbaValues;
            }
      
            /// <summary>
            /// predela nam obrazek tak, jak mame pole v parametru.. Pokud neni zadane, pouze odlockuje obrazek 
            /// </summary>
            /// <param name="rgbaValues"></param>
            public void OdlockujAVratUpravenyObrazek(byte[] rgbaValues = null)
            {
                if(rgbaValues!=null)
                    Marshal.Copy(rgbaValues, 0, ptr, numBytes);
                // Unlock the bits.
                obrazek.UnlockBits(bmpData);
               
            }

            public static Bitmap VratVyrezAUpravAutoPodleNej(ref Bitmap auto, Point pozice)
            {
                //porovnavame puvodni auto s pruhlednym pozadim s vyrezem auta ve hre a zjistujeme kolize/cil
                //co kdyz je auto mimo obraz

                Point levy_horni = new Point(0, 0);
                Size pravy_dolni = auto.Size;
                if (pozice.X < 0)
                {
                    levy_horni.X = -pozice.X;
                    pravy_dolni.Width += pozice.X;
                }
                if (pozice.Y < 0)
                {
                    levy_horni.Y = -pozice.Y;
                    pravy_dolni.Height += pozice.Y;
                }
                if (pozice.X + auto.Width > Hra.frame.Width)
                    pravy_dolni.Width = -pozice.X + Hra.frame.Width;
                if (pozice.Y + auto.Height > 600)
                    pravy_dolni.Height = -pozice.Y + Hra.frame.Height;

                auto = auto.Clone(new Rectangle(levy_horni, pravy_dolni), System.Drawing.Imaging.PixelFormat.DontCare);

                //dostaneme vyrez bitmapy s autem
                Bitmap vyrez = Hra.frame.Clone(new Rectangle(levy_horni.X + pozice.X, levy_horni.Y + pozice.Y, pravy_dolni.Width, pravy_dolni.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                return vyrez;

            }
        }

        class Kolize
        {
            static PotrebnyPixel []obrazek;
            static PotrebnyPixel[] hranice_auta;
            static int sirka_obrazku;
            static int vyska_obrazku;
            public static void NactiAuto(Bitmap auto)
            {
                PraceSObrazkem auto_pracovni_obr = new PraceSObrazkem();

                byte[] rgbaValues = auto_pracovni_obr.ZalockujObrazekAVratPoleBytu(ref auto);

                obrazek = new PotrebnyPixel[rgbaValues.Length/4];
                hranice_auta = new PotrebnyPixel[rgbaValues.Length/4];
                sirka_obrazku = auto.Width;
                vyska_obrazku = auto.Height;
                

                bool horizontalni_pixelove_pocitadlo = false;
                bool []vertikalni_pixely = new bool[sirka_obrazku];


                /* prochazime byty pixelů 
                    0. = Modrá
                    1. = Zelená
                    2. = Červená
                    3. = alpha
                */
                for (int counter = 0; counter < rgbaValues.Length; counter += 4)
                {
                    /* zjisteni vnitrnich bodu auta - ke zjisteni jestli jsme nabourali */
                    PotrebnyPixel pixel = new PotrebnyPixel();
                    pixel.pixel = new byte[4];
                    pixel.pixel[0] = rgbaValues[counter];
                    pixel.pixel[1] = rgbaValues[counter + 1];
                    pixel.pixel[2] = rgbaValues[counter + 2];
                    pixel.pixel[3] = rgbaValues[counter + 3];
                    //pokud je pruhledny, jedna se o okoli auta => nepotrebny
                    pixel.potrebny = (rgbaValues[counter + 3] != 255) ? false : true; 
                    
                    obrazek[counter / 4 ] = pixel;



                    /* zjisteni okrajovych bodu - ke zjisteni jestli jsme v cili */
                    //ignorujeme prvni radek
                    if (counter / 4 - 1 > sirka_obrazku)
                    {
                        //prvni nepruhledny bod horizontalne
                        if (rgbaValues[counter + 3] != 0 && !horizontalni_pixelove_pocitadlo)
                        {
                            hranice_auta[counter / 4 - 1] = new PotrebnyPixel();
                            hranice_auta[counter / 4 - 1].potrebny = true;
                            horizontalni_pixelove_pocitadlo = true;
                        }
                        //po nepruhlednem prvni pruhledny bod horizontalne
                        if (horizontalni_pixelove_pocitadlo && rgbaValues[counter + 3] == 0)
                        {
                            hranice_auta[counter / 4] = new PotrebnyPixel();
                            hranice_auta[counter / 4].potrebny = true;
                            horizontalni_pixelove_pocitadlo = false;
                        }

                        //prvni nepruhledny bod vertikalne
                        if (rgbaValues[counter + 3] != 0 && !vertikalni_pixely[(counter / 4) % sirka_obrazku])
                        {
                            hranice_auta[counter / 4 - sirka_obrazku] = new PotrebnyPixel();
                            hranice_auta[counter / 4 - sirka_obrazku].potrebny = true;
                            vertikalni_pixely[(counter / 4) % sirka_obrazku] = true;
                        }
                        //prvni pruhledny bod po nepruhlednem vertikalne
                        if (vertikalni_pixely[(counter / 4) % sirka_obrazku] && rgbaValues[counter + 3] == 0)
                        {
                            hranice_auta[counter / 4] = new PotrebnyPixel();
                            hranice_auta[counter / 4].potrebny = true;
                            vertikalni_pixely[(counter / 4) % sirka_obrazku] = false;
                        }

                    }
                }


            }
            
            public static StavHry VratStavHry(Bitmap auto, Point pozice_auta)
            {
                Bitmap vyrez = PraceSObrazkem.VratVyrezAUpravAutoPodleNej(ref auto, pozice_auta);
                
                NactiAuto(auto);
                
                PraceSObrazkem pracovni_vyrez = new PraceSObrazkem();
                byte[] vyrez_v_poli_bytu = pracovni_vyrez.ZalockujObrazekAVratPoleBytu(ref vyrez);
                 
                bool kolize = BylaKolize(ref vyrez_v_poli_bytu);
                bool cil = JsmeVCili(ref vyrez_v_poli_bytu);

                //Vykreslovani koliznich a cilovych bodu
                if (Hra.ShowDebug)
                {
                    pracovni_vyrez.OdlockujAVratUpravenyObrazek(vyrez_v_poli_bytu);
                    Hra.g.DrawImage(vyrez, Hra.frame.Width-vyrez.Width-20, 20);
                    Hra.g.DrawRectangle(Pens.White, Hra.frame.Width - vyrez.Width - 20, 20, vyrez.Width, vyrez.Height);
                }
                else
                {
                    pracovni_vyrez.OdlockujAVratUpravenyObrazek();
                }
                if (kolize) return StavHry.kolize;
                if (cil) return StavHry.cil;

                return StavHry.bezi;
            }

            public static bool BylaKolize(ref byte[] rgbaValues)
            {                               
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
                            if (Hra.ShowDebug)
                            {
                                rgbaValues[counter] = 0;
                                rgbaValues[counter + 1] = 0;
                                rgbaValues[counter + 2] = 255;
                            }
                            else {
                                return true;
                            }
                        }

                    }
                }
                return false;
            }


            public static bool JsmeVCili(ref byte[] rgbaValues)
            {
                
                /* prochazime byty pixelů 
                    0. = Modrá
                    1. = Zelená
                    2. = Červená
                    3. = alpha
                */
                bool jeVCili = true;
                for (int counter = 0; counter < hranice_auta.Length; counter ++)
                {
                    //pokud to byl pixel auta
                    if (hranice_auta[counter] != null && hranice_auta[counter].potrebny)
                    {
                        //pokud je barva cile

                        if (rgbaValues[counter*4] != 40 || rgbaValues[counter*4+1] != 173 || rgbaValues[counter*4+2] != 40 )
                        {
                            //vykresleni kontrolnich cilovych bodu
                            if (Hra.ShowDebug)
                            {
                                rgbaValues[counter * 4] = 40;
                                rgbaValues[counter * 4 + 1] = 173;
                                rgbaValues[counter * 4 + 2] = 40;
                            }
                            jeVCili = false;                            
                        }

                    }
                }
                
                return (jeVCili) ? true : false;
            }
        }
    }
}
