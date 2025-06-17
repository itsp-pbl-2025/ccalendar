namespace gemini.CodeReviewSample
{
    public class CodeReviewTestClass1
    {
        public string Username { get; }
        public int Age { get; }

        public CodeReviewTestClass1(string username, int age)
        {
            Username = username;
            Age = age;
        }

        public bool IsAdult()
        {
            return Age >= 18;
        }

        public string Greet()
        {
            return $"Hello, {Username}!";
        }
    }
}