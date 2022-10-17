namespace Faker
{
    public interface IValueGenerator
    {
        public object? Generate(Type typeToGenerate, GeneratorContext context);
        public bool CanGenerate(Type type);
    }
}
