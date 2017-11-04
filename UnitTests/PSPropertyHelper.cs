using System.Management.Automation;

namespace UnitTests
{
    public class PSPropertyHelper
    {
        private PSPropertyInfo _propertyInfo;

        public string Name { get { return _propertyInfo.Name; } }

        public string TypeNameOfValue { get { return _propertyInfo.TypeNameOfValue; } }

        public bool IsInstance { get { return _propertyInfo.IsInstance; } }
        
        public PSObjectHelper Value { get; private set; }

        public PSPropertyHelper(PSPropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            Value = new PSObjectHelper(propertyInfo.Value);
        }
    }
}