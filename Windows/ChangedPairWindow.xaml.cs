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
using System.Windows.Shapes;

namespace DatingProgram.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangedPairWindow.xaml
    /// </summary>
    public partial class ChangedPairWindow : Window
    {
        private Pair pair;
        public ChangedPairWindow(Pair pair)
        {
            InitializeComponent();
            this.pair = pair;

            cbPairStatus.ItemsSource = Enum.GetValues(typeof(Enums.PairStatus)).Cast<Enums.PairStatus>().ToList();
            cbPairStatus.SelectedItem = pair.PairStatus; 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectedStatus = (Enums.PairStatus)cbPairStatus.SelectedItem;
            Enums.PairStatus newStatus = 0;
            MessageBoxResult res;
            switch (selectedStatus)
            {
                case Enums.PairStatus.Общение:
                    newStatus = Enums.PairStatus.Общение;
                    break;

                case Enums.PairStatus.Отношение:
                    res = MessageBox.Show("Если вы поменяете на этот статус, анкеты у кандидатов удаляться.\nВы уверены в этом?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if(res == MessageBoxResult.Yes)
                    {
                        newStatus = Enums.PairStatus.Отношение;
                    }
                    break;

                case Enums.PairStatus.Семья:
                    res = MessageBox.Show("Если вы поменяете на этот статус, анкеты у кандидатов удаляться.\nВы уверены в этом?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                        newStatus = Enums.PairStatus.Семья;
                    break;

                case Enums.PairStatus.Расстались:
                    res = MessageBox.Show("Вы уверены в этом?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                        newStatus = Enums.PairStatus.Расстались;
                    break;
            }

            using var context = new MyDbContext();

            var existPair = context.Pairs.Include(e=>e.Man).Include(e=>e.Women).FirstOrDefault(p=>p.Id == pair.Id);
            existPair.PairStatus = newStatus;


            Logs logs = new()
            {
                Header = $"Изменение статус на {newStatus}",
                Message = $"у пары {existPair.Women.FullName} и {existPair.Man.FullName} был изменен статус!",
                DateAct = DateTime.Now,
            };

            context.Logs.Add(logs);
            context.SaveChanges();

            context.SaveChanges();
            DialogResult = true;
            this.Close();
        }
    }
}
