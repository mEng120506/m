Шаг 1. Создайте рабочую структуру проекта
Что делаем
Создайте папку проекта и подкаталоги для ресурсов и документов.

Команды
Откройте Командную строку (Win+R → cmd) и выполните:

cmd
cd C:\
mkdir building_materials_2026
cd building_materials_2026
mkdir resources
mkdir resources\photos
mkdir docs
mkdir sql
mkdir screenshots
Что делаем с файлами
Из папки с заданием (Прил_2_ОЗ_...) скопируйте в C:\building_materials_2026\resources:

Icon.ico

Icon.png

picture.png

Tovar.xlsx

user_import.xlsx

Пункты выдачи_import.xlsx

Заказ_import.xlsx

Все файлы .jpg из папки photos скопируйте в C:\building_materials_2026\resources\photos

Шаг 2. Запустите XAMPP и создайте базу данных
Что делаем
Запустите модули Apache и MySQL в XAMPP Control Panel.

Команды
Нажмите Пуск → XAMPP Control Panel.

Напротив Apache нажмите Start.

Напротив MySQL нажмите Start.

Откройте браузер, в адресной строке введите: http://localhost/phpmyadmin

Нажмите на вкладку Базы данных.

В поле Имя базы данных введите: building_materials_2026

Выберите сравнение: utf8mb4_unicode_ci

Нажмите Создать.

Шаг 3. Создайте таблицы базы данных
Что делаем
В phpMyAdmin создайте все необходимые таблицы через SQL-запрос.

Команды
В phpMyAdmin нажмите на базу building_materials_2026 слева.

Откройте вкладку SQL.

Скопируйте и выполните следующий код:

sql
USE building_materials_2026;

