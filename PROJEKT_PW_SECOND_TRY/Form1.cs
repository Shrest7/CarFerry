using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJEKT_PW_SECOND_TRY
{
    public partial class Form1 : Form
    {
        public const string sciezkaGrafikiSamochodu = "C:\\Users\\dravi\\source\\repos\\" +
           "PROJEKT_PW_SECOND_TRY\\PROJEKT_PW_SECOND_TRY\\samochod.png";
        public const int iloscMiejscParkingowych = 10;
        public static PictureBox[] parkingPierwszyBrzeg;
        public static PictureBox[] parkingDrugiBrzeg;
        public static PictureBox[] miejscaPromu;
        public static Form1 formInstance;
        public static int index;
        public const int maxPojemnoscPromu = 3;
        public static SemaphoreSlim wolne = new SemaphoreSlim(maxPojemnoscPromu, maxPojemnoscPromu);
        public static List<Samochod> samochodyPierwszyBrzeg = new List<Samochod>();
        public static List<Samochod> samochodyDrugiBrzeg = new List<Samochod>();
        static Prom prom;
        static Random rng = new Random();

        public Form1()
        {
            InitializeComponent();
            prom = new Prom(promPictureBox);
            formInstance = this;
            parkingPierwszyBrzeg = new PictureBox[iloscMiejscParkingowych] { pictureBox2, pictureBox3, pictureBox4,
                                                                        pictureBox6, pictureBox7, pictureBox8,pictureBox9,
                                                                        pictureBox10,pictureBox11,pictureBox12};
            parkingDrugiBrzeg = new PictureBox[iloscMiejscParkingowych] { pictureBox21, pictureBox20, pictureBox19,
                                                                        pictureBox5, pictureBox13, pictureBox14, pictureBox15,
                                                                        pictureBox16,pictureBox17,pictureBox18 };
            miejscaPromu = new PictureBox[6] {pictureBox23, pictureBox24, pictureBox25,
                                                pictureBox26, pictureBox27, pictureBox28};
            formInstance.Show();
            Thread watekGlowny = new Thread(Test);
            watekGlowny.Start();
        }

        public void Test()
        {
            //Thread generatorSamochodow = new Thread(GenerujSamochody);
            //generatorSamochodow.Start();

            while (true)
            {
                GenerujSamochody();
                if (prom.Brzeg == 1 && samochodyPierwszyBrzeg.Count >= maxPojemnoscPromu)
                {
                    //Thread.Sleep(500);
                    Console.WriteLine("Prom sie zapelnil samochodami");
                    prom.Plyn();
                }
                else if (prom.Brzeg == 2 && samochodyDrugiBrzeg.Count >= maxPojemnoscPromu)
                {
                    //Thread.Sleep(500);
                    Console.WriteLine("Prom sie zapelnil samochodami");
                    prom.Plyn();
                }
                else if (prom.Brzeg == 1 && samochodyDrugiBrzeg.Count >= maxPojemnoscPromu)
                {
                    //Thread.Sleep(500);
                    Console.WriteLine("Przeciwny (drugi) brzeg jest pelen.");
                    prom.Plyn();
                }
                else if (prom.Brzeg == 2 && samochodyPierwszyBrzeg.Count >= maxPojemnoscPromu)
                {
                    //Thread.Sleep(500);
                    Console.WriteLine("Przeciwny (pierwszy) brzeg jest pelen.");
                    prom.Plyn();
                }
                else if (3000 > prom.ProgCierpliwosci) //TO BE CHANGED IN WINFORMS
                {
                    //Thread.Sleep(500);
                    Console.WriteLine("Prom sie wkurwil");
                    prom.Plyn();
                }

                UsunSamochody();
            }
        }

        private static void UsunSamochody()
        {
            foreach (var samochod in samochodyPierwszyBrzeg.ToArray())
            {
                if(samochod is not null)
                {
                    if (samochod.PrzeprawaUkonczona == true)
                    {
                        samochodyPierwszyBrzeg.Remove(samochod);
                        prom.Samochody.Remove(samochod);
                    }
                }
            }

            foreach (var samochod in samochodyDrugiBrzeg.ToArray())
            {
                if (samochod is not null)
                {
                    if (samochod.PrzeprawaUkonczona == true)
                    {
                        samochodyDrugiBrzeg.Remove(samochod);
                        prom.Samochody.Remove(samochod);
                    }
                }
            }
        }

        private static void GenerujSamochody()
        {
            Samochod samochod;
            int idPictureBoxa;
            //while (true)
            {
                idPictureBoxa = index % (iloscMiejscParkingowych - 1);
                if (rng.Next(20) < 10)
                {
                    samochod = new Samochod(index, 1, prom, parkingPierwszyBrzeg[idPictureBoxa]);

                    parkingPierwszyBrzeg[idPictureBoxa].Invoke((Action)(() => WyswietlSamochod1()));
                    samochodyPierwszyBrzeg.Add(samochod);
                    samochod.Start();
                }
                else
                {
                    samochod = new Samochod(index, 2, prom, parkingDrugiBrzeg[idPictureBoxa]);

                    parkingDrugiBrzeg[idPictureBoxa].Invoke((Action)(() => WyswietlSamochod2()));
                    samochodyDrugiBrzeg.Add(samochod);
                    samochod.Start();
                }
                Thread.Sleep(rng.Next(1500, 3500));
                index++;
            }
        }

        public static void WyswietlSamochod1()
        {
            parkingPierwszyBrzeg[index % (iloscMiejscParkingowych - 1)].Image = Image.FromFile(sciezkaGrafikiSamochodu);
        }
        public static void WyswietlSamochod2()
        {
            parkingDrugiBrzeg[index % (iloscMiejscParkingowych - 1)].Image = Image.FromFile(sciezkaGrafikiSamochodu);
        }

    }
}
