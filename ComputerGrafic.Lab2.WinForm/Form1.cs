using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComputerGrafic.Lab2.WinForm
{
    public partial class Form1 : Form
    {
        private Rectangle tmpRectangle;

        private Graphics g;
        private Pen pen = new Pen(Brushes.Blue, 2f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
        private Bitmap bm;

        private Point basePoint;
        private Size rectangleSize;
        private Point rectStartPoint;
        public Form1()
        {
            InitializeComponent();
            bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(bm);
        }

        //Квадратик отрисовываем при зажатии
        void PaintRectAndEllipse(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, tmpRectangle);
            draw_ellipse(rectStartPoint.X + rectangleSize.Width/2, rectStartPoint.Y+rectangleSize.Height/2, rectangleSize.Width/2, rectangleSize.Height/2);
            g.Dispose();
        }

        private void BtnSelectionPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(pen, tmpRectangle);
        }

        (Rectangle, Size, Point) GetSelRectangle(Point orig, Point location)
        {
            //Расчитываем пряямоугл по точке, где нажал и по точке, где отпустил 
            int deltaX = location.X - orig.X, deltaY = location.Y - orig.Y;
            Size size = new Size(Math.Abs(deltaX), Math.Abs(deltaY));

            Rectangle rectangle = new Rectangle();

            int x = orig.X <= location.X ? orig.X : location.X;
            int y = orig.Y <= location.Y ? orig.Y : location.Y;

            rectangle = new Rectangle(new Point(x, y), size);

            return (rectangle, size, new Point(x,y));
        }

        void pixel4(int x, int y, int _x, int _y, Color color) // Рисование пикселя для первого квадранта, и, симметрично, для остальных
        {
            bm.SetPixel(x + _x, y + _y, color);
            bm.SetPixel(x + _x, y - _y, color);
            bm.SetPixel(x - _x, y - _y, color);
            bm.SetPixel(x - _x, y + _y, color);
        }

        void draw_ellipse(int x, int y, int a, int b)
        {
            int _x = 0; // Компонента x
            int _y = b; // Компонента y
            int a_sqr = a * a; // a^2, a - большая полуось
            int b_sqr = b * b; // b^2, b - малая полуось
            int delta = 4 * b_sqr * ((_x + 1) * (_x + 1)) + a_sqr * ((2 * _y - 1) * (2 * _y - 1)) - 4 * a_sqr * b_sqr; // Функция координат точки (x+1, y-1/2)
            while (a_sqr * (2 * _y - 1) > 2 * b_sqr * (_x + 1)) // Первая часть дуги
            {
                pixel4(x, y, _x, _y, Color.Red);
                if (delta < 0) // Переход по горизонтали
                {
                    _x++;
                    delta += 4 * b_sqr * (2 * _x + 3);
                }
                else // Переход по диагонали
                {
                    _x++;
                    delta = delta - 8 * a_sqr * (_y - 1) + 4 * b_sqr * (2 * _x + 3);
                    _y--;
                }
            }
            delta = b_sqr * ((2 * _x + 1) * (2 * _x + 1)) + 4 * a_sqr * ((_y + 1) * (_y + 1)) - 4 * a_sqr * b_sqr; // Функция координат точки (x+1/2, y-1)
            while (_y + 1 != 0) // Вторая часть дуги, если не выполняется условие первого цикла, значит выполняется a^2(2y - 1) <= 2b^2(x + 1)
            {
                pixel4(x, y, _x, _y, Color.Red);
                if (delta < 0) // Переход по вертикали
                {
                    _y--;
                    delta += 4 * a_sqr * (2 * _y + 3);
                }
                else // Переход по диагонали
                {
                    _y--;
                    delta = delta - 8 * b_sqr * (_x + 1) + 4 * a_sqr * (2 * _y + 3);
                    _x++;
                }
            }
        }

        private void picutreBox1_onMouseDown(object sender, MouseEventArgs e)
        {
            pictureBox1.Paint -= PaintRectAndEllipse;
            pictureBox1.Paint += BtnSelectionPaint;
            basePoint = e.Location;
        }
        //Если двигаем правой кнопкой мыши, то расчитываем каждый раз(!) и рефрешим полотно для нового квадрата
        //так и будет визуально меняться прямоугольник выделения
        private void pictureBox1_onMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //при движении мышкой считаем прямоугольник и обновляем picturebox
                (tmpRectangle, rectangleSize, rectStartPoint) = GetSelRectangle(basePoint, e.Location);
                //Привести к типу для обновления
                (sender as PictureBox).Refresh();
            }
        }

        private void pictureBox1_onMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                pictureBox1.Paint -= BtnSelectionPaint;
                pictureBox1.Paint += PaintRectAndEllipse;
                (sender as PictureBox).Refresh();
                pictureBox1.Invalidate(new Rectangle());
                pictureBox1.Image = bm;
                pictureBox1.Invalidate(new Rectangle());
            }

        }
    }
}
