using PLANET_proj01.Life;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PLANET_proj01
{
    class CellFactory
    {
        int size;
        public CellFactory(int size)
        {
            this.size = size;
        }

        public Rectangle GetRectangle(Cell cell, (int x, int y) offset)
        {
            var rect = new Rectangle();
            if (cell.state == CellState.Alive)
            {
                if (cell.born)
                    rect.Fill = new SolidColorBrush(Colors.Green);
                else
                    rect.Fill = new SolidColorBrush(Colors.Blue);

                if(cell.endangered)
                {
                    rect.Stroke = new SolidColorBrush(Colors.Red);
                }
            }

            rect.Width = rect.Height = size;

            rect.Margin = new Thickness(cell.pos.x * size + offset.x, cell.pos.y * size + offset.y, 0, 0);

            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.VerticalAlignment = VerticalAlignment.Top;

            return rect;
        }
    }
}
