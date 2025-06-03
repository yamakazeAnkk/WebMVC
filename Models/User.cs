using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? DepartmentId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<DepartmentManager> DepartmentManagers { get; set; } = new List<DepartmentManager>();

    public virtual ICollection<EmployeeCard> EmployeeCards { get; set; } = new List<EmployeeCard>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