-- Таблица ролей пользователей
CREATE TABLE roles (
    role_id TINYINT UNSIGNED PRIMARY KEY,
    role_name VARCHAR(60) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Таблица пользователей
CREATE TABLE users (
    user_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    role_id TINYINT UNSIGNED NOT NULL,
    full_name VARCHAR(200) NOT NULL,
    login VARCHAR(120) NOT NULL UNIQUE,
    password_plain VARCHAR(120) NOT NULL,
    FOREIGN KEY (role_id) REFERENCES roles(role_id) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Таблица категорий товаров
CREATE TABLE categories (
    category_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(120) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Таблица производителей
CREATE TABLE manufacturers (
    manufacturer_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(120) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Таблица поставщиков
CREATE TABLE suppliers (
    supplier_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(120) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Таблица товаров
CREATE TABLE products (
    product_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    article VARCHAR(50) NOT NULL UNIQUE,
    name VARCHAR(200) NOT NULL,
    unit_name VARCHAR(20) NOT NULL,
    price DECIMAL(12,2) NOT NULL CHECK (price >= 0),
    supplier_id INT UNSIGNED NOT NULL,
    manufacturer_id INT UNSIGNED NOT NULL,
    category_id INT UNSIGNED NOT NULL,
    discount_percent DECIMAL(5,2) NOT NULL CHECK (discount_percent >= 0),
    stock_quantity INT NOT NULL CHECK (stock_quantity >= 0),
    description_text TEXT NULL,
    photo_file VARCHAR(255) NULL,
    FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id) ON UPDATE CASCADE ON DELETE RESTRICT,
    FOREIGN KEY (manufacturer_id) REFERENCES manufacturers(manufacturer_id) ON UPDATE CASCADE ON DELETE RESTRICT,
    FOREIGN KEY (category_id) REFERENCES categories(category_id) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Таблица пунктов выдачи
CREATE TABLE pickup_points (
    pickup_point_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    address_text VARCHAR(255) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Таблица статусов заказов
CREATE TABLE order_statuses (
    status_id TINYINT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    status_name VARCHAR(60) NOT NULL UNIQUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Таблица заказов
CREATE TABLE orders (
    order_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    order_number INT UNSIGNED NOT NULL UNIQUE,
    article_text VARCHAR(500) NOT NULL,
    order_date DATE NULL,
    delivery_date DATE NULL,
    pickup_point_id INT UNSIGNED NOT NULL,
    status_id TINYINT UNSIGNED NOT NULL,
    FOREIGN KEY (pickup_point_id) REFERENCES pickup_points(pickup_point_id) ON UPDATE CASCADE ON DELETE RESTRICT,
    FOREIGN KEY (status_id) REFERENCES order_statuses(status_id) ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Индексы для ускорения запросов
CREATE INDEX idx_products_supplier ON products(supplier_id);
CREATE INDEX idx_products_category ON products(category_id);
CREATE INDEX idx_products_manufacturer ON products(manufacturer_id);
CREATE INDEX idx_orders_status ON orders(status_id);

-- Временные таблицы для импорта данных
CREATE TABLE users_import_raw (
    role_name VARCHAR(100),
    full_name VARCHAR(200),
    login_text VARCHAR(120),
    password_text VARCHAR(120)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE products_import_raw (
    article_text VARCHAR(60),
    name_text VARCHAR(200),
    unit_text VARCHAR(20),
    price_text VARCHAR(40),
    supplier_text VARCHAR(120),
    manufacturer_text VARCHAR(120),
    category_text VARCHAR(120),
    discount_text VARCHAR(40),
    stock_text VARCHAR(40),
    description_text TEXT,
    photo_text VARCHAR(120)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE pickup_points_import_raw (
    raw_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    address_text VARCHAR(255)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE orders_import_raw (
    raw_id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
    order_number_text VARCHAR(40),
    articles_text VARCHAR(500),
    order_date_text VARCHAR(40),
    delivery_date_text VARCHAR(40),
    pickup_point_text VARCHAR(40),
    client_fio_text VARCHAR(200),
    pickup_code_text VARCHAR(40),
    status_text VARCHAR(80)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
Шаг 4. Подготовьте CSV-файлы из Excel
Что делаем
Откройте каждый Excel-файл, удалите заголовки и сохраните как CSV.

Пошагово
Файл user_import.xlsx:

Откройте в Excel.

Удалите первую строку с заголовками Роль сотрудника | ФИО | Логин | Пароль.

Нажмите Файл → Сохранить как.

Выберите тип CSV UTF-8 (разделители — запятые).

Имя файла: user_import.csv

Нажмите Сохранить.

Файл Tovar.xlsx:

Откройте в Excel.

Удалите первую строку с заголовками.

Сохраните как CSV UTF-8 с именем Tovar.csv.

Файл Пункты выдачи_import.xlsx:

Откройте в Excel.

Не удаляйте первую строку (это данные адресов).

Сохраните как CSV UTF-8 с именем Пункты выдачи_import.csv.

Файл Заказ_import.xlsx:

Откройте в Excel.

Удалите первую строку с заголовками.

Удалите пустые столбцы справа (I, J, K, L).

Сохраните как CSV UTF-8 с именем Заказ_import.csv.

Шаг 5. Импортируйте CSV в phpMyAdmin
Что делаем
Загрузите каждый CSV-файл в соответствующую raw-таблицу.
Везде в разделителе вместо запятой писать ";". 

Пошагово
Импорт пользователей:

В phpMyAdmin нажмите на таблицу users_import_raw.

Нажмите вкладку Импорт.

Нажмите Выберите файл → выберите user_import.csv.

В разделе Формат выберите CSV.

В поле Названия столбцов введите: role_name,full_name,login_text,password_text

Нажмите Импортировать.

Импорт товаров:

Нажмите на таблицу products_import_raw.

Импорт → выберите Tovar.csv.

Формат: CSV.

Названия столбцов: article_text,name_text,unit_text,price_text,supplier_text,manufacturer_text,category_text,discount_text,stock_text,description_text,photo_text

Нажмите Импортировать.

Импорт пунктов выдачи:

Нажмите на таблицу pickup_points_import_raw.

Импорт → выберите Пункты выдачи_import.csv.

Формат: CSV.

Названия столбцов: address_text

Нажмите Импортировать.

Импорт заказов:

Нажмите на таблицу orders_import_raw.

Импорт → выберите Заказ_import.csv.

Формат: CSV.

Названия столбцов: order_number_text,articles_text,order_date_text,delivery_date_text,pickup_point_text,client_fio_text,pickup_code_text,status_text

Нажмите Импортировать.

Шаг 6. Перенесите данные из временных таблиц в основные
Что делаем
Выполните SQL-запросы для заполнения основных таблиц.

Команды
Откройте вкладку SQL в phpMyAdmin и выполните:

sql
-- Заполняем роли
INSERT INTO roles (role_id, role_name) VALUES
(1, 'Гость'),
(2, 'Авторизированный клиент'),
(3, 'Менеджер'),
(4, 'Администратор');

-- Заполняем пользователей
INSERT INTO users (role_id, full_name, login, password_plain)
SELECT
    CASE
        WHEN role_name LIKE '%Администратор%' THEN 4
        WHEN role_name LIKE '%Менеджер%' THEN 3
        ELSE 2
    END,
    TRIM(full_name),
    TRIM(login_text),
    TRIM(REPLACE(password_text, '\r', ''))
FROM users_import_raw
WHERE TRIM(login_text) != '';

-- Заполняем категории
INSERT INTO categories (name)
SELECT DISTINCT TRIM(category_text)
FROM products_import_raw
WHERE TRIM(category_text) != '';

-- Заполняем производителей
INSERT INTO manufacturers (name)
SELECT DISTINCT TRIM(manufacturer_text)
FROM products_import_raw
WHERE TRIM(manufacturer_text) != '';

-- Заполняем поставщиков
INSERT INTO suppliers (name)
SELECT DISTINCT TRIM(supplier_text)
FROM products_import_raw
WHERE TRIM(supplier_text) != '';

-- Заполняем товары
INSERT INTO products (
    article, name, unit_name, price,
    supplier_id, manufacturer_id, category_id,
    discount_percent, stock_quantity, description_text, photo_file
)
SELECT
    TRIM(p.article_text),
    TRIM(p.name_text),
    TRIM(p.unit_text),
    CAST(REPLACE(TRIM(p.price_text), ',', '.') AS DECIMAL(12,2)),
    s.supplier_id,
    m.manufacturer_id,
    c.category_id,
    CAST(REPLACE(TRIM(p.discount_text), ',', '.') AS DECIMAL(5,2)),
    CAST(TRIM(p.stock_text) AS UNSIGNED),
    NULLIF(TRIM(p.description_text), ''),
    NULLIF(TRIM(REPLACE(p.photo_text, '\r', '')), '')
FROM products_import_raw p
JOIN suppliers s ON s.name = TRIM(p.supplier_text)
JOIN manufacturers m ON m.name = TRIM(p.manufacturer_text)
JOIN categories c ON c.name = TRIM(p.category_text)
WHERE TRIM(p.article_text) != '';

-- Заполняем пункты выдачи
INSERT INTO pickup_points (pickup_point_id, address_text)
SELECT raw_id, TRIM(REPLACE(address_text, '\r', ''))
FROM pickup_points_import_raw
WHERE TRIM(REPLACE(address_text, '\r', '')) != ''
ORDER BY raw_id;

-- Заполняем статусы заказов
INSERT INTO order_statuses (status_name)
SELECT DISTINCT TRIM(REPLACE(status_text, '\r', ''))
FROM orders_import_raw
WHERE TRIM(REPLACE(status_text, '\r', '')) != '';

-- Заполняем заказы
INSERT INTO orders (
    order_number, article_text, order_date, delivery_date,
    pickup_point_id, status_id
)
SELECT
    CAST(TRIM(o.order_number_text) AS UNSIGNED),
    TRIM(o.articles_text),
    STR_TO_DATE(TRIM(o.order_date_text), '%d.%m.%Y'),
    STR_TO_DATE(TRIM(o.delivery_date_text), '%d.%m.%Y'),
    CAST(TRIM(o.pickup_point_text) AS UNSIGNED),
    st.status_id
FROM orders_import_raw o
JOIN order_statuses st ON st.status_name = REPLACE(TRIM(o.status_text), '\r', '')
WHERE TRIM(o.order_number_text) != '';
Шаг 7. Создайте проект в Visual Studio
Что делаем
Создайте новый WPF проект и подключите MySQL библиотеку.

Пошагово
Откройте Visual Studio Community 2026.

Нажмите Создать проект.

В шаблоне выберите Приложение WPF (.NET Framework).

Имя проекта: BuildingMaterialsApp

Расположение: C:\building_materials_2026

Платформа: .NET Framework 4.8

Нажмите Создать.

Установка MySQL библиотеки
В меню Проект → Управление пакетами NuGet...

Перейдите на вкладку Обзор.

В строке поиска введите MySql.Data.

Выберите версию 8.4.0 и нажмите Установить.

Нажмите ОК если появится окно с предупреждением.

Добавление ресурсов в проект
Откройте папку C:\building_materials_2026\resources в Проводнике.

Скопируйте папку resources целиком.

В Visual Studio в Обозревателе решений нажмите на название проекта BuildingMaterialsApp правой кнопкой мыши → Открыть папку в проводнике.

Вставьте скопированную папку resources в открывшуюся папку проекта.

В Visual Studio нажмите Показать все файлы (иконка вверху Обозревателя решений).

Папка resources появится серым цветом. Нажмите на неё правой кнопкой → Включить в проект.

Для каждого файла в папке resources и resources/photos:

Нажмите на файл правой кнопкой → Свойства.

В поле Действие при сборке выберите Resource.

Шаг 8. Создайте класс Db.cs
Что делаем
Создайте класс для подключения к базе данных.

Пошагово
В Обозревателе решений нажмите на проект BuildingMaterialsApp правой кнопкой мыши.

Выберите Добавить → Класс...

Имя: Db.cs

Нажмите Добавить.

Код Db.cs
Полностью замените содержимое файла на:

csharp
using MySql.Data.MySqlClient;

namespace BuildingMaterialsApp
{
    internal static class Db
    {
        
        public const string ConnectionString =
            "Server=127.0.0.1;Port=3306;Database=building_materials_2026;Uid=root;Pwd=;Charset=utf8mb4;Allow Zero Datetime=True;Convert Zero Datetime=True;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}


для проверки: Откройте `App.xaml.cs` и временно замените содержимое на:

```csharp
using System.Windows;

namespace ShoeStore2026PUApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                }
                MessageBox.Show("OK: подключение к MySQL успешно.", "Проверка БД",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка подключения к MySQL:\n" + ex.Message, "Проверка БД",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
```

Запустите `F5`.  
После проверки верните `App.xaml.cs` в исходное состояние.

Шаг 9. Создайте класс Models.cs
Что делаем
Создайте классы-модели для хранения данных.

Пошагово
Правой кнопкой по проекту → Добавить → Класс...

Имя: Models.cs

Нажмите Добавить.

Код Models.cs
Полностью замените содержимое:

csharp
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BuildingMaterialsApp
{
    public class UserInfo
    {
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
    }

    public class LookupItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => Name;
    }

    public class ProductRow
    {
        public int ProductId { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string Supplier { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal FinalPrice { get; set; }
        public string UnitName { get; set; }
        public int StockQuantity { get; set; }
        public bool HasDiscount { get; set; }
        public string PhotoFile { get; set; }
        public BitmapImage PhotoImage { get; set; }

        public Brush RowBrush
        {
            get
            {
                if (StockQuantity == 0)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ADD8E6"));
                if (DiscountPercent > 12)
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F4A460"));
                return Brushes.White;
            }
        }
    }

    public class ProductEditModel
    {
        public int? ProductId { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public int ManufacturerId { get; set; }
        public string SupplierName { get; set; }
        public decimal Price { get; set; }
        public string UnitName { get; set; }
        public int StockQuantity { get; set; }
        public decimal DiscountPercent { get; set; }
        public string PhotoFile { get; set; }
    }

    public class OrderRow
    {
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        public string ArticlesText { get; set; }
        public string StatusName { get; set; }
        public string PickupAddress { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }

    public class OrderEditModel
    {
        public int? OrderId { get; set; }
        public int OrderNumber { get; set; }
        public string ArticlesText { get; set; }
        public int StatusId { get; set; }
        public int PickupPointId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
Шаг 10. Создайте класс DataService.cs
Что делаем
Создайте класс для всех операций с базой данных.

Пошагово
Правой кнопкой по проекту → Добавить → Класс...

Имя: DataService.cs

Нажмите Добавить.

Код DataService.cs
Полностью замените содержимое:

csharp
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace BuildingMaterialsApp
{
    internal static class DataService
    {
        public static UserInfo Auth(string login, string password)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT u.user_id, u.full_name, r.role_name
                        FROM users u
                        JOIN roles r ON r.role_id = u.role_id
                        WHERE u.login = @login AND u.password_plain = @password";
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read()) return null;
                        return new UserInfo
                        {
                            UserId = rd.GetInt32("user_id"),
                            FullName = rd.GetString("full_name"),
                            RoleName = rd.GetString("role_name")
                        };
                    }
                }
            }
        }

        public static List<ProductRow> GetProducts()
        {
            var result = new List<ProductRow>();

            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            p.product_id,
                            p.article,
                            p.name,
                            c.name AS category_name,
                            p.description_text,
                            m.name AS manufacturer_name,
                            s.name AS supplier_name,
                            p.price,
                            p.discount_percent,
                            p.unit_name,
                            p.stock_quantity,
                            p.photo_file
                        FROM products p
                        JOIN categories c ON c.category_id = p.category_id
                        JOIN manufacturers m ON m.manufacturer_id = p.manufacturer_id
                        JOIN suppliers s ON s.supplier_id = p.supplier_id
                        ORDER BY p.product_id";

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            var price = Convert.ToDecimal(rd["price"]);
                            var discount = Convert.ToDecimal(rd["discount_percent"]);
                            var finalPrice = Math.Round(price * (100m - discount) / 100m, 2);
                            var photoFile = rd["photo_file"] == DBNull.Value ? "" : rd["photo_file"].ToString();

                            result.Add(new ProductRow
                            {
                                ProductId = Convert.ToInt32(rd["product_id"]),
                                Article = rd["article"].ToString(),
                                Name = rd["name"].ToString(),
                                Category = rd["category_name"].ToString(),
                                Description = rd["description_text"] == DBNull.Value ? "" : rd["description_text"].ToString(),
                                Manufacturer = rd["manufacturer_name"].ToString(),
                                Supplier = rd["supplier_name"].ToString(),
                                Price = price,
                                DiscountPercent = discount,
                                FinalPrice = finalPrice,
                                UnitName = rd["unit_name"].ToString(),
                                StockQuantity = Convert.ToInt32(rd["stock_quantity"]),
                                HasDiscount = discount > 0,
                                PhotoFile = photoFile,
                                PhotoImage = LoadPhoto(photoFile)
                            });
                        }
                    }
                }
            }
            return result;
        }

        public static List<string> GetManufacturerNames()
        {
            var result = new List<string>();
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM manufacturers ORDER BY name";
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                            result.Add(rd.GetString("name"));
                    }
                }
            }
            return result;
        }

        public static List<LookupItem> GetCategories()
        {
            var result = new List<LookupItem>();
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT category_id AS id, name FROM categories ORDER BY name";
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            result.Add(new LookupItem
                            {
                                Id = Convert.ToInt32(rd["id"]),
                                Name = rd["name"].ToString()
                            });
                        }
                    }
                }
            }
            return result;
        }

        public static List<LookupItem> GetManufacturers()
        {
            var result = new List<LookupItem>();
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT manufacturer_id AS id, name FROM manufacturers ORDER BY name";
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            result.Add(new LookupItem
                            {
                                Id = Convert.ToInt32(rd["id"]),
                                Name = rd["name"].ToString()
                            });
                        }
                    }
                }
            }
            return result;
        }

        public static List<LookupItem> GetStatuses()
        {
            var result = new List<LookupItem>();
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT status_id AS id, status_name AS name FROM order_statuses ORDER BY name";
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            result.Add(new LookupItem
                            {
                                Id = Convert.ToInt32(rd["id"]),
                                Name = rd["name"].ToString()
                            });
                        }
                    }
                }
            }
            return result;
        }

        public static List<LookupItem> GetPickupPoints()
        {
            var result = new List<LookupItem>();
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT pickup_point_id AS id, address_text AS name FROM pickup_points ORDER BY pickup_point_id";
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            result.Add(new LookupItem
                            {
                                Id = Convert.ToInt32(rd["id"]),
                                Name = rd["name"].ToString()
                            });
                        }
                    }
                }
            }
            return result;
        }

        public static ProductEditModel GetProductById(int productId)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            p.product_id,
                            p.article,
                            p.name,
                            p.category_id,
                            p.description_text,
                            p.manufacturer_id,
                            s.name AS supplier_name,
                            p.price,
                            p.unit_name,
                            p.stock_quantity,
                            p.discount_percent,
                            p.photo_file
                        FROM products p
                        JOIN suppliers s ON s.supplier_id = p.supplier_id
                        WHERE p.product_id = @id";
                    cmd.Parameters.AddWithValue("@id", productId);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read()) return null;
                        return new ProductEditModel
                        {
                            ProductId = Convert.ToInt32(rd["product_id"]),
                            Article = rd["article"].ToString(),
                            Name = rd["name"].ToString(),
                            CategoryId = Convert.ToInt32(rd["category_id"]),
                            Description = rd["description_text"] == DBNull.Value ? "" : rd["description_text"].ToString(),
                            ManufacturerId = Convert.ToInt32(rd["manufacturer_id"]),
                            SupplierName = rd["supplier_name"].ToString(),
                            Price = Convert.ToDecimal(rd["price"]),
                            UnitName = rd["unit_name"].ToString(),
                            StockQuantity = Convert.ToInt32(rd["stock_quantity"]),
                            DiscountPercent = Convert.ToDecimal(rd["discount_percent"]),
                            PhotoFile = rd["photo_file"] == DBNull.Value ? "" : rd["photo_file"].ToString()
                        };
                    }
                }
            }
        }

        public static void SaveProduct(ProductEditModel model)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        int supplierId;
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = "SELECT supplier_id FROM suppliers WHERE name = @name";
                            cmd.Parameters.AddWithValue("@name", model.SupplierName);
                            var existing = cmd.ExecuteScalar();
                            if (existing != null && existing != DBNull.Value)
                            {
                                supplierId = Convert.ToInt32(existing);
                            }
                            else
                            {
                                cmd.CommandText = "INSERT INTO suppliers(name) VALUES(@name)";
                                cmd.ExecuteNonQuery();
                                supplierId = (int)cmd.LastInsertedId;
                            }
                        }

                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            if (model.ProductId.HasValue)
                            {
                                cmd.CommandText = @"
                                    UPDATE products SET
                                        article = @article,
                                        name = @name,
                                        category_id = @categoryId,
                                        description_text = @description,
                                        manufacturer_id = @manufacturerId,
                                        supplier_id = @supplierId,
                                        price = @price,
                                        unit_name = @unitName,
                                        stock_quantity = @stock,
                                        discount_percent = @discount,
                                        photo_file = @photo
                                    WHERE product_id = @id";
                                cmd.Parameters.AddWithValue("@id", model.ProductId.Value);
                            }
                            else
                            {
                                cmd.CommandText = @"
                                    INSERT INTO products(
                                        article, name, category_id, description_text,
                                        manufacturer_id, supplier_id, price, unit_name,
                                        stock_quantity, discount_percent, photo_file
                                    ) VALUES(
                                        @article, @name, @categoryId, @description,
                                        @manufacturerId, @supplierId, @price, @unitName,
                                        @stock, @discount, @photo
                                    )";
                            }

                            cmd.Parameters.AddWithValue("@article", model.Article);
                            cmd.Parameters.AddWithValue("@name", model.Name);
                            cmd.Parameters.AddWithValue("@categoryId", model.CategoryId);
                            cmd.Parameters.AddWithValue("@description", string.IsNullOrWhiteSpace(model.Description) ? DBNull.Value : (object)model.Description);
                            cmd.Parameters.AddWithValue("@manufacturerId", model.ManufacturerId);
                            cmd.Parameters.AddWithValue("@supplierId", supplierId);
                            cmd.Parameters.AddWithValue("@price", model.Price);
                            cmd.Parameters.AddWithValue("@unitName", model.UnitName);
                            cmd.Parameters.AddWithValue("@stock", model.StockQuantity);
                            cmd.Parameters.AddWithValue("@discount", model.DiscountPercent);
                            cmd.Parameters.AddWithValue("@photo", string.IsNullOrWhiteSpace(model.PhotoFile) ? DBNull.Value : (object)model.PhotoFile);
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public static bool ProductExistsInOrders(int productId)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM orders WHERE article_text LIKE CONCAT('%', @article, '%')";
                    cmd.Parameters.AddWithValue("@article", productId.ToString());
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public static void DeleteProduct(int productId)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM products WHERE product_id = @id";
                    cmd.Parameters.AddWithValue("@id", productId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<OrderRow> GetOrders()
        {
            var result = new List<OrderRow>();
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            o.order_id,
                            o.order_number,
                            o.article_text,
                            os.status_name,
                            pp.address_text,
                            DATE_FORMAT(o.order_date, '%Y-%m-%d') AS order_date_text,
                            DATE_FORMAT(o.delivery_date, '%Y-%m-%d') AS delivery_date_text
                        FROM orders o
                        JOIN order_statuses os ON os.status_id = o.status_id
                        JOIN pickup_points pp ON pp.pickup_point_id = o.pickup_point_id
                        ORDER BY o.order_number";
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            result.Add(new OrderRow
                            {
                                OrderId = Convert.ToInt32(rd["order_id"]),
                                OrderNumber = Convert.ToInt32(rd["order_number"]),
                                ArticlesText = rd["article_text"].ToString(),
                                StatusName = rd["status_name"].ToString(),
                                PickupAddress = rd["address_text"].ToString(),
                                OrderDate = rd["order_date_text"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(rd["order_date_text"]),
                                DeliveryDate = rd["delivery_date_text"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(rd["delivery_date_text"])
                            });
                        }
                    }
                }
            }
            return result;
        }

        public static OrderEditModel GetOrderById(int orderId)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            order_id, order_number, article_text, status_id,
                            pickup_point_id, order_date, delivery_date
                        FROM orders
                        WHERE order_id = @id";
                    cmd.Parameters.AddWithValue("@id", orderId);
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read()) return null;
                        return new OrderEditModel
                        {
                            OrderId = Convert.ToInt32(rd["order_id"]),
                            OrderNumber = Convert.ToInt32(rd["order_number"]),
                            ArticlesText = rd["article_text"].ToString(),
                            StatusId = Convert.ToInt32(rd["status_id"]),
                            PickupPointId = Convert.ToInt32(rd["pickup_point_id"]),
                            OrderDate = rd["order_date"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(rd["order_date"]),
                            DeliveryDate = rd["delivery_date"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(rd["delivery_date"])
                        };
                    }
                }
            }
        }

        public static int GetNextOrderNumber()
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COALESCE(MAX(order_number), 0) + 1 FROM orders";
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void SaveOrder(OrderEditModel model)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            if (model.OrderId.HasValue)
                            {
                                cmd.CommandText = @"
                                    UPDATE orders SET
                                        article_text = @articles,
                                        status_id = @statusId,
                                        pickup_point_id = @pickupId,
                                        order_date = @orderDate,
                                        delivery_date = @deliveryDate
                                    WHERE order_id = @id";
                                cmd.Parameters.AddWithValue("@id", model.OrderId.Value);
                            }
                            else
                            {
                                cmd.CommandText = @"
                                    INSERT INTO orders(order_number, article_text, order_date, delivery_date, pickup_point_id, status_id)
                                    VALUES(@orderNumber, @articles, @orderDate, @deliveryDate, @pickupId, @statusId)";
                                cmd.Parameters.AddWithValue("@orderNumber", model.OrderNumber);
                            }

                            cmd.Parameters.AddWithValue("@articles", model.ArticlesText);
                            cmd.Parameters.AddWithValue("@statusId", model.StatusId);
                            cmd.Parameters.AddWithValue("@pickupId", model.PickupPointId);
                            cmd.Parameters.AddWithValue("@orderDate", model.OrderDate);
                            cmd.Parameters.AddWithValue("@deliveryDate", model.DeliveryDate);
                            cmd.ExecuteNonQuery();
                        }
                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void DeleteOrder(int orderId)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM orders WHERE order_id = @id";
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static BitmapImage LoadPhoto(string fileName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    var cleanName = Path.GetFileName(fileName.Trim());
                    var packUri = $"pack://application:,,,/resources/photos/{cleanName}";
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(packUri, UriKind.Absolute);
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
            catch { }

            var defaultImage = new BitmapImage();
            defaultImage.BeginInit();
            defaultImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultImage.UriSource = new Uri("pack://application:,,,/resources/picture.png", UriKind.Absolute);
            defaultImage.EndInit();
            defaultImage.Freeze();
            return defaultImage;
        }
    }
}
Шаг 11. Создайте окно входа LoginWindow
Что делаем
Создайте окно для ввода логина и пароля.

