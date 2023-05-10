
using SMS.Data.Entities;
	
namespace SMS.Data.Services;

// This interface describes the operations that a StudentService class should implement
public interface IStudentService
{
    // Initialise the repository (database)  
    void Initialise();
    
    // ---------------- Student Management --------------
    List<Student> GetStudents(string order="id");
    Student GetStudent(int id);
    Student GetStudentByEmail(string email);
    Student AddStudent(Student s);
    Student UpdateStudent(Student updated);  
    bool DeleteStudent(int id);
  

    // ---------------- Ticket Management ---------------
    Ticket CreateTicket(int studentId, string issue);
    Ticket GetTicket(int id);
    Ticket UpdateTicket(int id, string issue);
    Ticket CloseTicket(int id, string resolution);
    Ticket OpenTicket(int id);
    bool   DeleteTicket(int id);
    IList<Ticket> GetAllTickets();
    IList<Ticket> GetOpenTickets();        
    IList<Ticket> SearchTickets(TicketRange range, string query, string orderBy="id", string direction="asc");
    
    // -------------- Module Management -----------------
    Module AddModule(string name);
    StudentModule GetStudentModule(int id);
    StudentModule AddStudentToModule(int studentId, int moduleId, double mark=0);
    bool RemoveStudentFromModule(int studentId, int moduleId);
    IList<Module> GetAvailableModulesForStudent(int id);
    StudentModule UpdateStudentModuleMark(int studentId, int moduleId, double mark);
}
    
