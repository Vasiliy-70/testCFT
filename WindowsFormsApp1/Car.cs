using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Car
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string BodyStyle { get; set; }
        public string Image { get; set; }
        public int Year { get; set; }

        public Car(int id, string make, string model, int year, string bodyStyle, string image)
        {
            Id = id;
            Make = make;
            Model = model;
            Year = year;
            BodyStyle = bodyStyle;
            Image = image;
        }
    }
}