Пошагово
В Обозревателе решений нажмите на проект BuildingMaterialsApp правой кнопкой мыши.

Выберите Добавить → Окно (WPF)...

Имя: LoginWindow.xaml

Нажмите Добавить.

Код LoginWindow.xaml
Полностью замените содержимое файла:

xml
<Window x:Class="BuildingMaterialsApp.LoginWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Авторизация"
        Height="320"
        Width="520"
        FontFamily="Calibri"
        Background="#FFFFFF"
        Icon="resources/Icon.ico"
        WindowStartupLocation="CenterScreen">
    <Grid Margin="18">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <TextBlock Text="Логин" FontSize="16" FontWeight="Bold" Margin="0,0,0,6"/>
        <TextBox x:Name="LoginTextBox" Grid.Row="1" Height="34" FontSize="14" Margin="0,0,0,12" Padding="5"/>

        <TextBlock Text="Пароль" Grid.Row="2" FontSize="16" FontWeight="Bold" Margin="0,0,0,6"/>
        <PasswordBox x:Name="PasswordBox" Grid.Row="3" Height="34" FontSize="14" Margin="0,0,0,12"/>

        <StackPanel Grid.Row="4" Orientation="Horizontal" HorizontalAlignment="Right">
            <Button Content="Войти" Width="120" Height="36" Margin="0,0,10,0" Background="#DAA520" Click="Login_Click"/>
            <Button Content="Войти как гость" Width="150" Height="36" Background="#DAA520" Click="Guest_Click"/>
        </StackPanel>
    </Grid>
