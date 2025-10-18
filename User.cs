namespace App;

public class User : IUser
{
    public string Email;
    string _password;
    public string Region;
    List<Permission> _permissions = new();
    public Role UserRole;

    public User(string email, string password, string region, Role userRole)
    {
        Email = email;
        _password = password;
        Region = region;
        UserRole = userRole;
    }

    public string ToSaveString()
    {
        string result = $"{Email},{_password},{Region},{UserRole}";
        foreach (Permission permissions in _permissions)
        {
            result += $"{permissions}";
        }
        return result;
    }

    // Kontrollerar om e-post och lösenord matchar och returnerar true om det gör det.
    public bool TryLogin(string email, string password) => Email == email && _password == password;

    public void Accept(List<User> users, User pendingUser)
    {
        users.Add(
            new User(pendingUser.Email, pendingUser._password, pendingUser.Region, Role.Patient)
        );
        Console.WriteLine($"Accepted: {pendingUser.Email}");
    }

    public void Deny(string email)
    {
        Console.WriteLine($"Denied: {email}");
    }

    // Tilldelar en användare en ny behörighet, om den inte redan har den för då returnerar den false annars true
    public bool GivePermission(Permission permission)
    {
        if (_permissions.Contains(permission))
        {
            return false;
        }
        _permissions.Add(permission);
        return true;
    }

    /******  Hjälpmetoder ******/
    // Visar alla användare som har en viss roll, men hoppar över den som är inloggad just nu.
    public static List<User> ShowUsersWithRole(List<User> users, Role role, User activeUser)
    {
        List<User> filteredUsers = users
            .Where(user => user.UserRole == role && user != activeUser)
            .ToList();

        if (filteredUsers.Count == 0)
        {
            Console.WriteLine("No users found with that role.");
        }
        else
        {
            for (int i = 0; i < filteredUsers.Count; i++)
            {
                Console.WriteLine($"{i + 1}] {filteredUsers[i].Email}");
            }
        }
        return filteredUsers;
    }
}
