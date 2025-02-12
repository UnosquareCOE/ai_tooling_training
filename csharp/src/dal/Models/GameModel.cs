using Microsoft.EntityFrameworkCore;

namespace dal
{
    public class Game
    {
        public Guid Id { get; set; }
        public string Word { get; set; }
        public List<char> Guesses { get; set; } = [];
        public string Status { get; set; }
    }
}