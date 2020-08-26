namespace QwiqCache.Models
{
    public class AddStructBody
    {
        public string StructCode { get; set; }
    }

    public class AllocateMemoryBody
    {
        public int Length { get; set; }
    }

    public class BindItemBody
    {
        public int Address { get; set; }
        public string Key { get; set; }
        public string StructName { get; set; }
    }
}
