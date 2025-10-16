namespace App;

interface IUser
{
    public bool TryLogin(string email, string password);
}
