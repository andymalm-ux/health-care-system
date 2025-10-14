using App;
using System.ComponentModel;
using System.Diagnostics;

List<User> users = new List<User>();
User? activeUser = null;//startar programmet utan ett inloggat konto

bool running = true;

users.Add(new User("e", "a"));

while (running)
{
  if (activeUser == null)//ingen användare inloggad, inlogg meny
  {
    Console.WriteLine("---Welcome to health care system---\n1. Log in");
    string? input = Console.ReadLine();
    
    switch (input)
    {
      case "1":
        try { Console.Clear(); } catch { }
        Console.WriteLine("Enter Email: ");
        string? email = Console.ReadLine();
        try { Console.Clear(); } catch { }

        Console.WriteLine("Enter password: ");
        string? password = Console.ReadLine();
        try { Console.Clear(); } catch { }

        Debug.Assert(email != null);
        Debug.Assert(password != null);

        if(email == "" && password == "")
                {
          Console.WriteLine("Enter valid input");
                }

        foreach (User user in users)
        {
          if (user.TryLogin(email, password))
          {
            activeUser = user;
            break;
          }
          else
            {
            Console.WriteLine("Wrong email or password\nPress ENTER to return...");
            Console.ReadLine();
            Console.Clear();
            }
        }
        while (activeUser != null)
        {
          Console.WriteLine("---Welcome---");
          try { Console.ReadLine();} catch{}
          break;
        }
        break;
    }
  }
}
