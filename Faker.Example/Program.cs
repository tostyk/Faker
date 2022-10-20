using Faker;
static class Program
{
    public static void Main()
    {
        FakerConfig config = new FakerConfig();
        config.Add<TestClass, string>(o => o.ReadonlyString, new MorseStringGenerator());

        Faker.Faker faker = new Faker.Faker(config);

        faker.DownloadGenerators("GeneratorsDll");
        TestClass? testClass = faker.Create<TestClass>();
        Console.WriteLine(testClass);
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
        string chars = "-.";
        int length = context.Random.Next(10, 30);
        char[] c = new char[length];
        for (int i = 0; i < length; i++)
        {
            c[i] = chars[context.Random.Next(0, chars.Length)];
        }
        return new string(c);
    }
    public static bool IsMorse(string str)
    {
        for (int i = 0; i < str.Length; i++)
            if (str[i] != '-' && str[i] != '.')
                return false;
        return true;
    }
}
public class TestClass
{
    public int Int;
    public double Double;
    public string String;
    public bool Bool;
    public byte Byte;
    public char Char;
    public float Float;
    public long Long;
    public short Short;

    public string ReadonlyString { get; }
    public List<int> list;
    public TestClass testClass;
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
    public TestClass(int Int, double Double, string String, string readonlyString)
    {
        this.Int = Int;
        this.Double = Double;
        this.String = String;
        ReadonlyString = readonlyString;
    }
    private string Format(int offset)
    {
        string str = string.Empty;
        string strOffset = "\n";
        for (int i = 0; i < offset; i++) strOffset += "    ";
        str +=
            strOffset + "public int     Int     " + Int +
            strOffset + "public string  String  " + String +
            strOffset + "public bool    Bool    " + Bool +
            strOffset + "public byte    Byte    " + Byte +
            strOffset + "public char    Char    " + Char +
            strOffset + "public float   Float   " + Float +
            strOffset + "public float   Float   " + Float +
            strOffset + "public long    Long    " + Long +
            strOffset + "public short   Short   " + Short +
            strOffset + "public string  ReadonlyString { get; } " + ReadonlyString +
            strOffset + "private string privateString " + privateString +
            strOffset + "public List<int> list" + WriteList(offset + 1) +
            strOffset + "public TestClass testClass:" + testClass?.Format(offset + 1);
        return str;
    }
    private string WriteList(int offset)
    {
        string str = "";
        string strOffset = "\n";
        for (int i = 0; i < offset; i++) strOffset += "    ";
        foreach(int num in list)
        {
            str += strOffset + string.Format("{0, 15}", num);
        }
        return str;
    }
    public override string ToString()
    {
        return Format(0);
    }
}
