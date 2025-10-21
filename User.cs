namespace App;

public class User : IUser
{
    public string Email;
    string _password;
    public Regions region;
    List<Permission> _permissions = new();
    public Role UserRole;

    public User(string email, string password, Regions userReg, Role userRole)
    {
        Email = email;
        _password = password;
        region = userReg;
        UserRole = userRole;
    }

    public string ToSaveString()
    {
        string result = $"{Email},{_password},{region},{UserRole}";
        foreach (Permission permissions in _permissions)
        {
            result += $",{permissions}";
        }
        return result;
    }

    // Kontrollerar om e-post och lösenord matchar och returnerar true om det gör det.
    public bool TryLogin(string email, string password) => Email == email && _password == password;

    public void Accept(List<User> users, User pendingUser)
    {
        users.Add(
            new User(pendingUser.Email, pendingUser._password, pendingUser.region, Role.Patient)
        );
        Console.WriteLine($"Accepted: {pendingUser.Email}");
    }

    public void Deny(string email)
    {
        Console.WriteLine($"Denied: {email}");
    }

    // Kollar om en användare har en viss behörighet, returnerar true om den har den, annars false
    public bool Has(Permission permission) => _permissions.Contains(permission);

    // Tilldelar en användare en ny behörighet, om den inte redan har den för då returnerar den false annars true
    public bool AddPermission(Permission permission)
    {
        if (_permissions.Contains(permission))
        {
            return false;
        }
        _permissions.Add(permission);
        return true;
    }

    // Kollar om användaren har rätt roll och behörighet
    public static bool CheckAuth(User user, Role role, Permission permission) =>
        user.UserRole == role && user._permissions.Contains(permission);

    //Kollar om en användare har en viss roll och returnerar true om den har det, annars false
    public bool CheckRole(Role requireRole) => UserRole == requireRole;

    // Visar en lista med alla användare som har en viss roll och sorterar bort den aktiva användaren
    public static List<User> GetUsersWithRole(List<User> users, Role role, User activeUser) =>
        users.Where(user => user.UserRole == role && user != activeUser).ToList();

    // En lista med användare. Varje användare får ett "fake index" som börjar på 1
    public static void ShowUsers(List<User> users)
    {
        for (int i = 0; i < users.Count; i++)
        {
            Console.WriteLine($"{i + 1}] {users[i].Email}");
        }
    }

    // Visar alla användare som har en viss roll, men hoppar över den som är inloggad just nu.
    public static List<User> ShowUsersWithRole(
        List<User> users,
        Role role,
        User activeUser,
        Permission permission
    )
    {
        List<User> filteredUsers = users
            .Where(user => user.UserRole == role && user != activeUser && !user.Has(permission))
            .ToList();

        Console.WriteLine("{0,-10}{1, -10}{2, -10}", "Index", "Email", "Role");
        Console.WriteLine("----------------------------");
        for (int i = 0; i < filteredUsers.Count; i++)
        {
            Console.WriteLine(
                "{0,-10}{1, -10}{2, -10}",
                $"{i + 1}",
                $"{filteredUsers[i].Email}",
                $"{filteredUsers[i].UserRole}"
            );
        }

        return filteredUsers;
    }
}
