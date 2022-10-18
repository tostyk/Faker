using System.Collections;

namespace Faker.Generators
{
    public class RandomList : IValueGenerator
    {
        public bool CanGenerate(Type type)
        {
            if (type.IsGenericType)
            {
                Type E = type.GetGenericTypeDefinition();
                return E == typeof(List<>);
            }
            return false;
        }
        public object? Generate(Type typeToGenerate, GeneratorContext context)
        {
            int size = context.Random.Next(0, 20);
            IList? list = (IList?)Activator.CreateInstance(typeToGenerate);
            Type genericType = typeToGenerate.GenericTypeArguments[0];
            for (int i = 0; i < size; i++)
            {
                var el = context.Faker.Create(genericType);
                if (el != null)
                {
                    list?.Add(context.Faker.Create(genericType));
                }
            }
            return list;
        }
    }
}
