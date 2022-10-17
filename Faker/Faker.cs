using Faker.Generators;
using System.Reflection;

namespace Faker
{
    class SizeComparer : IComparer<ConstructorInfo>
    {
        public int Compare(ConstructorInfo? x, ConstructorInfo? y)
        {
            if (x != null && y != null)
                return y.GetParameters().Length - x.GetParameters().Length;
            else 
                return 0;
        }
    }
    class PrivateMembersComparer : IComparer<ConstructorInfo>
    {
        List<PropertyInfo> _privateProperties;
        List<FieldInfo> _privateFields;
        public PrivateMembersComparer()
        {
            _privateProperties = new();
            _privateFields = new();
        }
        public PrivateMembersComparer(List<PropertyInfo> privateProperties, List<FieldInfo> privateFields)
        {
            _privateProperties = privateProperties;
            _privateFields = privateFields;
        }
        public int Compare(ConstructorInfo? x, ConstructorInfo? y)
        {
            int xCount = 0;
            int yCount = 0;
            if (x != null && y != null)
            {
                foreach (ParameterInfo parameter in x.GetParameters())
                {
                    foreach (PropertyInfo property in _privateProperties)
                    {
                        if (parameter?.Name?.ToLower() == property.Name.ToLower())
                        {
                            xCount++;
                        }
                    }
                    foreach (FieldInfo field in _privateFields)
                    {
                        if (parameter?.Name?.ToLower() == field.Name.ToLower())
                        {
                            xCount++;
                        }
                    }

                }
                foreach (ParameterInfo parameter in y.GetParameters())
                {
                    foreach (PropertyInfo property in _privateProperties)
                    {
                        if (parameter?.Name?.ToLower() == property.Name.ToLower())
                        {
                            yCount++;
                        }
                    }
                    foreach (FieldInfo field in _privateFields)
                    {
                        if (parameter?.Name?.ToLower() == field.Name.ToLower())
                        {
                            yCount++;
                        }
                    }
                }
                return yCount - xCount;
            }
            else
                return 0;
        }
    }
    public class Faker : IFaker
    {
        private Random _random;
        private GeneratorContext _context;
        private List<IValueGenerator> _generators;
        private Dictionary<Type, int> _classesInRecursion;
        private FakerConfig _config; 
        private int _recursionDepthLevel = 2;
        public Faker()
        {
            _random = new Random(); 
            _context = new GeneratorContext(_random, this);
            _generators = new List<IValueGenerator>();
            _classesInRecursion = new Dictionary<Type, int>();
            _config = new FakerConfig();
            RegisterBaseTypeGenerators();
        }
        public Faker(FakerConfig config)
        {
            _random = new Random();
            _context = new GeneratorContext(_random, this);
            _generators = new List<IValueGenerator>();
            _classesInRecursion = new Dictionary<Type, int>();
            _config = config;
            RegisterBaseTypeGenerators();
        }
        private bool CanGenerate(Type type, out object? obj)
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
        private void SetProperties(object obj, Type type, Dictionary<MemberInfo, IValueGenerator> generators)
        {
            var properties = type.GetProperties();
            bool customGeneration = false;
            foreach (PropertyInfo property in properties)
            {
                foreach (MemberInfo memberInfo in generators.Keys)
                {
                    if (memberInfo == property && property.SetMethod != null && property.SetMethod.IsPublic && memberInfo.DeclaringType != null)
                    {
                        property.SetValue(obj, generators[memberInfo].Generate(memberInfo.DeclaringType, _context));
                        customGeneration = true;
                    }
                }
                if (!customGeneration)
                    property.SetValue(obj, Create(property.PropertyType));
            }
        }
        private void SetFields(object obj, Type type, Dictionary<MemberInfo, IValueGenerator> generators)
        {
            var fields = type.GetFields();
            bool customGeneration = false;
            foreach (FieldInfo field in fields)
            {
                foreach (MemberInfo memberInfo in generators.Keys)
                {
                    if (memberInfo == field && memberInfo.DeclaringType != null)
                    {
                        field.SetValue(obj, generators[memberInfo].Generate(memberInfo.DeclaringType, _context));
                        customGeneration = true;
                    }
                }
                if (!customGeneration)
                    field.SetValue(obj, Create(field.FieldType));
            }
        }
        private static object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            else
                return null;
        }
        private object? CreateByConstructor(Type type)
        {
            object? obj = null;
            List<PropertyInfo> privateProperties = new();
            List<FieldInfo> privateFields = new();
            Dictionary<MemberInfo, IValueGenerator>? generators = new();

            var publicFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            var publicProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (_config.CustomGenerators.TryGetValue(type, out generators))
            {
                foreach (MemberInfo memberInfo in generators.Keys)
                {
                    PropertyInfo? propertyInfo = memberInfo as PropertyInfo;
                    if (propertyInfo != null)
                        if (!publicProperties.Contains(propertyInfo))
                        {
                            privateProperties.Add(propertyInfo);
                        }
                    FieldInfo? fieldInfo = memberInfo as FieldInfo;
                    if (fieldInfo != null)
                        if (!publicFields.Contains(fieldInfo))
                        {
                            privateFields.Add(fieldInfo);
                        }
                }
            }
            if (generators == null) generators = new();

            List<ConstructorInfo> constructors = type.GetConstructors().ToList();
            constructors.Sort(new SizeComparer());
            constructors.Sort(new PrivateMembersComparer(privateProperties, privateFields));
            foreach (ConstructorInfo constructor in constructors)
            {
                try
                {
                    var parametersInfo = constructor.GetParameters();
                    object[] parameters = new object[parametersInfo.Length];
                    for (int i = 0; i < parametersInfo.Length; i++)
                    {
                        IValueGenerator? generator;
                        if (TryFindParameterGenerator(generators, parametersInfo[i], out generator))
                        {
                            parameters[i] = generator.Generate(parametersInfo[i].ParameterType, _context);
                        }
                        else
                        {
                            parameters[i] = Create(parametersInfo[i].ParameterType);
                        }
                    }
                    obj = Activator.CreateInstance(type, parameters);
                    break;
                }
                catch (Exception)
                {
                    obj = null;
                }
            }
            if (obj == null)
            {
                obj = GetDefaultValue(type);
            }
            if (obj != null)
            {
                SetProperties(obj, type, generators);
                SetFields(obj, type, generators);
            }
            return obj;
        }
        private bool TryFindParameterGenerator(Dictionary<MemberInfo, IValueGenerator> generators, ParameterInfo parameterInfo, out IValueGenerator? generator)
        {
            foreach (MemberInfo memberInfo in generators.Keys)
            {
                if (memberInfo.Name.ToLower() == parameterInfo?.Name?.ToLower() &&
                    generators[memberInfo].CanGenerate(memberInfo.DeclaringType))
                {
                    generator = generators[memberInfo];
                    return true;
                }
            }
            generator = null;
            return false;
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
        public T? Create<T>()
        {
            return (T?)Create(typeof(T?));
        }
        public object? Create(Type type)
        {
            int recursionDepthLevel;
            object? obj;
            if (!_classesInRecursion.TryGetValue(type, out recursionDepthLevel))
            {
                recursionDepthLevel = 0;
                _classesInRecursion.Add(type, recursionDepthLevel);
            }
            if (recursionDepthLevel < _recursionDepthLevel)
            {
                recursionDepthLevel++;
                _classesInRecursion[type] = recursionDepthLevel;
                if (!CanGenerate(type, out obj))
                {
                    if (type.IsPrimitive)
                    {
                        obj = Activator.CreateInstance(type);
                    }
                    else
                    {
                        obj = CreateByConstructor(type);
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
