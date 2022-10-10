namespace Faker.Generators
{
    public class RandomLong : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(long);
        }
        public object Generate(Type typeToGenerate, GeneratorContext context)
        {
            return (long)context.Random.NextInt64(long.MinValue, long.MaxValue);
        }
    }
}
