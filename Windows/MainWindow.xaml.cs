using DatingProgram.DB;
using DatingProgram.Models;
using DatingProgram.Windows;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatingProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btSignIn_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text;
            string password = pbPassword.Password;

            var user = SignIn(login, password);
            if(user != null)
            {
                var appWin = new DattingWindow(user);
                appWin.Show();
                this.Close();
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var singInWindow = new SignUpWindow();
            singInWindow.Show();
            this.Close();
        }

        private User? SignIn(string username, string password)
        {
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите свои данные для входа!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            using var context = new MyDbContext();

            var user = context.Users.FirstOrDefault(u => u.Login == username && u.Password == password);
            if(user == null)
            {
                MessageBox.Show("Неверный логин или пароль!", "Authentication Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            MessageBox.Show("Успешный вход!", "Welcome", MessageBoxButton.OK, MessageBoxImage.Information);
            return user;
        }
    }
}