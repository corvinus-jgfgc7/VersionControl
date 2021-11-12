using SantaFactory.Abstractions;
using SantaFactory.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SantaFactory
{
    public partial class Form1 : Form
    {
        List<Toy> _toys = new List<Toy>();

        private Toy _nextToy;

        private IToyFactory _factory;
        public IToyFactory ToyFactory
        {
            get { return _factory; }
            set
            {
                _factory = value;
                DisplayNext();
            }
        }

        public Form1()
        {
            InitializeComponent();
            ToyFactory = new BallFactory();

            var b = ToyFactory.CreateNew();
        }

        private void createTimer_Tick(object sender, EventArgs e)
        {
            Toy toy = (Toy)ToyFactory.CreateNew(); //Ball ball = (Ball)BallFactory.CreateNew();
            _toys.Add(toy);
            mainPanel.Controls.Add(toy);
            toy.Left = -toy.Width;
        }

        private void conveyorTimer_Tick(object sender, EventArgs e)
        {
            var lastPosition = 0;
            foreach (var item in _toys)
            {
                item.MoveToy();
                if (item.Left > lastPosition)
                {
                    lastPosition = item.Left;
                }
            }

            if (lastPosition >= 1000)
            {
                var oldestBall = _toys[0];
                _toys.Remove(oldestBall);
                mainPanel.Controls.Remove(oldestBall);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ToyFactory = new CarFactory();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ToyFactory = new BallFactory();
        }

        private void DisplayNext() 
        {
            if (_nextToy != null)
                this.Controls.Remove(_nextToy);

            _nextToy = ToyFactory.CreateNew();
            _nextToy.Top = lblNext.Top;
            _nextToy.Left = lblNext.Left + lblNext.Width;
            Controls.Add(_nextToy);
        }
    }
}
