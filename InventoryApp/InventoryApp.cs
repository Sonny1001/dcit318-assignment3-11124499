using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryApp
{
    // a) Immutable inventory record
    public interface IInventoryEntity { int Id { get; } }

    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // c) Generic inventory logger
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item) => _log.Add(item);
        public List<T> GetAll() => new List<T>(_log);

        public void SaveToFile()
        {
            try
            {
                var json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
                using var writer = new StreamWriter(_filePath);
                writer.Write(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SaveToFile] {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                using var reader = new StreamReader(_filePath);
                var json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                _log.Clear();
                if (items != null) _log.AddRange(items);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"[LoadFromFile] File not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadFromFile] {ex.Message}");
            }
        }
    }

    // f) Integration layer
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Monitor", 50, DateTime.Today));
            _logger.Add(new InventoryItem(2, "Notebook", 150, DateTime.Today.AddDays(-3)));
            _logger.Add(new InventoryItem(3, "Office Chair", 25, DateTime.Today.AddDays(-10)));
            _logger.Add(new InventoryItem(4, "Table", 30, DateTime.Today));
            _logger.Add(new InventoryItem(5, "Printer", 5, DateTime.Today.AddDays(-1)));
        }

        public void SaveData() => _logger.SaveToFile();
        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            Console.WriteLine("=== Inventory Items ===");
            foreach (var item in _logger.GetAll())
            {
                Console.WriteLine(item);
            }
        }

        public static void Main()
        {
            string path = "inventory_log.json";

            // Create app instance and seed + save
            var app = new InventoryApp(path);
            app.SeedSampleData();
            app.SaveData();
            Console.WriteLine("Data saved.");

            // Simulate new session: create a new instance and load
            var newApp = new InventoryApp(path);
            newApp.LoadData();
            newApp.PrintAllItems();
        }
    }
}