</Window>
Код LoginWindow.xaml.cs
Полностью замените содержимое:

csharp
using System;
using System.Windows;

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
Шаг 12. Создайте главное окно MainWindow
Что делаем
Создайте главное окно со списком товаров.

Пошагово
Правой кнопкой по проекту → Добавить → Окно (WPF)...

Имя: MainWindow.xaml

Нажмите Добавить.

Код MainWindow.xaml
Полностью замените содержимое:

xml
<Window x:Class="BuildingMaterialsApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="СтройМатериалы - Список товаров"
        Height="800"
        Width="1500"
        FontFamily="Calibri"
        Background="#FFFFFF"
        Icon="resources/Icon.ico"
        WindowStartupLocation="CenterScreen">
    <Grid Margin="12">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- Верхняя панель с логотипом и информацией о пользователе -->
        <Grid Grid.Row="0" Margin="0,0,0,10">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto"/>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>

            <Image Source="resources/Icon.png" Width="100" Height="60" Stretch="Uniform" Margin="0,0,15,0"/>

            <TextBlock Grid.Column="1" Text="ООО «СтройМатериалы» — Список товаров"
                       VerticalAlignment="Center" FontSize="24" FontWeight="Bold"/>

            <StackPanel Grid.Column="2" HorizontalAlignment="Right">
                <TextBlock x:Name="RoleTextBlock" FontSize="14" TextAlignment="Right" FontWeight="Bold"/>
                <TextBlock x:Name="UserTextBlock" FontSize="14" TextAlignment="Right" Margin="0,5,0,10"/>
                <Button x:Name="OrdersButton" Content="Заказы" Width="100" Height="32" 
                        Margin="0,0,0,8" Background="#DAA520" Click="Orders_Click"/>
                <Button Content="Выход" Width="100" Height="32" Background="#DAA520" Click="Logout_Click"/>
            </StackPanel>
        </Grid>

        <!-- Панель поиска, фильтра и сортировки (только для менеджера и администратора) -->
        <StackPanel x:Name="FilterPanel" Grid.Row="1" Orientation="Horizontal" Margin="0,0,0,10" Visibility="Collapsed">
            <TextBlock Text="Поиск:" VerticalAlignment="Center" Margin="0,0,8,0" FontSize="14"/>
            <TextBox x:Name="SearchTextBox" Width="300" Margin="0,0,20,0" Padding="5" TextChanged="SearchTextBox_TextChanged"/>

            <TextBlock Text="Производитель:" VerticalAlignment="Center" Margin="0,0,8,0" FontSize="14"/>
            <ComboBox x:Name="ManufacturerComboBox" Width="220" Margin="0,0,20,0" SelectionChanged="ManufacturerFilter_Changed"/>

            <TextBlock Text="Сортировка:" VerticalAlignment="Center" Margin="0,0,8,0" FontSize="14"/>
            <ComboBox x:Name="SortComboBox" Width="200" SelectionChanged="SortComboBox_Changed">
                <ComboBoxItem Content="Без сортировки" IsSelected="True"/>
                <ComboBoxItem Content="Цена (по возрастанию)"/>
                <ComboBoxItem Content="Цена (по убыванию)"/>
                <ComboBoxItem Content="Остаток (по возрастанию)"/>
                <ComboBoxItem Content="Остаток (по убыванию)"/>
                <ComboBoxItem Content="Скидка (по возрастанию)"/>
                <ComboBoxItem Content="Скидка (по убыванию)"/>
            </ComboBox>
        </StackPanel>

        <!-- Панель кнопок для администратора -->
        <StackPanel x:Name="AdminPanel" Grid.Row="2" Orientation="Horizontal" Margin="0,0,0,10" Visibility="Collapsed">
            <Button Content="Добавить товар" Width="140" Height="32" Margin="0,0,10,0" Background="#B8860B" Click="Add_Click"/>
            <Button Content="Редактировать товар" Width="150" Height="32" Margin="0,0,10,0" Background="#B8860B" Click="Edit_Click"/>
            <Button Content="Удалить товар" Width="130" Height="32" Background="#B8860B" Click="Delete_Click"/>
        </StackPanel>

        <!-- Таблица товаров -->
        <DataGrid Grid.Row="3"
                  x:Name="ProductsGrid"
                  AutoGenerateColumns="False"
                  IsReadOnly="True"
                  HeadersVisibility="Column"
                  GridLinesVisibility="Horizontal"
                  RowHeight="70"
                  MouseDoubleClick="ProductsGrid_MouseDoubleClick">
            <DataGrid.RowStyle>
                <Style TargetType="DataGridRow">
                    <Setter Property="Background" Value="{Binding RowBrush}"/>
                </Style>
            </DataGrid.RowStyle>

            <DataGrid.Columns>
                <DataGridTemplateColumn Header="Фото" Width="100">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <Image Source="{Binding PhotoImage}" Width="80" Height="60" Stretch="Uniform"/>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>

                <DataGridTextColumn Header="Артикул" Binding="{Binding Article}" Width="100"/>
                <DataGridTextColumn Header="Наименование" Binding="{Binding Name}" Width="180"/>
                <DataGridTextColumn Header="Категория" Binding="{Binding Category}" Width="140"/>
                <DataGridTextColumn Header="Описание" Binding="{Binding Description}" Width="200"/>
                <DataGridTextColumn Header="Производитель" Binding="{Binding Manufacturer}" Width="140"/>
                <DataGridTextColumn Header="Поставщик" Binding="{Binding Supplier}" Width="140"/>

                <DataGridTemplateColumn Header="Цена" Width="110">
                    <DataGridTemplateColumn.CellTemplate>
                        <DataTemplate>
                            <StackPanel>
                                <TextBlock Text="{Binding Price, StringFormat={}{0:N2}}">
                                    <TextBlock.Style>
                                        <Style TargetType="TextBlock">
                                            <Setter Property="Foreground" Value="Black"/>
                                            <Style.Triggers>
                                                <DataTrigger Binding="{Binding HasDiscount}" Value="True">
                                                    <Setter Property="Foreground" Value="Red"/>
                                                    <Setter Property="TextDecorations" Value="Strikethrough"/>
                                                </DataTrigger>
                                            </Style.Triggers>
                                        </Style>
                                    </TextBlock.Style>
                                </TextBlock>
                            </StackPanel>
                        </DataTemplate>
                    </DataGridTemplateColumn.CellTemplate>
                </DataGridTemplateColumn>

                <DataGridTextColumn Header="Цена со скидкой" Binding="{Binding FinalPrice, StringFormat={}{0:N2}}" Width="120"/>
                <DataGridTextColumn Header="Ед. изм." Binding="{Binding UnitName}" Width="80"/>
                <DataGridTextColumn Header="Остаток" Binding="{Binding StockQuantity}" Width="80"/>
                <DataGridTextColumn Header="Скидка %" Binding="{Binding DiscountPercent}" Width="90"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</Window>
