namespace gemini.CodeReviewSample
{
    public class CodeReviewTestClass3
    {
        public string USERNAME { get; } // プロパティ名の大文字小文字の誤りを指摘できるか
        public int Age { get; }

        public codeReviewTestClass3(string username, int age) // クラス名の大文字小文字の誤りを指摘できるか
        {
            USERNAME = username;
            Age = age;
        }

        public bool ISAdult() // メソッド名の大文字小文字の誤りを指摘できるか
        {
            return Age >= 18;
        }
        
        public int GetAge()
        {
            return Age; // Ageが未定義のため、コンパイルエラーになる
        }

        public string Greet()
        {
            return $"Hello, {USERNAME}!";
        }
    }
}