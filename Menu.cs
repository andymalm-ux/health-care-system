namespace App;

enum Menu
{
    None,
    Login,

    RegisterPatient,

    Main,

    RegisterLocation, // kräver Permission.AddLocations
    ReviewRegistrations, // kräver Permission.HandleRegistrations

    Personnel,

    HandlePermissions,
    Logout,
    Quit,
}
