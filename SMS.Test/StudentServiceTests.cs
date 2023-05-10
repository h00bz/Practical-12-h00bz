
using Xunit;
using SMS.Data.Entities;
using SMS.Data.Services;

namespace SMS.Test;

[Collection("Sequential")]
public class StudentServiceTests
{
    private readonly IStudentService svc;

    public StudentServiceTests()
    {
        // general arrangement
        svc = new StudentServiceDb();

        // ensure data source is empty before each test
        svc.Initialise();
    }
    
    // =========================== GET ALL STUDENT TESTS =================================

    [Fact] 
    public void Student_GetAllStudents_WhenNoneExist_ShouldReturn0()
    {
        // act 
        var students = svc.GetStudents();
        var count = students.Count;

        // assert
        Assert.Equal(0, count);
    }

    [Fact]
    public void Student_GetStudents_With2Added_ShouldReturnCount2()
    {
        // arrange       
        var s1 = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var s2 = svc.AddStudent(
            new Student { Name="YYY", Course="Engineering", Email="yyy@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );       

        // act
        var students = svc.GetStudents();
        var count = students.Count;

        // assert
        Assert.Equal(2, count);
    }

    // =========================== GET SINGLE STUDENT TESTS =================================

    [Fact] 
    public void Student_GetStudent_WhenNoneExist_ShouldReturnNull()
    {
        // act 
        var student = svc.GetStudent(1); // non existent student

        // assert
        Assert.Null(student);
    }

    [Fact] 
    public void Student_GetStudent_WhenAdded_ShouldReturnStudent()
    {
        // arrange 
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );

        // act
        var ns = svc.GetStudent(s.Id);

        // assert
        Assert.NotNull(ns);
        Assert.Equal(s.Id, ns.Id);
    }

    [Fact] 
    public void Student_GetStudent_WithTickets_RetrievesStudentAndTickets()
    {
        // arrange
        var s = svc.AddStudent( 
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        svc.CreateTicket(s.Id, "Issue 1");  

        // act      
        var ns = svc.GetStudent(s.Id);
        
        // assert
        Assert.NotNull(ns);
        Assert.Equal(1, ns.Tickets.Count);
  } 


    [Fact] 
    public void Student_GetStudentByEmail_WhenAdded_ShouldReturnStudent()
    {
        // arrange 
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );

        // act
        var ns = svc.GetStudentByEmail("xxx@email.com");

        // assert
        Assert.NotNull(ns);
        Assert.Equal(s.Email, ns.Email);
    }

    // =========================== ADD STUDENT TESTS =================================

    [Fact]
    public void Student_AddStudent_WhenValid_ShouldAddStudent()
    {
        // arrange - add new student
        var added = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        
        // act - try to retrieve the newly added student
        var s = svc.GetStudent(added.Id);

        // assert - that student is not null
        Assert.NotNull(s);
        
        // now assert that the properties were set properly
        Assert.Equal(s.Id, s.Id);
        Assert.Equal("XXX", s.Name);
        Assert.Equal("xxx@email.com", s.Email);
        Assert.Equal("Computing", s.Course);
        Assert.Equal(new DateTime(2000,1,1), s.Dob);
        Assert.Equal(0, s.Grade);
    }

    [Fact] // --- AddStudent Duplicate Test
    public void Student_AddStudent_WhenDuplicateEmail_ShouldReturnNull()
    {
        // arrange
        var s1 = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=50 }
        );

        // act 
        var s2 = svc.AddStudent(
            new Student { Name="YYY", Course="Maths", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=40 }
        );
        
