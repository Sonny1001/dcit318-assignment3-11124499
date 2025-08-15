using System;
using System.Collections.Generic;

namespace WareHouseManager
{
    // a) Marker interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b) ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
        }

        public override string ToString() => $"ElectronicItem {{ Id={Id}, Name={Name}, Brand={Brand}, Warranty={WarrantyMonths}m, Qty={Quantity} }}";
    }

    // c) GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
        }

        public override string ToString() => $"GroceryItem {{ Id={Id}, Name={Name}, Expiry={ExpiryDate:d}, Qty={Quantity} }}";
    }

    // d) Generic repository with constraints
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with Id {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with Id {id} not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Item with Id {id} not found.");
        }

        public List<T> GetAllItems() => new List<T>(_items.Values);

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // e) Custom exceptions
    public class DuplicateItemException : Exception
    {
        public DuplicateItemException(string message) : base(message) { }
    }

    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message) : base(message) { }
    }

    public class InvalidQuantityException : Exception
    {
        public InvalidQuantityException(string message) : base(message) { }
    }

    // f) Warehouse manager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Apple", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Headphones", 40, "JBL", 6));

            _groceries.AddItem(new GroceryItem(1, "Rice 5kg", 50, DateTime.Today.AddMonths(12)));
            _groceries.AddItem(new GroceryItem(2, "Milk", 80, DateTime.Today.AddDays(14)));
            _groceries.AddItem(new GroceryItem(3, "Eggs (Dozen)", 30, DateTime.Today.AddDays(10)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems())
                Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased for Id={id}. New Qty={item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IncreaseStock] Error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item with Id={id}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RemoveItem] Error: {ex.Message}");
            }
        }

        public static void Main()
        {
            var mgr = new WareHouseManager();
            mgr.SeedData();

            Console.WriteLine("=== Grocery Items ===");
            mgr.PrintAllItems(mgr._groceries);

            Console.WriteLine("\n=== Electronic Items ===");
            mgr.PrintAllItems(mgr._electronics);

            Console.WriteLine("\n=== Exception Scenarios ===");

            // Add a duplicate item
            try
            {
                mgr._electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine($"[Duplicate] {ex.Message}");
            }

            // Remove non-existent item
            mgr.RemoveItemById(mgr._groceries, 999);

            // Update with invalid quantity
            try
            {
                mgr._groceries.UpdateQuantity(1, -5);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine($"[InvalidQuantity] {ex.Message}");
            }
        }
    }
}