using Microsoft.EntityFrameworkCore;
using SMS.Data.Entities;
using SMS.Data.Repository;

namespace SMS.Data.Services;

public class StudentServiceDb : IStudentService
{
    private readonly DataContext db;

    public StudentServiceDb()
    {
        db = new DataContext();
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // ==================== Student Management =======================

    // retrieve list of Students
    public List<Student> GetStudents(string order="id")
    {
        var query = db.Students;
        var results = (order.ToLower()) switch
        {  
            "id"     => query.OrderBy(s => s.Id),
            "name"   => query.OrderBy(s => s.Name),
            "email"  => query.OrderBy(s => s.Email),
            "age"    => query.OrderByDescending(s => s.Dob),
            "course" => query.OrderBy(s => s.Course),
            _        => query.OrderBy(r => r.Id)
        };
        return results.ToList();
    }
    
    // Retrieve student by Id 
    public Student GetStudent(int id)
    {
        return db.Students
                 .Include(s => s.Tickets)   
                 .Include(s => s.StudentModules)
                 .ThenInclude(sm => sm.Module)                             
                 .FirstOrDefault(s => s.Id == id);
    }

    // Add a new student
    public Student AddStudent(Student s)
    {
        // check if student with email exists            
        var exists = GetStudentByEmail(s.Email);
        if (exists != null)
        {
            return null;
        } 
        // check grade is valid
        if (s.Grade < 0 || s.Grade > 100)
        {
            return null;
        }

        // create new student
        var student = new Student
        {
            Name = s.Name,
            Course = s.Course,
            Email = s.Email,
            Dob = s.Dob,
            Grade = s.Grade,
            PhotoUrl = s.PhotoUrl
        };
        db.Students.Add(student); // add student to the list
        db.SaveChanges();
        return student; // return newly added student
    }

    // Delete the student identified by Id returning true if 
    // deleted and false if not found
    public bool DeleteStudent(int id)
    {
        var s = GetStudent(id);
        if (s == null)
        {
            return false;
        }
        db.Students.Remove(s);
        db.SaveChanges();
        return true;
    }

    // Update the student with the details in updated 
    public Student UpdateStudent(Student updated)
    {
        // verify the student exists 
        var student = GetStudent(updated.Id);
        if (student == null)
        {
            return null;
        }

        // verify email is still unique
        var exists = GetStudentByEmail(updated.Email);
        if (exists != null && exists.Id != updated.Id)
        {
            return null;
        }

        // verify grade is valid
        if (updated.Grade < 0 || updated.Grade > 100)
        {
            return null;
        }

        // update the details of the student retrieved and save
        student.Name = updated.Name;
        student.Email = updated.Email;
        student.Course = updated.Course;
        student.Dob = updated.Dob;
        student.Grade = updated.Grade;
        student.PhotoUrl = updated.PhotoUrl;

        db.SaveChanges();
        return student;
    }

    public Student GetStudentByEmail(string email)
    {
        return db.Students.FirstOrDefault(s => s.Email == email);
    }

    // ===================== Ticket Management ==========================

     public Ticket CreateTicket(int studentId, string issue)
        {
            var student = GetStudent(studentId);
            if (student == null) return null;

            var ticket = new Ticket
            {
                // Id created by Database
                Issue = issue,        
                StudentId = studentId,
                // set by default in model but we can override here if required
                CreatedOn = DateTime.Now,
                Active = true,
            };
            db.Tickets.Add(ticket);
            db.SaveChanges(); // write to database
            return ticket;
        }

        public Ticket GetTicket(int id)
        {
            // return ticket and related student or null if not found
            return db.Tickets
                     .Include(t => t.Student)
                     .FirstOrDefault(t => t.Id == id);
        }

        public Ticket UpdateTicket(int id, string issue)
        {
            var ticket = GetTicket(id);
            // cannot update a non-existent or inactive ticket
            if (ticket == null || !ticket.Active) return null;

            ticket.Issue = issue;              
                        
            db.Tickets.Update(ticket);
            db.SaveChanges(); // write to database
            return ticket;
        }


        public Ticket OpenTicket(int id)
        {
            // return ticket or null if not found
            var ticket = GetTicket(id);

            // if ticket does not exist or is already active return null
            if (ticket == null || ticket.Active) return null;
            
            // ticket exists and is inactive so reopen
            ticket.Active = true;
            ticket.ResolvedOn = DateTime.Now;
           
            db.SaveChanges(); // write to database
            return ticket;
        }
        
        public Ticket CloseTicket(int id, string resolution)
        {
            var ticket = GetTicket(id);
            // if ticket does not exist or is already closed return null
            if (ticket == null || !ticket.Active) return null;
            
            // ticket exists and is active so close
            ticket.Active = false;
            ticket.Resolution = resolution;
            ticket.ResolvedOn = DateTime.Now;
           
            db.SaveChanges(); // write to database
            return ticket;
        }

        public bool DeleteTicket(int id)
        {
            // find ticket
            var ticket = GetTicket(id);
            if (ticket == null) return false;
            
            // remove ticket 
            var result = db.Tickets.Remove(ticket);
            
            db.SaveChanges();
            return true;
        }

        // Retrieve all tickets and the student associated with the ticket
        public IList<Ticket> GetAllTickets()
        {
            return db.Tickets
                     .Include(t => t.Student)
                     .ToList();
        }

        // Retrieve all open tickets (Active)
        public IList<Ticket> GetOpenTickets()
        {
            // return open tickets with associated students
            return db.Tickets
                     .Include(t => t.Student) 
                     .Where(t => t.Active)
                     .ToList();
        } 

        // perform a search of the tickets based on a query and
        // an active range 'ALL', 'OPEN', 'CLOSED'
        public IList<Ticket> SearchTickets(TicketRange range, string query, string orderBy="id", string direction="asc") 
        {
            // ensure query is not null    
            query = query == null ? "" : query.ToLower();

            // search ticket issue, active status and student name
            var search = db.Tickets
                            .Include(t => t.Student)
                            .OrderBy(t => t.Student.Name)
                            .Where(t => (t.Issue.ToLower().Contains(query) || 
                                         t.Student.Name.ToLower().Contains(query)
                                        ) &&
                                        (range == TicketRange.OPEN && t.Active ||
                                         range == TicketRange.CLOSED && !t.Active ||
                                         range == TicketRange.ALL
                                        ) 
                            
                            );
            return Ordered(search, orderBy, direction).ToList();           
        }

        private IQueryable<Ticket> Ordered(IQueryable<Ticket> query, string orderby, string direction)
        {
            query = (orderby,direction) switch {
                ("id", "asc") => query.OrderBy(t => t.Id),
                ("id", "desc") => query.OrderByDescending(t => t.Id),
                ("name", "asc") => query.OrderBy(t => t.Student.Name),
                ("name", "desc") => query.OrderByDescending(t => t.Student.Name),
                ("createdon", "asc") => query.OrderBy(t => t.CreatedOn),
                ("createdon", "desc") => query.OrderByDescending(t => t.CreatedOn),                
                _ => query
            };
            return query;
        }

        // ========================= Module Management ========================
     
         public Module AddModule(string title)
        {
            var m = new Module { Title = title };
            db.Modules.Add(m);
            db.SaveChanges();

            return m;
        }

        public StudentModule GetStudentModule(int id)
        {
            return db.StudentModules
                     .Include(sm => sm.Student)
                     .Include(sm => sm.Module)
                     .FirstOrDefault(sm => sm.Id == id);
        }

        public StudentModule AddStudentToModule(int studentId, int moduleId, double mark=0)
        {
            // check if this student module already exists and return null if found
            var sm = db.StudentModules
                       .FirstOrDefault(o => o.StudentId == studentId && o.ModuleId == moduleId);
            if (sm != null)
            {
                return null;
            }

            // locate the student and the module
            var student = GetStudent(studentId);
            var module = db.Modules.FirstOrDefault(m => m.Id == moduleId);
            
            // if either don't exist then return null
            if (student == null || module == null)
            {
                return null;
            }

            // create the student module and add to database (either of following are fine)
            //var nsm = new StudentModule { StudentId = s.Id, ModuleId = m.Id, Mark = mark };
            var nsm = new StudentModule { 
                Student = student, 
                Module = module,
                Mark = mark 
            };
            db.StudentModules.Add(nsm);

            // set the student Grade with average of module grades or 0 if no modules
            student.Grade = RecalculateGrade(student);
            db.SaveChanges();

            return nsm;
        }        

        public bool RemoveStudentFromModule(int studentId, int moduleId)
        {
            var student = GetStudent(studentId);
            var sm = student.StudentModules.FirstOrDefault(sm => sm.ModuleId == moduleId);
            if (sm == null)
            {
                return false;
            }
            student.StudentModules.Remove(sm);

            // set the student Grade with average of module grades or 0 if no modules
            student.Grade = RecalculateGrade(student);
            db.SaveChanges();

            return true;
        }
       
        public StudentModule UpdateStudentModuleMark(int studentId, int moduleId, double mark)
        {
            var student = GetStudent(studentId);

            var sm = student.StudentModules.FirstOrDefault(sm => sm.ModuleId == moduleId); 
            if (sm == null)
            {
                return null; // no such student module
            }
            sm.Mark = mark;
            
            // set the student Grade with average of module grades or 0 if no modules
            student.Grade = RecalculateGrade(student); 

            db.SaveChanges();
           
            return sm;
        }

        public IList<Module> GetAvailableModulesForStudent(int id)
        {
            var student = GetStudent(id);           
            var modules = db.Modules.ToList();
            // return modules not already taken by student
            return modules.Where(m => student.StudentModules.All(sm => sm.ModuleId != m.Id)).ToList();            
        } 

        // ======================== Private Utility Methods ================
        // pre-condition: StudentModules have been loaded
        private double RecalculateGrade(Student student)
        {
            var sum = student.StudentModules.Sum(sm => sm.Mark);
            var count = student.StudentModules.Count();
            return count == 0 ? 0 : sum / count;
        }
}
