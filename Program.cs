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

if (File.Exists("locations.txt"))
{
    string[] lines = File.ReadAllLines("locations.txt");
    foreach (string line in lines)
    {
        string[] locationData = line.Split(',');
        if (locationData.Length == 4)
        {
            string locationname = locationData[0];
            string locationadress = locationData[1];
            string locationdesc = locationData[2];
            Regions region = Enum.Parse<Regions>(locationData[3]);

            locations.Add(new Location(locationname, locationadress, locationdesc, region));
        }
    }
}

List<Regions> regions = new List<Regions>((Regions[])Enum.GetValues(typeof(Regions)));

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
            Regions[] regionContent = (Regions[])Enum.GetValues(typeof(Regions));
            for (int i = 0; i < regionContent.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {regionContent[i]}");
            }
            Console.WriteLine("Enter index of the region you want to choose: ");
            string inputLocation = Console.ReadLine();

            Regions chosenRegion;

            if (
                int.TryParse(inputLocation, out int enteredIndex)
                && enteredIndex >= 1
                && enteredIndex <= regionContent.Length
            )
            {
                chosenRegion = regionContent[enteredIndex - 1];
                Console.WriteLine($"{chosenRegion} chosen");
            }
            else
            {
                Console.WriteLine("Invalid input\nPress ENTER to return");
                Console.ReadLine();
                break;
            }

            Location addLocation = new Location(newLocation, newAdress, newDesc, chosenRegion);
            locations.Add(addLocation);

            List<string> newLine = new List<string>
            {
                $"{addLocation.LocationName},{addLocation.LocationAdress},{addLocation.LocationDesc},{addLocation.region}",
            };
            File.AppendAllLines("Locations.txt", newLine);

            Console.WriteLine("New location added whitin region\nPress ENTER to continue");
            Console.ReadLine();
            Console.Clear();
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