        // assert
        Assert.NotNull(s1);
        Assert.Null(s2);       
    }

    [Fact] // --- AddStudent Invalid Grade Test
    public void Student_AddStudent_WhenInvalidGrade_ShouldReturnNull()
    {
        // arrange

        // act 
        var added = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=120 }
        );
        
        // assert
        Assert.Null(added);
    }

    // =========================== UPDATE STUDENT TESTS =================================

    [Fact]
    public void Student_UpdateStudent_ThatExists_ShouldSetAllProperties()
    {
        // arrange - create test student        
        var s = svc.AddStudent(
            new Student { Name="ZZZ", Course="Computing", Email="zzz@email.com", Dob = new DateTime(2000,1,1), Grade=100 }
        );
                              
        // act - ** create a copy and update all student properties (except Id) **
        var u = svc.UpdateStudent(           
            new Student {
                Id = s.Id, // use original Id
                Name = "XXX",
                Email = "xxx@email.com",
                Course = "Engineering",
                Dob = new DateTime(1990,2,2),
                Grade = 50
            }
        ); 

        // reload updated student from database into new student object (us)
        var us = svc.GetStudent(s.Id);

        // assert
        Assert.NotNull(us);           

        // now assert that the properties were set properly           
        Assert.Equal(u.Name, us.Name);
        Assert.Equal(u.Email, us.Email);
        Assert.Equal(u.Course, us.Course);
        Assert.Equal(u.Age, us.Age);
        Assert.Equal(u.Grade, us.Grade);
    }

    [Fact] // --- UpdateStudent Invalid Grade Test
    public void Student_UpdateStudent_WhenInvalidGrade_ShouldReturnNull()
    {
        // arrange
        var added = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=70 }
        );

        // act - update Grade with invalid value
        var updated = svc.UpdateStudent(
            new Student { Id = added.Id, Grade=170, Name=added.Name, Course=added.Course, Email="xxx@email.com", Dob = new DateTime(2000,1,1) }    
        );
        
        // assert
        Assert.NotNull(added);
        Assert.Null(updated);
    }

    [Fact]
    public void Student_UpdateStudent_WhenDuplicateEmail_ShouldReturnNull()
    {
        // arrange
        var s1 = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=70 }
        );
        var s2 = svc.AddStudent(
            new Student { Name="YYY", Course="Maths", Email="yyy@email.com", Dob = new DateTime(2000,1,1), Grade=50 }
        );

        // act - update s2 Email with duplicate value from s1
        var updated = svc.UpdateStudent(
            new Student { Email = s1.Email, Id = s2.Id, Name=s2.Name, Course=s2.Course, Dob = new DateTime(2000,1,1), Grade=s2.Grade }    
        );
        
        // assert
        Assert.NotNull(s1);
        Assert.NotNull(s2);
        Assert.Null(updated);
    }

    // ===========================  DELETE STUDENT TESTS =================================
    [Fact]
    public void Student_DeleteStudent_ThatExists_ShouldReturnTrue()
    {
        // arrange 
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
       
        // act
        var deleted = svc.DeleteStudent(s.Id);

        // try to retrieve deleted student
        var s1 = svc.GetStudent(s.Id);

        // assert
        Assert.True(deleted); // delete student should return true
        Assert.Null(s1);      // s1 should be null
    }


    [Fact]
    public void Student_DeleteStudent_ThatDoesntExist_ShouldReturnFalse()
    {
        // act 	
        var deleted = svc.DeleteStudent(0);

        // assert
        Assert.False(deleted);
    }  


    // ---------------------- Ticket Tests ------------------------
        
    [Fact] 
    public void Ticket_CreateTicket_ForExistingStudent_ShouldBeCreated()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );

        // act
        var t = svc.CreateTicket(s.Id, "Dummy Ticket 1");
        
        // assert
        Assert.NotNull(t);
        Assert.Equal(s.Id, t.StudentId);
        Assert.True(t.Active); 
    }

    [Fact] 
    public void Ticket_UpdateTicket_WhenActive_ShouldUpdateIssue()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket 1");
        
        
        // act
        svc.UpdateTicket(t.Id, "DUMMY TICKET");
        var u = svc.GetTicket(t.Id);

        // assert
        Assert.NotNull(u);
        Assert.Equal(u.StudentId, s.Id);
        Assert.Equal("DUMMY TICKET", u.Issue);
        Assert.True(u.Active); 
    }

    [Fact] 
    public void Ticket_UpdateTicket_WhenInactive_ShouldNotUpdateIssue()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket 1");
        svc.CloseTicket(t.Id, "Resolved");
       
        
        // act
        var r = svc.UpdateTicket(t.Id, "DUMMY TICKET");
        t = svc.GetTicket(t.Id);

        // assert
        Assert.Null(r);                 // Update result should be null
        Assert.False(t.Active);         // ticket should be inactive
        Assert.Equal("Dummy Ticket 1", t.Issue);        
    }

    [Fact] // --- GetTicket should include Student
    public void Ticket_GetTicket_WhenExists_ShouldReturnTicketAndStudent()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket 1");

        // act
        var ticket = svc.GetTicket(t.Id);

        // assert
        Assert.NotNull(ticket);
        Assert.NotNull(ticket.Student);
        Assert.Equal(s.Name, ticket.Student.Name); 
    }

    [Fact] // --- GetOpenTickets When two added should return two 
    public void Ticket_GetOpenTickets_WhenTwoAdded_ShouldReturnTwo()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var t1 = svc.CreateTicket(s.Id, "Dummy Ticket 1");
        var t2 = svc.CreateTicket(s.Id, "Dummy Ticket 2");

        // act
        var open = svc.GetOpenTickets();

        // assert
        Assert.Equal(2,open.Count);                        
    }

    [Fact] 
    public void Ticket_CloseTicket_WhenOpen_ShouldReturnTicket()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket");

        // act
        var r = svc.CloseTicket(t.Id, "Resolved");

        // assert
        Assert.NotNull(r);              // verify closed ticket returned          
        Assert.False(r.Active);
        Assert.Equal("Resolved",r.Resolution);
    }

    [Fact] 
    public void Ticket_CloseTicket_WhenAlreadyClosed_ShouldReturnNull()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket");

        // act
        var closed = svc.CloseTicket(t.Id, "Solved");     // close active ticket    
        closed = svc.CloseTicket(t.Id,"Solved");         // close non active ticket

        // assert
        Assert.Null(closed);                    // no ticket returned as already closed
    }

    [Fact] 
    public void Ticket_DeleteTicket_WhenExists_ShouldReturnTrue()
    {
        // arrange
        var s = svc.AddStudent(
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var t = svc.CreateTicket(s.Id, "Dummy Ticket");

        // act
        var deleted = svc.DeleteTicket(t.Id);     // delete ticket    
        
        // assert
        Assert.True(deleted);                    // ticket should be deleted
    }   

    [Fact] 
    public void Ticket_DeleteTicket_WhenNonExistant_ShouldReturnFalse()
    {
        // arrange
        
        // act
        var deleted = svc.DeleteTicket(1);     // delete non-existent ticket    
        
        // assert
        Assert.False(deleted);                  // ticket should not be deleted
    }  


    // ======================== Student Module Tests ======================
    
    [Fact] 
    public void Module_AddStudentToModule_WhenNotAlreadyTakingModule_ShouldAddModule()
    {
        // arrange
        var s = svc.AddStudent(       
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var m = svc.AddModule("XXXX");

        // act
        var sm = svc.AddStudentToModule(s.Id, m.Id, 50);
        var student = svc.GetStudent(s.Id);

        Assert.Equal(1, student.StudentModules.Count);
        Assert.Equal(50, student.Grade);
    }

    [Fact] // --- AddStudentToModule duplicate test
    public void Module_AddStudentToModule_WhenAlreadyTakingModule_ShouldReturNull()
    {
        // arrange
        var s = svc.AddStudent( 
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var m = svc.AddModule("XXXX");

        // act
        var sm1 = svc.AddStudentToModule(s.Id, m.Id);
        var sm2 = svc.AddStudentToModule(s.Id, m.Id); // duplicate

        var r = svc.GetStudent(s.Id);

        // assert
        Assert.Null(sm2);
        Assert.Equal(1, r.StudentModules.Count);
    }

    [Fact] 
    public void Module_RemoveStudentFromModule_WhenTakingModule_ShouldRemoveModuleAndUpdateGrade()
    {
        // arrange
        var s = svc.AddStudent(       
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var m1 = svc.AddModule("XXXX");
        var m2 = svc.AddModule("YYYY");
        
        var sm1 = svc.AddStudentToModule(s.Id, m1.Id, 50);
        var sm2 = svc.AddStudentToModule(s.Id, m2.Id, 60);
        
        // act
        svc.RemoveStudentFromModule(s.Id, m1.Id);

        var student = svc.GetStudent(s.Id);

        Assert.Equal(1, student.StudentModules.Count);
        Assert.Equal(60, student.Grade);
    }

    [Fact]  // -- UpdateStudentgrade
    public void Module_UpdateStudentModuleMark_WhenModuleMarksChanged_ShouldRecalculateGrade()
    {
        // arrange
        var s = svc.AddStudent( 
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        var m1 = svc.AddModule("Maths");
        var m2 = svc.AddModule("Physics");
        var m3 = svc.AddModule("Computing");
        var sm1 = svc.AddStudentToModule(s.Id, m1.Id,50);
        var sm2 = svc.AddStudentToModule(s.Id, m2.Id,50);
        var sm3 = svc.AddStudentToModule(s.Id, m3.Id,50);

        // act
        sm1 = svc.UpdateStudentModuleMark(s.Id, m1.Id, 60.0);
        sm2 = svc.UpdateStudentModuleMark(s.Id, m2.Id, 60.0);  
        sm3 = svc.UpdateStudentModuleMark(s.Id, m3.Id, 60.0);         
        var student = svc.GetStudent(s.Id);

        // assert
        Assert.Equal(60, student.Grade);
    }

    [Fact] // --- AddStudentToModule duplicate test
    public void Module_GetAvailableModulesForStudent_WhenOneAvailable_ShouldReturOne()
    {
        // arrange
        var s = svc.AddStudent( 
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        
        var m1 = svc.AddModule("XXXX");
        var m2 = svc.AddModule("YYYY");
        var m3 = svc.AddModule("ZZZZ");

        var sm1 = svc.AddStudentToModule(s.Id, m1.Id);
        var sm2 = svc.AddStudentToModule(s.Id, m2.Id); 
        
        // act
        var modules = svc.GetAvailableModulesForStudent(s.Id);

        // assert      
        Assert.Equal(1, modules.Count);
    }

    [Fact] // --- AddStudentToModule duplicate test
    public void Module_GetAvailableModulesForStudent_WhenAllAvailable_ShouldReturAll()
    {
        // arrange
        // add one student
        var s = svc.AddStudent( 
            new Student { Name="XXX", Course="Computing", Email="xxx@email.com", Dob = new DateTime(2000,1,1), Grade=0 }
        );
        // add 3 modules
        var m1 = svc.AddModule("XXXX");
        var m2 = svc.AddModule("YYYY");
        var m3 = svc.AddModule("ZZZZ");
       
        // act
        // retrieve available modules for this student
        var modules = svc.GetAvailableModulesForStudent(s.Id);

        // assert      
        Assert.Equal(3, modules.Count);
    }

}
