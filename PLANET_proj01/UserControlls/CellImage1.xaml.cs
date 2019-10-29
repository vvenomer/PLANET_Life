using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PLANET_proj01.UserControlls
{
    /// <summary>
    /// Interaction logic for CellImage1.xaml
    /// </summary>
    public partial class CellImage1 : UserControl, Rect
    {
        public Brush Fill
        {
            set
            {
                backgound.Fill = value; 
            }
        }
        public Brush Stroke
        {
            set
            {
                backgound.Stroke = value;
            }
        }
        public int StrokeThickness
        {
            set
            {
                backgound.StrokeThickness = value;
            }
        }
        public UIElement GetMe()
        {
            return this;
        }
        public CellImage1()
        {
            InitializeComponent();
        }
    }
}
