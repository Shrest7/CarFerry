using PROJEKT_PW_WINFORMS.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace PROJEKT_PW_FINAL_TRY
{
    public class Prom
    {
        public List<Samochod> Samochody { get; set; } = new List<Samochod>();
        public int ProgCierpliwosci { get; } = 6500;
        public Stopwatch czasomierzOczekiwania = new Stopwatch();
        public bool WTrakciePrzeprawy { get; set; }
        private readonly int CzasTrwaniaKursu = 3000;
        private readonly PictureBox pictureBoxPromu;
        private readonly PictureBox[] _miejscaPromu;
        private readonly Label _lblReason;


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

        public Prom(PictureBox pictureBoxPromu, PictureBox[] miejscaPromu, Label lblReason)
        {
            this.pictureBoxPromu = pictureBoxPromu;
            _miejscaPromu = miejscaPromu;
            _lblReason = lblReason;
            czasomierzOczekiwania.Start();
        }

        public void WjazdZjazd(Samochod samochod)
        {
            lock (Samochody)
            {
                Thread.Sleep(300);
                if (samochod.PrzeprawaUkonczona == false)
                {
                    lock (MainForm.listObj)
                    {
                        Samochody.Add(samochod);
                    }
                    int i = 0;
                    while (_miejscaPromu[(samochod.Id + i) % 6].Image != null)
                    {
                        i++;
                    }
                    samochod._pictureBoxPromu = _miejscaPromu[(samochod.Id + i) % 6];
                    samochod.zajeteMiejsce = (samochod.Id + i) % 6;

                    samochod._pictureBoxBrzegu.Image = null;
                    samochod._labelBrzegu.Invoke((Action)(() => samochod._labelBrzegu.Text = ""));
                    _miejscaPromu[(samochod.Id + i) % 6].Image = Resources.samochod;
                    
                }
                else
                {
                    var samochodDoUsuniecia = Samochody.SingleOrDefault(s => s.Id == samochod.Id);
                    lock (MainForm.listObj)
                    {
                        Samochody.Remove(samochodDoUsuniecia);
                    }

                    _miejscaPromu[samochod.zajeteMiejsce].Image = null;
                }
            }
        }

        public void Plyn()
        {
            czasomierzOczekiwania.Stop();
            WTrakciePrzeprawy = true;

            Stopwatch stopwatch = new Stopwatch();  
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < CzasTrwaniaKursu) 
            {
                Thread.Sleep(CzasTrwaniaKursu / 7);
                pictureBoxPromu.Invoke((Action)(() => PrzesuwajPromOrazSamochody()));
            }

            UstawStatusNaUkonczony();
            UstawPrzeciwnyBrzeg();
            ObudzSamochody();

            _lblReason.Invoke((Action)(() => _lblReason.Text = ""));

            WTrakciePrzeprawy = false;
            czasomierzOczekiwania.Restart();
        }

        public void PrzesuwajPromOrazSamochody()
        {
            if(Brzeg == 1)
            {
                pictureBoxPromu.Location = new Point(pictureBoxPromu.Location.X, pictureBoxPromu.Location.Y - 50);
                foreach(var miejsce in _miejscaPromu)
                {
                    miejsce.Location = new Point(miejsce.Location.X,
                        miejsce.Location.Y - 50);
                }
            }
            else
            {
                pictureBoxPromu.Location = new Point(pictureBoxPromu.Location.X, pictureBoxPromu.Location.Y + 50);
                foreach (var miejsce in _miejscaPromu)
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
