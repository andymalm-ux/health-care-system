using System.ComponentModel;
using System.Diagnostics;
using App;

/*string eventStart = DateTime.Now.ToString();
Console.Write("Title: ");
string? title = Console.ReadLine();
Console.Write("Descrip: ");
string? descrip = Console.ReadLine();

Events event1 = Events.NewEntry(title, descrip, eventStart);
Console.WriteLine(event1.EventStart);
Console.ReadLine();*/

List<User> users = new List<User>();
if (File.Exists("Users.txt"))
{
    string[] lines = File.ReadAllLines("Users.txt");
    foreach (string line in lines)
    {
        string[] userData = line.Split(',');
        if (userData.Length == 4)
        {
            string email = userData[0];
            string password = userData[1];
            string region = userData[2];
            string role = userData[3];
            if (Enum.TryParse<Role>(role, true, out Role userRole))
            {
                users.Add(new User(email, password, region, userRole));
            }
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
            pendings.Add(new User(email, password, region, Role.Patient));
        }
    }
}
User? activeUser = null; //startar programmet utan ett inloggat konto
Menu menu = Menu.None;

bool running = true;

users.Add(new User("e", "a", "Halland", Role.Admin));

// users.Add(new User("r", "a", "Halland", Role.Personnel));
// users.Add(new User("t", "a", "Halland", Role.Patient));

User admin = new User("admin", "admin", "Halland", Role.Admin);
admin.GivePermission(Permission.HandlePermissionSystem);
users.Add(admin);

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
            ClearConsole();
            Console.Write("Enter your email:");
            string? regEmail = Console.ReadLine();
            Console.Write("Enter password:");
            string? pwd = Console.ReadLine();
            Console.Write("Enter your region: ");
            string? region = Console.ReadLine();
            string[] new_patient = { $"{regEmail},{pwd},{region}" };
            File.AppendAllLines("Pending.Save", new_patient);
            pendings.Add(new User(regEmail, pwd, region, Role.Patient));
            Console.WriteLine("Request sent. press ENTER to continue");
            Console.ReadLine();
            menu = Menu.None;
            break;

        case Menu.Main:

            ClearConsole();
            Debug.Assert(activeUser != null);
            Console.WriteLine($"Welcome {activeUser.Email}");
            Console.WriteLine("1] Give access to permission system");
            if (activeUser.UserRole == Role.Admin)
            {
                Console.WriteLine("2] Handle registrations");
            }
            Console.WriteLine("'Q' for quit and 'L' for log out");

            switch (Console.ReadLine()?.ToLower())
            {
                case "1":
                    menu = Menu.HandlePermissions;
                    break;
                case "2":
                    Debug.Assert(activeUser != null);
                    if (activeUser != null && activeUser.UserRole == Role.Admin)
                        menu = Menu.ReviewRegistration;
                    else
                    {
                        Console.WriteLine("Not authoritized");
                        Console.ReadLine();
                    }
                    break;
                case "q":
                    running = false;
                    break;
                case "l":
                    activeUser = null;
                    menu = Menu.None;
                    break;
            }
            break;

        case Menu.ReviewRegistration:
            ClearConsole();

            Debug.Assert(activeUser != null);
            Debug.Assert(activeUser.UserRole == Role.Admin);

            if (pendings.Count == 0)
            {
                Console.WriteLine("No pending requests found.");
                Console.ReadLine();
            }
            bool reviewing = true;
            while (reviewing)
            {
                Console.Clear();
                Console.WriteLine("---Pending Registrations---");

                for (int i = 0; i < pendings.Count; i++)
                {
                    Console.WriteLine(
                        $"[{i + 1}] {pendings[i].Email}, selected region: {pendings[i].Region}"
                    );
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
                            $"\nSelected: {pendingUser.Email}, selected region: {pendingUser.Region}"
                        );
                        Console.Write("[A]ccept or [D]eny: ");
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

                SaveUsers(users);
                SavePendings(pendings);

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

        case Menu.HandlePermissions:
        {
            Debug.Assert(activeUser != null);

            // Kallar på en metod från User-klassen, kollar om användaren har rätt autentisering (roll & behörighet).
            if (!User.CheckAuth(activeUser, Role.Admin, Permission.HandlePermissionSystem))
            {
                Console.WriteLine("You don't have permissions to do this.");
                Console.ReadLine();

                menu = Menu.Main;
                break;
            }
            ClearConsole();

            Console.WriteLine("----- Give admin access to permission system -----\n");

            // Kallar på en metod från User-klassen som skriver ut alla användare med en specifik roll (förutom den inloggade användaren)
            List<User> adminUsers = User.ShowUsersWithRole(users, Role.Admin, activeUser);

            Console.WriteLine("Enter user index or press ENTER to go back: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                menu = Menu.Main;
                break;
            }
            if (
                !int.TryParse(input, out int selectedIndex)
                || selectedIndex < 1
                || selectedIndex > adminUsers.Count
            )
            {
                Console.WriteLine("Invalid input");
                Console.ReadLine();
                break;
            }

            // Hämtar den valda användaren från listan (minus 1 eftersom listan egentligen börjar på 0)
            User selectedUser = adminUsers[selectedIndex - 1];
            Permission permission = Permission.HandlePermissionSystem;

            ClearConsole();

            // Kallar på metod från User-klassen som försöker lägga till en ny behörighet.
            if (selectedUser.GivePermission(permission))
            {
                Console.WriteLine($"Permission added to {selectedUser.Email}");
            }
            else
            {
                Console.WriteLine($"{selectedUser.Email} already has this permission.");
            }

            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
            menu = Menu.Main;
            break;
        }
    }
}

static void SaveUsers(List<User> users)
{
    List<string> lines = new List<string>();
    int i = 0;
    while (i < users.Count)
    {
        lines.Add(users[i].ToSaveString());
        i++;
    }
    File.WriteAllLines("Users.txt", lines);
}

static void SavePendings(List<User> pendings)
{
    List<string> lines = new List<string>();
    int i = 0;
    while (i < pendings.Count)
    {
        lines.Add(pendings[i].ToSaveString());
        // lines.Add(pendings[i].Email + "," + pendings[i].password + ", " + pendings[i].Region);
        i++;
    }
    File.WriteAllLines("Pending.Save", lines);
}

static void ClearConsole()
{
    try
    {
        Console.Clear();
    }
    catch { }
}
