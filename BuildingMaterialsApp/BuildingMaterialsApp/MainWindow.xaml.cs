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
    public partial class MainWindow : Window
    {
        private readonly UserInfo _currentUser;
        private List<ProductRow> _allProducts = new List<ProductRow>();
        private ProductFormWindow _openedEditor;
        private bool _isUILoaded = false;

        public MainWindow(UserInfo user)
        {
            InitializeComponent();
            _currentUser = user;

            RoleTextBlock.Text = $"Роль: {GetRoleDisplayName(_currentUser.RoleName)}";
            UserTextBlock.Text = $"Пользователь: {_currentUser.FullName}";

            bool isManagerOrAdmin = _currentUser.RoleName == "Менеджер" || _currentUser.RoleName == "Администратор";
            OrdersButton.Visibility = isManagerOrAdmin ? Visibility.Visible : Visibility.Collapsed;

            FilterPanel.Visibility = isManagerOrAdmin ? Visibility.Visible : Visibility.Collapsed;

            AdminPanel.Visibility = _currentUser.RoleName == "Администратор" ? Visibility.Visible : Visibility.Collapsed;

            if (isManagerOrAdmin)
            {
                LoadManufacturersFilter();
            }

            _isUILoaded = true;
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                _allProducts = DataService.GetProducts();
                ApplyFiltersAndSorting();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadManufacturersFilter()
        {
            ManufacturerComboBox.Items.Clear();
            ManufacturerComboBox.Items.Add("Все производители");
            var manufacturers = DataService.GetManufacturerNames();
            foreach (var manufacturer in manufacturers)
            {
                ManufacturerComboBox.Items.Add(manufacturer);
            }
            ManufacturerComboBox.SelectedIndex = 0;
        }

        private void ApplyFiltersAndSorting()
        {
            if (!_isUILoaded) return;

            var query = _allProducts.AsEnumerable();

            // Только для менеджера и администратора применяем фильтры
            if (_currentUser.RoleName == "Менеджер" || _currentUser.RoleName == "Администратор")
            {
                // Поиск по всем текстовым полям
                string searchText = SearchTextBox.Text?.Trim().ToLower() ?? "";
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(p =>
                        (p.Article?.ToLower().Contains(searchText) ?? false) ||
                        (p.Name?.ToLower().Contains(searchText) ?? false) ||
                        (p.Category?.ToLower().Contains(searchText) ?? false) ||
                        (p.Description?.ToLower().Contains(searchText) ?? false) ||
                        (p.Manufacturer?.ToLower().Contains(searchText) ?? false) ||
                        (p.Supplier?.ToLower().Contains(searchText) ?? false) ||
                        (p.UnitName?.ToLower().Contains(searchText) ?? false)
                    );
                }

                // Фильтр по производителю
                string selectedManufacturer = ManufacturerComboBox.SelectedItem?.ToString() ?? "Все производители";
                if (selectedManufacturer != "Все производители")
                {
                    query = query.Where(p => p.Manufacturer == selectedManufacturer);
                }

                // Сортировка
                string sortBy = (SortComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content?.ToString() ?? "Без сортировки";
                switch (sortBy)
                {
                    case "Цена (по возрастанию)":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "Цена (по убыванию)":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case "Остаток (по возрастанию)":
                        query = query.OrderBy(p => p.StockQuantity);
                        break;
                    case "Остаток (по убыванию)":
                        query = query.OrderByDescending(p => p.StockQuantity);
                        break;
                    case "Скидка (по возрастанию)":
                        query = query.OrderBy(p => p.DiscountPercent);
                        break;
                    case "Скидка (по убыванию)":
                        query = query.OrderByDescending(p => p.DiscountPercent);
                        break;
                }
            }

            ProductsGrid.ItemsSource = query.ToList();
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ApplyFiltersAndSorting();
        }

        private void ManufacturerFilter_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSorting();
        }

        private void SortComboBox_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFiltersAndSorting();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.RoleName != "Администратор")
            {
                MessageBox.Show("Только администратор может добавлять товары.", "Доступ запрещён",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_openedEditor != null && _openedEditor.IsVisible)
            {
                MessageBox.Show("Окно редактирования уже открыто.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _openedEditor = new ProductFormWindow(null);
            _openedEditor.Owner = this;
            if (_openedEditor.ShowDialog() == true)
            {
                LoadProducts();
                LoadManufacturersFilter();
            }
            _openedEditor = null;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.RoleName != "Администратор")
            {
                MessageBox.Show("Только администратор может редактировать товары.", "Доступ запрещён",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = ProductsGrid.SelectedItem as ProductRow;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар для редактирования.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_openedEditor != null && _openedEditor.IsVisible)
            {
                MessageBox.Show("Окно редактирования уже открыто.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _openedEditor = new ProductFormWindow(selected.ProductId);
            _openedEditor.Owner = this;
            if (_openedEditor.ShowDialog() == true)
            {
                LoadProducts();
                LoadManufacturersFilter();
            }
            _openedEditor = null;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser.RoleName != "Администратор")
            {
                MessageBox.Show("Только администратор может удалять товары.", "Доступ запрещён",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selected = ProductsGrid.SelectedItem as ProductRow;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар для удаления.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (DataService.ProductExistsInOrders(selected.ProductId))
            {
                MessageBox.Show("Товар присутствует в заказах. Удаление невозможно.", "Удаление запрещено",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Удалить товар \"{selected.Name}\"?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    DataService.DeleteProduct(selected.ProductId);
                    LoadProducts();
                    LoadManufacturersFilter();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ProductsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_currentUser.RoleName == "Администратор")
            {
                Edit_Click(sender, e);
            }
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            var ordersWindow = new OrdersWindow(_currentUser);
            ordersWindow.Owner = this;
            ordersWindow.ShowDialog();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private string GetRoleDisplayName(string role)
        {
            switch (role)
            {
                case "Авторизированный клиент": return "Клиент";
                case "Администратор": return "Администратор";
                case "Менеджер": return "Менеджер";
                default: return "Гость";
            }
        }
    }
}
