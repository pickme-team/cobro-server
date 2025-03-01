using Prod.Models.Database;

namespace Prod.Exceptions;

public class AlreadyBookedException(Type bookType) : Exception
{
    private string PlaceTypeString => bookType.Name switch
    {
        nameof(PlaceBook) => "Место",
        nameof(RoomBook) => "Комната",
        _ => throw new ArgumentOutOfRangeException(bookType.Name, bookType, null)
    };
    
    public override string Message => $"Тип: {PlaceTypeString}, уже зарезервирован";
}