using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NullFX.Win32;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Parkovaci_hra
{
    namespace Auto
    {


        class StisknuteSipky
        {
            //pole, v pořadí hore, dole, vlevo, vpravo
            public static bool[] sipky = new bool[4];

            public static void Zjisti()
            {
                /* hack klávesnice */
                var left = KeyboardInfo.GetKeyState(Keys.Left);
                var right = KeyboardInfo.GetKeyState(Keys.Right);
                var up = KeyboardInfo.GetKeyState(Keys.Up);
                var down = KeyboardInfo.GetKeyState(Keys.Down);


                StisknuteSipky.sipky[0] = (up.IsPressed) ? true : false;
                StisknuteSipky.sipky[1] = (down.IsPressed) ? true : false;
                StisknuteSipky.sipky[2] = (left.IsPressed) ? true : false;
                StisknuteSipky.sipky[3] = (right.IsPressed) ? true : false;
                if (sipky[0] && sipky[1])
                    sipky[0] = sipky[1] = false;
                if (sipky[2] && sipky[3])
                    sipky[2] = sipky[3] = false;


            }
        }

        class Vlastnosti
        {
            public int pridani_plynu_v_kazdem_kroku;
            public int maximalni_plyn;
            public int minimalni_plyn;
            public int max_otoceni;
            public int rychlost_toceni;
            
        }
        class Stav
        {
            //nabyva -10000 - 10000
            private int _plyn = 0;
            public int Plyn
            {
                get { return _plyn; }
                set {
                    if (value < -10000)
                        _plyn = -10000;
                    else if (value > 10000)
                        _plyn = 10000;
                    else
                        _plyn = value;
                }
            }
            //uhel natoceni kol: -90 - +90
            private int _uhel_kol = 0;
            public int UhelKol {
                get { return _uhel_kol; }
                set
                {
                    if (value < -90)
                        _uhel_kol = -90;
                    else if (value > 90)
                        _uhel_kol= 90;
                    else
                        _uhel_kol = value;                    
                }
            }

            public PointF pozice;
            private float _uhel_natoceni = 0;
            public float UhelNatoceni {
                get { return _uhel_natoceni; }
                set { _uhel_natoceni = value % 360; }
            }      
        }
        class Auto
        {
            private Graphics g;
            private Bitmap obrazek;

            public Vlastnosti vlastnosti = new Vlastnosti();
            public Stav stav = new Stav();


            public Auto(string cesta, PointF pozice_auta, int uhel_natoceni)
            {
                NactiSe("pomocne_soubory\\auta\\" + cesta);
                stav.UhelNatoceni = uhel_natoceni;
                stav.pozice = pozice_auta;
                this.g = Hra.g;
            }
            public void NactiSe(string cesta)
            {
                using (StreamReader sr = new StreamReader(cesta))
                {
                    String cteni = "pomocne_soubory\\auta\\" + sr.ReadLine().Split(':')[1].Substring(1);
                    obrazek = new Bitmap(cteni);

                    vlastnosti.pridani_plynu_v_kazdem_kroku = Int32.Parse( sr.ReadLine().Split(':')[1].Substring(1) );
                    vlastnosti.maximalni_plyn = Int32.Parse(sr.ReadLine().Split(':')[1].Substring(1));
                    vlastnosti.minimalni_plyn = Int32.Parse(sr.ReadLine().Split(':')[1].Substring(1));
                    vlastnosti.max_otoceni = Int32.Parse(sr.ReadLine().Split(':')[1].Substring(1));
                    vlastnosti.rychlost_toceni = Int32.Parse(sr.ReadLine().Split(':')[1].Substring(1));
                    Console.WriteLine(vlastnosti.ToString());
                }
                

            }
            public void SpoctiPlyn()
            {
                //at nam plyn roste dle kvadraticky
                int hodnota_plynu = stav.Plyn;
                int pridavek = vlastnosti.pridani_plynu_v_kazdem_kroku;
                int max_plyn = vlastnosti.maximalni_plyn;
                int min_plyn = vlastnosti.minimalni_plyn;
                StisknuteSipky.Zjisti();

                //pridavame
                if (StisknuteSipky.sipky[0])
                {
                    hodnota_plynu += pridavek;
                }
                //ubirame
                else if (StisknuteSipky.sipky[1])
                {
                    hodnota_plynu -= pridavek;
                }
                //volnobeh
                else
                {
                    hodnota_plynu -= Math.Sign(hodnota_plynu) * pridavek * 2;
                    if (Math.Abs(hodnota_plynu) <= pridavek * 2)
                        hodnota_plynu = 0;
                }

                //osetreni maxima
                if (hodnota_plynu > max_plyn)
                    hodnota_plynu = max_plyn;
                //osetreni minima
                if (hodnota_plynu < min_plyn)
                    hodnota_plynu = min_plyn;

                stav.Plyn = hodnota_plynu;

            }
            public void SpoctiUhelKol()
            {
                StisknuteSipky.Zjisti();
                //toci vlevo
                int toceni = vlastnosti.rychlost_toceni;
                if (StisknuteSipky.sipky[2])
                {
                    stav.UhelKol -= toceni;
                    if (stav.UhelKol < -vlastnosti.max_otoceni)
                        stav.UhelKol = -vlastnosti.max_otoceni; 
                }
                else if (StisknuteSipky.sipky[3])
                {
                    stav.UhelKol += toceni;
                    if (stav.UhelKol > vlastnosti.max_otoceni)
                        stav.UhelKol = vlastnosti.max_otoceni;
                }
                else
                {
                    stav.UhelKol = Math.Sign(stav.UhelKol) * (Math.Abs(stav.UhelKol) - toceni);
                    if (Math.Abs(stav.UhelKol) <= toceni)
                        stav.UhelKol = 0;
                       
                }
            }
            public void SpoctiUhelAuta()
            {
                stav.UhelNatoceni += ((float)stav.UhelKol / 90) * 4 * ((float)stav.Plyn / vlastnosti.maximalni_plyn);
            }
            public Bitmap VratOtoceneAuto(ref Size posunObrazku)
            {
                int max = Math.Max(obrazek.Width, obrazek.Height);

                //create an empty Bitmap image
                Bitmap bmp = new Bitmap(max * 3, max * 3);

                //turn the Bitmap into a Graphics object
                Graphics gfx = Graphics.FromImage(bmp);

                //now we set the rotation point to the center of our image
                gfx.TranslateTransform((float)bmp.Width / 2 , (float)bmp.Height / 2);

                //upraveni bodu stredu rotace podle toho kam zatacime + kde ma kola
                float stredOtaceniX = 0;
                float stredOtaceniY = obrazek.Height / 2 -10;
                
               
                gfx.TranslateTransform(stredOtaceniX, stredOtaceniY);
                
                
                    
                //now rotate the image
                gfx.RotateTransform(stav.UhelNatoceni);

                gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2 );
                
                // vraceni bodu stredu rotace podle toho kam zatacime + kde ma kola
                gfx.TranslateTransform(-stredOtaceniX, -stredOtaceniY);
                
                
                //set the InterpolationMode to HighQualityBicubic so to ensure a high
                //quality image once it is transformed to the specified size
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //now draw our new image onto the graphics object
               
                posunObrazku = new Size(-(3 * max) / 2 + obrazek.Size.Width / 2, -(3 * max) / 2 + obrazek.Size.Width / 2);
                gfx.DrawImage(obrazek, new Point((3 * max) / 2 - obrazek.Size.Width / 2, (3 * max) / 2 - obrazek.Size.Height / 2));

                //dispose of our Graphics object
                gfx.Dispose();

                //return the image
                return bmp;
        }

            public void Krok(out Bitmap obrazek_auta, out Point pozice)
            {
                SpoctiPlyn();
                SpoctiUhelKol();
                SpoctiUhelAuta();
                /* nyni mam v stav.Plyn hodnotu plynu
                            v stav.stav.UhelKol jak jsou natocene kola
                            */

                Bitmap rotatedBmp;
                Size posunObrazku = new Size() ;
                rotatedBmp = VratOtoceneAuto(ref posunObrazku);

                
                
                //stav.Plyn/1000 = delka vektoru pohybu
                //stav.UhelNatoceni = uhel vektoru pohybu;

                stav.pozice.X += (float) (Math.Sin((Math.PI / 180) * stav.UhelNatoceni) * (stav.Plyn / 1000) ); 
                stav.pozice.Y -= (float) (Math.Cos((Math.PI / 180) * stav.UhelNatoceni) * (stav.Plyn / 1000) );

                

                //return
                obrazek_auta = rotatedBmp;
                pozice = Point.Add( new Point((int) stav.pozice.X , (int) stav.pozice.Y), posunObrazku);




            }
        }

   }

}
