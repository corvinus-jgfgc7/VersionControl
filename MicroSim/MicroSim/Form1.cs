using MicroSim.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicroSim
{
    public partial class Form1 : Form
    {
        Random rng = new Random(1234);

        List<Person> Population = null; //= new List<Person>();
        List<BirthProbability> BirthProbabilities = null;
        List<DeathProbability> DeathProbabilities = null;

        List<int> CountOfMales;
        List<int> CountOfFemales;

        public Form1()
        {
            InitializeComponent();

            BirthProbabilities = GetBirthprobabilities(@"C:\Temp\születés.csv");
            DeathProbabilities = GetDeathprobabilities(@"C:\Temp\halál.csv");

        }

        private void SimStep(int year, Person person)
        {
            //Ha halott akkor kihagyjuk, ugrunk a ciklus következő lépésére
            if (!person.IsAlive) return;

            // Letároljuk az életkort, hogy ne kelljen mindenhol újraszámolni
            byte age = (byte)(year - person.BirthYear);

            // Halál kezelése
            // Halálozási valószínűség kikeresése
            double pDeath = (from x in DeathProbabilities
                             where x.Gender == person.Gender && x.Age == age
                             select x.P).FirstOrDefault();
            // Meghal a személy?
            if (rng.NextDouble() <= pDeath)
                person.IsAlive = false;

            //Születés kezelése - csak az élő nők szülnek
            if (person.IsAlive && person.Gender == Gender.Female)
            {
                //Szülési valószínűség kikeresése
                double pBirth = (from x in BirthProbabilities
                                 where x.Age == age
                                 select x.P).FirstOrDefault();
                //Születik gyermek?
                if (rng.NextDouble() <= pBirth)
                {
                    Person újszülött = new Person();
                    újszülött.BirthYear = year;
                    újszülött.NbrOfChildren = 0;
                    újszülött.Gender = (Gender)(rng.Next(1, 3));
                    Population.Add(újszülött);
                }
            }
        }

        public List<Person> GetPopulation(string csvPath)
        {
            List<Person> population = new List<Person>();

            using (StreamReader sr = new StreamReader(csvPath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');

                    population.Add(new Person()
                    {
                        BirthYear = int.Parse(line[0]),
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
                        NbrOfChildren = int.Parse(line[2])
                    });

                }
            }

            return population;
        }

        public List<BirthProbability> GetBirthprobabilities(string csvPath)
        {
            List<BirthProbability> birthProbabilities = new List<BirthProbability>();

            using (StreamReader sr = new StreamReader(csvPath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');

                    birthProbabilities.Add(new BirthProbability()
                    {
                        Age = byte.Parse(line[0]),
                        NbrOfChildren = int.Parse(line[1]),
                        P = double.Parse(line[2])
                    });

                }
            }

            return birthProbabilities;
        }

        public List<DeathProbability> GetDeathprobabilities(string csvPath)
        {
            List<DeathProbability> deathProbabilities = new List<DeathProbability>();

            using (StreamReader sr = new StreamReader(csvPath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split(';');

                    deathProbabilities.Add(new DeathProbability()
                    {
                        Gender = (Gender)Enum.Parse(typeof(Gender), line[0]),
                        Age = byte.Parse(line[1]),
                        P = double.Parse(line[2])
                    });

                }
            }

            return deathProbabilities;
        }




        private void StartSimulation(int endYear, string csvPath)
        {
            txtMain.Text = "";
            CountOfMales = new List<int>();
            CountOfFemales = new List<int>();

            Population = GetPopulation(@"C:\Temp\nép-teszt.csv");

            for (int year = Convert.ToInt32(nudYear.Value); year <= 2024; year++)
            {
                for (int i = 0; i < Population.Count; i++)
                {
                    SimStep(year, Population[i]);
                }

                int nbrOfMales = (from x in Population
                                  where x.Gender == Gender.Male && x.IsAlive
                                  select x).Count();
                int nbrOfFemales = (from x in Population
                                    where x.Gender == Gender.Female && x.IsAlive
                                    select x).Count();

                CountOfMales.Add(nbrOfMales);
                CountOfFemales.Add(nbrOfFemales);

                Console.WriteLine(
                    string.Format("Év: {0}\nFiúk: {1}\nLányok: {2}\n", year, nbrOfMales, nbrOfFemales));
            }

            //List<int> Males = nbrOfMales.ToList();

        }

        private void DisplayResults()
        {
            int i = 0;
            string kiiras = "";
            for (int year = Convert.ToInt32(nudYear.Value); year <= 2024; year++)
            {
                
                kiiras += string.Format("Év: {0}\n\tFiúk: {1}\n\tLányok: {2}\n\n", year, CountOfMales[i], CountOfFemales[i]);
                i++;
            }
            txtMain.Text = kiiras;
        }


        private void startBtn_Click(object sender, EventArgs e)
        {


            StartSimulation((int)nudYear.Value, txtPath.Text);

            DisplayResults();
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.FileName = txtPath.Text;

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            txtPath.Text = ofd.FileName;
        }
    }
}
