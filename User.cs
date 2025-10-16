namespace App;

class User : IUser
{
    public string Email;
    string _password;
    string Region;
    string Role;

    public User(string email, string password, string region, string role)
    {
        Email = email;
        _password = password;
        Region = region;
        Role = role;
    }

    public string ToSaveString()
    {
        return $"{Email},{_password},{Region}, {Role}";
    }

    public bool TryLogin(string email, string password) => Email == email && _password == password;
}
