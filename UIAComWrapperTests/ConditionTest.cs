// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.


using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIAComWrapperTests
{
    /// <summary>
    ///This is a test class for Conditions and is intended
    ///to contain all Conditions Unit Tests
    ///</summary>
    [TestClass()]
    public class ConditionTests
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
        ///A test for AndCondition
        ///</summary>
        [TestMethod()]
        public void AndConditionTest()
        {
            // Positive test
            Condition condition = Condition.TrueCondition;
            Condition condition2 = Condition.FalseCondition;
            AndCondition target = new AndCondition(condition, condition2);
            Condition[] actual;
            actual = target.GetConditions();
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Length, 2);

            // Negative test - include a null
            try
            {
                target = new AndCondition(condition, null);

                Assert.Fail("expected exception");
            }
            catch (System.ArgumentException)
            {
            }
        }

        /// <summary>
        ///A test for GetConditions
        ///</summary>
        [TestMethod()]
        public void OrConditionTest()
        {
            Condition condition = Condition.TrueCondition;
            Condition condition2 = OrCondition.FalseCondition;
            OrCondition target = new OrCondition(condition, condition2);
            Condition[] actual;
            actual = target.GetConditions();
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Length, 2);

            // Negative test - include a null
            try
            {
                target = new OrCondition(condition, null);

                Assert.Fail("expected exception");
            }
            catch (System.ArgumentException)
            {
            }
        }

        /// <summary>
        ///A test for NotCondition
        ///</summary>
        [TestMethod()]
        public void NotConditionTest()
        {
            Condition condition = Condition.TrueCondition;
            NotCondition target = new NotCondition(condition);
            Assert.IsNotNull(target);
            Condition child = target.Condition;
            Assert.IsNotNull(child);
        }

        /// <summary>
        ///A test for PropertyCondition
        ///</summary>
        [TestMethod()]
        public void PropertyConditionTest()
        {
            PropertyCondition cond1 = new PropertyCondition(
                AutomationElement.NameProperty, 
                "foo");
            Assert.IsNotNull(cond1);
            Assert.AreEqual(cond1.Value, "foo");
            Assert.AreEqual(cond1.Property.ProgrammaticName, "AutomationElementIdentifiers.NameProperty");

            System.Windows.Rect rect = new System.Windows.Rect(0, 0, 20, 20);
            PropertyCondition cond2 = new PropertyCondition(
                AutomationElement.BoundingRectangleProperty,
                rect);
            Assert.IsNotNull(cond2);
            object value = cond2.Value;
            Assert.IsInstanceOfType(value, typeof(double[]));
            Assert.AreEqual(((double[])value).Length, 4);
            Assert.AreEqual(cond2.Property.ProgrammaticName, "AutomationElementIdentifiers.BoundingRectangleProperty");

            PropertyCondition cond3 = new PropertyCondition(
                AutomationElement.ClickablePointProperty,
                new System.Windows.Point(0, 0));
            Assert.IsNotNull(cond3);
            value = cond3.Value;
            Assert.IsInstanceOfType(value, typeof(double[]));
            Assert.AreEqual(((double[])value).Length, 2);
            Assert.AreEqual(cond3.Property.ProgrammaticName, "AutomationElementIdentifiers.ClickablePointProperty");

            // Negative case
            try
            {
                PropertyCondition cond4 = new PropertyCondition(
                    null, null);

                Assert.Fail("expected exception");
            }
            catch (System.ArgumentException)
            {
            }



        }
    }
}
