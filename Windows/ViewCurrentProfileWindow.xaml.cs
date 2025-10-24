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
using System.Windows.Shapes;

namespace DatingProgram.Windows
{
    /// <summary>
    /// Логика взаимодействия для ViewCurrentProfileWindow.xaml
    /// </summary>
    public partial class ViewCurrentProfileWindow : Window
    {
        Client currentUser;
        Client otherUser;
        public ViewCurrentProfileWindow(Client otherUser, Client currentUser)
        {
            InitializeComponent();

            this.otherUser = otherUser;
            this.currentUser = currentUser;

            this.Title = $"Анкета: {otherUser.FirstName} {otherUser.LastName}";

            DattingFormControll dattingFormControll = new DattingFormControll(otherUser, currentUser, Visibility.Collapsed);
            spProfile.Children.Add(dattingFormControll);
        }
    }
}
