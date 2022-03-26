using CarFerry.Properties;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJEKT_PW_FINAL_TRY
{
    public partial class MainForm : Form
    {
        public static object LockObj { get; set; } =
            new object();
        private const int _numberOfParkingSpaces = 10;
        private const int _ferryCapacity = 6;
        private readonly PictureBox[] _parkingFirstRiverbank;
        private readonly PictureBox[] _parkingSecondRiverbank;
        private readonly PictureBox[] _ferryParkingSpaces;
        private readonly Label[] _firstRiverbankLabels;
        private readonly Label[] _secondRiverbankLabels;
        private readonly SemaphoreSlim _onBoardPlacesAvailable =
            new SemaphoreSlim(_ferryCapacity, _ferryCapacity);
        private int _carId;
        private int _firstRiverbankIndex;
        private int _secondRiverbankIndex;
        private readonly List<Car> _carsFirstRiverbank = new List<Car>();
        private readonly List<Car> _carsSecondRiverbank = new List<Car>();
        private readonly Ferry _ferry;

        public MainForm()
        {
            InitializeComponent();

            _parkingFirstRiverbank = new PictureBox[_numberOfParkingSpaces] { pictureBox2, pictureBox3, pictureBox4,
                                                                        pictureBox6, pictureBox7, pictureBox8, pictureBox9,
                                                                        pictureBox10, pictureBox11, pictureBox12};
            _parkingSecondRiverbank = new PictureBox[_numberOfParkingSpaces] { pictureBox21, pictureBox20, pictureBox19,
                                                                        pictureBox5, pictureBox13, pictureBox14, pictureBox15,
                                                                        pictureBox16,pictureBox17,pictureBox18 };
            _firstRiverbankLabels = new Label[_numberOfParkingSpaces] {label1, label3, label4, label5, label6, label7,
                                                                        label8, label9, label10, label11 };
            _secondRiverbankLabels = new Label[_numberOfParkingSpaces] {label12, label13, label14, label15, label16, label17,
                                                                        label18, label19, label20, label21 };
            _ferryParkingSpaces = new PictureBox[_ferryCapacity] {pictureBox23, pictureBox24, pictureBox25,
                                                pictureBox26, pictureBox27, pictureBox28};
            _ferry = new Ferry(promPictureBox, _ferryParkingSpaces, _departureReasonLbl);

            CosmeticChanges();
            Thread mainThread = new Thread(MainThreadMethod);
            mainThread.Start();
        }

        private void MainThreadMethod()
        {
            Thread carGenerator = new Thread(GenerateCars);
            carGenerator.Start();

            while (true)
            {
                if (_ferry.RiverBank == 1 && _carsFirstRiverbank.Count >= _ferryCapacity)
                {
                    if (_ferry.Cars.Count == _ferryCapacity)
                    {
                        _departureReasonLbl.Invoke((Action)(() => _departureReasonLbl.Text = "Ferry is full."));
                        _ferry.Travel();
                    }
                }
                else if (_ferry.RiverBank == 2 && _carsSecondRiverbank.Count >= _ferryCapacity)
                {
                    if (_ferry.Cars.Count == _ferryCapacity)
                    {
                        _departureReasonLbl.Invoke((Action)(() =>
                            _departureReasonLbl.Text = "Ferry is full."));
                        _ferry.Travel();
                    }
                }
                else if (_ferry.RiverBank == 1 && _carsSecondRiverbank.Count >= _ferryCapacity)
                {
                    int numberOfCarsFirstRiverbank = _carsFirstRiverbank.Count;
                    while (numberOfCarsFirstRiverbank != _ferry.Cars.Count)
                    {

                    }
                    _departureReasonLbl.Invoke((Action)(() => 
                        _departureReasonLbl.Text = "Opposite riverbank has enough cars" +
                        " to fully fill the ferry."));
                    _ferry.Travel();
                    
                }
                else if (_ferry.RiverBank == 2 && _carsFirstRiverbank.Count >= _ferryCapacity)
                {
                    int numberOfCarsSecondRiverbank = _carsSecondRiverbank.Count;
                    while (numberOfCarsSecondRiverbank != _ferry.Cars.Count)
                    {

                    }
                    _departureReasonLbl.Invoke((Action)(() =>
                        _departureReasonLbl.Text = "Opposite riverbank has enough cars" +
                        " to fully fill the ferry."));
                    _ferry.Travel();
                    
                }
                else if (_ferry.WaitStopwatch.ElapsedMilliseconds > _ferry.PatienceThreshold)
                {
                    _departureReasonLbl.Invoke((Action)(() =>
                        _departureReasonLbl.Text = "Ferry has lost patience."));
                    _ferry.Travel();
                }

                ClearCars();
            }
        }

        private void ClearCars()
        {
            foreach (var car in _carsFirstRiverbank.ToArray())
            {
                if(car is not null)
                {
                    if (car.TravelFinished == true)
                    {
                        lock (LockObj)
                        {
                            _carsFirstRiverbank.Remove(car);
                            _ferry.Cars.Remove(car);
                        }
                    }
                }
            }

            foreach (var car in _carsSecondRiverbank.ToArray())
            {
                if (car is not null)
                {
                    if (car.TravelFinished == true)
                    {
                        lock (LockObj)
                        {
                            _carsSecondRiverbank.Remove(car);
                            _ferry.Cars.Remove(car);
                        }
                    }
                }
            }
        }

        private void GenerateCars()
        {
            Random rng = new Random();
            Car car;
            int pictureBoxId;
            while (true)
            {
                if (rng.Next(20) < 10)
                {
                    pictureBoxId = _firstRiverbankIndex % (_numberOfParkingSpaces - 1);
                    car = new Car(_carId, 1, _ferry, _parkingFirstRiverbank[pictureBoxId],
                        _firstRiverbankLabels[pictureBoxId], _onBoardPlacesAvailable);

                    _parkingFirstRiverbank[pictureBoxId].Invoke((Action)(() => ShowCarFirstRiverbank(pictureBoxId)));

                    lock (LockObj)
                    {
                        _carsFirstRiverbank.Add(car);
                    }
                    car.Start();
                    _firstRiverbankIndex++;
                }
                else
                {
                    pictureBoxId = _secondRiverbankIndex % (_numberOfParkingSpaces - 1);
                    car = new Car(_carId, 2, _ferry, _parkingSecondRiverbank[pictureBoxId],
                        _secondRiverbankLabels[pictureBoxId], _onBoardPlacesAvailable);

                    _parkingSecondRiverbank[pictureBoxId].Invoke((Action)(() => ShowCarSecondRiverbank(pictureBoxId)));

                    lock (LockObj)
                    {
                        _carsSecondRiverbank.Add(car);
                    }
                    car.Start();
                    _secondRiverbankIndex++;
                }
                Thread.Sleep(rng.Next(1000, 1750));
                _carId++;
            }
        }

        private void ShowCarFirstRiverbank(int pictureBoxId)
        {
            _parkingFirstRiverbank[pictureBoxId].Image = Resources.Car;
            _firstRiverbankLabels[pictureBoxId].Text = $"{_carId}";
        }
        private void ShowCarSecondRiverbank(int pictureBoxId)
        {
            _parkingSecondRiverbank[pictureBoxId].Image = Resources.Car;
            _secondRiverbankLabels[pictureBoxId].Text = $"{_carId}";
        }

        private void CosmeticChanges()
        {
            _departureReasonLbl.Parent = pictureBox1;
            _departureReasonLbl.BackColor = Color.Transparent;
            _departureReasonLbl.ForeColor = Color.White;

            foreach(var label in _firstRiverbankLabels)
            {
                label.Text = "";
            }

            foreach (var label in _secondRiverbankLabels)
            {
                label.Text = "";
            }
        }
    }
}
