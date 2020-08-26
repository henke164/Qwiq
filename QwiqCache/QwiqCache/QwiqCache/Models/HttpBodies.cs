namespace QwiqCache.Models
{
    public class AllocateMemoryBody
    {
        public int Length { get; set; }
    }

    public class BindItemBody
    {
        public int Address { get; set; }
        public int Length { get; set; }
        public string Key { get; set; }
    }
}
