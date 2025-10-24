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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatingProgram.Pages
{
    /// <summary>
    /// Логика взаимодействия для NotifictationPage.xaml
    /// </summary>
    public partial class NotifictationPage : Page
    {
        Client client;
        public NotifictationPage(User user)
        {
            InitializeComponent();
            client = new MyDbContext().Client.Find(user.Id);

            InitData();
        }

        private void InitData()
        {
            var context = new MyDbContext();



            var likes = context.Likes
                .Where(l => l.ToUserId == client.Id)
                .Include(l=>l.ToUser)
                .Include(l=>l.FromUser)
                .ToList();

          

            foreach (var l in likes)
            {
                if (l.LikesStatus == Enums.LikesStatus.Симпатия) continue;

                var existPair = context.Pairs.FirstOrDefault(p => p.ManId == l.FromUser.Id || p.GirlId == l.FromUser.Id
                                                         && p.ManId == l.ToUserId || p.GirlId == l.ToUserId);

                if( existPair != null)
                {
                    l.LikesStatus = Enums.LikesStatus.Симпатия;
                    context.Likes.Update(l);
                    context.SaveChanges();
                    continue;
                }

                var notificationControl = new UserControll.NotificationControll(l);
                notificationControl.UpdateNotify += NotificationControl_UpdateNotify;
                lbNotify.Items.Add(notificationControl);
            }
        }

        private void NotificationControl_UpdateNotify(object? sender, EventArgs e)
        {
            InitData();
        }
    }
}
