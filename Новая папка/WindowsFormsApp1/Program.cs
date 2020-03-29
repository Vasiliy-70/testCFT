using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class Car
    { 
        public int Id { get; set; } //Идентификационный номер для БД
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
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
