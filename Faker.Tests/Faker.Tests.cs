using System.Collections;

namespace Faker.Tests
{
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
            for(int i = 0; i <  str.Length; i++)
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
        public string GetPrivateString()
        {
            return privateString;
        }
    }
    public struct TestStruct
    {
        public int intNumber;
        public double realNumber;
        string privateString;
        public TestStruct(string privateString)
        {
            intNumber = 0;
            realNumber = 0;
            this.privateString = privateString;
        }
        public string GetPrivateString()
        {
            return privateString;
        }
    }
    public class ClassWithPrivateConstructor
    {
        private ClassWithPrivateConstructor() { }
    }
    public class CyclicalClassA
    {
        public CyclicalClassB b;
    }
    public class CyclicalClassB
    {
        public CyclicalClassC c;
    }
    public class CyclicalClassC
    {
        public CyclicalClassA a;
    }
    public class Tests
    {
        Faker _faker;
        [SetUp]
        public void Setup()
        {
            _faker = new Faker();
        }
        [Test]
        public void DownloadDll()
        {
            _faker.DownloadGenerators("GeneratorsDll");
            List<int>?list = _faker.Create<List<int>>();
            Assert.IsNotNull(list);
            Assert.That(list.Count, Is.GreaterThan(0));
        }
        [Test]
        public void SelectConstructor()
        {
            TestClass? testClass = _faker.Create<TestClass>();
            Assert.IsNotNull(testClass);
            Assert.IsNull(testClass.GetPrivateString());
            Assert.IsNotNull(testClass.ReadonlyString);
        }
        [Test]
        public void PrivateConstructor()
        {
            ClassWithPrivateConstructor? classWithPrivateConstructor = _faker.Create<ClassWithPrivateConstructor>();
            Assert.IsNull(classWithPrivateConstructor);
        }
        [Test]
        public void SelectConstructorWithConfig()
        {
            FakerConfig config = new FakerConfig();
            config.Add<TestClass, string>(t => t.ReadonlyString, new MorseStringGenerator());
            Faker fakerWithConfig = new Faker(config);
            TestClass? testClass = fakerWithConfig.Create<TestClass>();
            Assert.IsNotNull(testClass);
            Assert.IsNotNull(testClass.ReadonlyString);
            Assert.True(MorseStringGenerator.IsMorse(testClass.ReadonlyString));
        }
        [Test]
        public void SetFieldWithConfig()
        {
            FakerConfig config = new FakerConfig();
            config.Add<TestClass, string>(t => t.String, new MorseStringGenerator());
            Faker fakerWithConfig = new Faker(config);
            TestClass? testClass = fakerWithConfig.Create<TestClass>();
            Assert.IsNotNull(testClass);
            Assert.IsTrue(MorseStringGenerator.IsMorse(testClass.String));
        }
        [Test]
        public void CreateStruct()
        {
            Assert.IsNotNull(_faker.Create<TestStruct>().GetPrivateString());
        }
        [Test]
        public void CyclicalClasses()
        {
            CyclicalClassA? a = _faker.Create<CyclicalClassA>();
            Assert.IsNotNull(a);
            Assert.IsNotNull(a.b);
            Assert.IsNotNull(a.b.c);
            Assert.IsNotNull(a.b.c.a);
            Assert.IsNotNull(a.b.c.a.b);
            Assert.IsNotNull(a.b.c.a.b.c);
            Assert.IsNull(a.b.c.a.b.c.a);
        }
        [Test]
        public void ChangeCycleCounter()
        {
            FakerConfig config = new FakerConfig();
            config.SetTypeCycleCounter(3);
            Faker fakerWithConfig = new Faker(config);
            CyclicalClassA? a = fakerWithConfig.Create<CyclicalClassA>();
            Assert.IsNotNull(a);
            Assert.IsNotNull(a.b);
            Assert.IsNotNull(a.b.c);
            Assert.IsNotNull(a.b.c.a);
            Assert.IsNotNull(a.b.c.a.b);
            Assert.IsNotNull(a.b.c.a.b.c);
            Assert.IsNotNull(a.b.c.a.b.c.a);
            Assert.IsNotNull(a.b.c.a.b.c.a.b);
            Assert.IsNotNull(a.b.c.a.b.c.a.b.c);
            Assert.IsNull(a.b.c.a.b.c.a.b.c.a);
        }
        [Test]
        public void FakerExceptions()
        {
            FakerConfig config = new FakerConfig();
            config.ThrowExceptions = true; ;
            Faker fakerWithConfig = new Faker(config);
            TestClass? testClass;
            Assert.Throws<FakerException>(Method_CyclicalClass);
            Assert.Throws<FakerException>(Method_ClassWithPrivateConstructor);
        }
        public void Method_CyclicalClass()
        {
            FakerConfig config = new FakerConfig();
            config.ThrowExceptions = true; ;
            Faker fakerWithConfig = new Faker(config);
            CyclicalClassA? a;
            a = fakerWithConfig.Create<CyclicalClassA>();
        }
        public void Method_ClassWithPrivateConstructor()
        {
            FakerConfig config = new FakerConfig();
            config.ThrowExceptions = true; ;
            Faker fakerWithConfig = new Faker(config);
            ClassWithPrivateConstructor? c;
            c = fakerWithConfig.Create<ClassWithPrivateConstructor>();
        }
    }
}