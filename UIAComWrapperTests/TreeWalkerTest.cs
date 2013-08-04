// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.


using System;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIAComWrapperTests
{
    /// <summary>
    /// Summary description for TreeWalker
    /// </summary>
    [TestClass]
    public class TreeWalkerTest
    {
        public TreeWalkerTest()
        {
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void PreDefinedConditionsTest()
        {
            Condition rawView = Automation.RawViewCondition;
            Assert.IsNotNull(rawView);

            Condition controlView = Automation.ControlViewCondition;
            Assert.IsInstanceOfType(controlView, typeof(NotCondition));
            NotCondition notCond = (NotCondition)controlView;
            Assert.IsInstanceOfType(notCond.Condition, typeof(PropertyCondition));

            Condition contentView = Automation.ContentViewCondition;
            Assert.IsInstanceOfType(contentView, typeof(NotCondition));
            NotCondition notCond2 = (NotCondition)contentView;
            Assert.IsInstanceOfType(notCond2.Condition, typeof(OrCondition));
        }

        //
        // TreeIterationTest moved to ExplorerTargetTests.
        //
    }
}
