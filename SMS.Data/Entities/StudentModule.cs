using System;
using System.ComponentModel.DataAnnotations;

namespace SMS.Data.Entities;

public class StudentModule
{
    public int Id { get; set; }

    [Range(0,100)]
    public double Mark {get; set; }

    // Foreign key for related Student model
    public int StudentId { get; set; }
    public Student Student { get; set; }

    // Foreign key for related Module model
    public int ModuleId { get; set; }
    public Module Module { get; set; }
}