Код MainWindow.xaml.cs
Полностью замените содержимое:

csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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

            // Отображаем информацию о пользователе
            RoleTextBlock.Text = $"Роль: {GetRoleDisplayName(_currentUser.RoleName)}";
            UserTextBlock.Text = $"Пользователь: {_currentUser.FullName}";

            // Показываем кнопку Заказы только для менеджера и администратора
            bool isManagerOrAdmin = _currentUser.RoleName == "Менеджер" || _currentUser.RoleName == "Администратор";
            OrdersButton.Visibility = isManagerOrAdmin ? Visibility.Visible : Visibility.Collapsed;

            // Показываем панель фильтрации только для менеджера и администратора
            FilterPanel.Visibility = isManagerOrAdmin ? Visibility.Visible : Visibility.Collapsed;

            // Показываем панель администратора только для администратора
            AdminPanel.Visibility = _currentUser.RoleName == "Администратор" ? Visibility.Visible : Visibility.Collapsed;

            // Загружаем список производителей для фильтра
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
Шаг 13. Создайте форму для редактирования товара ProductFormWindow
Что делаем
Создайте окно для добавления и редактирования товаров.

Пошагово
Правой кнопкой по проекту → Добавить → Окно (WPF)...

Имя: ProductFormWindow.xaml

