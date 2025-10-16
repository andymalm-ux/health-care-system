using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
List<Location> locations = new List<Location>();

List<Regions> regions = new List<Regions>((Regions[])Enum.GetValues(typeof(Regions)));

locations.Add(new Location("Hej", "Finvägen1337", "Bästa i stan", Regions.Halland));

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
                    Console.WriteLine("3] Register Location");
                    Console.WriteLine("Q] Quit");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            menu = Menu.Login;
                            break;
                        case "2":
                            menu = Menu.RegisterPatient;
                            break;
                        case "3":
                            menu = Menu.RegisterLocation;
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

        case Menu.RegisterLocation:
            try
            {
                Console.Clear();
            }
            catch { }
            Console.WriteLine("Add a new location");
            string newLocation = "";
            bool locationExists = false;
            while (true)
            {
                Console.WriteLine("Enter name of the new location: ");
                newLocation = Console.ReadLine();

                locationExists = false;

                foreach (Location location in locations)
                {
                    if (location is Location l && l.LocationName.ToLower() == newLocation.ToLower())
                    {
                        locationExists = true;
                        break;
                    }
                }
                if (locationExists)
                {
                    try
                    {
                        Console.Clear();
                    }
                    catch { }
                    Console.WriteLine(
                        "Location with that name already exists\nPress ENTER to return"
                    );
                    Console.ReadLine();
                    try
                    {
                        Console.Clear();
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        Console.Clear();
                    }
                    catch { }
                    break;
                }
            }
            Console.WriteLine("Enter adress of the new location: ");
            string newAdress = Console.ReadLine();
            try
            {
                Console.Clear();
            }
            catch { }
            Console.WriteLine("Enter description for the new location: ");
            string newDesc = Console.ReadLine();
            try
            {
                Console.Clear();
            }
            catch { }

            Console.WriteLine("Chose which region the location exists whitin: ");
            int index = 1;
            foreach (Regions reg in regions)
            {
                Console.WriteLine($"{index}. {reg}");
                index++;
            }
            Console.ReadLine();
            Console.WriteLine("Enter index of the region you want to choose: ");
            string input = Console.ReadLine();
            if (
                int.TryParse(input, out int enteredIndex)
                && enteredIndex >= 0
                && enteredIndex <= regions.Count
            )
            {
                // Regions enteredRegion = (Regions)regions.GetValue(enteredIndex);
                Regions enteredRegion = (Regions)enteredIndex;
                Console.WriteLine($"{enteredRegion} chosen");

                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
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
