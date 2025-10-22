using System.Diagnostics;
using App;

string eventStart = DateTime.Now.ToString();

EventType type = EventType.None;
string etype = "";

switch (type)
{
    case EventType.None:
        Console.WriteLine("---Type of appointment---");
        Console.WriteLine("1] Appointment");
        Console.WriteLine("2] Journal Entry");
        Console.WriteLine("3] Meeting");

        bool isRunning = true;

        while (isRunning)
        {
            switch (Console.ReadLine())
            {
                case "1":
                    etype = nameof(EventType.Appointment);
                    isRunning = false;
                    break;

                case "2":
                    etype = nameof(EventType.JournalEntry);
                    isRunning = false;
                    break;

                case "3":
                    etype = nameof(EventType.Meeting);
                    isRunning = false;
                    break;
            }
        }

        break;
}

Console.Write("Title: ");
string? title = Console.ReadLine();
Console.Write("Descrip: ");
string? descrip = Console.ReadLine();

Events event1 = Events.NewEntry(etype, title, descrip, eventStart);

List<User> users = new List<User>();
if (File.Exists("Users.txt"))
{
    string[] lines = File.ReadAllLines("Users.txt");
    foreach (string line in lines)
    {
        string[] userData = line.Split(',');

        string email = userData[0];
        string password = userData[1];
        Regions region = Enum.Parse<Regions>(userData[2]);
        Role role = Enum.Parse<Role>(userData[3]);

        User user = new(email, password, region, role);

        if (userData.Length > 4)
        {
            foreach (string permissionString in userData)
            {
                if (Enum.TryParse(permissionString, out Permission permission))
                {
                    user.AddPermission(permission);
                }
            }
        }
        users.Add(user);
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
            Regions region = Enum.Parse<Regions>(pendingData[2]);
            pendings.Add(new User(email, password, region, Role.Patient));
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

User? activeUser = null; //startar programmet utan ett inloggat konto
Menu menu = Menu.None;

bool running = true;

AddDefaultUsers(users);

while (running)
{
    ClearConsole();

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

                    switch (Console.ReadLine()?.ToLower())
                    {
                        case "1":
                            menu = Menu.Login;
                            break;
                        case "2":
                            menu = Menu.RegisterPatient;
                            break;
                        case "q":
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
            ClearConsole();
            Console.WriteLine("Enter Email: ");
            string? email = Console.ReadLine();
            ClearConsole();

            Console.WriteLine("Enter password: ");
            string? password = Console.ReadLine();
            ClearConsole();

            Debug.Assert(email != null);
            Debug.Assert(password != null);

            if (email == "" && password == "")
            {
                Console.WriteLine("Enter valid input");
                Console.ReadLine();
                menu = Menu.None;
                break;
            }

            bool foundUser = false;
            foreach (User user in users)
            {
                if (user.TryLogin(email, password))
                {
                    activeUser = user;
                    foundUser = true;
                    break;
                }
            }
            if (!foundUser)
            {
                Console.WriteLine("Invalid input...");
                Console.WriteLine("Press 'ENTER' to try again or register new account");
                Console.ReadLine();
            }
            menu = Menu.None;
            break;

        case Menu.RegisterPatient:
            ClearConsole();
            Console.WriteLine("---Register account---");
            Console.WriteLine("Press ENTER to start registration or 'Q' to quit...");
            string? quit = Console.ReadLine();
            Debug.Assert(quit != null);
            if (quit?.ToLower() == "q")
            {
                menu = Menu.None;
                break;
            }
            ClearConsole();
            Console.Write("Enter your email: ");
            string? regEmail = Console.ReadLine();
            Console.Write("Enter password: ");
            string? pwd = Console.ReadLine();

            Debug.Assert(regEmail != null);
            Debug.Assert(pwd != null);
            if (regEmail == "" && pwd == "")
            {
                Console.WriteLine("Invalid input..");
                Console.WriteLine("Press ENTER to go back to menu and try again.");
                Console.ReadLine();
                menu = Menu.None;
                break;
            }

            Console.WriteLine("Chose which region: ");
            Regions[] regLocation = Enum.GetValues<Regions>();
            for (int i = 0; i < regLocation.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {regLocation[i]}");
            }
            Console.WriteLine("Enter index of the region you want to choose: ");
            string? userInput = Console.ReadLine();

            Regions userLocation;

            if (
                int.TryParse(userInput, out int locationIndex)
                && locationIndex >= 1
                && locationIndex <= regLocation.Length
            )
            {
                userLocation = regLocation[locationIndex - 1];
                ClearConsole();
                Console.WriteLine($"{userLocation} chosen");
            }
            else
            {
                Console.WriteLine("Invalid input\nPress ENTER to return");
                Console.ReadLine();
                break;
            }

            string[] new_patient = { $"{regEmail},{pwd},{userLocation}" };
            File.AppendAllLines("Pending.Save", new_patient);
            pendings.Add(new User(regEmail, pwd, userLocation, Role.Patient));
            Console.WriteLine("Request sent. press ENTER to continue");
            Console.ReadLine();
            menu = Menu.None;
            break;

        case Menu.Main:

            ClearConsole();
            Debug.Assert(activeUser != null);
            Console.WriteLine($"Welcome {activeUser.Email}");

            Dictionary<int, Menu> dynamicMenu = new();
            int menuIndex = 1;

            if (User.CheckAuth(activeUser, Role.Admin, Permission.HandleRegistrations))
            {
                dynamicMenu.Add(menuIndex, Menu.ReviewRegistrations);
                Console.WriteLine($"{menuIndex}] View registration request");
                menuIndex++;
            }

            if (User.CheckAuth(activeUser, Role.Admin, Permission.RegisterLocation))
            {
                dynamicMenu.Add(menuIndex, Menu.RegisterLocation);
                Console.WriteLine($"{menuIndex}] Add location");
                menuIndex++;
            }

            if (User.CheckAuth(activeUser, Role.Admin, Permission.HandlePermissionSystem))
            {
                dynamicMenu.Add(menuIndex, Menu.HandlePermissions);
                Console.WriteLine($"{menuIndex}] Give access to permission system");
                menuIndex++;
            }

            dynamicMenu.Add(menuIndex, Menu.Logout);
            Console.WriteLine($"{menuIndex}] Log out");
            menuIndex++;

            dynamicMenu.Add(menuIndex, Menu.Quit);
            Console.WriteLine($"{menuIndex}] Quit\n");

            Console.Write("Select: ");
            string? selectedInput = Console.ReadLine();

            if (
                int.TryParse(selectedInput, out int userIndex) && dynamicMenu.ContainsKey(userIndex)
            )
            {
                menu = dynamicMenu[userIndex];
            }

            break;

        case Menu.ReviewRegistrations:
            ClearConsole();
            Debug.Assert(activeUser != null);
            Debug.Assert(activeUser.UserRole == Role.Admin);
            if (!User.CheckAuth(activeUser, Role.Admin, Permission.HandleRegistrations))
            {
                Console.WriteLine("You don't have permissions to do this.");
                Console.ReadLine();
                menu = Menu.Main;
                break;
            }
            if (pendings.Count == 0)
            {
                Console.WriteLine("No pending requests found.");
                Console.ReadLine();
                menu = Menu.Main;
                break;
            }
            bool reviewing = true;
            while (reviewing)
            {
                ClearConsole();
                Console.ForegroundColor = ConsoleColor.Magenta;

                Console.WriteLine("--------------------------------------------");
                Console.WriteLine("           Pending Registrations");
                Console.WriteLine("--------------------------------------------");

                Console.WriteLine("{0,-10}{1,-15}{2,-15}", "Index", "Email", "Selected region");
                Console.WriteLine("--------------------------------------------");
                Console.ResetColor();

                for (int i = 0; i < pendings.Count; i++)
                {
                    Console.WriteLine(
                        "{0,-10}{1,-15}{2,-15}",
                        $"{i + 1}",
                        $"{pendings[i].Email}",
                        $"{pendings[i].region}"
                    );

                    Console.WriteLine("--------------------------------------------");
                }
                Console.WriteLine("\nEnter number to handle, or Q to quit:");
                string input = Console.ReadLine()?.ToLower() ?? "";
                if (input == "q")
                {
                    reviewing = false;
                    break;
                }
                if (int.TryParse(input, out int choice))
                {
                    int selectedIndex = choice - 1;
                    if (selectedIndex >= 0 && selectedIndex < pendings.Count)
                    {
                        User pendingUser = pendings[selectedIndex];
                        Console.WriteLine(
                            $"\nSelected: {pendingUser.Email}, selected region: {pendingUser.region}"
                        );
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[A]ccept");
                        Console.ResetColor();
                        Console.Write(" or ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[D]eny: ");
                        Console.ResetColor();
                        string pick = Console.ReadLine()?.ToLower() ?? "";
                        if (pick == "a")
                        {
                            activeUser.Accept(users, pendingUser);
                            pendings.Remove(pendingUser);
                        }
                        else if (pick == "d")
                        {
                            activeUser.Deny(pendingUser.Email);
                            pendings.Remove(pendingUser);
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice, pick A or D.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid index.");
                    }
                }
                else
                {
                    Console.WriteLine("Please enter a valid number or Q.");
                }
                SaveUsers(users, "Users.txt");
                SaveUsers(pendings, "Pending.Save");
                if (pendings.Count == 0)
                {
                    Console.WriteLine("\nAll requests handled!");
                    Console.ReadLine();
                    reviewing = false;
                }
                else
                {
                    Console.WriteLine("\nPress ENTER to refresh list...");
                    Console.ReadLine();
                }
            }
            menu = Menu.Main;
            break;

        case Menu.RegisterLocation:
            ClearConsole();
            Debug.Assert(activeUser != null);
            Debug.Assert(activeUser.UserRole == Role.Admin);
            if (!User.CheckAuth(activeUser, Role.Admin, Permission.RegisterLocation))
            {
                Console.WriteLine("You don't have permissions to do this.");
                Console.ReadLine();
                menu = Menu.Main;
                break;
            }
            Console.WriteLine("---Add a new location---");
            Console.WriteLine("Press 'Q' to quit and enter to continue..");
            string? locationQuit = Console.ReadLine();
            Debug.Assert(locationQuit != null);
            if (locationQuit?.ToLower() == "q")
            {
                menu = Menu.Main;
                break;
            }
            string? newLocation = "";
            bool locationExists = false;
            while (true)
            {
                ClearConsole();
                Console.WriteLine("Enter name of the new location: ");
                newLocation = Console.ReadLine();
                Debug.Assert(newLocation != null);

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
                    ClearConsole();

                    Console.WriteLine(
                        "Location with that name already exists\nPress ENTER to return"
                    );
                    Console.ReadLine();

                    ClearConsole();
                }
                else
                {
                    ClearConsole();
                    break;
                }
            }
            Console.WriteLine("Enter adress of the new location: ");
            string? newAdress = Console.ReadLine();
            Debug.Assert(newAdress != null);
            ClearConsole();

            Console.WriteLine("Enter description for the new location: ");
            string? newDesc = Console.ReadLine();
            Debug.Assert(newDesc != null);
            ClearConsole();

            Console.WriteLine("Chose which region the location exists whitin: ");
            Regions[] regionContent = Enum.GetValues<Regions>();
            for (int i = 0; i < regionContent.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {regionContent[i]}");
            }
            Console.WriteLine("Enter index of the region you want to choose: ");
            string? inputLocation = Console.ReadLine();
            Debug.Assert(newLocation != null);

            Regions chosenRegion;

            if (
                int.TryParse(inputLocation, out int enteredIndex)
                && enteredIndex >= 1
                && enteredIndex <= regionContent.Length
            )
            {
                chosenRegion = regionContent[enteredIndex - 1];
                ClearConsole();
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
            ClearConsole();
            menu = Menu.None;
            break;

        case Menu.HandlePermissions:
        {
            Debug.Assert(activeUser != null);

            ShowUsersAndGiveAccessRights(
                users,
                activeUser,
                Role.Admin,
                Permission.HandlePermissionSystem
            );
            menu = Menu.Main;
            break;
        }
        case Menu.Logout:
            activeUser = null;
            menu = Menu.None;
            break;
        case Menu.Quit:
            running = false;
            break;
    }
}
static void SaveUsers(List<User> users, string path)
{
    List<string> lines = new List<string>();
    int i = 0;
    while (i < users.Count)
    {
        lines.Add(users[i].ToSaveString());
        i++;
    }
    File.WriteAllLines(path, lines);
}

static void ClearConsole()
{
    try
    {
        Console.Clear();
    }
    catch { }
}

static void ShowUsersAndGiveAccessRights(
    List<User> users,
    User activeUser,
    Role role,
    Permission userPermission
)
{
    // Kallar på en metod från User-klassen, kollar om användaren har rätt autentisering (roll & behörighet).
    if (!User.CheckAuth(activeUser, role, userPermission))
    {
        Console.WriteLine("You don't have permissions to do this.");
        Console.ReadLine();
        return;
    }
    ClearConsole();

    // Kallar på en metod från User-klassen som skriver ut alla användare förutom den inloggade användaren
    List<User> usersList = User.ShowUsersWithRole(users, activeUser);

    if (usersList.Count == 0)
    {
        Console.WriteLine("No users found.");
        Console.ReadLine();
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Enter user index or press ENTER to go back");

    string? input = Console.ReadLine();

    if (string.IsNullOrEmpty(input))
    {
        return;
    }

    if (
        !int.TryParse(input, out int selectedIndex)
        || selectedIndex < 1
        || selectedIndex > usersList.Count
    )
    {
        Console.WriteLine("Invalid input");
        Console.ReadLine();
        return;
    }

    // Hämtar den valda användaren från listan (minus 1 eftersom listan egentligen börjar på 0)
    User selectedUser = usersList[selectedIndex - 1];

    ClearConsole();
    List<Permission> availablePermissions = new();

    switch (selectedUser.UserRole)
    {
        case Role.Admin:
            availablePermissions.Add(Permission.HandleRegistrations);
            availablePermissions.Add(Permission.RegisterLocation);
            availablePermissions.Add(Permission.CreatePersonnelAccount);
            break;

        case Role.Personnel:
            availablePermissions.Add(Permission.ViewJournal);
            availablePermissions.Add(Permission.RegisterAppointment);
            availablePermissions.Add(Permission.ModifyAppointment);
            availablePermissions.Add(Permission.ApproveAppointment);
            break;

        case Role.Patient:
            availablePermissions.Add(Permission.ViewOwnJournal);
            break;
    }

    availablePermissions = availablePermissions
        .Where(permission => !selectedUser.Has(permission))
        .ToList();

    if (availablePermissions.Count == 0)
    {
        Console.WriteLine(
            $"{selectedUser.Email} already has all permissions for user role '{selectedUser.UserRole}'."
        );
        Console.ReadLine();
        return;
    }

    ClearConsole();

    Console.WriteLine(
        $"---------- Chose an permission to add to {selectedUser.Email} ----------\n"
    );
    for (int i = 0; i < availablePermissions.Count; i++)
    {
        Console.WriteLine($"{i + 1}] {availablePermissions[i]}");
    }
    Console.WriteLine();

    Console.WriteLine("Enter index or press ENTER to go back");
    string? permissionIndex = Console.ReadLine();

    if (string.IsNullOrEmpty(permissionIndex))
    {
        return;
    }

    if (
        !int.TryParse(permissionIndex, out int selectedPermissionIndex)
        || selectedPermissionIndex < 1
        || selectedPermissionIndex > availablePermissions.Count
    )
    {
        Console.WriteLine("Invalid input");
        Console.ReadLine();
        return;
    }

    Permission selectedPermission = availablePermissions[selectedPermissionIndex - 1];
    ClearConsole();

    Console.WriteLine($"Permission {selectedPermission} added to {selectedUser.Email}");

    // Kallar på metod från User-klassen som försöker lägga till en ny behörighet.
    selectedUser.AddPermission(selectedPermission);

    SaveUsers(users, "Users.txt");

    Console.WriteLine("Press Enter to continue...");
    Console.ReadLine();
    return;
}

// Skapar några hårdkodade användare från början, om de inte redan finns
static void AddDefaultUsers(List<User> users)
{
    // en admin som inte har några behörigheter från början
    if (!users.Any(user => user.Email == "e"))
    {
        users.Add(new("e", "a", Regions.Halland, Role.Admin));
    }

    // en admin med behörigheter att ge andra admins behörigheter
    if (!users.Any(user => user.Email == "admin"))
    {
        User admin = new("admin", "admin", Regions.Halland, Role.Admin);
        admin.AddPermission(Permission.HandlePermissionSystem);
        users.Add(admin);
    }

    // en personal utan behörigheter från början
    if (!users.Any(user => user.Email == "nurse"))
    {
        users.Add(new("nurse", "nurse", Regions.Halland, Role.Personnel));
    }
}
