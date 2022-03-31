using CarFerry.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace PROJEKT_PW_FINAL_TRY
{
    public class Ferry
    {
        public List<Car> Cars { get; set; } = new List<Car>();
        public Stopwatch WaitStopwatch { get; set; } = new Stopwatch();
        public int PatienceThreshold { get; } = 6500;
        public bool Travelling { get; set; }

        private readonly int _travelTime = 3000;
        private readonly PictureBox _ferryPictureBox;
        private readonly PictureBox[] _ferryParkingSpaces;
        private readonly Label _departureReasonLbl;


        private int _riverBank = 1;
        public int RiverBank 
        {
            get => _riverBank;
            set
            {
                if(value != 1 && value != 2)
                {
                    throw new ArgumentOutOfRangeException(
                        $"{nameof(RiverBank)} must have a value of either 1 or 2.");
                }
                _riverBank = value;
            } 
        }

        public Ferry(PictureBox pictureBoxFerry, PictureBox[] pictureBoxesOnBoardPlaces,
            Label lblDepartureReason)
        {
            _ferryPictureBox = pictureBoxFerry;
            _ferryParkingSpaces = pictureBoxesOnBoardPlaces;
            _departureReasonLbl = lblDepartureReason;
            WaitStopwatch.Start();
        }

        public void EnterOrLeave(Car car)
        {
            lock (Cars)
            {
                Thread.Sleep(300);
                if (car.TravelFinished == false)
                {
                    lock (MainForm.LockObj)
                    {
                        Cars.Add(car);
                    }
                    int i = 0;
                    while (_ferryParkingSpaces[(car.Id + i) % 6].Image != null)
                    {
                        i++;
                    }
                    car.PictureBoxFerry = _ferryParkingSpaces[(car.Id + i) % 6];
                    car.OnBoardParkingSpaceId = (car.Id + i) % 6;

                    car.PictureBoxRiverbank.Image = null;
                    car.LabelRiverbank.Invoke((Action)(() => car.LabelRiverbank.Text = ""));
                    _ferryParkingSpaces[(car.Id + i) % 6].Image = Resources.Car;
                    
                }
                else
                {
                    var carToRemove = Cars.SingleOrDefault(s => s.Id == car.Id);
                    lock (MainForm.LockObj)
                    {
                        Cars.Remove(carToRemove);
                    }

                    _ferryParkingSpaces[car.OnBoardParkingSpaceId].Image = null;
                }
            }
        }

        public void Travel()
        {
            WaitStopwatch.Stop();
            Travelling = true;

            Stopwatch stopwatch = new Stopwatch();  
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < _travelTime) 
            {
                Thread.Sleep(_travelTime / 7);
                _ferryPictureBox.Invoke((Action)(() => MoveFerryAndCars()));
            }

            SetTravelStateToFinished();
            SetOppositeRiverbank();
            WakeUpCarThreads();

            _departureReasonLbl.Invoke((Action)(() => _departureReasonLbl.Text = ""));

            Travelling = false;
            WaitStopwatch.Restart();
        }

        private void MoveFerryAndCars()
        {
            if(RiverBank == 1)
            {
                _ferryPictureBox.Location = new Point(_ferryPictureBox.Location.X, _ferryPictureBox.Location.Y - 50);
                foreach(var parkingSpace in _ferryParkingSpaces)
                {
                    parkingSpace.Location = new Point(parkingSpace.Location.X,
                        parkingSpace.Location.Y - 50);
                }
            }
            else
            {
                _ferryPictureBox.Location = new Point(_ferryPictureBox.Location.X, _ferryPictureBox.Location.Y + 50);
                foreach (var parkingSpace in _ferryParkingSpaces)
                {
                    parkingSpace.Location = new Point(parkingSpace.Location.X,
                        parkingSpace.Location.Y + 50);
                }
            }
        }

        private void SetOppositeRiverbank()
        {
            if (RiverBank == 1)
            {
                RiverBank = 2;
            }
            else
            {
                RiverBank = 1;
            }
        }

        private void SetTravelStateToFinished()
        {
            foreach(var car in Cars)
            {
                car.TravelFinished = true;
            }
        }

        private void WakeUpCarThreads()
        {
            foreach (var car in Cars)
            {
                car.Interrupt();
            }
        }
    }
}
