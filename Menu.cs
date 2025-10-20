namespace App;

enum Menu
{
    None,
    Login,
    Main,
    RegisterPatient,
    ReviewRegistration, //För att godkänna eller neka registreringar
    HandlePermissions,
    HandleRegistrations, //För att ge admin behörighet att godkänna eller neka registreringar
}
