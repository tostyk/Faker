namespace Faker.Generators
{
    public class RandomFloat : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(float);
        }
        public object Generate(Type typeToGenerate, GeneratorContext context)
        {
            // Generate sign of number
            bool positiveNumber = context.Random.Next(0, 2) == 1;
            float result;
            if (positiveNumber)
            {
                result = (float)(context.Random.NextDouble() * float.MaxValue);
            }
            else
            {
                result = (float)(context.Random.NextDouble() * float.MinValue);
            }
            return result;
        }
    }
}
