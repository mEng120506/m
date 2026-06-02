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
    public partial class OrdersWindow : Window
    {
        private readonly UserInfo _currentUser;

        public OrdersWindow(UserInfo currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;

            bool isAdmin = _currentUser.RoleName == "Администратор";
            AddButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            EditButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            DeleteButton.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                OrdersGrid.ItemsSource = DataService.GetOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var form = new OrderFormWindow(null);
            form.Owner = this;
            if (form.ShowDialog() == true)
            {
                LoadOrders();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersGrid.SelectedItem as OrderRow;
            if (selected == null)
            {
                MessageBox.Show("Выберите заказ для редактирования.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var form = new OrderFormWindow(selected.OrderId);
            form.Owner = this;
            if (form.ShowDialog() == true)
            {
                LoadOrders();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selected = OrdersGrid.SelectedItem as OrderRow;
            if (selected == null)
            {
                MessageBox.Show("Выберите заказ для удаления.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show($"Удалить заказ №{selected.OrderNumber}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataService.DeleteOrder(selected.OrderId);
                    LoadOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OrdersGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_currentUser.RoleName == "Администратор")
            {
                Edit_Click(sender, e);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
