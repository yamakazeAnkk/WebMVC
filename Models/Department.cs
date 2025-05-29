using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ManagerId { get; set; }

    public virtual User? Manager { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
