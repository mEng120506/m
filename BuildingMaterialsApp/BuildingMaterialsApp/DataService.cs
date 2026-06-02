using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.IO;
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
