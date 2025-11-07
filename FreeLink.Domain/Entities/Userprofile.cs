using System;
using System.Collections.Generic;

namespace FreeLink.Infrastructure;

public partial class Userprofile
{
    public int ProfileId { get; set; }

    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? ProfilePicture { get; set; }

    public string? Bio { get; set; }

    public virtual User User { get; set; } = null!;
}
