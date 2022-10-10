namespace Faker
{
    public class GeneratorContext
    {
        public Random Random { get; }
        public Faker Faker { get; }
        public GeneratorContext(Random random, Faker faker)
        {
            Random = random;
            Faker = faker;
        }
    }
}
