using Faker.Generators;
using System.Reflection;

namespace Faker
{
    public interface IFaker
    {
        public T Create<T>();
        public object Create(Type t);
    }
    public class Faker : IFaker
    {
        private Random _random;
        private GeneratorContext _context;
        private List<IValueGenerator> _generators;
        private Dictionary<Type, int> _classesInRecursion;
        private FakerConfig _config; 
        public int RecursionDepthLevel = 2;
        public Faker()
        {
            _random = new Random(); 
            _context = new GeneratorContext(_random, this);
            _generators = new List<IValueGenerator>();
            _classesInRecursion = new Dictionary<Type, int>();
            _config = new FakerConfig();
        }
        public Faker(FakerConfig config)
        {
            _random = new Random();
            _context = new GeneratorContext(_random, this);
            _generators = new List<IValueGenerator>();
            _classesInRecursion = new Dictionary<Type, int>();
            _config = config;
        }
        private void SetProperties(object obj, Type type, Dictionary<Type, int> classesInRecursion)
        {
            var properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                property.SetValue(obj, Create(property.PropertyType));
            }
        }
        private bool CanGenerate(Type type, out object obj)
        {
            bool isCanGenerate = false;
            obj = null;
            foreach (IValueGenerator generator in _generators)
            {
                if (generator.CanGenerate(type))
                {
                    isCanGenerate = true;
                    obj = generator.Generate(type, _context);
                }
            }
            return isCanGenerate;
        }
        private void SetFields(object obj, Type type, Dictionary<Type, int> classesInRecursion)
        {
            var fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(obj, Create(field.FieldType));
            }
        }
        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            else
                return null;
        }
        public void AddGenerator(IValueGenerator generator)
        {
            _generators.Add(generator);
        }
        public void RegisterBaseTypeGenerators()
        {
            AddGenerator(new RandomInt());
            AddGenerator(new RandomLong());
            AddGenerator(new RandomShort());
            AddGenerator(new RandomByte());

            AddGenerator(new RandomDouble());
            AddGenerator(new RandomFloat());

            AddGenerator(new RandomChar());
            AddGenerator(new RandomString());

            AddGenerator(new RandomBool());

            AddGenerator(new RandomDateTime());
            AddGenerator(new RandomList());
        }
        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }
        public object Create(Type type)
        {
            int recursionDepthLevel;
            object obj;
            if (!_classesInRecursion.TryGetValue(type, out recursionDepthLevel))
            {
                recursionDepthLevel = 0;
                _classesInRecursion.Add(type, recursionDepthLevel);
            }
            if (recursionDepthLevel < RecursionDepthLevel)
            {
                recursionDepthLevel++;
                _classesInRecursion[type] = recursionDepthLevel;
                IValueGenerator generator;
                if (!CanGenerate(type, out obj))
                {
                    if (type.IsValueType)
                    {
                        obj = Activator.CreateInstance(type);
                    }
                    // reference type
                    else
                    {
                        var constructors = type.GetConstructors();
                        int maxCount = -1;
                        int index = -1;
                        for (int i = 0; i < constructors.Length; i++)
                        {
                            if (maxCount < constructors[i].GetParameters().Length)
                            {
                                maxCount = constructors[i].GetParameters().Length;
                                index = i;
                            }
                        }
                        if (index >= 0)
                        {
                            if (maxCount > 0)
                            {
                                var parametersInfo = constructors[index].GetParameters();
                                object[] parameters = new object[parametersInfo.Length];
                                for (int i = 0; i < parametersInfo.Length; i++)
                                {
                                    parameters[i] = Create(parametersInfo[i].ParameterType);
                                }
                                obj = constructors[index].Invoke(parameters);
                            }
                            // parameterless constructor
                            else if (maxCount == 0)
                            {
                                obj = Activator.CreateInstance(type);
                            }
                            SetProperties(obj, type, _classesInRecursion);
                            SetFields(obj, type, _classesInRecursion);
                        }
                        // constructors not found
                        else
                        {
                            obj = null;
                        }
                    }
                }
                if (_classesInRecursion.TryGetValue(type, out recursionDepthLevel))
                {
                    if (recursionDepthLevel > 0)
                    {
                        _classesInRecursion[type]--;
                    }
                    if (_classesInRecursion[type] == 0)
                    {
                        _classesInRecursion.Remove(type);
                    }
                }
            }
            else
            {
                obj = GetDefaultValue(type);
            }
            return obj;
        }
    }
}
