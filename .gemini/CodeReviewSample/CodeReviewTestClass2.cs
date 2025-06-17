namespace gemini.CodeReviewSample
{
    public class CodeReviewTestClass2
    {
        public string Username { get; }
        public int Age { get; }

        public CodeReviewTestClass2(string username, int age)
        {
            Username = username;
            // Age = age; // age未定義を指摘できるか
        }

        public bool IsAdult()
        {
            return Age >= 18;
        }
        
        public int GetAge()
        {
            return Age; // Ageが未定義のため、コンパイルエラーになる
        }

        public string Greet()
        {
            return $"Hello, Username}!"; // 構文の誤りを指摘できるか
        }
    }
}