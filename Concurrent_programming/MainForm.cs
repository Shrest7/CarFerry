using PROJEKT_PW_WINFORMS.Properties;
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
        public readonly static object listObj = new object();
        public const int iloscMiejscParkingowych = 10;
        public PictureBox[] parkingPierwszyBrzeg;
        public PictureBox[] parkingDrugiBrzeg;
        public Label[] labelePierwszyBrzeg;
        public Label[] labeleDrugiBrzeg;
        private readonly PictureBox[] miejscaPromu;
        private readonly SemaphoreSlim wolneMiejsca =
            new SemaphoreSlim(maxPojemnoscPromu, maxPojemnoscPromu);
        public int indexSamochodu;
        public int indexP1;
        public int indexP2;
        public const int maxPojemnoscPromu = 4;
        public List<Samochod> samochodyPierwszyBrzeg = new List<Samochod>();
        public List<Samochod> samochodyDrugiBrzeg = new List<Samochod>();
        private readonly Prom prom;
        private readonly Random rng = new Random();

        public MainForm()
        {
            InitializeComponent();
            parkingPierwszyBrzeg = new PictureBox[iloscMiejscParkingowych] { pictureBox2, pictureBox3, pictureBox4,
                                                                        pictureBox6, pictureBox7, pictureBox8, pictureBox9,
                                                                        pictureBox10, pictureBox11, pictureBox12};
            labelePierwszyBrzeg = new Label[iloscMiejscParkingowych] {label1, label3, label4, label5, label6, label7,
                                                                        label8, label9, label10, label11 };
            labeleDrugiBrzeg = new Label[iloscMiejscParkingowych] {label12, label13, label14, label15, label16, label17,
                                                                        label18, label19, label20, label21 };
            parkingDrugiBrzeg = new PictureBox[iloscMiejscParkingowych] { pictureBox21, pictureBox20, pictureBox19,
                                                                        pictureBox5, pictureBox13, pictureBox14, pictureBox15,
                                                                        pictureBox16,pictureBox17,pictureBox18 };
            miejscaPromu = new PictureBox[6] {pictureBox23, pictureBox24, pictureBox25,
                                                pictureBox26, pictureBox27, pictureBox28};
            prom = new Prom(promPictureBox, miejscaPromu, lblPowodOdplyniecia);
            ZmianyKosmetyczne();

            Thread watekGlowny = new Thread(FunkcjaGlownegoWatka);
            watekGlowny.Start();
        }

        public void FunkcjaGlownegoWatka()
        {
            Thread generatorSamochodow = new Thread(GenerujSamochody);
            generatorSamochodow.Start();

            while (true)
            {
                if (prom.Brzeg == 1 && samochodyPierwszyBrzeg.Count >= maxPojemnoscPromu)
                {
                    if (prom.Samochody.Count == maxPojemnoscPromu)
                    {
                        lblPowodOdplyniecia.Invoke((Action)(() => lblPowodOdplyniecia.Text = "Prom zapelnil sie samochodami"));
                        prom.Plyn();
                    }
                }
                else if (prom.Brzeg == 2 && samochodyDrugiBrzeg.Count >= maxPojemnoscPromu)
                {
                    if (prom.Samochody.Count == maxPojemnoscPromu)
                    {
                        lblPowodOdplyniecia.Invoke((Action)(() => lblPowodOdplyniecia.Text = "Prom zapelnil sie samochodami"));
                        prom.Plyn();
                    }
                }
                else if (prom.Brzeg == 1 && samochodyDrugiBrzeg.Count >= maxPojemnoscPromu)
                {
                    int stanBrzegu = samochodyPierwszyBrzeg.Count;
                    while (stanBrzegu != prom.Samochody.Count)
                    {

                    }
                    lblPowodOdplyniecia.Invoke((Action)(() => lblPowodOdplyniecia.Text = "Przeciwny (drugi) brzeg jest pelen."));
                    prom.Plyn();
                    
                }
                else if (prom.Brzeg == 2 && samochodyPierwszyBrzeg.Count >= maxPojemnoscPromu)
                {
                    int stanBrzegu = samochodyDrugiBrzeg.Count;
                    while (stanBrzegu != prom.Samochody.Count)
                    {

                    }
                    lblPowodOdplyniecia.Invoke((Action)(() => lblPowodOdplyniecia.Text = "Przeciwny (pierwszy) brzeg jest pelen."));
                    prom.Plyn();
                    
                }
                else if (prom.czasomierzOczekiwania.ElapsedMilliseconds > prom.ProgCierpliwosci)
                {
                    lblPowodOdplyniecia.Invoke((Action)(() => lblPowodOdplyniecia.Text = "Prom stracil cierpliwosc."));
                    prom.Plyn();
                }

                UsunSamochody();
            }
        }

        private void UsunSamochody()
        {
            foreach (var samochod in samochodyPierwszyBrzeg.ToArray())
            {
                if(samochod is not null)
                {
                    if (samochod.PrzeprawaUkonczona == true)
                    {
                        lock (listObj)
                        {
                            samochodyPierwszyBrzeg.Remove(samochod);
                            prom.Samochody.Remove(samochod);
                        }
                    }
                }
            }

            foreach (var samochod in samochodyDrugiBrzeg.ToArray())
            {
                if (samochod is not null)
                {
                    if (samochod.PrzeprawaUkonczona == true)
                    {
                        lock (listObj)
                        {
                            samochodyDrugiBrzeg.Remove(samochod);
                            prom.Samochody.Remove(samochod);
                        }
                    }
                }
            }
        }

        private void GenerujSamochody()
        {
            Samochod samochod;
            int idPictureBoxa;
            while (true)
            {
                if (rng.Next(20) < 10)
                {
                    idPictureBoxa = indexP1 % (iloscMiejscParkingowych - 1);
                    samochod = new Samochod(indexSamochodu, 1, prom, parkingPierwszyBrzeg[idPictureBoxa],
                        labelePierwszyBrzeg[idPictureBoxa], wolneMiejsca);

                    parkingPierwszyBrzeg[idPictureBoxa].Invoke((Action)(() => WyswietlSamochodPierwszyBrzeg(idPictureBoxa)));

                    lock (listObj)
                    {
                        samochodyPierwszyBrzeg.Add(samochod);
                    }
                    samochod.Start();
                    indexP1++;
                }
                else
                {
                    idPictureBoxa = indexP2 % (iloscMiejscParkingowych - 1);
                    samochod = new Samochod(indexSamochodu, 2, prom, parkingDrugiBrzeg[idPictureBoxa],
                        labeleDrugiBrzeg[idPictureBoxa], wolneMiejsca);

                    parkingDrugiBrzeg[idPictureBoxa].Invoke((Action)(() =>WyswietlSamochodDrugiBrzeg(idPictureBoxa)));

                    lock (listObj)
                    {
                        samochodyDrugiBrzeg.Add(samochod);
                    }
                    samochod.Start();
                    indexP2++;
                }
                Thread.Sleep(rng.Next(1500, 2250));
                indexSamochodu++;
            }
        }

        public void WyswietlSamochodPierwszyBrzeg(int idPictureBoxa)
        {
            parkingPierwszyBrzeg[idPictureBoxa].Image = Resources.samochod;
            labelePierwszyBrzeg[idPictureBoxa].Text = $"{indexSamochodu}";
        }
        public void WyswietlSamochodDrugiBrzeg(int idPictureBoxa)
        {
            parkingDrugiBrzeg[idPictureBoxa].Image = Resources.samochod;
            labeleDrugiBrzeg[idPictureBoxa].Text = $"{indexSamochodu}";
        }

        public void ZmianyKosmetyczne()
        {
            label2.Parent = pictureBox1;
            label2.BackColor = Color.Transparent;
            lblPowodOdplyniecia.Parent = pictureBox1;
            lblPowodOdplyniecia.BackColor = Color.Transparent;
            label2.ForeColor = Color.White;
            lblPowodOdplyniecia.ForeColor = Color.White;

            foreach(var label in labelePierwszyBrzeg)
            {
                label.Text = "";
            }

            foreach (var label in labeleDrugiBrzeg)
            {
                label.Text = "";
            }
        }
    }
}
