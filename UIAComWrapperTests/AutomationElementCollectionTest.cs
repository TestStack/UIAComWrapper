// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.


using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace UIAComWrapperTests
{
    
    
    /// <summary>
    ///This is a test class for AutomationElementCollectionTest and is intended
    ///to contain all AutomationElementCollectionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AutomationElementCollectionTest
    {


        private TestContext testContextInstance;
        private AutomationElementCollection testColl;

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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            // Get all children of the desktop for our target collection
            CacheRequest cacheReq = new CacheRequest();
            cacheReq.Add(AutomationElement.NameProperty);
            cacheReq.Add(AutomationElement.NativeWindowHandleProperty);
            using (cacheReq.Activate())
            {
                this.testColl = AutomationElement.RootElement.FindAll(
                    TreeScope.Children,
                    Condition.TrueCondition);
                Assert.IsNotNull(this.testColl);
                Assert.IsTrue(this.testColl.Count > 0);
            }
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Item
        ///</summary>
        [TestMethod()]
        public void ItemTest()
        {
            AutomationElement actual = this.testColl[0];
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Count
        ///</summary>
        [TestMethod()]
        public void CountTest()
        {
            Assert.IsTrue(this.testColl.Count > 0);
        }

        /// <summary>
        ///A test for GetEnumerator
        ///</summary>
        [TestMethod()]
        public void GetEnumeratorTest()
        {
            IEnumerator actual = this.testColl.GetEnumerator();
            int count = 0;
            while (actual.MoveNext())
            {
                AutomationElement elem = (AutomationElement)actual.Current;
                Assert.IsNotNull(elem);
                ++count;
            } 

            actual.Reset();
            actual.MoveNext();
            Assert.AreEqual(actual.Current, this.testColl[0]);
        }

        /// <summary>
        ///A test for CopyTo
        ///</summary>
        [TestMethod()]
        public void CopyToTest()
        {
            AutomationElement[] array = new AutomationElement[this.testColl.Count+1];
            this.testColl.CopyTo(array, 1);
            for (int i = 0; i < this.testColl.Count; ++i)
            {
                Assert.AreEqual(this.testColl[i], array[i + 1]);
            }
        }
    }
}
