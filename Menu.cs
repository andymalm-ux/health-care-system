namespace App;

enum Menu
{
    None,
    Login,

    RegisterPatient,

    Main,

    RegisterLocation, // kräver Permission.AddLocations
    ReviewRegistrations, // kräver Permission.HandleRegistrations

    HandlePermissions,
    Logout,
    Quit,
}
