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

namespace ComputerGrafic.Lab1.WinForm
{
    public partial class Form1 : Form
    {
        List<Point> points = new List<Point>();
        Point firstPoint;
        private Bitmap bm;
        public Form1()
        {
            InitializeComponent();
        }

        //Метод, устанавливающий пиксел на форме с заданными цветом и прозрачностью
        private static void PutPixel(Graphics g, Color col, int x, int y, int alpha)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(alpha, col)), x, y, 1, 1);
        }

        //Алгоритм брезенхема
        protected void Bresenhaim_alg(Graphics g, Point a, Point b, Color color)
        {
            int dx = (b.X - a.X) >= 0 ? 1 : -1;
            int dy = (b.Y - a.Y) >= 0 ? 1 : -1;

            int lengthX = Math.Abs(b.X - a.X);
            int lengthY = Math.Abs(b.Y - a.Y);

            int length = Math.Max(lengthX, lengthY);

            if (length == 0)
            {
                PutPixel(g, color, a.X, a.Y, 255 );
                return;
            }

            if (lengthY <= lengthX)
            {
                int x = a.X;
                int y = a.Y;
                int d = -lengthX;

                length++;
                while (length != 0)
                {
                    PutPixel(g, color, x, y, 255);
                    x += dx;
                    d += 2 * lengthY;
                    if (d > 0)
                    {
                        d -= 2 * lengthX;
                        y += dy;
                    }

                    length--;
                }
            }
            else
            {
                int x = a.X;
                int y = a.Y;
                int d = -lengthY;

                length++;
                while (length != 0)
                {
                    PutPixel(g, color, x, y, 255);
                    y += dy;
                    d += 2 * lengthX;

                    if (d > 0)
                    {
                        d -= 2 * lengthY;
                        x += dx;
                    }

                    length--;
                }
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //Инициализируем объект графики
            Graphics g = pictureBox1.CreateGraphics();

            if (e.Button == MouseButtons.Left)
            {
                //"Создаем" точку, отрисовываем и добавляем в коллекцию
                Pen pen = new Pen(Color.Black, 1);
                Point point = new Point(e.X, e.Y);
                Rectangle rect1 = new Rectangle(point, new Size(1, 1));
                points.Add(point);
                g.DrawEllipse(pen, rect1);
            }
            else if (e.Button == MouseButtons.Right)
            {
                //По нажатию на правую кнопку мыши, очищаем поле и отрисовываем новую фигуру по точкам
                g.Clear(Color.WhiteSmoke);

                //Перебираем точки и рисуем фигуру, алгоритмом брезенхема
                for (int i = 0; i < points.Count - 1; i++)
                {
                    Bresenhaim_alg(g, points[i], points[i+1], Color.Red);
                }

                //Соединяем первую и последнюю точки и очищаем колекцию точек
                Bresenhaim_alg(g, points.First(), points.Last(), Color.Red);
                points = new List<Point>(){};
            }
        }
    }
}
