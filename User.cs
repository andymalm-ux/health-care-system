namespace App;

class User : IUser
{
    public string Email;
    string _password;
    string Region;
    List<Permission> PermissionList = new();

    public User(string email, string password, string region)
    {
        Email = email;
        _password = password;
        Region = region;
    }

    public string ToSaveString()
    {
        return $"{Email},{_password},{Region}";
    }

    public bool TryLogin(string email, string password) => Email == email && _password == password;

    public bool Has(Permission permission) => PermissionList.Contains(permission);
}
