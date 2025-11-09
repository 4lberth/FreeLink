using System;

namespace FreeLink.Application.UseCase.Admin.Queries.GetAllUsers;

public class AdminUserDto
{
    public int UserId { get; set; } // Tu User.cs usa 'int'
    public string Email { get; set; }
    public string Name { get; set; } // AutoMapper tendrá que sacar esto de UserProfile
    public bool? IsActive { get; set; }
    public string UserType { get; set; } // Usamos el 'string' que ya tenías
}