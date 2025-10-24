using DatingProgram.DB;
using DatingProgram.Models;
using DatingProgram.UserControll;
using DatingProgram.Windows;
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
        Models.DatingForm datingForm;


        int profileCounter = 0;
        bool isAutorizate = false;
        public ViewingProfilesPages(User user)
        {
            InitializeComponent();
            if(user.RoleId == 1)
            {
                spAdminButtons.Visibility = Visibility.Visible;
                btChangedProfile.Visibility = Visibility.Collapsed;
                isAutorizate = true;
                InitAllProfiles();
            }
            else
            {
                InitData(user.Id);
            }
        }


        private void InitAllProfiles()
        {
            // Очистка текущих данных
            spProfile.Children.Clear();
            profilesPages.Clear();

            using var context = new MyDbContext();
            var clients = context.Client.Include(c=>c.DatingForm).ToList();
            if(clients.Count > 0)
            {
                foreach (var c in clients)
                {
                    if (c.DatingForm.Count == 0) continue;

                    var profilePage = new DattingFormControll(c);
                    profilesPages.Add(profilePage);
                }
                if(profilesPages.Count > 0)
                    spProfile.Children.Add(profilesPages[0]);
            }
            else
            {
                MessageBox.Show("Нет доступных анкет для просмотра!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        // Инициализация данных
        private void InitData(int userId)
        {
            // Получение клиента из базы данных
            using var context = new MyDbContext();
            client = context.Client.Include(c=>c.Characteristic).FirstOrDefault(c=>c.UserId == userId);

            // Проверка на существование клиента
            if (client == null)
            {
                MessageBox.Show("Ошибка загрузки данных пользователя!\nСкорее всего вы не добавили основную информацию в вашем профиле", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Получение анкеты пользователя
            datingForm = context.DatingForms.FirstOrDefault(df=>df.ClientId == client.Id);

            // Проверка на существование анкеты
            if (datingForm == null)
            {
                MessageBox.Show("Вы не зарегистрировали свою анкету!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            isAutorizate = true;

            // Получение всех клиентов, кроме текущего пользователя
            var clients = context.Client.Where(c=>c.Id != client.Id).Include(c=>c.Characteristic).ToList();

            // Создание страницы профиля для каждого клиента
            foreach (var cl in clients)
            {
                var profilePage = new DattingFormControll(cl, client);

                // Фильтрация анкет на основе предпочтений пользователя в анкете
                if (profilePage.DatingForm == null || 
                    datingForm.MinAge > profilePage.Characteristic.Age || profilePage.Characteristic.Age > datingForm.MaxAge 
                    || client.Characteristic.Gender == profilePage.Characteristic.Gender)
                    continue;

                profilesPages.Add(profilePage);
            }
            
            if (profilesPages.Count == 0)
            {
                MessageBox.Show("Нет доступных анкет для просмотра!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            // Отображение первой анкеты
            spProfile.Children.Add(profilesPages[0]);
        }

        private void btNextProfile_Click(object sender, RoutedEventArgs e)
        {
            NextProfile();
        }

        // Переход к следующему профилю 
        private void NextProfile()
        {
            if (profilesPages.Count == 0) return;
            // Проверка на авторизацию пользователя
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
        // Изменения профиля во время просмотра анкет
        private void btChangedProfile_Click(object sender, RoutedEventArgs e)
        {
            // Запуск окна своей анкеты для изменения
            Windows.DattingFormWindow dattingFormWindow = new(client);

            var res = dattingFormWindow.ShowDialog();
            // Обновление данных после изменения анкеты
            if (res == true)
            {
                // Очистка текущих данных
                spProfile.Children.Clear();
                profilesPages.Clear();

                // Повторная инициализация данных
                InitData(client.UserId);

                // Обновление локальной переменной анкеты
                datingForm = dattingFormWindow.DatingForm;
            }
        }

        // Обработчик события при нажатии на кнопку редактировать
        private void btEditProfile_Click(object sender, RoutedEventArgs e)
        {
            var currentProfile = profilesPages[profileCounter].Client;
            DattingFormWindow dattingFormWindow = new(currentProfile);

            if(dattingFormWindow.ShowDialog() == true)
            {
                //Повторная инициализация
                InitAllProfiles();
            }

        }

        // Обработчик события при нажатии на кнопку удалить
        private void btRemoveProfile_Click(object sender, RoutedEventArgs e)
        {
            // Получаем конкретную анкету
            var currentDatingForm = profilesPages[profileCounter].DatingForm;

            // Обращаемся к БД и удаляем форму
            using var context = new MyDbContext();
            context.DatingForms.Remove(currentDatingForm);
            context.SaveChanges();

            MessageBox.Show("Анкета удалена!", "Успех", MessageBoxButton.OK);

            //Инициализируем по новой
            InitAllProfiles();
        }
    }
}
