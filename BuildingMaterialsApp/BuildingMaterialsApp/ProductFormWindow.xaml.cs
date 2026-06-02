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
using Microsoft.Win32;
using System.Globalization;
using System.IO;

namespace BuildingMaterialsApp
{
    public partial class ProductFormWindow : Window
    {
        private readonly int? _productId;
        private string _currentPhotoFile = "";
        private string _newPhotoPath = "";

        public ProductFormWindow(int? productId = null)
        {
            InitializeComponent();
            _productId = productId;
            LoadComboBoxes();

            if (_productId.HasValue)
            {
                LoadProductData(_productId.Value);
            }
            else
            {
                IdTextBox.Visibility = Visibility.Collapsed;
                LoadDefaultPhoto();
            }
        }

        private void LoadComboBoxes()
        {
            CategoryComboBox.DisplayMemberPath = "Name";
            CategoryComboBox.SelectedValuePath = "Id";
            CategoryComboBox.ItemsSource = DataService.GetCategories();

            ManufacturerComboBox.DisplayMemberPath = "Name";
            ManufacturerComboBox.SelectedValuePath = "Id";
            ManufacturerComboBox.ItemsSource = DataService.GetManufacturers();
        }

        private void LoadProductData(int productId)
        {
            var product = DataService.GetProductById(productId);
            if (product == null)
            {
                MessageBox.Show("Товар не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            IdTextBox.Text = product.ProductId.ToString();
            ArticleTextBox.Text = product.Article;
            NameTextBox.Text = product.Name;
            DescriptionTextBox.Text = product.Description;
            SupplierTextBox.Text = product.SupplierName;
            PriceTextBox.Text = product.Price.ToString("0.##", CultureInfo.InvariantCulture);
            UnitTextBox.Text = product.UnitName;
            StockTextBox.Text = product.StockQuantity.ToString();
            DiscountTextBox.Text = product.DiscountPercent.ToString("0.##", CultureInfo.InvariantCulture);

            CategoryComboBox.SelectedValue = product.CategoryId;
            ManufacturerComboBox.SelectedValue = product.ManufacturerId;

            _currentPhotoFile = product.PhotoFile ?? "";
            LoadPhoto(_currentPhotoFile);
        }

        private void LoadPhoto(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var cleanName = Path.GetFileName(fileName);
                var packUri = $"pack://application:,,,/resources/photos/{cleanName}";
                try
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(packUri, UriKind.Absolute);
                    image.EndInit();
                    PhotoImage.Source = image;
                    return;
                }
                catch { }
            }
            LoadDefaultPhoto();
        }

        private void LoadDefaultPhoto()
        {
            var defaultImage = new BitmapImage();
            defaultImage.BeginInit();
            defaultImage.UriSource = new Uri("pack://application:,,,/resources/picture.png", UriKind.Absolute);
            defaultImage.EndInit();
            PhotoImage.Source = defaultImage;
        }

        private void ChoosePhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dialog.ShowDialog() == true)
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(dialog.FileName, UriKind.Absolute);
                image.EndInit();

                if (image.PixelWidth > 300 || image.PixelHeight > 200)
                {
                    MessageBox.Show("Размер изображения не должен превышать 300×200 пикселей.",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _newPhotoPath = dialog.FileName;
                PhotoImage.Source = image;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(ArticleTextBox.Text))
            {
                MessageBox.Show("Введите артикул.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите наименование товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (CategoryComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (ManufacturerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите производителя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(SupplierTextBox.Text))
            {
                MessageBox.Show("Введите поставщика.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(UnitTextBox.Text))
            {
                MessageBox.Show("Введите единицу измерения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка числовых значений
            if (!decimal.TryParse(PriceTextBox.Text?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную цену (неотрицательное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(StockTextBox.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Введите корректное количество (неотрицательное целое число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!decimal.TryParse(DiscountTextBox.Text?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal discount) || discount < 0)
            {
                MessageBox.Show("Введите корректную скидку (неотрицательное число).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Сохранение фото
            string photoFile = _currentPhotoFile;
            if (!string.IsNullOrWhiteSpace(_newPhotoPath))
            {
                string ext = Path.GetExtension(_newPhotoPath);
                string newFileName = $"product_{DateTime.Now.Ticks}{ext}";
                string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "photos", newFileName);

                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                File.Copy(_newPhotoPath, targetPath, true);
                photoFile = $"resources/photos/{newFileName}";

                // Удаляем старое фото если оно не стандартное
                if (!string.IsNullOrWhiteSpace(_currentPhotoFile) && !_currentPhotoFile.Contains("picture.png"))
                {
                    string oldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _currentPhotoFile);
                    if (File.Exists(oldPath)) File.Delete(oldPath);
                }
            }

            var model = new ProductEditModel
            {
                ProductId = _productId,
                Article = ArticleTextBox.Text.Trim(),
                Name = NameTextBox.Text.Trim(),
                CategoryId = ((LookupItem)CategoryComboBox.SelectedItem).Id,
                Description = DescriptionTextBox.Text.Trim(),
                ManufacturerId = ((LookupItem)ManufacturerComboBox.SelectedItem).Id,
                SupplierName = SupplierTextBox.Text.Trim(),
                Price = price,
                UnitName = UnitTextBox.Text.Trim(),
                StockQuantity = stock,
                DiscountPercent = discount,
                PhotoFile = photoFile
            };

            try
            {
                DataService.SaveProduct(model);
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
