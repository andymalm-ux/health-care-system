namespace App;

class User : IUser
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
            result += $",{permissions}";
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

    // Kollar om en användare har en viss behörighet, returenerar true om den hard en, annars false
    public bool Has(Permission permission) => _permissions.Contains(permission);

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

    //Kollar om en användare har en viss roll och returnerar true om den har det, annars false
    public static bool CheckRole(User user, Role requireRole) => user.UserRole == requireRole;

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

    // En kombination av metoderna Has och CheckRole (kanske överflödig, eller så behövs inte de andra två?)
    public static bool CheckAuth(User user, Role role, Permission permission)
    {
        return CheckRole(user, role) && user.Has(permission);
    }

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
