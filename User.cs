namespace App;

class User : IUser
{
    public string Email;
    string _password;

    public User(string email, string password)
    {
        Email = email;
        _password = password;
    }

    public bool TryLogin(string email, string password) => Email == email && _password == password;
}
