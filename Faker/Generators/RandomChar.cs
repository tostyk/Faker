namespace Faker.Generators
{
    public class RandomChar : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(char);
        }
        public object Generate(Type typeToGenerate, GeneratorContext context)
        {
            return (char)context.Random.Next(0, 256);
        }
    }
}
