using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace СomputerGrafic.Lab3.WinForm
{
    public partial class Form1 : Form
    {
        //Кортеж с точками и списки для их хранения (линии описывают можно сказать p.s. границы)
        Point prevDot;
        List<Point> dots = new List<Point>(100);

        //Прямоугольник, которые будет отрисовывать и служить границами
        Rectangle tmpRectangle;

        Graphics g;
        Pen pen = new Pen(Brushes.Blue, 2f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
        Bitmap _pixels;

        //Для алгоритма константы 
        const int LEFT = 8;
        const int RIGHT = 4;
        const int BOT = 2;
        const int TOP = 1;

        //Начальная точка для прямоуга
        Point basePoint;
        public Form1()
        {
            InitializeComponent();
            _pixels = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(_pixels);
        }

        
        private void BresenhaimAlg(Point a, Point b, Color color)
        {
            int dx = (b.X - a.X) >= 0 ? 1 : -1;
            int dy = (b.Y - a.Y) >= 0 ? 1 : -1;

            int lengthX = Math.Abs(b.X - a.X);
            int lengthY = Math.Abs(b.Y - a.Y);

            int length = Math.Max(lengthX, lengthY);

            if (length == 0)
            {
                _pixels.SetPixel(a.X, a.Y, color);
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
                    _pixels.SetPixel(x, y, color);
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
                    _pixels.SetPixel(x, y, color);
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

        //По умолчанию - соединяем линии
        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _pixels.SetPixel(e.X, e.Y, Color.FromArgb(0, 0, 0));
                pictureBox1.Image = _pixels;
                pictureBox1.Invalidate(new Rectangle());
                if (!prevDot.IsEmpty)
                {
                    BresenhaimAlg(new Point(prevDot.X, prevDot.Y),
                        new Point(e.X, e.Y), Color.Red);
                    pictureBox1.Image = _pixels;
                    pictureBox1.Invalidate();
                }

                //Сохраняем точки в листы
                dots.Add(new Point(e.X, e.Y));
                prevDot = new Point(e.X, e.Y);
            }
        }
        private void onMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                pictureBox1.Image = _pixels;
                _pixels = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                int quantity = dots.Count;
                if (quantity > 1)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        if (i != 0)
                        {
                            BresenhaimAlg(new Point(dots[i - 1].X, dots[i - 1].Y),
                                new Point(dots[i].X, dots[i].Y),
                                Color.Red);
                        }
                    }
                }
                pictureBox1.Paint -= PaintRect;
                pictureBox1.Paint += PaintDashedRect;
                basePoint = e.Location;
            }
        }

        //Квадратик отрисовываем при зажатии
        void PaintRect(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, tmpRectangle);
            g.Dispose();
        }
        private void PaintDashedRect(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(pen, tmpRectangle);
        }

        Rectangle GetSelRectangle(Point orig, Point location)
        {
            //Расчитываем пряямоугл по точке, где нажал и по точке, где отпустил 
            int deltaX = location.X - orig.X, deltaY = location.Y - orig.Y;
            Size size = new Size(Math.Abs(deltaX), Math.Abs(deltaY));

            Rectangle rectangle = new Rectangle();

            if (deltaX >= 0 & deltaY >= 0)
            {
                rectangle = new Rectangle(orig, size);
            }
            else if (deltaX < 0 & deltaY > 0)
            {
                rectangle = new Rectangle(location.X, orig.Y, size.Width, size.Height);
            }
            else if
                (deltaX < 0 & deltaY < 0)
            {
                rectangle = new Rectangle(location, size);
            }
            else if (deltaX > 0 & deltaY < 0)
            {
                rectangle = new Rectangle(orig.X, location.Y, size.Width, size.Height);
            }

            return rectangle;
        }

        //По движению мышки обновляем дэшед прямоугольник
        private void onMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                tmpRectangle = GetSelRectangle(basePoint, e.Location);
                (sender as PictureBox).Refresh();
            }
        }

        private void onMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                pictureBox1.Paint -= PaintDashedRect;
                pictureBox1.Paint += PaintRect;
                pictureBox1.Invalidate(new Rectangle());
                if (dots.Count > 1)
                {
                    for (int i = 0; i < dots.Count; i++)
                    {
                        if (i != 0)
                        {
                            CohenSutherlend(tmpRectangle, dots[i - 1], dots[i], i);
                        }
                    }
                }

                pictureBox1.Image = _pixels;
                pictureBox1.Invalidate(new Rectangle());
            }
        }

        //Алгоритм Коэна Сазерленда
        public int CohenSutherlend(Rectangle rect, Point point1, Point point2, int i)
        {
            //Проверяем границы и отсекаем отрезки
            int codeA, codeB, code;

            //Проверяем с какой стороны от выделенной области лежит прямая
            codeA = GetDotCode(rect, point1.X, point1.Y);
            codeB = GetDotCode(rect, point2.X, point2.Y);
            Point tmpPoint = point1;
            while (Convert.ToBoolean(codeA | codeB))
            {
                if (Convert.ToBoolean(codeA & codeB))
                {
                    return -1;
                }

                if (Convert.ToBoolean(codeA))
                {
                    code = codeA;
                    tmpPoint = point1;
                }
                else
                {
                    code = codeB;
                    tmpPoint = point2;
                }

                int x = tmpPoint.X;
                int y = tmpPoint.Y;

                if (Convert.ToBoolean(code & LEFT))
                {
                    y += (point1.Y - point2.Y) * (rect.X - x) / (point1.X - point2.X);
                    x = rect.X;
                }
                else if (Convert.ToBoolean(code & RIGHT))
                {
                    y += (point1.Y - point2.Y) * ((rect.X + rect.Width) - x) / (point1.X - point2.X);
                    x = (rect.X + rect.Width);
                }

                if (Convert.ToBoolean(code & BOT))
                {
                    x += (point1.X - point2.X) * (rect.Y - y) / (point1.Y - point2.Y);
                    y = rect.Y;
                }
                else if (Convert.ToBoolean(code & TOP))
                {
                    x += (point1.X - point2.X) * ((rect.Y + rect.Height) - y) / (point1.Y - point2.Y);
                    y = (rect.Y + rect.Height);
                }

                tmpPoint = new Point(x, y);

                if (Convert.ToBoolean(code == codeA))
                {
                    point1 = tmpPoint;
                    codeA = GetDotCode(rect, point1.X, point1.Y);
                }
                else
                {
                    point2 = tmpPoint;
                    codeB = GetDotCode(rect, point2.X, point2.Y);
                }
            }

            BresenhaimAlg(new Point(point1.X, point1.Y),
                new Point(point2.X, point2.Y), Color.Blue);

            return 0;
        }

        public int GetDotCode(Rectangle rect, int x, int y)
        {
            //Console.WriteLine((x < rect.X ? LEFT : 0).ToString());
            //Console.WriteLine((x > (rect.X + rect.Width) ? RIGHT : 0).ToString());
            //Console.WriteLine((y < rect.Y ? BOT : 0).ToString());
            //Console.WriteLine((y > (rect.Y + rect.Height) ? TOP : 0).ToString());
            return (x < rect.X ? LEFT : 0) + (x > (rect.X + rect.Width) ? RIGHT : 0) + (y < rect.Y ? BOT : 0) +
                   (y > (rect.Y + rect.Height) ? TOP : 0);
        }
    }
}
