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
using VaR.Entities;

namespace VaR
{
    public partial class Form1 : Form
    {
        PortfolioEntities context = new PortfolioEntities();
        List<Tick> Ticks;
        List<PortfolioItem> Portfolio = new List<PortfolioItem>();
        List<decimal> nyereségekRendezve;

        public Form1()
        {
            InitializeComponent();
            Ticks = context.Ticks.ToList();
            dataGridView1.DataSource = Ticks;

            CreatePortfolio();

            List<decimal> Nyereségek = new List<decimal>();

            int intervalum = 30; //érdemes lenne felhasználótól TextBox-ban kérni
            DateTime kezdőDátum = (from x in Ticks select x.TradingDay).Min(); //szintén adatbevitel, dateTimePicker1.Value
            DateTime záróDátum = new DateTime(2016, 12, 30); //szintén adatbevitel

            TimeSpan z = záróDátum - kezdőDátum;

            for (int i = 0; i < z.Days - intervalum; i++)
            {
                DateTime ablakZáró = kezdőDátum.AddDays(i + intervalum); //30 nappal később
                DateTime ablakNyitó = kezdőDátum.AddDays(i);

                decimal ny = GetPortfolioValue(ablakZáró) - GetPortfolioValue(ablakNyitó); //GetPortfolioValue(kezdőDátum.AddDays(i + intervalum))
                           //- GetPortfolioValue(kezdőDátum.AddDays(i));
                Nyereségek.Add(ny);
                Console.WriteLine(i + " " + ny);
            }

            nyereségekRendezve = (from x in Nyereségek
                                      orderby x
                                      select x)
                                        .ToList();
            MessageBox.Show(nyereségekRendezve[nyereségekRendezve.Count() / 5].ToString());

        }

        private void CreatePortfolio() 
        {
            Portfolio.Add(new PortfolioItem() { Index = "OTP", Volume = 10 });
            Portfolio.Add(new PortfolioItem() { Index = "ZWACK", Volume = 10 });
            Portfolio.Add(new PortfolioItem() { Index = "ELMU", Volume = 10 });

            dataGridView2.DataSource = Portfolio;

            /*PortfolioItem p = new PortfolioItem();
            p.Index = "OTP";
            p.Volume = 10;
            Portfolio.Add(p); leegyszerűsíthető 1 sorba*/
        }

        private decimal GetPortfolioValue(DateTime date)
        {
            decimal value = 0;
            foreach (var item in Portfolio)
            {
                var last = (from x in Ticks
                            where item.Index == x.Index.Trim()
                               && date <= x.TradingDay
                            orderby x.TradingDay
                            select x)
                            .First();
                value += (decimal)last.Price * item.Volume;
            }
            return value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.InitialDirectory = Application.StartupPath; //app indulókönyvtárában nyíljon meg

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (var sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8)) 
            {
                sw.WriteLine("Időszak;Nyereség");
                for (int i = 0; i < nyereségekRendezve.Count(); i++)
                {
                    sw.WriteLine(string.Format("{0};{1}", Math.Round((double)i / (double)nyereségekRendezve.Count(), 2), nyereségekRendezve[i]));
                }
            }
        }
    }
}
