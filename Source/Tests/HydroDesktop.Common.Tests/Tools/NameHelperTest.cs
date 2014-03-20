using HydroDesktop.Common.Tools;
using NUnit.Framework;

namespace HydroDesktop.Common.Tests.Tools
{
    [TestFixture]
    public class NameHelperTest
    {
        private class Class1
        {
            public object A { get; set; }
        }

        private class Class2
        {
            public Class1 B { get; set; }
        }

        private Class2 MyProperty { get; set; }


        [Test]
        public void ThisClass()
        {
            var propertyName = NameHelper.Name(() => MyProperty);
            Assert.AreEqual("MyProperty", propertyName);
        }

        [Test]
        public void OtherClass()
        {
            var propertyName1 = NameHelper<Class1>.Name(o => o.A);
            Assert.AreEqual("A", propertyName1);

#pragma warning disable 1720
            var propertyName2 = NameHelper.Name(() => default(Class1).A);
#pragma warning restore 1720
            Assert.AreEqual("A", propertyName2);
        }

        [Test]
        public void DeepProperty()
        {
            var propertyName1 = NameHelper<Class2>.Name(o => o.B.A, true);
            Assert.AreEqual("B.A", propertyName1);

            var propertyName2 = NameHelper<Class2>.Name(o => o.B.A);
            Assert.AreEqual("A", propertyName2);

            var propertyName3 = NameHelper<Class2>.Name(o => o.B, true);
            Assert.AreEqual("B", propertyName3);

            var propertyName4 = NameHelper.Name(() => MyProperty.B.A, true);
            Assert.AreEqual("MyProperty.B.A", propertyName4);

            var propertyName5 = NameHelper.Name(() => MyProperty.B.A);
            Assert.AreEqual("A", propertyName5);
        }
    }
}