Нажмите Добавить.

Код ProductFormWindow.xaml
xml
<Window x:Class="BuildingMaterialsApp.ProductFormWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Добавление/редактирование товара"
        Height="750"
        Width="950"
        FontFamily="Calibri"
        Background="#FFFFFF"
        Icon="resources/Icon.ico"
        WindowStartupLocation="CenterOwner">
    <Grid Margin="15">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <Grid Grid.Row="0">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="2*"/>
                <ColumnDefinition Width="1*"/>
            </Grid.ColumnDefinitions>

            <!-- Левая панель с полями -->
            <Grid Grid.Column="0" Margin="0,0,15,0">
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="180"/>
                    <ColumnDefinition Width="*"/>
                </Grid.ColumnDefinitions>

                <TextBlock Text="ID товара:" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <TextBox x:Name="IdTextBox" Grid.Column="1" IsReadOnly="True" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="1" Text="Артикул:" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <TextBox x:Name="ArticleTextBox" Grid.Row="1" Grid.Column="1" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="2" Text="Наименование:" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <TextBox x:Name="NameTextBox" Grid.Row="2" Grid.Column="1" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="3" Text="Категория:" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <ComboBox x:Name="CategoryComboBox" Grid.Row="3" Grid.Column="1" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="4" Text="Описание:" VerticalAlignment="Top" Margin="0,0,0,8"/>
                <TextBox x:Name="DescriptionTextBox" Grid.Row="4" Grid.Column="1" Height="80" 
                         TextWrapping="Wrap" AcceptsReturn="True" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="5" Text="Производитель:" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <ComboBox x:Name="ManufacturerComboBox" Grid.Row="5" Grid.Column="1" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="6" Text="Поставщик:" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <TextBox x:Name="SupplierTextBox" Grid.Row="6" Grid.Column="1" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="7" Text="Цена:" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <TextBox x:Name="PriceTextBox" Grid.Row="7" Grid.Column="1" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="8" Text="Единица измерения:" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <TextBox x:Name="UnitTextBox" Grid.Row="8" Grid.Column="1" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="9" Text="Количество на складе:" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <TextBox x:Name="StockTextBox" Grid.Row="9" Grid.Column="1" Margin="0,0,0,8"/>

                <TextBlock Grid.Row="10" Text="Скидка (%):" VerticalAlignment="Center" Margin="0,0,0,8"/>
                <TextBox x:Name="DiscountTextBox" Grid.Row="10" Grid.Column="1" Margin="0,0,0,8"/>
            </Grid>

            <!-- Правая панель с фото -->
            <StackPanel Grid.Column="1">
                <TextBlock Text="Фото товара" FontSize="14" FontWeight="Bold" Margin="0,0,0,10"/>
                <Border BorderBrush="#DAA520" BorderThickness="2" Width="300" Height="200">
                    <Image x:Name="PhotoImage" Stretch="Uniform"/>
                </Border>
                <Button Content="Выбрать фото" Width="150" Height="35" Margin="0,15,0,0" 
                        Background="#DAA520" Click="ChoosePhoto_Click"/>
            </StackPanel>
        </Grid>

        <!-- Кнопки внизу -->
        <StackPanel Grid.Row="1" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,20,0,0">
            <Button Content="Сохранить" Width="120" Height="36" Margin="0,0,10,0" Background="#DAA520" Click="Save_Click"/>
            <Button Content="Отмена" Width="120" Height="36" Background="#DAA520" Click="Cancel_Click"/>
        </StackPanel>
    </Grid>
