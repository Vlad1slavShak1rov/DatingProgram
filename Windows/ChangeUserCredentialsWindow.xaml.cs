using DatingProgram.DB;
using System.Linq;
using System.Windows;

namespace DatingProgram.Windows
{
    public partial class ChangeUserCredentialsWindow : Window
    {
        private int _userId;

        public ChangeUserCredentialsWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text.Trim();
            string password = pbPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            try
            {
                using var context = new MyDbContext();
                var user = context.Users.FirstOrDefault(u => u.Id == _userId);

                if (user != null)
                {
                    user.Login = login;
                    user.Password = password;
                    context.SaveChanges();

                    MessageBox.Show("Данные обновлены!");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения!");
            }

            this.DialogResult = true;
            this.Close();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}