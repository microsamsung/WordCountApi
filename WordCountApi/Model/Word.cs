namespace WordCountApi.Model
{
    public class Word
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
