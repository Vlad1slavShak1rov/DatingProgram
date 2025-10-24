using DatingProgram.DB;
using DatingProgram.Models;
using DatingProgram.Windows;
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

namespace DatingProgram.UserControll
{
    /// <summary>
    /// Логика взаимодействия для NotificationControll.xaml
    /// </summary>
    public partial class NotificationControll : UserControl
    {
        public Likes Likes {get; set;}
        public string Message { get; set; }
        public event EventHandler UpdateNotify;

        public NotificationControll(Likes likes)
        {
            // Удаление уведомления по истечении 12 часов с момента создания лайка
            if (DateTime.Now >= likes.Created.AddDays(1))
            {
                using (var context = new MyDbContext())
                {
                    var existingLike = context.Likes.Find(likes.Id);
                    if (existingLike != null)
                    {
                        context.Likes.Remove(existingLike);
                        context.SaveChanges();
                    }
                }
            }

            InitializeComponent();

            Likes = likes;
            InitMessage();

            DataContext = this;
        }

        // Инициализация сообщения в зависимости от статуса лайка 
        private void InitMessage()
        {
            if (Likes.LikesStatus == Enums.LikesStatus.Отправлено)
            {
                Message = $"Пользователь {Likes.FromUser.FirstName} {Likes.FromUser.LastName} поставил вам лайк!";
            }
            else if(Likes.LikesStatus == Enums.LikesStatus.Симпатия)
            {
                Message = $"Пользователь {Likes.FromUser.FirstName} {Likes.FromUser.LastName} также поставил вам лайк! У вас совпадение!\n" +
                    $"Контакт для связи: {Likes.FromUser.Contact}";
            }
        }

        // Просмотр профиля отправителя лайка
        private void btCheckProfile_Click(object sender, RoutedEventArgs e)
        {
            ViewCurrentProfileWindow viewCurrentProfileWindow = new(Likes.FromUser, Likes.ToUser);
            viewCurrentProfileWindow.ShowDialog();
        }

        // Добавление пары при совпадении симпатий
        private void btAddSymp_Click(object sender, RoutedEventArgs e)
        {

            var context = new MyDbContext();
            var existPair = context.Pairs.FirstOrDefault(p => p.ManId == Likes.FromUser.Id || p.GirlId == Likes.FromUser.Id
                                                            && p.ManId == Likes.ToUserId || p.GirlId == Likes.ToUserId);

            if (existPair != null)
            {
                MessageBox.Show("Пара уже существует!", "Ошибка!", MessageBoxButton.OK);
                return;
            }

            // Создание новой пары  
            Pair pair = new()
            {
                ManId = Likes.FromUser.Id,
                GirlId = Likes.ToUser.Id,
                DateCreated = DateTime.Now,
                PairStatus = Enums.PairStatus.Общение
            };

            // Сохранение пары в базе данных
            /*
            var existLike = context.Likes.FirstOrDefault(l => l.Id == Likes.Id);
            context.Likes.Remove(existLike);
            */
            context.Pairs.Add(pair);
            context.SaveChanges();

            MessageBox.Show("Пара успешно создана!", "Успех!", MessageBoxButton.OK);
            UpdateNotify?.Invoke(this, e);
        }
    }
}
