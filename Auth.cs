namespace App;

public static class Auth
{
    static Dictionary<Role, List<Permission>> PermissionsByRole = new();

    static Auth()
    {
        PermissionsByRole.Add(
            Role.Admin,
            new()
            {
                Permission.HandleLocations,
                Permission.HandlePermissionSystem,
                Permission.HandleRegistrations,
            }
        );
        PermissionsByRole.Add(Role.Personnel, new() { Permission.HandleJournals });
        PermissionsByRole.Add(Role.Patient, new());
    }

    // kollar om användaren har rätt roll och rätt behörighet
    public static bool HasRoleAndPermission(User user, Role role, Permission permission)
    {
        if (user.UserRole != role)
        {
            return false;
        }
        if (!PermissionsByRole[role].Contains(permission))
        {
            return false;
        }

        return PermissionsByRole[role].Contains(permission);
    }

    // skickar tillbaka en lista med vilka behörigheter den rollen kan ha
    public static List<Permission> GetPermissionsByRole(Role role) => PermissionsByRole[role];
}
