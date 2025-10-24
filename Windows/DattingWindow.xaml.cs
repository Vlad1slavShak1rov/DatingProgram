using DatingProgram.Models;
using DatingProgram.Pages;
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
            if(user.RoleId == 1) spAdminButtons.Visibility = Visibility.Visible;
            else spUserButtons.Visibility = Visibility.Visible;
        }

        // ОБРАБОТЧИКИ СОБЫТИЙ ДЛЯ ОБЫЧНОГО КЛИЕНТА

        private void spMyProfile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Pages.MyProfile(User));
        }

        private void spDatting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Pages.ViewingProfilesPages(User));
        }

        private void spNotify_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Pages.NotifictationPage(User));
        }

        private void spMyPairs_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Pages.MyPairsPage(User));
        }

        //ОБРАБОТЧИКИ СОБЫТИЙ ДЛЯ АДМИНА

        private void spReports_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void spAllPairs_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void spAllUsers_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        
        private void spAllProfile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(new Pages.ViewingProfilesPages(User));
        }
    }
}
