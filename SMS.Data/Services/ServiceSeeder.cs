using SMS.Data.Entities;

namespace SMS.Data.Services;

public static class ServiceSeeder
{
    // use this class to seed the database with dummy test data using an IStudentService 

    public static void Seed(IUserService userSvc, IStudentService studentSvc)
    {  
        // only do this once
        studentSvc.Initialise();

        SeedUsers(userSvc);
        SeedStudents(studentSvc);
    }

    // use this method FIRST to seed the database with dummy test data using an IUserService
    private static void SeedUsers(IUserService svc)
    {
        // Note: do not call initialise here
        var u1 = svc.Register("admin","admin@mail.com","password",Role.admin, "data:image/jpeg;base64,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");
        var u2 = svc.Register("support","support@mail.com","password",Role.support);
        var u3 = svc.Register("guest","guest@mail.com","password",Role.guest);
      
    }

    public static void SeedStudents(IStudentService svc)
    {
        // Note: do not call initialise here
        
        var s1 = svc.AddStudent( new Student {
            Name = "Homer Simpson", Course = "Physics", Email = "homer@mail.com", Dob = new DateTime(1980,2,3), Grade = 56, PhotoUrl = "https://static.wikia.nocookie.net/simpsons/images/b/bd/Homer_Simpson.png"
        });
        var s2 = svc.AddStudent( new Student {
            Name = "Marge Simpson", Course = "English", Email = "marge@mail.com", Dob = new DateTime(1985,12,5), Grade = 69, PhotoUrl = "https://static.wikia.nocookie.net/simpsons/images/4/4d/MargeSimpson.png" 
        });
        var s3 = svc.AddStudent(
            new Student { Name = "Bart Simpson",Course = "Maths", Email = "bart@mail.com", Dob = new DateTime(2011,2,3), Grade = 61, PhotoUrl = "https://upload.wikimedia.org/wikipedia/en/a/aa/Bart_Simpson_200px.png" 
        });
        var s4 = svc.AddStudent(
            new Student { Name = "Lisa Simpson", Course = "Poetry", Email = "lisa@mail.com", Dob = new DateTime(2013,7,8), Grade = 80, PhotoUrl = "https://upload.wikimedia.org/wikipedia/en/e/ec/Lisa_Simpson.png" 
        });
        var s5 = svc.AddStudent(
            new Student { Name = "Mr Burns", Course = "Management", Email = "burns@mail.com", Dob = new DateTime(1940,5,13), Grade = 63, PhotoUrl = "https://static.wikia.nocookie.net/simpsons/images/a/a7/Montgomery_Burns.png" 
        });
        var s6 = svc.AddStudent(           
            new Student { Name = "Barney Gumble", Course = "Brewing", Email = "barney@mail.com", Dob = new DateTime(1981,7,20), Grade = 49, PhotoUrl = "https://static.wikia.nocookie.net/simpsons/images/6/68/Barney_Gumble_-_shading.png" 
        });


        // seed tickets

        // add tickets for homer
        var t1 = svc.CreateTicket(s1.Id, "Cannot login");
        var t2 = svc.CreateTicket(s1.Id, "Printing doesnt work");
        var t3 = svc.CreateTicket(s1.Id, "Forgot my password");

        // add ticket for marge
        var t4 = svc.CreateTicket(s2.Id, "Please reset password");

        // add ticket for bart
        var t5 = svc.CreateTicket(s3.Id, "No internet connection");
        
        // close homers first ticket 
        svc.CloseTicket(t1.Id,"""
        ## Markdown Text
        * Item 1
        * Item 2

        The ticket is closed: ** as of @DateTime.Now **
        """);


        // create some modules
        var m1 = svc.AddModule("Programming");
        var m2 = svc.AddModule("Maths");
        var m3 = svc.AddModule("English");
        var m4 = svc.AddModule("French");
        var m5 = svc.AddModule("Science");
        var m6 = svc.AddModule("Geograpy");
        var m7 = svc.AddModule("Physics");


        // Homer is taking programming
        svc.AddStudentToModule(s1.Id, m1.Id, 50);

        // Marge is taking maths
        svc.AddStudentToModule(s2.Id, m2.Id, 60);

        // Bart is taking English 
        svc.AddStudentToModule(s3.Id, m3.Id,55);

        // Lisa is taking Programming Maths and English
        svc.AddStudentToModule(s4.Id, m1.Id, 70);
        svc.AddStudentToModule(s4.Id, m2.Id, 80);
        svc.AddStudentToModule(s4.Id, m3.Id, 75);

        
    }
}

