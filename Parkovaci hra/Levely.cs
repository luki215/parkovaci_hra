using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkovaci_hra
{
    namespace Levely
    {
        class SpravceLevelu
        {
            public ArrayList levely = new ArrayList();
            public int aktualni_level;

            public SpravceLevelu()
            {
                DirectoryInfo d = new DirectoryInfo(Hra.cestaLevelu);

                foreach (var file in d.GetFiles("*.txt"))
                {
                    NakonfigurujLevel(file.ToString());
                }
            }

            private void NakonfigurujLevel(string cesta_konfiguracniho_souboru)
            {
                levely.Add(new Level(cesta_konfiguracniho_souboru));
            }

            public void NastavLevel(int cislo)
            {
               ((Level)levely[cislo]).NactiSe();
            }

            public void VykresliPozadiLevelu()
            {
                Hra.g.DrawImage(((Level)levely[aktualni_level]).pozadi, 0, 0);
            }
            public void VykresliPrekazkyLevelu()
            {
                Hra.g.DrawImage(((Level)levely[aktualni_level]).prekazky, 0, 0);
            }
        }
        class Level
        {
            public Bitmap pozadi;
            public Bitmap miniatura;
            public Bitmap prekazky;
            PointF pozice_auta;
            int uhel_auta;
            public Color barva_cile;
            string cesta_k_autu;

            public Level(string cesta_konfig_souboru)
            {
                using (StreamReader sr = new StreamReader(Hra.cestaLevelu + cesta_konfig_souboru))
                {
                    String cteni = Hra.cestaLevelu + sr.ReadLine().Split(':')[1].Substring(1);
                    pozadi = new Bitmap(cteni);

                    cteni = Hra.cestaLevelu + sr.ReadLine().Split(':')[1].Substring(1);
                    miniatura = new Bitmap(cteni);

                    cteni = Hra.cestaLevelu + sr.ReadLine().Split(':')[1].Substring(1);
                    prekazky = new Bitmap(cteni);

                    cteni = sr.ReadLine().Split(':')[1].Substring(1);
                    pozice_auta = new Point();
                    pozice_auta.X = float.Parse(cteni.Split(';')[0]);
                    pozice_auta.Y = float.Parse(cteni.Split(';')[1]);
                   
                    uhel_auta = Int32.Parse(sr.ReadLine().Split(':')[1]);

                    cteni = sr.ReadLine().Split(':')[1].Substring(1);
                    int r = Int32.Parse(cteni.Split(',')[0].Substring(1)),
                        g = Int32.Parse(cteni.Split(',')[1].Substring(1)),
                        b = Int32.Parse(cteni.Split(',')[1].Substring(1));

                    barva_cile = Color.FromArgb(r, g, b);

                    cesta_k_autu = sr.ReadLine().Split(':')[1].Substring(1);

                }
            }
            public void NactiSe()
            {
                Hra.a = new Auto.Auto(cesta_k_autu, pozice_auta, uhel_auta);
            }
        }
    }
}
