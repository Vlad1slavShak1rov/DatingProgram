using DatingProgram.Models;
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
    /// Логика взаимодействия для DattingWindow.xaml
    /// </summary>
    public partial class DattingWindow : Window
    {
        private User User { get; set; }
        public DattingWindow(User user)
        {
            InitializeComponent();

            User = user;
        }

        private void spMyProfile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Pages.MyProfile(User));
        }

        private void spDatting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("2");
        }

        private void spNotify_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("3");
        }
    }
}
