using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJEKT_PW_FINAL_TRY
{
    public class Car : BaseThread
    {
        public int Id { get; set; }
        public int OnBoardParkingSpaceId { get; set; }
        public PictureBox PictureBoxRiverbank { get; set; }
        public Label LabelRiverbank { get; set; }
        public PictureBox PictureBoxFerry { get; set; }
        public bool TravelFinished { get; set; } = false;
        private readonly Ferry _ferry;
        private readonly SemaphoreSlim _onBoardParkingSpaces;

        private int _baseRiverbank;
        public int BaseRiverbank
        {
            get => _baseRiverbank;
            set
            {
                if (value != 1 && value != 2)
                {
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(value)} must have a value of either 1 or 2.");
                }
                _baseRiverbank = value;
            }
        }
        public Car(int id, int baseRiverbank, Ferry ferry, PictureBox pictureBoxRiverbank,
            Label labelRiverbank, SemaphoreSlim onBoardParkingSpaces) : base()
        {
            Id = id;
            BaseRiverbank = baseRiverbank;
            PictureBoxRiverbank = pictureBoxRiverbank;
            LabelRiverbank = labelRiverbank;
            _ferry = ferry;
            _onBoardParkingSpaces = onBoardParkingSpaces;
        }

        public override void RunThread() 
        {
            while (TravelFinished == false)
            {
                _onBoardParkingSpaces.Wait();
                if (_ferry.RiverBank == BaseRiverbank && _ferry.Travelling == false)
                {
                    _ferry.EnterOrLeave(this);

                    try
                    {
                        Thread.Sleep(Timeout.Infinite);
                    }
                    catch (ThreadInterruptedException) { } 

                    _ferry.EnterOrLeave(this);

                    TravelFinished = true;
                }
                _onBoardParkingSpaces.Release();
            }
        }
    }
}
