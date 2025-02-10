namespace YourNamespace.ViewModels
{
    public class ProcessGameViewModel
    {
        public string MaskedWord { get; set; }
        public int AttemptsRemaining { get; set; }
        public List<string> Guesses { get; set; }
        public string Status { get; set; }
    }
}