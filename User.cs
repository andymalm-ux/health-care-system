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
        if (!users.Any(u => u.Email.Equals(pendingUser.Email)))
        {
            users.Add(
                new User(pendingUser.Email, pendingUser._password, pendingUser.region, Role.Patient)
            );
        }
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

    // Visar alla användare som har en viss roll, men hoppar över den som är inloggad just nu.
    public static List<User> ShowUsersWithRole(List<User> users, User activeUser)
    {
        List<User> filteredUsers = users
            .Where(user => user != activeUser)
            .OrderByDescending(user => user.UserRole)
            .ThenBy(user => user.Email)
            .ToList();

        if (filteredUsers.Count == 0)
        {
            return new();
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("----------------------------------------------------------------------");
        Console.WriteLine("                  Give users permission");
        Console.WriteLine("----------------------------------------------------------------------");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("{0,-10}{1,-20}{2,-20}{3,-20}", "Index", "Email", "Role", "Permissions");
        Console.WriteLine("----------------------------------------------------------------------");
        Console.ResetColor();

        for (int i = 0; i < filteredUsers.Count; i++)
        {
            User user = filteredUsers[i];

            if (user._permissions.Count == 0)
            {
                Console.WriteLine(
                    "{0,-10}{1,-20}{2,-20}{3,-20}",
                    $"{i + 1}",
                    $"{filteredUsers[i].Email}",
                    $"{filteredUsers[i].UserRole}",
                    "No permissions"
                );
            }
            else
            {
                Console.WriteLine(
                    "{0,-10}{1,-20}{2,-20}{3,-20}",
                    $"{i + 1}",
                    $"{filteredUsers[i].Email}",
                    $"{filteredUsers[i].UserRole}",
                    $"{user._permissions[0]}"
                );

                for (int j = 1; j < user._permissions.Count; j++)
                {
                    Console.WriteLine(
                        "{0,-10}{1,-20}{2,-20}{3,-20}",
                        "",
                        "",
                        "",
                        $"{user._permissions[j]}"
                    );
                }
            }

            Console.WriteLine(
                "----------------------------------------------------------------------"
            );
        }

        return filteredUsers;
    }
}
