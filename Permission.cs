namespace App;

public enum Permission
{
    HandleRegistrations, // för att godkänna/neka patient-registreringar

    RegisterLocation, // för att kunna lägga till locations

    HandlePermissionSystem, // för att kunna hantera permissionssystemet (ge admins behörigheter)

    CreatePersonnelAccount,

    ViewJournal,
    RegisterAppointment,
    ModifyAppointment,
    ApproveAppointment,
}
