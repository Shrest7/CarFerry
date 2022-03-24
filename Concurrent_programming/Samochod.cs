using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJEKT_PW_FINAL_TRY
{
    public class Samochod : BaseThread
    {
        public int Id { get; set; }
        private int _bazowyBrzeg;
        public int zajeteMiejsce;
        public PictureBox _pictureBoxBrzegu;
        public Label _labelBrzegu;
        public PictureBox _pictureBoxPromu;
        public bool PrzeprawaUkonczona { get; set; } = false;
        private Prom _prom;
        private SemaphoreSlim _wolne;

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
        public Samochod(int id, int bazowyBrzeg, Prom prom, PictureBox pictureBoxBrzegu,
            Label labelBrzegu, SemaphoreSlim wolne) : base()
        {
            BazowyBrzeg = bazowyBrzeg;
            Id = id;
            _prom = prom;
            _pictureBoxBrzegu = pictureBoxBrzegu;
            _wolne = wolne;
            _labelBrzegu = labelBrzegu;
        }

        public override void RunThread() 
        {
            while (PrzeprawaUkonczona == false)
            {
                _wolne.Wait();
                if (_prom.Brzeg == BazowyBrzeg && _prom.WTrakciePrzeprawy == false)
                {
                    _prom.WjazdZjazd(this);

                    try
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadInterruptedException) { } 

                    _prom.WjazdZjazd(this);

                    PrzeprawaUkonczona = true;
                }
                _wolne.Release();
            }
        }
    }
}
