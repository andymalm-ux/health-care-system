namespace App;

public enum Permission
{
    HandlePermissionSystem, // för att kunna hantera permissionssystemet (ge andra admins behörigheter)

    HandleRegistrations, // för att godkänna/neka patient-registreringar
    RegisterLocation, // för att kunna lägga till locations
    CreatePersonnelAccount, // för att skapa personal konto

    // --- Personal behörigheter ---
    ViewJournal,
    RegisterAppointment,
    ModifyAppointment,
    ApproveAppointment,
}
