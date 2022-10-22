namespace Faker
{
    public class FakerException : Exception
    {
        static string? nullObjectMessage = "Created object or object member is null";
        public FakerException () : base(nullObjectMessage) { }
    }
}
