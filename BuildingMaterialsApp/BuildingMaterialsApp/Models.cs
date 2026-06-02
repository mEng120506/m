using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
