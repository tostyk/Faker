namespace Faker.Generators
{
    public class RandomDateTime : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(DateTime);
        }
        public object Generate(Type typeToGenerate, GeneratorContext context)
        {
            return new DateTime(context.Random.NextInt64(DateTime.MinValue.Ticks, DateTime.MaxValue.Ticks));
        }
    }
}
