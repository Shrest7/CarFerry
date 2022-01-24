using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJEKT_PW_SECOND_TRY
{
    public class Samochod : BaseThread
    {
        public int Id { get; set; }
        private int _bazowyBrzeg;
        public int zajeteMiejsce;
        public PictureBox _pictureBoxBrzegu;
        public PictureBox _pictureBoxPromu;
        public bool naPromie = false;
        public bool PrzeprawaUkonczona { get; set; } = false;
        private Prom _prom;

        public int BazowyBrzeg
        {
            get => _bazowyBrzeg;
            set
            {
                if (value != 1 && value != 2)
                {
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(value)} musi miec wartosc 1 lub 2!");
                }
                _bazowyBrzeg = value;
            }
        }
        public Samochod(int id, int bazowyBrzeg, Prom prom, PictureBox pictureBoxBrzegu) : base()
        {
            BazowyBrzeg = bazowyBrzeg;
            Id = id;
            _prom = prom;
            _pictureBoxBrzegu = pictureBoxBrzegu;
        }

        public override void RunThread() //do wjazdzjazd
        {
            while (true)
            {
                Form1.wolne.Wait();//NO TOUCH - WORKED
                if (_prom.Brzeg == BazowyBrzeg && PrzeprawaUkonczona == false && _prom.wTrakciePrzeprawy == false)
                {
                    _prom.WjazdSamochodu(this);


                    try
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadInterruptedException) { }


                    _prom.ZjazdSamochodu(this);

                    PrzeprawaUkonczona = true;
                }
                Form1.wolne.Release();//NO TOUCH - WORKED
            }

        }
    }
}
