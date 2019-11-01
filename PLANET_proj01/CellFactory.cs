using PLANET_proj01.Life;
using PLANET_proj01.UserControlls;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PLANET_proj01
{
    internal class CellFactory
    {
        public bool ColorBorn { get; set; } = true;
        public bool ColorEndangered { get; set; } = true;
        public Type Type { get; set; } = Type.Rect;
        private Func<int> GetScale;
        private Func<(int w, int h)> GetSize;

        public CellFactory(Func<int> GetScale, Func<(int w, int h)> GetSize)
        {
            this.GetScale = GetScale;
            this.GetSize = GetSize;
        }

        public bool IsWithinBounds(Cell cell)
        {
            var size = GetSize();
            return cell.pos.x <= size.w && cell.pos.y <= size.h && cell.pos.x >= 0 && cell.pos.y >= 0;
        }

        public UIElement GetRectangle(Cell cell, (int x, int y) offset)
        {
            Rect rect;
            switch (Type)
            {
                case Type.Img1:
                    rect = new CellImage1();
                    break;
                case Type.Img2:
                    rect = new CellImage2();
                    break;
                default:
                    rect = new myRect();
                    break;
            }

            if (ColorBorn && cell.born)
                rect.Fill = new SolidColorBrush(Colors.Green);
            else if(cell.died)
                rect.Fill = new SolidColorBrush(Colors.Black);
            else
                rect.Fill = new SolidColorBrush(Colors.Blue);

            if (ColorEndangered && cell.endangered)
            {
                rect.Stroke = new SolidColorBrush(Colors.Red);
                rect.StrokeThickness = 2;
            }

            int scale = GetScale();
            rect.Width = rect.Height = scale;
            rect.Visibility = Visibility.Visible;
            rect.Margin = new Thickness(cell.pos.x * scale + offset.x, cell.pos.y * scale + offset.y, 0, 0);

            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.VerticalAlignment = VerticalAlignment.Top;

            return rect.GetMe();
        }
    }

    class myRect : Rect
    {
        Rectangle Rectangle = new Rectangle();

        public Brush Fill { set => Rectangle.Fill = value; }
        public Brush Stroke { set => Rectangle.Stroke = value; }
        public int StrokeThickness { set => Rectangle.StrokeThickness = value; }
        public double Width { set => Rectangle.Width = value; }
        public double Height { set => Rectangle.Height = value; }
        public Thickness Margin { set => Rectangle.Margin = value; }
        public HorizontalAlignment HorizontalAlignment { set => Rectangle.HorizontalAlignment = value; }
        public VerticalAlignment VerticalAlignment { set => Rectangle.VerticalAlignment = value; }

        public Visibility Visibility { set => Rectangle.Visibility = value; }

        public UIElement GetMe()
        {
            return Rectangle;
        }
    }
    enum Type
    {
        Rect,
        Img1,
        Img2
    }

    interface Rect
    {
        Brush Fill { set; }
        Brush Stroke { set; }
        int StrokeThickness { set; }
        double Width { set; }
        double Height { set; }
        Thickness Margin { set; }
        HorizontalAlignment HorizontalAlignment { set; }
        VerticalAlignment VerticalAlignment { set; }
        Visibility Visibility { set; }
        UIElement GetMe();
    }
}