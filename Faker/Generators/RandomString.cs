namespace Faker.Generators
{
    public class RandomString : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type == typeof(string);
        }
        public object Generate(Type typeToGenerate, GeneratorContext context)
        {
            string chars = "0123456789ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            int length = context.Random.Next(5, 30);
            char[] c = new char[length];
            for (int i = 0; i < length; i++)
            {
                c[i] = chars[context.Random.Next(0, chars.Length)];
            }
            return new string(c);
        }
    }
}
