using Microsoft.EntityFrameworkCore;
using Prod.Exceptions;
using Prod.Models.Database;
using Prod.Models.Requests;

namespace Prod.Services;

public class BookService(ProdContext context) : IBookService
{
    public async Task Book(Guid zoneId, Guid? seatId, Guid userId, BookRequest bookRequest)
    {
    }
}
