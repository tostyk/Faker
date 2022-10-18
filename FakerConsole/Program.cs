using Faker;
using System.Reflection;

static class Program
{
    public static void Main()
    {
        FakerConfig config = new FakerConfig();
        config.Add<TestClass, string>(o => o.ReadonlyString, new MorseStringGenerator());
        Faker.Faker faker = new Faker.Faker(config);
        TestClass? testClass = faker.Create<TestClass?>();
        if (testClass != null)
            Console.WriteLine(testClass.ToString(0));
        ClassWithPrivateConstructor? classWithPrivateConstructor = faker.Create<ClassWithPrivateConstructor>();
        if (classWithPrivateConstructor == null) Console.WriteLine("classWithPrivateConstructor is null");
        CycleClassA? cycleClassA = faker.Create<CycleClassA>();
        Console.WriteLine(cycleClassA);
    }
}
public class MorseStringGenerator : IValueGenerator
{
    public bool CanGenerate(Type type)
    {
        return type == typeof(string);
    }
    public object? Generate(Type typeToGenerate, GeneratorContext context)
    {
        string chars = ".-";
        int length = context.Random.Next(10, 30);
        char[] c = new char[length];
        for (int i = 0; i < length; i++)
        {
            c[i] = chars[context.Random.Next(0, chars.Length)];
        }
        return new string(c);
    }
}
public class TestClass
{
    public int intNumber;
    public double realNumber;
    public string publicString;
    public string ReadonlyString { get; }
    public List<TestClass> list;
    public TestClass test;
    public DateTime dateTime;
    private string privateString { get; set; }

    public TestClass() { }
    public TestClass(string readonlyString) 
    {
        ReadonlyString = readonlyString;
    }
    public TestClass(string readonlyString, string privateString)
    {
        ReadonlyString = readonlyString;
        this.privateString = privateString;
    }
    public TestClass(int intNumber, double realNumber, string publicString)
    {
        this.intNumber = intNumber;
        this.publicString = publicString;
        this.realNumber = realNumber;
    }
    public string ToString(int nestingLevel)
    {
        { 
        /*string offset = "\n";
        for (int i = 0; i < nestingLevel * 3; i++) offset += " ";
        nestingLevel++;
        string str = offset + typeof(TestClass).Name +
            offset + "public int     intNumber       " + intNumber +
            offset + "public double  realNumber      " + realNumber +
            offset + "public string  publicString    " + publicString +
            offset + "public string  ReadonlyString  " + ReadonlyString +
            offset + "public TestClass test          " + test?.ToString(nestingLevel) +
            offset + "public DateTime dateTime       " + dateTime.ToString() +
            offset + "private string privateString   " + privateString +
            offset + "public List<TestClass> list    ";
        nestingLevel++;
        foreach (TestClass test in list)
        {
            str += test?.ToString(nestingLevel);
        }
        str += "\n";*/
    }
        string str = string.Empty;
        MemberInfo[] members = typeof(TestClass).GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        foreach (MemberInfo member in members)
        {
            PropertyInfo? property = member as PropertyInfo;
            if (property != null)
            {
                object value = property?.GetValue(this);
                str += string.Format("{0, -10} {1, -15}: {2, -15} {3, -15} \n", property.MemberType, property.PropertyType, property.Name, value?.ToString());
            }
                FieldInfo? field = member as FieldInfo;
            if (field != null)
            {
                object value = field?.GetValue(this);
                str += string.Format("{0, -10} {1, -15}: {2, -15} {3, -15} \n", field.MemberType, field.FieldType, field.Name, value?.ToString());
            }
        }
        return str;
    }
}
public struct TestStruct
{
    public int intNumber;
    public double realNumber;
    string publicString;
    public TestStruct()
    {
        intNumber = 0;
        realNumber = 0;
        publicString = string.Empty;
    }
}
public class ClassWithPrivateConstructor
{
    private ClassWithPrivateConstructor() { }
}
public class CycleClassA
{
    public CycleClassB CycleClassB;
}
public class CycleClassB
{
    public CycleClassC cycleClassC;
}
public class CycleClassC
{
    public CycleClassA cycleClassA;
}
