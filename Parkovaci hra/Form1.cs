using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parkovaci_hra
{

   
    public partial class Form1 : Form
    {
        Auto.Auto a;

        Graphics g;
        Bitmap frame = new Bitmap(980, 600);
        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(980, 600);
            
            g = Graphics.FromImage(frame);
            this.DoubleBuffered = true;
            a = new Auto.Auto("Auto.txt", g);

            timer1.Enabled = true;
            
        }

       
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(frame, 0, 0);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {

            /* TODO 
            myslím že tohle by měla řešit už nějaká metoda... pravděpodobně ve hře */

            //vykreslime mapu
            g.DrawImage(new Bitmap("obrazky\\mapy\\MAPA1.png"), 0, 0);


            Bitmap auto;
            PointF pozice_float;
            a.Krok(out auto, out pozice_float);
            Point pozice = new Point((int)pozice_float.X, (int)pozice_float.Y);
                       
            //vykreslime auto
            g.DrawImage(auto, pozice.X, pozice.Y);
            

            //vykreslime prekazky
            g.DrawImage(new Bitmap("obrazky\\mapy\\prekazky1.png"), 0, 0);





            /* TODO:
            Tohle nějak předělat a nejlépe asi strčit, ať si pořeší kolize, nebo něco jiného */

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
            if (pozice.X + auto.Width > 980)
                pravy_dolni.Width = -pozice.X + 980;
            if (pozice.Y + auto.Height > 600)
                pravy_dolni.Height = -pozice.Y + 600; ;

            auto = auto.Clone(new Rectangle(levy_horni, pravy_dolni), System.Drawing.Imaging.PixelFormat.DontCare);

            //Console.WriteLine(auto.Size);
            //zjistime kolize
            Kolize.Kolize.NactiAuto(auto);


            //dostaneme vyrez bitmapy s autem
                 
            Bitmap vyrez = frame.Clone(new Rectangle(levy_horni.X + pozice.X, levy_horni.Y + pozice.Y, pravy_dolni.Width, pravy_dolni.Height), System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            
            Kolize.Kolize.BylaKolize(ref vyrez);

            Kolize.Kolize.NactiPotrebneBodyProOvereniCile();
            Console.WriteLine(Kolize.Kolize.JsmeVCili(ref vyrez));

            g.DrawImage(vyrez, 200, 20);


            textBox1.Text = a.stav.UhelKol.ToString();
            trackBar1.Value = a.stav.UhelKol;
            this.Invalidate();
        }

      
    }

}
