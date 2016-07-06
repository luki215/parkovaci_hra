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

            //vykreslime mapu
            g.DrawImage(new Bitmap("obrazky\\mapy\\MAPA1.png"), 0, 0);


            Bitmap auto;
            PointF pozice;
            a.Krok(out auto, out pozice);

            
            //vykreslime auto
            g.DrawImage(auto, pozice);


            //vykreslime prekazky
            g.DrawImage(new Bitmap("obrazky\\mapy\\prekazky1.png"), 0, 0);


            //zjistime kolize




            textBox1.Text = a.stav.UhelKol.ToString();
            trackBar1.Value = a.stav.UhelKol;
            this.Invalidate();
        }

      
    }

}
