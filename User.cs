namespace App;

class User : IUser
{
    public string Email;
    string _password;
    string Region;
    List<Permission> PermissionList = new();
    Role UserRole;

    public User(string email, string password, string region, Role userRole)
    {
        Email = email;
        _password = password;
        Region = region;
        UserRole = userRole;
    }

    public string ToSaveString()
    {
        return $"{Email},{_password},{Region}, {UserRole}";
    }

    public bool TryLogin(string email, string password) => Email == email && _password == password;

    public bool Has(Permission permission) => PermissionList.Contains(permission);
}
