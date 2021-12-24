using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGrafic.Lab0.WinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private List<Point> points = new List<Point>();
        private void Form1_Load(object sender, EventArgs e)
        {
            //Вешаем на клик обработчик события
            MouseClick += new MouseEventHandler(Form1_Load);
        }

        //Метод срабатывающий по клику на форму
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //Создаем экземпляр класса графики
            Graphics g = pictureBox1.CreateGraphics();
            //Рисовальщик
            Pen pen = new Pen(Color.Black, 2);
            Point point = new Point(e.X, e.Y);
            Rectangle rect1 = new Rectangle(point, new Size(1, 1));

            if (e.Button == MouseButtons.Left)
            {
                points.Add(point);
                g.DrawEllipse(pen, rect1);
            }
            else if (e.Button == MouseButtons.Right)
            {
                Random rand = new Random();
                pen = new Pen(Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255)));
                foreach (var p in points)
                {
                    rect1 = new Rectangle(p, new Size(1, 1));
                    g.DrawEllipse(pen, rect1);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
