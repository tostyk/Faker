namespace Faker.Generators
{
    public class RandomBool : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(bool);
        }
        public object Generate(Type typeToGenerate, GeneratorContext context)
        {
            return (context.Random.Next(0, 2) == 1);
        }
    }
}
