using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class EmployeeCard
{
    public int Id { get; set; }

    public string CardNumber { get; set; } = null!;

    public int UserId { get; set; }

    public DateTime IssuedAt { get; set; }

    public DateTime? ExpiredAt { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual User User { get; set; } = null!;
}
