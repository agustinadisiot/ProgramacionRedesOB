using Common.Domain;

namespace Common.Domain
{
    public class GameView
    {
        public Game Game { get; set; }
        public bool CanBuy { get; set; } 
        public bool IsPublisher { get; set; }
    }
}