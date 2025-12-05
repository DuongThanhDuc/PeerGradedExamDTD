using Models;
using Utilities;

namespace Services;

public class UserServices
{
    private readonly List<User> _users = new List<User>();
    private int _nextId = 1;
    private readonly FileStorageHelper _fileStorage;

    public UserServices()
    {
        _fileStorage = new FileStorageHelper();
        LoadDataFromFiles();
    }

    private void LoadDataFromFiles()
    {
        var (usersJson, nextIdJson) = _fileStorage.LoadFromJson();
        
        if (usersJson.Count > 0)
        {
            _users.AddRange(usersJson);
            _nextId = nextIdJson;
            Console.WriteLine("Loaded data from JSON");
        }
        else
        {
            Console.WriteLine("No existing data found. Starting with empty database.");
        }
    }

    private void SaveDataToAllFormats()
    {
        _fileStorage.SaveToJson(_users, _nextId);
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _users;
    }

    public User? GetUserById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public User CreateUser(string name, int age)
    {
        var user = new User
        {
            Id = _nextId++,
            Name = name,
            Age = age
        };
        _users.Add(user);
        SaveDataToAllFormats();
        return user;
    }

    public bool UpdateUser(int id, string name, int age)
    {
        var user = GetUserById(id);
        if (user == null)
        {
            return false;
        }
        user.Name = name;
        user.Age = age;
        SaveDataToAllFormats();
        return true;
    }

    public bool DeleteUser(int id)
    {
        var user = GetUserById(id);
        if (user == null)
        {
            return false;
        }
        _users.Remove(user);
        SaveDataToAllFormats();
        return true;
    }
}
