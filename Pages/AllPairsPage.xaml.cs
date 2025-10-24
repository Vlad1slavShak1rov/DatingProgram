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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatingProgram.Pages
{
    /// <summary>
    /// Логика взаимодействия для AllPairsPage.xaml
    /// </summary>
    public partial class AllPairsPage : Page
    {
        public List<Pair> Pairs { get; set; }

        public AllPairsPage()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            DataContext = null;

            using var context = new MyDbContext();
            Pairs = context.Pairs.Include(p => p.Man).Include(p => p.Women).ToList();

            CleanupCompletedPairs();

            DataContext = this;
        }

        private void CleanupCompletedPairs()
        {
            using var context = new MyDbContext();
            var pairsToDelete = context.Pairs
                .Where(p => p.PairStatus == Enums.PairStatus.Расстались)
                .ToList();

            if (!pairsToDelete.Any()) return;

            var clientIds = pairsToDelete
                .SelectMany(p => new[] { p.ManId, p.GirlId })
                .Distinct()
                .ToList();

            var formsToDelete = context.DatingForms
                .Where(d => clientIds.Contains(d.ClientId))
                .ToList();

            if (formsToDelete.Any())
            {
                context.DatingForms.RemoveRange(formsToDelete);
            }

            context.Pairs.RemoveRange(pairsToDelete);
            context.SaveChanges();
        }

        private void dgAllPairs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = dgAllPairs.SelectedItem;
            if (selectedItem != null)
            {
                var pair = selectedItem as Pair;
                ChangedPairWindow changedPairWindow = new(pair);
                if (changedPairWindow.ShowDialog() == true)
                {
                    InitData();
                }
            }
        }

        private void rbAllTime_Checked(object sender, RoutedEventArgs e)
        {
            if (spDate == null) return;
            spDate.IsEnabled = false;

            dpFromDate.SelectedDate = null;
            dpToDate.SelectedDate = null;

            using var context = new MyDbContext();
            Pairs = context.Pairs.Include(p => p.Man).Include(p => p.Women).ToList();

            DataContext = null;
            DataContext = this;
        }

        private void rbCurrentDate_Checked(object sender, RoutedEventArgs e)
        {
            spDate.IsEnabled = true;
        }

        private void dpToDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CheckDate(dpToDate.SelectedDate))
            {
                FilterPairsByDateRange();
            }
        }

        private void dpFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CheckDate(dpFromDate.SelectedDate))
            {
                FilterPairsByDateRange();
            }
        }

        private bool CheckDate(DateTime? date)
        {
            if (date == null) return false;

            var fromDate = dpFromDate.SelectedDate;
            var toDate = dpToDate.SelectedDate;

            if (fromDate != null && toDate != null && fromDate > toDate)
            {
                MessageBox.Show("Начальная дата не может быть больше конечной!");
                return false;
            }

            return true;
        }

        private void FilterPairsByDateRange()
        {
            var fromDate = dpFromDate.SelectedDate;
            var toDate = dpToDate.SelectedDate;

            if (fromDate == null || toDate == null) return;

            using var context = new MyDbContext();

            Pairs = context.Pairs
                .Include(p => p.Man)
                .Include(p => p.Women)
                .Where(p => p.DateCreated >= fromDate && p.DateCreated <= toDate.Value.AddDays(1))
                .ToList();

            DataContext = null;
            DataContext = this;
        }
    }
}
