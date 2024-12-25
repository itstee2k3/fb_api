namespace api.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Data { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public User? User { get; set; }
    }
}