</Window>
Код ProductFormWindow.xaml.cs
csharp
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

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
Шаг 14. Создайте окно списка заказов OrdersWindow
Что делаем
Создайте окно для просмотра заказов (для менеджера и администратора).

Пошагово
Правой кнопкой по проекту → Добавить → Окно (WPF)...

Имя: OrdersWindow.xaml

Нажмите Добавить.

Код OrdersWindow.xaml
xml
<Window x:Class="BuildingMaterialsApp.OrdersWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Управление заказами"
        Height="650"
        Width="1100"
        FontFamily="Calibri"
        Background="#FFFFFF"
        Icon="resources/Icon.ico"
        WindowStartupLocation="CenterOwner">
    <Grid Margin="15">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <StackPanel Orientation="Horizontal" Margin="0,0,0,15">
            <TextBlock Text="Список заказов" FontSize="22" FontWeight="Bold" VerticalAlignment="Center"/>
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Right" Margin="20,0,0,0">
                <Button x:Name="AddButton" Content="Добавить заказ" Width="130" Height="34" 
                        Margin="0,0,10,0" Background="#DAA520" Click="Add_Click"/>
                <Button x:Name="EditButton" Content="Редактировать" Width="130" Height="34" 
                        Margin="0,0,10,0" Background="#DAA520" Click="Edit_Click"/>
                <Button x:Name="DeleteButton" Content="Удалить" Width="100" Height="34" 
                        Margin="0,0,10,0" Background="#DAA520" Click="Delete_Click"/>
                <Button Content="Назад" Width="100" Height="34" Background="#DAA520" Click="Back_Click"/>
            </StackPanel>
        </StackPanel>

        <DataGrid Grid.Row="1"
                  x:Name="OrdersGrid"
                  AutoGenerateColumns="False"
                  IsReadOnly="True"
                  HeadersVisibility="Column"
                  MouseDoubleClick="OrdersGrid_MouseDoubleClick">
            <DataGrid.Columns>
                <DataGridTextColumn Header="Номер заказа" Binding="{Binding OrderNumber}" Width="100"/>
                <DataGridTextColumn Header="Артикулы" Binding="{Binding ArticlesText}" Width="350"/>
                <DataGridTextColumn Header="Статус" Binding="{Binding StatusName}" Width="120"/>
                <DataGridTextColumn Header="Адрес выдачи" Binding="{Binding PickupAddress}" Width="250"/>
                <DataGridTextColumn Header="Дата заказа" Binding="{Binding OrderDate, StringFormat=yyyy-MM-dd}" Width="110"/>
                <DataGridTextColumn Header="Дата выдачи" Binding="{Binding DeliveryDate, StringFormat=yyyy-MM-dd}" Width="110"/>
            </DataGrid.Columns>
        </DataGrid>
    </Grid>
</Window>
Код OrdersWindow.xaml.cs
csharp
using System;
using System.Windows;

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
Шаг 15. Создайте форму редактирования заказа OrderFormWindow
Что делаем
Создайте окно для добавления и редактирования заказов.

Пошагово
Правой кнопкой по проекту → Добавить → Окно (WPF)...

Имя: OrderFormWindow.xaml

Нажмите Добавить.

Код OrderFormWindow.xaml
xml
<Window x:Class="BuildingMaterialsApp.OrderFormWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Добавление/редактирование заказа"
        Height="450"
        Width="650"
        FontFamily="Calibri"
        Background="#FFFFFF"
        Icon="resources/Icon.ico"
        WindowStartupLocation="CenterOwner">
    <Grid Margin="20">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <Grid>
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="150"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <TextBlock Text="Номер заказа:" VerticalAlignment="Center" Margin="0,0,0,10"/>
            <TextBox x:Name="OrderNumberTextBox" Grid.Column="1" IsReadOnly="True" Margin="0,0,0,10"/>

            <TextBlock Grid.Row="1" Text="Артикулы (артикул,кол-во):" VerticalAlignment="Center" Margin="0,0,0,10"/>
            <TextBox x:Name="ArticlesTextBox" Grid.Row="1" Grid.Column="1" Height="60" 
                     TextWrapping="Wrap" AcceptsReturn="True" Margin="0,0,0,10"/>

            <TextBlock Grid.Row="2" Text="Статус заказа:" VerticalAlignment="Center" Margin="0,0,0,10"/>
            <ComboBox x:Name="StatusComboBox" Grid.Row="2" Grid.Column="1" Margin="0,0,0,10"/>

            <TextBlock Grid.Row="3" Text="Пункт выдачи:" VerticalAlignment="Center" Margin="0,0,0,10"/>
            <ComboBox x:Name="PickupComboBox" Grid.Row="3" Grid.Column="1" Margin="0,0,0,10"/>

            <TextBlock Grid.Row="4" Text="Дата заказа:" VerticalAlignment="Center" Margin="0,0,0,10"/>
            <DatePicker x:Name="OrderDatePicker" Grid.Row="4" Grid.Column="1" Margin="0,0,0,10"/>

            <TextBlock Grid.Row="5" Text="Дата выдачи:" VerticalAlignment="Center" Margin="0,0,0,10"/>
            <DatePicker x:Name="DeliveryDatePicker" Grid.Row="5" Grid.Column="1" Margin="0,0,0,10"/>
        </Grid>

        <StackPanel Grid.Row="1" Orientation="Horizontal" HorizontalAlignment="Right" Margin="0,20,0,0">
            <Button Content="Сохранить" Width="100" Height="34" Margin="0,0,10,0" Background="#DAA520" Click="Save_Click"/>
            <Button Content="Отмена" Width="100" Height="34" Background="#DAA520" Click="Cancel_Click"/>
        </StackPanel>
    </Grid>
