using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Models;

namespace Utilities;

public class FileStorageHelper
{
    private readonly string _dataDirectory;
    private const string JsonFileName = "users.json";

    public FileStorageHelper()
    {
        var projectRoot = AppDomain.CurrentDomain.BaseDirectory;
        _dataDirectory = Path.Combine(projectRoot, "Data");
        if (!Directory.Exists(_dataDirectory))
        {
            _dataDirectory = Path.Combine(projectRoot, "..", "..", "..", "Data");
            _dataDirectory = Path.GetFullPath(_dataDirectory);
        }

        if (!Directory.Exists(_dataDirectory))
        {
            _dataDirectory = Path.Combine(projectRoot, "Data");
        }

        EnsureDataDirectory();
        Console.WriteLine($"Data directory: {_dataDirectory}");
    }

    private void EnsureDataDirectory()
    {
        if (!Directory.Exists(_dataDirectory))
        {
            Directory.CreateDirectory(_dataDirectory);
        }
    }

    public void SaveToJson(List<User> users, int nextId)
    {
        try
        {
            var data = new { Users = users, NextId = nextId };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            var filePath = Path.Combine(_dataDirectory, JsonFileName);
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Data saved to JSON: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to JSON: {ex.Message}");
        }
    }
    public (List<User> Users, int NextId) LoadFromJson()
    {
        try
        {
            var filePath = Path.Combine(_dataDirectory, JsonFileName);
            if (!File.Exists(filePath))
                return (new List<User>(), 1);

            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            var users = JsonSerializer.Deserialize<List<User>>(jsonElement.GetProperty("Users").GetRawText(), options) ?? new List<User>();
            var nextId = jsonElement.GetProperty("NextId").GetInt32();

            Console.WriteLine($"Data loaded from JSON: {filePath}");
            return (users, nextId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from JSON: {ex.Message}");
            return (new List<User>(), 1);
        }
    }

    public string GetDataDirectory()
    {
        return _dataDirectory;
    }
}
