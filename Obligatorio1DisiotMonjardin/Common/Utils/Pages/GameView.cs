namespace Common.Domain
{
    public struct GameView
    {
        public Game Game { get; set; }
        public bool IsOwned { get; set; }
        public bool IsPublisher { get; set; }
    }
}