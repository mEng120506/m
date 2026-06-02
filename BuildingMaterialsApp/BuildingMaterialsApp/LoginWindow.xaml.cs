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

namespace BuildingMaterialsApp
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = DataService.Auth(login, password);
                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка входа",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            var guest = new UserInfo
            {
                UserId = null,
                FullName = "Гость",
                RoleName = "Гость"
            };
            var mainWindow = new MainWindow(guest);
            mainWindow.Show();
            this.Close();
        }
    }
}
