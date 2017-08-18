using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for CellDisplay.xaml
    /// </summary>
    public partial class Cell : UserControl
    {
        private int location;
        public int Location
        {
            get { return location; }
            set { location = value; }
        }

        private SolidColorBrush color;
        public SolidColorBrush Color
        {
           get { return color; }
           set { color = value; }  
        }

        public bool IsAlive
        {
          get
          {
             if (color.Color == Colors.Gray)
                 return false;
             return true;
          }
        }

        public Cell()
        {
            InitializeComponent();

            color = new SolidColorBrush();
            Kill();
            DataContext = this;
        }

        public void Kill()
        {
            color.Color = Colors.Gray;
        }
        public void Lives()
        {
            color.Color = Colors.Yellow;
        }
        public void UpdateState(int neighbours)
        {
            if (!IsAlive)
            {
                if (neighbours == 3)
                    Lives();
            }
            else
            {
                if (neighbours < 2 || neighbours > 3)
                    Kill();
            }
        }
    }
}
