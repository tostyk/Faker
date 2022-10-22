using System.Collections;

namespace Faker.Generators
{
    public class RandomDictionary : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            return type.GetInterfaces().Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }
        public object? Generate(Type typeToGenerate, GeneratorContext context)
        {
            int size = context.Random.Next(2, 5);
            IDictionary? dictionary = (IDictionary?)Activator.CreateInstance(typeToGenerate);
            Type keyType = typeToGenerate.GenericTypeArguments[0];
            Type valueType = typeToGenerate.GenericTypeArguments[1];
            for (int i = 0; i < size; i++)
            {
                var key = context.Faker.Create(keyType);
                var value = context.Faker.Create(valueType);
                if (key != null && value != null)
                {
                    dictionary?.Add(key, value);
                }
            }
            return dictionary;
        }
    }
}
