using Service.Constants;

namespace service.utils
{
    public static class LetterCalculationHelper
    {
        public static int CalculateRemainingAtempts(string word, List<char> guesses)
        {
            var remainingAtempts = Rules.MaxAttempts;
            var uniqueWordLetters = word.Distinct().ToList();
            foreach (var guess in guesses)
            {
                if (!uniqueWordLetters.Contains(guess))
                {
                    remainingAtempts--;
                }
            }
            return remainingAtempts;
        }
    }
}
