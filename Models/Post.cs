    namespace api_bui_xuan_thang.Models
    {
        public class Post
        {
            public int Id { get; set; }
            public string UserId { get; set; }
            public string? Title { get; set; }
            public DateTime DateCreate { get; set; }
            public string? Image { get; set; }
            public string? Description { get; set; }
            public User? User { get; set; }
        }
    }
