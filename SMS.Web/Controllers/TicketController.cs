using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

using SMS.Data.Entities;
using SMS.Data.Services;

using SMS.Web.Models;

namespace SMS.Web.Controllers;

[Authorize]
public class TicketController : BaseController
{
    private readonly IStudentService svc;

    public TicketController()
    {
        svc = new StudentServiceDb();
    } 
     
    // GET/POST /ticket/index
    public IActionResult Index(TicketSearchViewModel search)
    {                  
        // set the viewmodel Tickets property by calling service method 
        // using the range and query values from the viewmodel 
        search.Tickets = svc.SearchTickets(search.Range, search.Query, search.OrderBy, search.Direction);

        // build custom alert message                
        var alert = $"{search.Tickets.Count} result(s) found searching '{search.Range}' Tickets";
        if (search.Query != null )
        {
            alert += $" for '{search.Query}'"; 
        }
        // display alert
        Alert(alert, AlertType.info);         
        
        return View(search);
    } 

    // display page containg JS query 
    public IActionResult Search()
    {
        return View();
    }

    [AllowAnonymous]
    // GET /tickets/query
    [HttpGet("api/ticket/search")]
    public IActionResult Search(string query, TicketRange range=TicketRange.ALL)
    {    
        // search tickets   
        var tickets = svc.SearchTickets(range, query);
        // map tickets to custom DTO object
        var data = tickets.Select( t => new {   
            Id = t.Id,
            Issue = t.Issue, 
            CreatedOn = t.CreatedOn,
            Active = t.Active,
            Student = t.Student?.Name
        });
        // return json containing custom tickets list
        return Ok(data);
    } 
                    
    // GET/ticket/{id}
    public IActionResult Details(int id)
    {
        var ticket = svc.GetTicket(id);
        if (ticket == null)
        {
            Alert("Ticket Not Found", AlertType.warning);  
            return RedirectToAction(nameof(Index));             
        }

        return View("Details",ticket);
    }

    [HttpPost]
    [Authorize(Roles="admin,support")]
    public IActionResult Open(int id)
    {
        svc.OpenTicket(id);
        return RedirectToAction(nameof(Details), new { Id = id });
    }
    
    // POST /ticket/close/{id}
    [HttpPost]
    [Authorize(Roles="admin,support")]
    public IActionResult Close([Bind("Id, Resolution")] Ticket t)
    {
        // close ticket via service
        var ticket = svc.CloseTicket(t.Id, t.Resolution);
        if (ticket == null)
        {
            Alert("Ticket Not Found", AlertType.warning);                               
        }
        else
        {
            Alert($"Ticket {t.Id } closed", AlertType.info);  
        }

        // redirect to the index view
        return RedirectToAction(nameof(Details), new { Id = t.Id });
    }
    
    // GET /ticket/create
    [Authorize(Roles="admin,support")]
    public IActionResult Create()
    {
        var students = svc.GetStudents();
        // populate viewmodel select list property
        var tvm = new TicketCreateViewModel {
            Students = new SelectList(students,"Id","Name") 
        };
        
        // render blank form
        return View( tvm );
    }
    
    // POST /ticket/create
    [HttpPost]
    [Authorize(Roles="admin,support")]
    public IActionResult Create(TicketCreateViewModel tvm)
    {
        if (ModelState.IsValid)
        {
            svc.CreateTicket(tvm.StudentId, tvm.Issue);
    
            Alert($"Ticket Created", AlertType.info);  
            return RedirectToAction(nameof(Index));
        }
        
        // redisplay the form for editing
        return View(tvm);
    }

}

