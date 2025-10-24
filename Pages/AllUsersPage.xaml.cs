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
    /// Логика взаимодействия для AllUsersPage.xaml
    /// </summary>
    public partial class AllUsersPage : Page
    {
        public List<Client> Clients { get; set; }
        private Client selectedClient;
        public AllUsersPage()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            DataContext = null;

            using var context = new MyDbContext();
            Clients = context.Client.Include(c => c.DatingForm).ToList();

            DataContext = Clients;
        }

        //Обработчик события считывание выбранного пользователя
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            //Получаем выбранную строку
            var selectedRow = dataGrid.SelectedItem;

            if (selectedRow != null)
            {
                //Запоминаем выбранного пользователя
                selectedClient = selectedRow as Client;
            }
        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedClient == null) return;


        }

        //Обработчик события удаления пользователя
        private void btRemove_Click(object sender, RoutedEventArgs e)
        {
            if (selectedClient == null) return;

            using var context = new MyDbContext();

            var existClient = context.Client.Find(selectedClient.Id);
            context.Client.Remove(existClient);
            context.SaveChanges();

            MessageBox.Show("Пользователь успешно удален!", "Успех", MessageBoxButton.OK);

            dgAllUsers.SelectedItem = null;

            InitData();
        }
    }
}
