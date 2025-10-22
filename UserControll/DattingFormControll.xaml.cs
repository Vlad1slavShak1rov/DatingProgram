using DatingProgram.DB;
using DatingProgram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
    /// Логика взаимодействия для DattingFormControll.xaml
    /// </summary>
    public partial class DattingFormControll : UserControl
    {
        //Остальные пользователи
        public Client Client { get; set; }
        //Пользователь просматривающий анкету
        private Client user;
        public Characteristic Characteristic { get; set; }
        public DatingForm DatingForm { get; set; }

        private List<string> imagePath = new();
        int imagePageCounter = 0;
        public DattingFormControll(Client client, Client user)
        {
            InitializeComponent();
            Client = client;
            this.user = user;
            InitData();
        }

        private void InitData()
        {
            using var context = new MyDbContext();

            Characteristic = context.Characteristics.FirstOrDefault(c=>c.Id == Client.CharacteristicId);
            DatingForm = context.DatingForms.FirstOrDefault(df=>df.ClientId == Client.Id);
            imagePath = context.ClientPhotos.Where(cp => cp.ClientId == Client.Id)
                .Select(cp => cp.Path)
                .ToList();


            DataContext = this;
        }

        private void btPhotoRight_Click(object sender, RoutedEventArgs e)
        {
            ChangedPhoto(1);
        }

        private void btPhotoLeft_Click(object sender, RoutedEventArgs e)
        {
            ChangedPhoto(-1);
        }

        private void ChangedPhoto(int k)
        {
            if (imagePath.Count == 0)
                return;

            imagePageCounter += 1 * k;
            PhotoImage.Source = new BitmapImage(new Uri(imagePath[Math.Abs(imagePageCounter) % imagePath.Count]));
        }

        private void btLike_Click(object sender, RoutedEventArgs e)
        {
            var likes = new Likes()
            {
                FromUserId = user.Id,
                ToUserId = Client.Id,
                Created = DateTime.Now.Date,
                LikesStatus = Enums.LikesStatus.Отправлено
            };

            var notification = new Notification()
            {
                ClientId = Client.Id,
                Message = $"Пользователь {user.FirstName} {user.LastName} отправил вам симпатию.",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            using var context = new MyDbContext();

            context.Notifications.Add(notification);
            context.Likes.Add(likes);
            context.SaveChanges();

            MessageBox.Show("Лайк отправлен!\nПодождем, пока пользователь увидит его!","Лайк", MessageBoxButton.OK);
        }
    }
}
