using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGrafic.Lab4.WinForm
{
    public partial class Form1 : Form
    {
        private Bitmap _pixels;
        //Точки для соед линий
        private List<Point> _points = new List<Point>();
        //Список списков точек для линий внутри фигуры
        private List<List<Point>> pointsInLine = new List<List<Point>>();
        public Form1()
        {
            InitializeComponent();
            //Примерно заполняем
            for (int i = 0; i < 1000; i++)
            {
                pointsInLine.Add(new List<Point>(1000));
            }
            _pixels = new Bitmap(pictureBox.Width, pictureBox.Height);
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            pictureBox.Image = _pixels;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    _pixels.SetPixel(e.X, e.Y, Color.Black);
                    _points.Add(new Point(e.X, e.Y));
                    //Сразу соед
                    if (_points.Count > 1)
                    {
                        Bresenhaim_alg(_points[_points.Count - 2], _points.Last(), Color.Black);
                    }
                    break;
                case MouseButtons.Right:
                    if (_points.Count > 0)
                    {
                        //Соединяем первую и последнюю точки
                        Bresenhaim_alg(_points[0], _points[_points.Count - 1], Color.Black);
                        //Закрашиваем
                        if (_points.Count > 2)
                        {
                            FillShape();
                        }
                        _points.Clear();
                    }
                    break;
            }
        }
        private int sortPoint(Point a, Point b)
        {
            //Возвращает значение (1,0,-1) для сортировки (по x координате)
            return a.X.CompareTo(b.X);
        }

        private void FillShape()
        {
            int minPY = _points.First().Y,
                maxPY = _points.Last().Y;

            foreach (Point p in _points)
            {
                if (p.Y < minPY)
                {
                    minPY = p.Y;
                }
                if (p.Y > maxPY)
                {
                    maxPY = p.Y;
                }
            }

            //Перебираем от минимального y до максимального  
            for (int y = minPY; y <= maxPY; y++)
            {
                bool isY2smaller;
                int yMin,
                    yMax;
                for (int i = 0; i < _points.Count; i++)
                {
                    int ptNext;

                    if (i + 1 != _points.Count)
                        ptNext = i + 1;
                    else
                        ptNext = 0;

                    if (_points[i].Y < _points[ptNext].Y)
                    {
                        yMin = _points[i].Y;
                        yMax = _points[ptNext].Y;
                        isY2smaller = false;
                    }
                    else
                    {
                        yMin = _points[ptNext].Y;
                        yMax = _points[i].Y;
                        isY2smaller = true;
                    }

                    //Добавляем точки в массив линиий
                    if (y >= yMin && y <= yMax)
                    {
                        if (_points[i].X - _points[ptNext].X == 0)
                        {
                            pointsInLine[y].Add(new Point(_points[i].X, y));
                        }
                        //Составим уравнение прямой и найдем тчк пересечения
                        else if (y != yMin)
                        {
                            //tg = 0 и y = b;
                            int tmp;
                            if (isY2smaller)
                            {
                                tmp = _points[ptNext].X - _points[i].X;
                            }
                            else
                            {
                                tmp = _points[i].X - _points[ptNext].X;
                            }
                            double k = (double)(yMin - yMax) / tmp;
                            double b = _points[i].Y - k * _points[i].X;

                            //и точку пересечения
                            pointsInLine[y].Add(new Point((int)Math.Round((b - y) / (0 - k)), y));
                        }
                    }
                }
            }

            //Также перебираем от мин y к макс y
            for (int line = minPY; line <= maxPY; line++)
            {
                pointsInLine[line].Sort(sortPoint);
                Point prevPoint = new Point();

                //Перебираем массив с массивами точек и закрашиваем точки
                foreach (Point pt in pointsInLine[line])
                {
                    if (!prevPoint.IsEmpty)
                    {
                        Bresenhaim_alg(new Point(prevPoint.X, prevPoint.Y), new Point(pt.X, pt.Y), Color.Blue);
                        prevPoint = new Point();
                    }
                    else
                    {
                        prevPoint = pt;
                    }
                }
            }
        }
        //Старый добрый 
        protected void Bresenhaim_alg(Point a, Point b, Color color)
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
    }
}
