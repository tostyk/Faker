using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Faker
{
    public class FakerConfig
    {
        // Type - class type
        internal Dictionary<Type, Dictionary<MemberInfo, IValueGenerator>> CustomGenerators = new();
        private int _typeCycleCounter = 2;
        public bool ThrowExceptions = false;
        public void Add<C, T>(Expression<Func<C, T>> expression, IValueGenerator valueGenerator)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            MemberInfo memberInfo = memberExpression.Member;

            Dictionary<MemberInfo, IValueGenerator> typeGenerators = new Dictionary<MemberInfo, IValueGenerator>();

            if (!CustomGenerators.TryGetValue(typeof(C), out typeGenerators))
            {
                CustomGenerators.Add(typeof(C), new Dictionary<MemberInfo, IValueGenerator>());
            }
            CustomGenerators[typeof(C)].Add(memberInfo, valueGenerator);
        }
        public void SetTypeCycleCounter(int depth)
        {
            _typeCycleCounter = depth;
        }
        public int GetTypeCycleCounter()
        {
            return _typeCycleCounter;
        }
    }
}
