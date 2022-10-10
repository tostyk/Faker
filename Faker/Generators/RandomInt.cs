namespace Faker.Generators
{
    public class RandomInt : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(int);
        }
        public object Generate(Type typeToGenerate, GeneratorContext context)
        {
            return context.Random.Next(int.MinValue, int.MaxValue);
        }
    }
}