</Window>
Код OrderFormWindow.xaml.cs
csharp
using System;
using System.Windows;

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
Шаг 16. Настройте запуск приложения
Что делаем
Установите LoginWindow как стартовое окно.

Пошагово
Откройте файл App.xaml и замените его содержимое:

xml
<Application x:Class="BuildingMaterialsApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="LoginWindow.xaml">
    <Application.Resources>
    </Application.Resources>
</Application>
Шаг 17. Запустите и проверьте приложение
Что делаем
Запустите проект и проверьте все функции.

Пошагово
Нажмите F5 или кнопку Запуск в Visual Studio.

Проверьте вход с разными ролями:

Для входа используйте данные из БД:

Администратор: 94d5ous@gmail.com / uzWC67

Менеджер: 1diph5e@tutanota.com / 8ntwUp

Клиент: 5d4zbu@tutanota.com / rwVDh9

Проверьте:

Гость видит только товары (без фильтров)

Менеджер видит поиск, фильтр по производителю, сортировку и кнопку "Заказы"

Администратор видит всё то же + кнопки добавления/редактирования/удаления товаров + может редактировать заказы

Цвета строк: скидка >12% → оранжевый, нет на складе → голубой

При наличии скидки базовая цена красная зачёркнутая

## Шаг 17.1. Зафиксируйте локальный git-коммит (модули 2-4)
### Что делаем
Сделайте локальный коммит после реализации функционала модулей 2–4.

### Команды/код
```cmd
cd C:\building_materials_2026\BuildingMaterialsApp
git init
git add .
git commit -m "Реализованы модули 2-4"
```

Шаг 18. Подготовьте отчетные документы
Что делаем
Создайте все необходимые для экзамена документы.

1. ER-диаграмма (PDF)
В phpMyAdmin откройте базу building_materials_2026.

Нажмите Дизайнер (вкладка "Ещё" → "Дизайнер").

Нажмите Экспорт схемы → выберите PDF.

Сохраните как C:\building_materials_2026\sql\er_diagram.pdf

2. Скрипт базы данных (SQL)
В phpMyAdmin выберите базу building_materials_2026.

Нажмите вкладку Экспорт.

Выберите метод "Быстрый", формат SQL.

Нажмите Выполнить и сохраните как C:\building_materials_2026\sql\building_materials_2026.sql

3. Блок-схема алгоритма (PDF)
Откройте draw.io (диаграммы.net) в браузере.

Создайте новую диаграмму.

Нарисуйте блок-схему по ГОСТ 19.701-90:

Основные блоки:

Овал "Начало"

Прямоугольник "Открыть окно входа"

Параллелограмм "Ввести логин/пароль или нажать 'Войти как гость'"

Ромб "Выбран вход как гость?"

Да → Прямоугольник "Открыть главное окно с ролью Гость"

Нет → Ромб "Логин и пароль верны?"

Нет → Параллелограмм "Показать ошибку" → стрелка к вводу

Да → Прямоугольник "Определить роль пользователя"

Прямоугольник "Открыть главное окно с соответствующей ролью"

Ромб "Роль = Менеджер или Администратор?"

Да → Прямоугольник "Показать поиск, фильтр, сортировку"

Ромб "Роль = Администратор?"

Да → Прямоугольник "Показать кнопки CRUD товаров"

Прямоугольник "При нажатии 'Заказы' открыть окно заказов"

Ромб "Роль = Администратор?"

Да → Прямоугольник "Показать кнопки CRUD заказов"

Ромб "Нажата кнопка 'Выход'?"

Да → Овал "Конец"

Сохраните как C:\building_materials_2026\docs\algorithm_gost.drawio

Экспортируйте в PDF: Файл → Экспорт → PDF → C:\building_materials_2026\docs\algorithm_gost.pdf

4. Отчет со скриншотами (DOCX)
Создайте файл C:\building_materials_2026\docs\report_screenshots.docx

Добавьте скриншоты (через программу "Ножницы" или PrintScreen):

Окно входа

Список товаров от имени гостя

Список товаров от имени менеджера (с поиском/фильтром/сортировкой)

Список товаров от имени администратора (с кнопками)

Окно добавления товара

Окно редактирования товара

Подтверждение удаления товара

Окно списка заказов

Окно добавления заказа

Окно редактирования заказа

5. Исполняемые файлы
В Visual Studio переключите конфигурацию на Release.

Меню Сборка → Собрать решение.

Папка с результатами: C:\building_materials_2026\BuildingMaterialsApp\bin\Release\

Скопируйте всю папку Release в C:\building_materials_2026\executable

6. Финальная структура папок
text
C:\building_materials_2026\
├── resources\           (исходные фото, иконки)
├── docs\
│   ├── algorithm_gost.pdf
│   └── report_screenshots.docx
├── sql\
│   ├── building_materials_2026.sql
│   └── er_diagram.pdf
├── executable\          (вся папка Release)
└── BuildingMaterialsApp\ (исходный код проекта)

Зафиксируйте итог в локальном git-репозитории:

```cmd
cd C:\building_materials_2026\BuildingMaterialsApp
git add .
git status
git commit -m "Финальная версия проекта"
```


если выдает ошибку после гит адд то: Создай файл:

.gitignore

в корне проекта В РЕШЕНИЕ
C:\building_materials_2026\BuildingMaterialsApp

И вставь туда:

.vs/
bin/
obj/
*.user
*.suo
packages/


Для переноса всей бд в новую пустую В phpMyAdmin:

Создай пустую БД с именем building_materials_2026

Зайди в эту БД

Нажми вкладку Импорт

Нажми Выберите файл → выбери твой building_materials_2026.sql

Нажми Импортировать
в конце сделать заново экспорт бд.




итог завтра: Создать БД через импорт SQL
phpMyAdmin → создать БД building_materials_2026

Импорт → выбрать building_materials_2026.sql

 Открыть проект в Visual Studio
Дважды кликнуть BuildingMaterialsApp.sln

 Проверить строку подключения в Db.cs
csharp
"Server=127.0.0.1;Port=3306;Database=building_materials_2026;Uid=root;Pwd=;..."
Если на экзаменационном компьютере пароль MySQL отличается (например, root с паролем или пустой) — поменяй.

 Собрать проект
Сборка → Собрать решение

 Запустить (F5)