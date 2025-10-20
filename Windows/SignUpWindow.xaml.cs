using DatingProgram.DB;
using DatingProgram.Models;
using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void btSignUp_Click(object sender, RoutedEventArgs e)
        {
            var user = Registrations(tbLogin.Text, pbPassword.Password, pbPassword2.Password);
            if(user == null)
            {
                return;
            }

            var appWin = new DattingWindow(user);
            appWin.Show();
            this.Close();

        }

        private User Registrations(string login, string password1, string password2)
        {
            if(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password1) || string.IsNullOrEmpty(password2))
            {
                MessageBox.Show("Логин и пароль не должны быть пустыми!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            var context = new MyDbContext();
            var existingUser = context.Users.FirstOrDefault(u => u.Login == login);
            if(existingUser != null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            if(password1 != password2)
            {
                MessageBox.Show("Пароли не совпадают!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            var newUser = new User
            {
                Login = login,
                Password = password1,
                RoleId = 2 
            };

            context.Users.Add(newUser);
            context.SaveChanges();

            MessageBox.Show("Успешная регистрация!", "Successfully", MessageBoxButton.OK, MessageBoxImage.Information);
            return newUser;
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
