using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<DepartmentManager> DepartmentManagers { get; set; } = new List<DepartmentManager>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
