    namespace Prod.Models.Database;

    public class Request
    {
        public Guid id { get; set; }
        public string Text { get; set; }
        public Action Action { get; set; }
        public long ActionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; }
        public Book Book { get; set; }
        public Guid BookId { get; set; }
    }
