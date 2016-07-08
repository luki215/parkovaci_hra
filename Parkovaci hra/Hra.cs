﻿using System;
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
        public static bool ShowDebug = false;
        public static string cestaLevelu = "pomocne_soubory\\levely\\";
        public static Levely.SpravceLevelu levely = new Levely.SpravceLevelu();
        public static StavHry stav;
        public static Bitmap TlacitkaObr = new Bitmap("pomocne_soubory\\program\\ikonky.png");
        public static void Nainicializuj(Bitmap frame, Form1 form)
        {
            Hra.frame = frame;
            Hra.form = form;

            Stavy.UdelejFormulareVsechStavu();

            casovac = new Timer();
            casovac.Interval = 10;
            casovac.Tick += Tick;

            g = Graphics.FromImage(Hra.frame);

            ZmenStavHryNa(StavHry.start);
            




        }
        



        public static void ZmenStavHryNa(StavHry stav, int cislo_levelu=0)
        {
            //uklidime minuly stav
            switch (Hra.stav)
            {
                case StavHry.start: Stavy.start.Hide();
                    break;
                case StavHry.napoveda: Stavy.napoveda.Hide();
                    break;
                case StavHry.menu: Stavy.menu.Hide();
                    break;
                case StavHry.bezi:
                    { casovac.Enabled = false;  }
                    break;
                case StavHry.kolize:
                    { Stavy.kolize.Hide(); }
                    break;
                case StavHry.cil:
                    { Stavy.cil.Hide(); }
                    break;
                default:
                    break;
            }


            //udelame novinky
            switch (stav)
            {
                case StavHry.start:
                    { Stavy.start.Show(); Hra.stav = StavHry.start; break; }
                    
                case StavHry.napoveda:
                    { Stavy.napoveda.Show(); Hra.stav = StavHry.napoveda; break; }
                case StavHry.menu:
                    { Stavy.menu.Show(); Hra.stav = StavHry.menu; break; }
                case StavHry.bezi:
                    {
                        levely.NastavLevel(cislo_levelu);
                        casovac.Enabled = true;
                        Hra.stav = StavHry.bezi;
                        break;
                    }
                case StavHry.kolize:
                    {
                        Hra.stav = StavHry.kolize;
                        Stavy.kolize.Show();
                        break;
                    }
                case StavHry.cil:
                    {
                        Hra.stav = StavHry.cil;
                        Stavy.cil.Show();
                        break;
                    }
                default:
                    break;
            }

        }









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
                Console.WriteLine(stav);
                if (novyStav != StavHry.bezi)
                    ZmenStavHryNa(novyStav);

                //vykreslime uzivateli
                if(stav == StavHry.bezi)
                    form.Invalidate();
         
        }
    }
    


    class Stavy
    {
        public static Panel start;
        public static Panel napoveda;
        public static Panel menu;
        public static Panel kolize;
        public static Panel cil;

        public static void UdelejFormulareVsechStavu()
        {
            UdelejStart();
            UdelejNapovedu();
            UdelejMenu();
            UdelejKolize();
            UdelejCil();
        }

       

        private static void UdelejStart() {

            start = new Panel();
            start.Width = Hra.frame.Width;
            start.Height = Hra.frame.Height;
            start.BackgroundImage = new Bitmap("pomocne_soubory\\program\\start_bg.png");
            start.Hide();
            Hra.form.Controls.Add(start);

            Button hrat = new Button();
            hrat.Size = new Size(218, 54);
            hrat.Location = new Point(542, 300);
            hrat.Image = Hra.TlacitkaObr.Clone(new Rectangle(0, 0, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            hrat.Image.Tag = 0;
            hrat.FlatAppearance.BorderSize = 0;
            hrat.FlatAppearance.MouseDownBackColor = Color.Transparent;
            hrat.FlatAppearance.MouseOverBackColor = Color.Transparent;
            hrat.FlatStyle = FlatStyle.Flat;
            hrat.BackColor = Color.Transparent;
            hrat.MouseEnter += new EventHandler(tlacitko_hoverPozadiIn);
            hrat.MouseLeave += new EventHandler(tlacitko_hoverPozadiOut);
            hrat.MouseDown += new MouseEventHandler(tlacitko_mysKlikPozadi);
            hrat.MouseUp += new MouseEventHandler(tlacitko_mysOdklikPozadi);
            hrat.Click += tlacitko_prepni_Menu_Click; 

            start.Controls.Add(hrat);

            Button napoveda = new Button();
            napoveda.Size = new Size(218, 54);
            napoveda.Location = new Point(542, 370);
            napoveda.Image = Hra.TlacitkaObr.Clone(new Rectangle(0, 54*3, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            napoveda.Image.Tag = 54*3;
            napoveda.FlatAppearance.BorderSize = 0;
            napoveda.FlatAppearance.MouseDownBackColor = Color.Transparent;
            napoveda.FlatAppearance.MouseOverBackColor = Color.Transparent;
            napoveda.FlatStyle = FlatStyle.Flat;
            napoveda.BackColor = Color.Transparent;
            napoveda.MouseEnter += new EventHandler(tlacitko_hoverPozadiIn);
            napoveda.MouseLeave += new EventHandler(tlacitko_hoverPozadiOut);
            napoveda.MouseDown += new MouseEventHandler(tlacitko_mysKlikPozadi);
            napoveda.MouseUp += new MouseEventHandler(tlacitko_mysOdklikPozadi);
            napoveda.Click += tlacitko_prepni_Napoveda_Click;
            start.Controls.Add(napoveda);

        }
        private static void UdelejNapovedu()
        {

            napoveda = new Panel();
            napoveda.Width = Hra.frame.Width;
            napoveda.Height = Hra.frame.Height;
            napoveda.BackgroundImage = new Bitmap("pomocne_soubory\\program\\napoveda_bg.png");
            napoveda.Hide();
            Hra.form.Controls.Add(napoveda);

            Button zpet = new Button();
            zpet.Size = new Size(218, 54);
            zpet.Location = new Point(210, 480);
            zpet.Image = Hra.TlacitkaObr.Clone(new Rectangle(0, 2 * 3 * 54, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            zpet.Image.Tag = (2 * 3 * 54).ToString();
            zpet.FlatAppearance.BorderSize = 0;
            zpet.FlatAppearance.MouseDownBackColor = Color.Transparent;
            zpet.FlatAppearance.MouseOverBackColor = Color.Transparent;
            zpet.FlatStyle = FlatStyle.Flat;
            zpet.BackColor = Color.Transparent;
            zpet.MouseEnter += new EventHandler(tlacitko_hoverPozadiIn);
            zpet.MouseLeave += new EventHandler(tlacitko_hoverPozadiOut);
            zpet.MouseDown += new MouseEventHandler(tlacitko_mysKlikPozadi);
            zpet.MouseUp += new MouseEventHandler(tlacitko_mysOdklikPozadi);
            zpet.Click += tlacitko_prepni_Start_Click;

            napoveda.Controls.Add(zpet);
        }

        private static void UdelejMenu()
        {
            menu = new Panel();
            menu.Width = Hra.frame.Width;
            menu.Height = Hra.frame.Height;
            menu.BackgroundImage = new Bitmap("pomocne_soubory\\program\\menu_bg.png");
            menu.Hide();
            Hra.form.Controls.Add(menu);


            Button zpet = new Button();
            zpet.Size = new Size(218, 54);
            zpet.Location = new Point(210, 480);
            zpet.Image = Hra.TlacitkaObr.Clone(new Rectangle(0, 2 * 3 * 54, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            zpet.Image.Tag = (2 * 3 * 54).ToString();
            zpet.FlatAppearance.BorderSize = 0;
            zpet.FlatAppearance.MouseDownBackColor = Color.Transparent;
            zpet.FlatAppearance.MouseOverBackColor = Color.Transparent;
            zpet.FlatStyle = FlatStyle.Flat;
            zpet.BackColor = Color.Transparent;
            zpet.MouseEnter += new EventHandler(tlacitko_hoverPozadiIn);
            zpet.MouseLeave += new EventHandler(tlacitko_hoverPozadiOut);
            zpet.MouseDown += new MouseEventHandler(tlacitko_mysKlikPozadi);
            zpet.MouseUp += new MouseEventHandler(tlacitko_mysOdklikPozadi);
            zpet.Click += tlacitko_prepni_Start_Click;

            menu.Controls.Add(zpet);

            FlowLayoutPanel panel_levelu = new FlowLayoutPanel();
            panel_levelu.Location = new Point(144, 179);
            panel_levelu.Size = new Size(615, 266);
            panel_levelu.BackColor = Color.Transparent;
            panel_levelu.Padding = new Padding(10, 0, 10, 0);
            panel_levelu.AutoScroll = true;

            for(int i = 0; i < Hra.levely.levely.Count; i++)
            {
                Button tlacitko_levelu = new Button();
                tlacitko_levelu.Size = new Size(170, 104);
                tlacitko_levelu.Image = ((Levely.Level)Hra.levely.levely[i]).miniatura;
                tlacitko_levelu.Tag = i;
                tlacitko_levelu.FlatStyle = FlatStyle.Standard;
                tlacitko_levelu.BackColor = Color.Transparent;
                tlacitko_levelu.Margin = new Padding(10);
                tlacitko_levelu.Click += tlacitko_prepni_Bezi_Click;

                panel_levelu.Controls.Add(tlacitko_levelu);
            }

            menu.Controls.Add(panel_levelu);


        }
        private static void UdelejKolize()
        {
            kolize = new Panel();
            kolize.Width = Hra.frame.Width;
            kolize.Height = Hra.frame.Height;
            kolize.BackgroundImage = new Bitmap("pomocne_soubory\\program\\kolize_bg.png");
            kolize.Hide();
            kolize.BackColor = Color.Transparent;
            Hra.form.Controls.Add(kolize);

            

            Button zpet = new Button();
            zpet.Size = new Size(218, 54);
            zpet.Location = new Point(218, 362);
            zpet.Image = Hra.TlacitkaObr.Clone(new Rectangle(0, 3 * 3 * 54, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            zpet.Image.Tag = (3 * 3 * 54).ToString();
            zpet.FlatAppearance.BorderSize = 0;
            zpet.FlatAppearance.MouseDownBackColor = Color.Transparent;
            zpet.FlatAppearance.MouseOverBackColor = Color.Transparent;
            zpet.FlatStyle = FlatStyle.Flat;
            zpet.BackColor = Color.Transparent;
            zpet.MouseEnter += new EventHandler(tlacitko_hoverPozadiIn);
            zpet.MouseLeave += new EventHandler(tlacitko_hoverPozadiOut);
            zpet.MouseDown += new MouseEventHandler(tlacitko_mysKlikPozadi);
            zpet.MouseUp += new MouseEventHandler(tlacitko_mysOdklikPozadi);
            zpet.Click += tlacitko_prepni_Menu_Click;

            kolize.Controls.Add(zpet);

            Button znovu = new Button();
            znovu.Size = new Size(218, 54);
            znovu.Location = new Point(546, 362);
            znovu.Image = Hra.TlacitkaObr.Clone(new Rectangle(0, 4 * 3 * 54, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            znovu.Image.Tag = (4 * 3 * 54).ToString();
            znovu.FlatAppearance.BorderSize = 0;
            znovu.FlatAppearance.MouseDownBackColor = Color.Transparent;
            znovu.FlatAppearance.MouseOverBackColor = Color.Transparent;
            znovu.FlatStyle = FlatStyle.Flat;
            znovu.BackColor = Color.Transparent;
            znovu.MouseEnter += new EventHandler(tlacitko_hoverPozadiIn);
            znovu.MouseLeave += new EventHandler(tlacitko_hoverPozadiOut);
            znovu.MouseDown += new MouseEventHandler(tlacitko_mysKlikPozadi);
            znovu.MouseUp += new MouseEventHandler(tlacitko_mysOdklikPozadi);
            znovu.Tag = Hra.levely.aktualni_level;
            znovu.Click += tlacitko_prepni_Bezi_Click;

            kolize.Controls.Add(znovu);

        }

        private static void UdelejCil()
        {
            cil = new Panel();
            cil.Width = Hra.frame.Width;
            cil.Height = Hra.frame.Height;
            cil.BackgroundImage = new Bitmap("pomocne_soubory\\program\\cil_bg.png");
            cil.Hide();
            cil.BackColor = Color.Transparent;
            Hra.form.Controls.Add(cil);



            Button zpet = new Button();
            zpet.Size = new Size(218, 54);
            zpet.Location = new Point(218, 362);
            zpet.Image = Hra.TlacitkaObr.Clone(new Rectangle(0, 3 * 3 * 54, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            zpet.Image.Tag = (3 * 3 * 54).ToString();
            zpet.FlatAppearance.BorderSize = 0;
            zpet.FlatAppearance.MouseDownBackColor = Color.Transparent;
            zpet.FlatAppearance.MouseOverBackColor = Color.Transparent;
            zpet.FlatStyle = FlatStyle.Flat;
            zpet.BackColor = Color.Transparent;
            zpet.MouseEnter += new EventHandler(tlacitko_hoverPozadiIn);
            zpet.MouseLeave += new EventHandler(tlacitko_hoverPozadiOut);
            zpet.MouseDown += new MouseEventHandler(tlacitko_mysKlikPozadi);
            zpet.MouseUp += new MouseEventHandler(tlacitko_mysOdklikPozadi);
            zpet.Click += tlacitko_prepni_Menu_Click;

            cil.Controls.Add(zpet);

            Button znovu = new Button();
            znovu.Size = new Size(218, 54);
            znovu.Location = new Point(546, 362);
            znovu.Image = Hra.TlacitkaObr.Clone(new Rectangle(0, 4 * 3 * 54, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            znovu.Image.Tag = (4 * 3 * 54).ToString();
            znovu.FlatAppearance.BorderSize = 0;
            znovu.FlatAppearance.MouseDownBackColor = Color.Transparent;
            znovu.FlatAppearance.MouseOverBackColor = Color.Transparent;
            znovu.FlatStyle = FlatStyle.Flat;
            znovu.BackColor = Color.Transparent;
            znovu.MouseEnter += new EventHandler(tlacitko_hoverPozadiIn);
            znovu.MouseLeave += new EventHandler(tlacitko_hoverPozadiOut);
            znovu.MouseDown += new MouseEventHandler(tlacitko_mysKlikPozadi);
            znovu.MouseUp += new MouseEventHandler(tlacitko_mysOdklikPozadi);
            znovu.Tag = Hra.levely.aktualni_level;
            znovu.Click += tlacitko_prepni_Bezi_Click;

            cil.Controls.Add(znovu);
        }

        private static void tlacitko_prepni_Bezi_Click(object sender, EventArgs e)
        {
            Hra.ZmenStavHryNa(StavHry.bezi, Int32.Parse( ((Button)sender).Tag.ToString() ) );
        }

        private static void tlacitko_prepni_Napoveda_Click(object sender, EventArgs e)
        {
            Hra.ZmenStavHryNa(StavHry.napoveda);
        }

        private static void tlacitko_prepni_Menu_Click(object sender, EventArgs e)
        {
            Hra.ZmenStavHryNa(StavHry.menu);
        }
        private static void tlacitko_prepni_Start_Click(object sender, EventArgs e)
        {
            Hra.ZmenStavHryNa(StavHry.start);
        }

        private static void tlacitko_mysKlikPozadi(object sender, EventArgs e)
        {
            int poziceHoverIkonky = Int32.Parse(((Button)sender).Image.Tag.ToString()) + 2 * 54;
            ((Button)sender).Image = Hra.TlacitkaObr.Clone(new Rectangle(0, poziceHoverIkonky, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            ((Button)sender).Image.Tag = poziceHoverIkonky - 2 * 54;
        }
        private static void tlacitko_mysOdklikPozadi(object sender, EventArgs e)
        {
            int poziceHoverIkonky = Int32.Parse(((Button)sender).Image.Tag.ToString());
            ((Button)sender).Image = Hra.TlacitkaObr.Clone(new Rectangle(0, poziceHoverIkonky, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            ((Button)sender).Image.Tag = poziceHoverIkonky;
        }

        private static void tlacitko_hoverPozadiOut(object sender, EventArgs e)
        {
            int poziceHoverIkonky = Int32.Parse( ((Button)sender).Image.Tag.ToString()) ;
            ((Button)sender).Image = Hra.TlacitkaObr.Clone(new Rectangle(0, poziceHoverIkonky, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            ((Button)sender).Image.Tag = poziceHoverIkonky;
        }

        private static void tlacitko_hoverPozadiIn(object sender, EventArgs e)
        {
            int poziceHoverIkonky = Int32.Parse( ((Button)sender).Image.Tag.ToString() ) + 54;
            ((Button)sender).Image = Hra.TlacitkaObr.Clone(new Rectangle(0, poziceHoverIkonky, 218, 54), System.Drawing.Imaging.PixelFormat.DontCare);
            ((Button)sender).Image.Tag = poziceHoverIkonky - 54;
            ((Button)sender).BackColor = Color.Transparent;
        }
    }


}
