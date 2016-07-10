using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Parkovaci_hra
{
    enum StavHry { start, napoveda, menu, bezi, kolize, cil}
    static class Hra
    {
        public static Bitmap frame;
        public static Auto.Auto a;
        public static Graphics g;
        public static Timer casovac;
        public static Form1 form;
        public static string cestaLevelu = "pomocne_soubory\\levely\\";
        public static Levely.SpravceLevelu levely = new Levely.SpravceLevelu();
        public static StavHry stav;
        public static Bitmap TlacitkaObr = new Bitmap("pomocne_soubory\\program\\ikonky.png");

        //zobrazi nam debug vyrez - kolizni + parkovaci body
        public static bool ShowDebug = false;

        public static void Nainicializuj(Bitmap frame, Form1 form)
        {
            Hra.frame = frame;
            Hra.form = form;

            Stavy.UdelejFormulareVsechStavu();

            casovac = new Timer();
            casovac.Interval = 10;
            casovac.Tick += Tick;

            g = Graphics.FromImage(Hra.frame);

            Stavy.ZmenStavHryNa(StavHry.start);
            
        }
        
        
        //vykreslovani bezici hry - ve stavu bezi
        private static void Tick(object sender, EventArgs e)
        {
               levely.VykresliPozadiLevelu();

                //ziskame pozici a obrazek auta
                Bitmap auto;
                Point pozice;
                a.Krok(out auto, out pozice);

                //vykreslime auto
                g.DrawImage(auto, pozice.X, pozice.Y);

                levely.VykresliPrekazkyLevelu();


                //zkontrolujeme jestli se neco stalo
                StavHry novyStav = Kolize.Kolize.VratStavHry(auto, pozice);
                Console.WriteLine(levely.aktualni_level);
                if (novyStav != StavHry.bezi)
                    Stavy.ZmenStavHryNa(novyStav);

                //vykreslime uzivateli
                if(stav == StavHry.bezi)
                    form.Invalidate();
         
        }
    }
    

}
