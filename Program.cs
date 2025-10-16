using System.ComponentModel;
using System.Diagnostics;
using App;

List<User> users = new List<User>();
if (File.Exists("Users.txt"))
{
    string[] lines = File.ReadAllLines("Users.txt");
    foreach (string line in lines)
    {
        string[] userData = line.Split(',');
        if (userData.Length == 3)
        {
            string email = userData[0];
            string password = userData[1];
            string region = userData[2];
            users.Add(new User(email, password, region));
        }
    }
}
List<User> pendings = new List<User>();

if (File.Exists("Pending.Save"))
{
    string[] lines = File.ReadAllLines("Pending.Save");
    foreach (string line in lines)
    {
        string[] pendingData = line.Split(',');
        if (pendingData.Length == 3)
        {
            string email = pendingData[0];
            string password = pendingData[1];
            string region = pendingData[2];
            pendings.Add(new User(email, password, region));
        }
    }
}
User? activeUser = null; //startar programmet utan ett inloggat konto
Menu menu = Menu.None;

bool running = true;

users.Add(new User("e", "a", "Halland"));

while (running)
{
    try
    {
        Console.Clear();
    }
    catch { }
    switch (menu)
    {
        case Menu.None:
            {
                if (activeUser == null)
                {
                    Console.WriteLine("---Welcome to health care system---");
                    Console.WriteLine("1] Login");
                    Console.WriteLine("2] Register");
                    Console.WriteLine("Q] Quit");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            menu = Menu.Login;
                            break;
                        case "2":
                            menu = Menu.RegisterPatient;
                            break;
                        case "Q":
                            running = false;
                            break;
                    }
                }
                else
                {
                    menu = Menu.Main;
                }
            }
            break;

        case Menu.Login:
            try
            {
                Console.Clear();
            }
            catch { }
            Console.WriteLine("Enter Email: ");
            string? email = Console.ReadLine();
            try
            {
                Console.Clear();
            }
            catch { }

            Console.WriteLine("Enter password: ");
            string? password = Console.ReadLine();
            try
            {
                Console.Clear();
            }
            catch { }

            Debug.Assert(email != null);
            Debug.Assert(password != null);

            if (email == "" && password == "")
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
            }
            menu = Menu.None;
            break;

        case Menu.RegisterPatient:
            try
            {
                Console.Clear();
            }
            catch { }
            Console.Write("Enter your email: ");
            string? regEmail = Console.ReadLine();
            Console.Write("Enter password: ");
            string? pwd = Console.ReadLine();
            Console.Write("Enter your region: ");
            string? region = Console.ReadLine();
            string[] new_patient = { $"{regEmail},{pwd},{region}" };
            File.WriteAllLines("Pending.Save", new_patient);
            Console.WriteLine("Request sent. press ENTER to continue");
            Console.ReadLine();
            menu = Menu.None;
            break;

        case Menu.Main:

            try
            {
                Console.Clear();
            }
            catch { }
            Debug.Assert(activeUser != null);
            Console.WriteLine($"Welcome {activeUser.Email}");
            Console.WriteLine("'Q' for quit and 'L' for log out");

            switch (Console.ReadLine().ToLower())
            {
                case "q":
                    running = false;
                    break;
                case "l":
                    activeUser = null;
                    menu = Menu.None;
                    break;
            }
            break;
    }
}
