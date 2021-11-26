using SantaFactory.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SantaFactory.Entities
{
    class Car : Toy
    {
        protected override void DrawImage(Graphics g)
        {
            Image imageFile = Image.FromFile("Images/car.png"); //@"Images\car.png"
            g.DrawImage(imageFile, new Rectangle(0, 0, Width, Height)); //(image, 0, 0, Width, Height)
        }
    }
}
