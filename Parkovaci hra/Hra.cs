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
        public static bool ShowDebug = true;
        public static void Nainicializuj(Bitmap frame, Form1 form)
        {
            Hra.frame = frame;
            Hra.form = form;


            g = Graphics.FromImage(Hra.frame);

            a = new Auto.Auto("Auto.txt", g);


            casovac = new Timer();
            casovac.Interval = 10;
            casovac.Tick += Tick;
            casovac.Enabled = true;
        }
        
        private static void Tick(object sender, EventArgs e)
        {
            //vykreslime mapu
            g.DrawImage(new Bitmap("obrazky\\mapy\\MAPA1.png"), 0, 0);


            //ziskame pozici a obrazek auta
            Bitmap auto;
            Point pozice;
            a.Krok(out auto, out pozice);

            //vykreslime auto
            g.DrawImage(auto, pozice.X, pozice.Y);


            //vykreslime prekazky
            g.DrawImage(new Bitmap("obrazky\\mapy\\prekazky1.png"), 0, 0);


            Console.WriteLine(  Kolize.Kolize.VratStavHry(auto, pozice) );

            form.Invalidate();
        }

    }
    
}
