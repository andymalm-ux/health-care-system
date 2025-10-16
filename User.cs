namespace App;

class User : IUser
{
    public string Email;
    string _password;
    string Region;
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
        return $"{Email},{_password},{Region},{UserRole}";
    }

    // Kontrollerar om e-post och lösenord matchar och returnerar true om det gör det.
    public bool TryLogin(string email, string password) => Email == email && _password == password;

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
    public static void ShowRoleList(List<User> users)
    {
        for (int i = 0; i < users.Count; i++)
        {
            Console.WriteLine($"{i + 1}] {users[i].Email}");
        }
    }
}
