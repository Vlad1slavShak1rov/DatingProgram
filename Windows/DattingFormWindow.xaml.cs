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
using System.Windows.Shapes;

namespace DatingProgram.Windows
{
    /// <summary>
    /// Логика взаимодействия для DattingFormWindow.xaml
    /// </summary>
    public partial class DattingFormWindow : Window
    {
        int pageCounter = 0;
        int maxPageCounter = 2;
        public DattingFormWindow()
        {
            InitializeComponent();
        }

        private void btRight_Click(object sender, RoutedEventArgs e)
        {
            if(maxPageCounter != pageCounter)
            {
                pageCounter++;
                SelectPage();
            }
        }

        private void btLeft_Click(object sender, RoutedEventArgs e)
        {
            if (maxPageCounter != 0)
            {
                pageCounter--;
                SelectPage();
            }
        }

        private void SelectPage()
        {
            switch (pageCounter)
            {
                case 0:
                    {
                        btLeft.IsEnabled = false;
                        spFirst.Visibility = Visibility.Visible;
                        spSecond.Visibility = Visibility.Collapsed;
                        spThird.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 1:
                    {
                        btLeft.IsEnabled = true;
                        btRight.IsEnabled = true;
                        spFirst.Visibility = Visibility.Collapsed;
                        spSecond.Visibility = Visibility.Visible;
                        spThird.Visibility = Visibility.Collapsed;
                        break;
                    }
                case 2:
                    {
                        btRight.IsEnabled = false;
                        spFirst.Visibility = Visibility.Collapsed;
                        spSecond.Visibility = Visibility.Collapsed;
                        spThird.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }
    }
}
