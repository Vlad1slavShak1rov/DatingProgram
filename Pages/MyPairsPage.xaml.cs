using DatingProgram.DB;
using DatingProgram.Models;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatingProgram.Pages
{
    /// <summary>
    /// Логика взаимодействия для MyPairsPage.xaml
    /// </summary>
    public partial class MyPairsPage : Page
    {
        // Модель представления для пары и партнера
        public class PairViewModel
        {
            public Pair Pair { get; set; }
            public Client Partner { get; set; }
            public Client CurrentUser { get; set; }
        }

        private Client client;
        public List<PairViewModel> MyPairs { get; set; }
        public MyPairsPage(User user)
        {
            InitializeComponent();

            this.client = new MyDbContext().Client.FirstOrDefault(c=>c.UserId == user.Id); 

            InitData();
        }

        // Инициализация данных
        private void InitData()
        {
            using var context = new MyDbContext();
            // Получение пар клиента с включением данных о мужчине и женщине
            var pairs = context.Pairs
               .Include(p => p.Man)
               .Include(p => p.Women)
               .Where(p => p.ManId == client.Id || p.GirlId == client.Id)
               .ToList();

            // Создание списка моделей представления пар
            MyPairs = new List<PairViewModel>();

            // Заполнение списка моделей представления
            foreach (var pair in pairs)
            {
                //Определение партнера и текущего клиента
                Client partner = pair.ManId == client.Id ? pair.Women : pair.Man;
                Client currentClient = partner.Id == client.Id ? pair.Women : pair.Man;

                // Добавление модели представления в список
                MyPairs.Add(new PairViewModel
                {
                    Pair = pair,
                    Partner = partner,
                    CurrentUser = currentClient
                });
            }

            DataContext = this;
        }

        // Обработчик клика по кнопке "Показать профиль"
        private void btShowProfile_Click(object sender, RoutedEventArgs e)
        {
            // Получение кнопки, по которой был выполнен клик
            var button = sender as Button;

            // Получение модели представления пары из контекста данных кнопки
            var pairView = button?.DataContext as PairViewModel;

            if (pairView != null)
            {
                ViewCurrentProfileWindow viewCurrentProfileWindow = new(pairView.Partner,pairView.CurrentUser);
                viewCurrentProfileWindow.ShowDialog();
            }
        }
    }
}
