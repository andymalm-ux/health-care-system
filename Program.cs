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
User? activeUser = null; //startar programmet utan ett inloggat konto
Menu menu = Menu.None;

bool running = true;

users.Add(new User("e", "a", "Halland", Role.Admin));
users.Add(new User("r", "a", "Halland", Role.Personnel));
users.Add(new User("t", "a", "Halland", Role.Patient));

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
            File.WriteAllLines("Pending.Save", new_patient);
            Console.WriteLine("Request sent. press ENTER to continue");
            Console.ReadLine();
            menu = Menu.None;
            break;

        case Menu.Main:

            ClearConsole();
            Debug.Assert(activeUser != null);
            Console.WriteLine($"Welcome {activeUser.Email}");
            Console.WriteLine("1] Give access to permission system");
            Console.WriteLine("'Q' for quit and 'L' for log out");

            switch (Console.ReadLine()?.ToLower())
            {
                case "1":
                    menu = Menu.HandlePermissions;
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

        case Menu.HandlePermissions:
            Debug.Assert(activeUser != null);

            // Kallar på två metoder från User-klassen, den första kollar om den aktiva användaren är admin
            // och den andra om användaren har behörighet att hantera permissionsystemet.
            if (
                !User.CheckRole(activeUser, Role.Admin)
                || !activeUser.Has(Permission.HandlePermissionSystem)
            )
            {
                Console.WriteLine("You don't have permissions to do this.");
                Console.ReadLine();

                menu = Menu.Main;
                break;
            }

            ClearConsole();

            Console.WriteLine("----- Give admin access to permission system -----\n");

            // Kallar på metod från User-klassen som hämtar alla användare med rollen Admin
            // förutom den inloggade användaren.
            List<User> adminUsers = User.GetUsersWithRole(users, Role.Admin, activeUser);

            if (adminUsers.Count == 0)
            {
                Console.WriteLine("No admins found.");
                Console.ReadLine();

                menu = Menu.Main;
                break;
            }

            // Kallar på metod från User-klassen som skriver ut alla användare i listan adminUsers
            User.ShowUsers(adminUsers);

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

static void ClearConsole()
{
    try
    {
        Console.Clear();
    }
    catch { }
}
