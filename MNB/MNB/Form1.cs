using MNB.Entities;
using MNB.MnbServiceReference;
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
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace MNB
{
    public partial class Form1 : Form
    {

        BindingList<RateData> Rates = new BindingList<RateData>();
        public Form1()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            Rates.Clear();
            string xmlstring = Consume();
            LoadXML(xmlstring);
            dataGridView1.DataSource = Rates;
            Charting();
        }

        string Consume() 
        {
            MNBArfolyamServiceSoapClient mnbService = new MNBArfolyamServiceSoapClient();
            GetExchangeRatesRequestBody request = new GetExchangeRatesRequestBody();
            request.currencyNames = cbxValuta.SelectedItem.ToString(); //"EUR";
            request.startDate = TolPicker.Value.ToString("yyyy-MM-dd"); //"2020-01-01";
            request.endDate = IgPicker.Value.ToString("yyyy-MM-dd"); //"2020-06-30";
            mnbService.GetExchangeRates(request);
            var response = mnbService.GetExchangeRates(request);
            string result = response.GetExchangeRatesResult;
            return result;
        }

        private void LoadXML(string input) 
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(input);
            foreach (XmlElement item in xml.DocumentElement)
            {
                RateData r = new RateData();
                r.Date = DateTime.Parse(item.GetAttribute("date"));
                XmlElement child = (XmlElement)item.FirstChild; //ChildNodes[0]
                r.Currency = child.GetAttribute("curr");
                r.Value = decimal.Parse(child.InnerText);
                int unit = int.Parse(child.GetAttribute("unit"));
                if (unit != 0)
                {
                    r.Value = r.Value / unit;
                }
                Rates.Add(r);
            }
        }

        private void Charting() 
        {
            chartRateData.DataSource = Rates;
            var series = chartRateData.Series[0]; //lehet Series var helyett
            series.ChartType = SeriesChartType.Line;
            series.XValueMember = "Date";
            series.YValueMembers = "Value";
            series.BorderWidth = 2;

            var legend = chartRateData.Legends[0];
            legend.Enabled = false;

            var chartArea = chartRateData.ChartAreas[0];
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.AxisY.IsStartedFromZero = false;
        }

        private void filterChanged(object sender, EventArgs e)
        {
            RefreshData();
        }
    }
}
