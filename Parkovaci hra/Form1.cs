﻿using System;
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
        //vyrez do ktereho kreslime
        static Bitmap frame = new Bitmap(980, 600);
        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(980, 600);
            this.DoubleBuffered = true;

            Hra.Nainicializuj(frame, this);           
           
        }


        /*  Hack pro plynule vykreslovani grafiky - nekreslíme do formuláře, 
            ale do bitmapy frame, tu pak dáme vykreslit až budeme chtít pomocí form.Invalidate() */
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(frame, 0, 0);
        }


    }

}
