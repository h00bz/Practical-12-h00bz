
using SMS.Data.Entities;

namespace SMS.Web.Models;

public class TicketSearchViewModel
{
    // result set
    public IList<Ticket> Tickets { get; set;} = new List<Ticket>();

    // search options        
    public string Query { get; set; } = string.Empty;
    public TicketRange Range { get; set; } = TicketRange.ALL;

    public string OrderBy { get; set; } = "id";
    public string Direction { get; set; } = "asc";
}

