using DatingProgram.DB;
using DatingProgram.Models;
using Microsoft.VisualBasic.Logging;
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
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace DatingProgram.Pages
{
    /// <summary>
    /// Логика взаимодействия для MyProfile.xaml
    /// </summary>
    public partial class MyProfile : Page
    {
        public User User { get; set; }
        public MyProfile(User user)
        {
            InitializeComponent();
            User = user;
            if(user.AvatarPath != null)
            {
                pcAvatar.Source = new BitmapImage(new Uri(user.AvatarPath));
            }


            DataContext = this;
        }

        private void btSaveUserInfo_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text;
            string password = pbPassword.Text;
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Логин и пароль не должны быть пустыми!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var context = new MyDbContext();
            var existingUser = context.Users.FirstOrDefault(u => u.Id != User.Id && u.Login == login);
            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User.Login = login;
            User.Password = password;

            context.Users.Update(User);
            context.SaveChanges();

            DataContext = User;
            MessageBox.Show("Успешно обновлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btMyForm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.jpg;*.jpeg;*.png";
            dlg.Title = "Выберите вашу новую фотографию для профиля";

            var result = dlg.ShowDialog();
            if(result == true)
            {
                string path = dlg.FileName;

                string projectFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;
                string destFolder = Path.Combine(projectFolder, "Resource", "otherPict");


                string fileName = Path.GetFileName(path); 
                string destPath = Path.Combine(destFolder, fileName); 
                File.Copy(path, destPath, overwrite: true);

                pcAvatar.Source = new BitmapImage(new Uri(destPath));
                User.AvatarPath = destPath;

                MessageBox.Show($"Картинка добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
