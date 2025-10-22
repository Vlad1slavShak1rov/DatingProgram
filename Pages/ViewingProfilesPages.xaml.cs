using DatingProgram.DB;
using DatingProgram.Models;
using DatingProgram.UserControll;
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

namespace DatingProgram.Pages
{
    /// <summary>
    /// Логика взаимодействия для ViewingProfilesPages.xaml
    /// </summary>
    public partial class ViewingProfilesPages : Page
    {

        List<DattingFormControll> profilesPages = new();
        Client client;

        int profileCounter = 0;
        bool isAutorizate = false;
        public ViewingProfilesPages(User user)
        {
            InitializeComponent();
            InitData(user);
        }

        // Инициализация данных
        private void InitData(User user)
        {
            // Получение клиента из базы данных
            using var context = new MyDbContext();
            client = context.Client.FirstOrDefault(c=>c.UserId == user.Id);

            // Проверка на существование клиента
            if (client == null)
            {
                MessageBox.Show("Ошибка загрузки данных пользователя!\nСкорее всего вы не добавили основную информацию в вашем профиле", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Получение анкеты пользователя
            var datingProfile = context.DatingForms.FirstOrDefault(df=>df.ClientId == client.Id);

            // Проверка на существование анкеты
            if (datingProfile == null)
            {
                MessageBox.Show("Вы не зарегистрировали свою анкету!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            isAutorizate = true;

            // Получение всех клиентов, кроме текущего пользователя
            var clients = context.Client.Where(c=>c.Id != client.Id).ToList();

            // Создание страницы профиля для каждого клиента
            foreach (var cl in clients)
            {
                var profilePage = new DattingFormControll(cl, client);
                profilesPages.Add(profilePage);
            }
            if(profilesPages.Count == 0)
            {
                MessageBox.Show("Нет доступных анкет для просмотра!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            spProfile.Children.Add(profilesPages[0]);
        }

        private void btNextProfile_Click(object sender, RoutedEventArgs e)
        {
            NextProfile();
        }

        // Переход к следующему профилю 
        private void NextProfile()
        {
            if (!isAutorizate)
            {
                MessageBox.Show("Добавите свой анкету!");
                return;
            }
            if (profileCounter < profilesPages.Count - 1)
            {
                profileCounter++;
                
            }
            else
            {
                profileCounter = 0;
            }

            spProfile.Children.Clear();
            spProfile.Children.Add(profilesPages[profileCounter]);

        }
    }
}
