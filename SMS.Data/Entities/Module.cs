using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Data.Entities;

public class Module
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }
        
    // Navigation property
    public IList<StudentModule> StudentModules { get; set; } = new List<StudentModule>();
}

