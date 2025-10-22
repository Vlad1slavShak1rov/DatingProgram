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
using DatingProgram.Windows;

namespace DatingProgram.Pages
{
    /// <summary>
    /// Логика взаимодействия для MyProfile.xaml
    /// </summary>
    public partial class MyProfile : Page
    {
        public User User { get; set; }
        public Client? Client { get; set; }
        public MyProfile(User user)
        {
            InitializeComponent();
            User = user;
            if(user.AvatarPath != string.Empty)
            {
                pcAvatar.Source = new BitmapImage(new Uri(user.AvatarPath));
            }

            using var context = new MyDbContext();
            Client = context.Client.FirstOrDefault(c => c.UserId == user.Id);

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

        //Кнопка Сохранить в разделе личной информации
        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            string firstName = tbName.Text;
            string secondName = tbSurname.Text;
            string lastName = tbLastName.Text;
            string contact = tbPhoneNum.Text;

            //Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(firstName) || 
                string.IsNullOrWhiteSpace(secondName) || 
                string.IsNullOrWhiteSpace(lastName) || 
                string.IsNullOrWhiteSpace(contact))
            {
                MessageBox.Show("Поля не должны быть пустыми!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            var context = new MyDbContext();
            var client = context.Client.FirstOrDefault(c => c.UserId == User.Id);

            //Если клиента нет, создается новый клиент
            if (client == null)
            {
                client = new Client
                {
                    UserId = User.Id,
                    FirstName = firstName,
                    SecondName = secondName,
                    LastName = lastName,
                    Contact = contact
                };


                context.Client.Add(client);
            }
            else
            {
                client.FirstName = firstName;
                client.SecondName = secondName;
                client.LastName = lastName;
                client.Contact = contact;
                context.Client.Update(client);
            }

            context.SaveChanges();

            MessageBox.Show("Успешно обновлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //Событие нажатия кнопки Моя анкета
        private void btMyForm_Click(object sender, RoutedEventArgs e)
        {
            /*
            //Проверка на заполнение личной информации
            if (Client is null)
            {
                MessageBox.Show("Сначала заполните личную информацию!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //Подключение к БД и поиск анкеты клиента
            using var context = new MyDbContext();
            var form = context.DatingForms.FirstOrDefault(f => f.ClientId == Client.Id);

            if(form is null)
            {
                MessageBox.Show("У вас не создана анкета. Сейчас вы будете перенаправлены на страницу создания анкеты.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                //запуск окна моя анкета
            }
            */

            DattingFormWindow dattingFormWindow = new DattingFormWindow();
            dattingFormWindow.ShowDialog();
            
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
