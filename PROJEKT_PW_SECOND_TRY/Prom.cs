using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace PROJEKT_PW_SECOND_TRY
{
    public class Prom
    {
        public int ProgCierpliwosci { get; } = 10000;
        public int CzasTrwaniaKursu { get; } = 3000;
        public PictureBox pictureBox;
        public List<Samochod> Samochody { get; set; } = new List<Samochod>();
        public bool Gotowy { get; set; }

        public bool wTrakciePrzeprawy;

        private int _brzeg = 1;
        public int Brzeg 
        {
            get => _brzeg;
            set
            {
                if(value != 1 && value != 2)
                {
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(Brzeg)} musi miec wartosc 1 lub 2!");
                }
                _brzeg = value;
            } 
        }

        public Prom(PictureBox pictureBox)
        {
            this.pictureBox = pictureBox;
        }

        public void WjazdSamochodu(Samochod samochod)
        {
            lock (Samochody)
            {
                int i = 0;
                Thread.Sleep(250); //wjezdanie
                while(Form1.miejscaPromu[(samochod.Id + i) % 6].Image != null)
                {
                    i++;
                }
                samochod._pictureBoxPromu = Form1.miejscaPromu[(samochod.Id + i) % 6];
                samochod.zajeteMiejsce = (samochod.Id + i) % 6;
                Samochody.Add(samochod);
                samochod._pictureBoxBrzegu.Image = null;
                Form1.miejscaPromu[(samochod.Id + i) % 6].Image = Image.FromFile(Form1.sciezkaGrafikiSamochodu);
            }
        }

        public void ZjazdSamochodu(Samochod samochod)
        {
            lock (Samochody)
            {
                Thread.Sleep(250); //zjezdanie
                var samochodDoUsuniecia = Samochody.SingleOrDefault(s => s.Id == samochod.Id);
                Samochody.Remove(samochodDoUsuniecia);
                Form1.miejscaPromu[samochod.zajeteMiejsce].Image = null;
            }
        }

        public void WjazdZjazd(Samochod samochod)
        {
            lock (Samochody)
            {
                if (samochod.PrzeprawaUkonczona == false)
                {
                    Samochody.Add(samochod);
                    samochod.naPromie = true;
                }
                else
                {
                    var samochodDoUsuniecia = Samochody.SingleOrDefault(s => s.Id == samochod.Id);
                    Samochody.Remove(samochodDoUsuniecia);
                    samochod.naPromie = false;
                }
            }
        }

        public void Plyn()
        {
            Stopwatch stopwatch = new Stopwatch();

            wTrakciePrzeprawy = true;

            string idPlynacychSamochodow = "";
                idPlynacychSamochodow = string.Join(", ", Samochody.Select(x => x.Id));
            
            //MessageBox.Show($"Prom odplywa z brzegu {Brzeg}" +
                //$" z samochodami {idPlynacychSamochodow}.");

            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < CzasTrwaniaKursu) 
            {
                Thread.Sleep(CzasTrwaniaKursu / 7);
                pictureBox.Invoke((Action)(() => PrzesuwajPromOrazSamochody()));
            }

            UstawStatusNaUkonczony();
            UstawPrzeciwnyBrzeg();
            ObudzSamochody();
            Console.WriteLine($"Prom doplynal do brzegu {Brzeg}" +
                $" z samochodami {idPlynacychSamochodow}.");

            wTrakciePrzeprawy = false;
        }

        public void PrzesuwajPromOrazSamochody()
        {
            if(Brzeg == 1)
            {
                pictureBox.Location = new Point(pictureBox.Location.X, pictureBox.Location.Y - 50);
                foreach(var miejsce in Form1.miejscaPromu)
                {
                    miejsce.Location = new Point(miejsce.Location.X,
                        miejsce.Location.Y - 50);
                }
            }
            else
            {
                pictureBox.Location = new Point(pictureBox.Location.X, pictureBox.Location.Y + 50);
                foreach (var miejsce in Form1.miejscaPromu)
                {
                    miejsce.Location = new Point(miejsce.Location.X,
                        miejsce.Location.Y + 50);
                }
            }
        }

        public void UstawPrzeciwnyBrzeg()
        {
            if (Brzeg == 1)
            {
                Brzeg = 2;
            }
            else
            {
                Brzeg = 1;
            }
        }

        public void UstawStatusNaUkonczony()
        {
            foreach(var samochod in Samochody)
            {
                samochod.PrzeprawaUkonczona = true;
            }
        }

        public void ObudzSamochody()
        {
            foreach (var samochod in Samochody)
            {
                samochod.Interrupt();
            }
        }
    }
}
