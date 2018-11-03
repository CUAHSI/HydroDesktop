using Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace Test
{
    
    
    /// <summary>
    ///This is a test class for NumericsTest and is intended
    ///to contain all NumericsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NumericsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SecantMethod
        ///</summary>
        [TestMethod()]
        public void SecantMethodTest()
        {
            Numerics.Numerics target = new Numerics.Numerics(); // TODO: Initialize to an appropriate value
            double fx1 = 0F; // TODO: Initialize to an appropriate value
            double x1 = 0F; // TODO: Initialize to an appropriate value
            double fx2 = 0F; // TODO: Initialize to an appropriate value
            double x2 = 0F; // TODO: Initialize to an appropriate value
            Dictionary<string, double> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<string, double> actual;
            actual = target.SecantMethod(fx1, x1, fx2, x2);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModifiedSecantMethod
        ///</summary>
        [TestMethod()]
        public void ModifiedSecantMethodTest()
        {
            Numerics.Numerics target = new Numerics.Numerics(); // TODO: Initialize to an appropriate value
            double fx = 0F; // TODO: Initialize to an appropriate value
            double x = 0F; // TODO: Initialize to an appropriate value
            double fxdel = 0F; // TODO: Initialize to an appropriate value
            double delta = 0F; // TODO: Initialize to an appropriate value
            Dictionary<string, double> expected = null; // TODO: Initialize to an appropriate value
            Dictionary<string, double> actual;
            actual = target.ModifiedSecantMethod(fx, x, fxdel, delta);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Numerics Constructor
        ///</summary>
        [TestMethod()]
        public void NumericsConstructorTest()
        {
            Numerics.Numerics target = new Numerics.Numerics();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }


        /// <summary>
        /// A test for NewtonRaphson
        /// </summary>
        [TestMethod()]
        public void NewtonRaphson()
        {
            Parameter E1 = new Parameter(0.0);
            Parameter E2 = new Parameter(0.0);
            Parameter E3 = new Parameter(0.0);
            Parameter E4 = new Parameter(0.0);
            Func<double>[] functions = new Func<double>[]
            {
                () => 0.005 * (100.0 - E1 - 2.0 * E2) * (1.0 - E1 - E3) - 100.0 * E1,
                () => 500.0 * Math.Pow(100.0 - E1 - 2.0 * E2, 2.0) - 100.0 * E2,
                () => 0.5 * (100.0 - E1 - E3 - 2.0 * E4) - 100.0 * E3,
                () => 10000.0 * Math.Pow(100.0 * E3 - 2.0 * E4, 2.0) - 100.0 * E4
            };

            Parameter[] parameters = new Parameter[] { E1, E2, E3, E4 };

            NewtonRaphson nr = new NewtonRaphson(parameters, functions);
            for (int i = 0; i < 15; i++)
            {
                nr.Iterate();
            }

            double[] result = nr.GetResult();
            for (int i = 0; i <= result.Length - 1; i++)
                System.Diagnostics.Debug.WriteLine("E" + i.ToString() + "\t" + result[i].ToString());

        }
    }
}
