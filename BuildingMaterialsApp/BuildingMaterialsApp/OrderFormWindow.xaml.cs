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
    public partial class OrderFormWindow : Window
    {
        private readonly int? _orderId;

        public OrderFormWindow(int? orderId = null)
        {
            InitializeComponent();
            _orderId = orderId;
            LoadComboBoxes();

            if (_orderId.HasValue)
            {
                LoadOrderData(_orderId.Value);
            }
            else
            {
                OrderNumberTextBox.Text = DataService.GetNextOrderNumber().ToString();
                OrderDatePicker.SelectedDate = DateTime.Today;
                DeliveryDatePicker.SelectedDate = DateTime.Today.AddDays(7);
            }
        }

        private void LoadComboBoxes()
        {
            StatusComboBox.DisplayMemberPath = "Name";
            StatusComboBox.SelectedValuePath = "Id";
            StatusComboBox.ItemsSource = DataService.GetStatuses();

            PickupComboBox.DisplayMemberPath = "Name";
            PickupComboBox.SelectedValuePath = "Id";
            PickupComboBox.ItemsSource = DataService.GetPickupPoints();
        }

        private void LoadOrderData(int orderId)
        {
            var order = DataService.GetOrderById(orderId);
            if (order == null)
            {
                MessageBox.Show("Заказ не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            OrderNumberTextBox.Text = order.OrderNumber.ToString();
            ArticlesTextBox.Text = order.ArticlesText;
            StatusComboBox.SelectedValue = order.StatusId;
            PickupComboBox.SelectedValue = order.PickupPointId;
            OrderDatePicker.SelectedDate = order.OrderDate;
            DeliveryDatePicker.SelectedDate = order.DeliveryDate;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ArticlesTextBox.Text))
            {
                MessageBox.Show("Введите артикулы заказа в формате: артикул,количество", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StatusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус заказа.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PickupComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!OrderDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату заказа.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DeliveryDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату выдачи.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var model = new OrderEditModel
            {
                OrderId = _orderId,
                OrderNumber = int.Parse(OrderNumberTextBox.Text),
                ArticlesText = ArticlesTextBox.Text.Trim(),
                StatusId = ((LookupItem)StatusComboBox.SelectedItem).Id,
                PickupPointId = ((LookupItem)PickupComboBox.SelectedItem).Id,
                OrderDate = OrderDatePicker.SelectedDate.Value,
                DeliveryDate = DeliveryDatePicker.SelectedDate.Value
            };

            try
            {
                DataService.SaveOrder(model);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
