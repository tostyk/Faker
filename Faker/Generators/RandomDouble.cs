namespace Faker.Generators
{
    public class RandomDouble : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(double);
        }
        public object Generate(Type typeToGenerate, GeneratorContext context)
        {
            // Generate sign of double number
            bool positiveNumber = context.Random.Next(0, 2) == 1;
            double result;
            if (positiveNumber)
            {
                result = context.Random.NextDouble();
            }
            else
            {
                result = context.Random.NextDouble() *  -1;
            }
            return result;
        }
    }
}
