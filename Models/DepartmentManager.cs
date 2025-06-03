using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class DepartmentManager
{
    public int DepartmentId { get; set; }

    public int ManagerId { get; set; }

    public DateTime AssignedAt { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual User Manager { get; set; } = null!;
}
