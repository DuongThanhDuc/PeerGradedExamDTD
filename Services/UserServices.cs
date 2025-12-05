using Models;


namespace Services;
public class UserServices
{
    private readonly List<User> _users = new List<User>();
    private int _nextId = 1;

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
        return true;
    }
}