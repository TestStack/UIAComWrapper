// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.


using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;

namespace UIAComWrapperTests
{
    
    
    /// <summary>
    ///This is a test class for AutomationElementTest and is intended
    ///to contain all AutomationElementTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AutomationElementTest
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

        public static AutomationElement GetStartButton()
        {
            AndCondition cond = new AndCondition(
                new PropertyCondition(AutomationElement.AccessKeyProperty, "Ctrl+Esc"),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));
            return AutomationElement.RootElement.FindFirst(TreeScope.Subtree, cond);
        }

        public static AutomationElement GetTaskbar()
        {
            PropertyCondition cond = new PropertyCondition(AutomationElement.ClassNameProperty, "Shell_TrayWnd");
            return AutomationElement.RootElement.FindFirst(TreeScope.Subtree, cond);
        }

        public static AutomationElement GetClock()
        {
            return GetTaskbar().FindFirst(TreeScope.Subtree,
                new PropertyCondition(AutomationElement.ClassNameProperty, "TrayClockWClass"));
        }

        /// <summary>
        ///A test for RootElement
        ///</summary>
        [TestMethod()]
        public void RootElementTest()
        {
            AutomationElement actual;
            actual = AutomationElement.RootElement;
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.GetCurrentPropertyValue(AutomationElement.ClassNameProperty), "#32769");
            Assert.AreEqual(actual.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty), 0x10010);
        }

        /// <summary>
        /// Test retrieval of the focused element
        /// </summary>
        [TestMethod()]
        public void FocusedElementTest()
        {
            AutomationElement actual = AutomationElement.FocusedElement;
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Current.HasKeyboardFocus);
        }

        /// <summary>
        ///A test for FromPoint
        ///</summary>
        [TestMethod()]
        public void FromPointTest()
        {
            Point pt = new Point(); 
            AutomationElement actual;
            actual = AutomationElement.FromPoint(pt);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for FromHandle
        ///</summary>
        [TestMethod()]
        public void FromHandleTest()
        {
            int rootHwnd = (int)AutomationElement.RootElement.GetCurrentPropertyValue(
                AutomationElement.NativeWindowHandleProperty);
            AutomationElement actual = AutomationElement.FromHandle((IntPtr)rootHwnd);
            Assert.AreEqual(actual, AutomationElement.RootElement);
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            Point pt = new Point();
            AutomationElement actual1 = AutomationElement.FromPoint(pt);
            Assert.IsNotNull(actual1);
            AutomationElement actual2 = AutomationElement.FromPoint(pt);
            Assert.IsNotNull(actual2);
            Assert.AreEqual(actual1, actual2);
        }

        /// <summary>
        ///A test for FindFirst
        ///</summary>
        [TestMethod()]
        public void FindFirstTest()
        {
            // Find a child
            CacheRequest cacheReq = new CacheRequest();
            cacheReq.Add(AutomationElement.NativeWindowHandleProperty);
            using (cacheReq.Activate())
            {
                Assert.AreSame(CacheRequest.Current, cacheReq);
                AutomationElement actualCached = AutomationElement.RootElement.FindFirst(
                    TreeScope.Children,
                    Condition.TrueCondition);
                Assert.IsNotNull(actualCached);
                int nativeHwnd = (int)actualCached.GetCachedPropertyValue(AutomationElement.NativeWindowHandleProperty);
                Assert.IsTrue(nativeHwnd != 0);
            }
        }

        /// <summary>
        ///A test for FindAll
        ///</summary>
        [TestMethod()]
        public void FindAllTest()
        {
            // Find all children
            CacheRequest cacheReq = new CacheRequest();
            cacheReq.Add(AutomationElement.NameProperty);
            cacheReq.Add(AutomationElement.NativeWindowHandleProperty);
            using (cacheReq.Activate())
            {
                AutomationElementCollection actual = AutomationElement.RootElement.FindAll(
                    TreeScope.Children,
                    Condition.TrueCondition);
                Assert.IsNotNull(actual);
                Assert.IsTrue(actual.Count > 0);

                foreach (AutomationElement elem in actual)
                {
                    Assert.IsNotNull(elem);
                    int nativeHwnd = (int)elem.GetCachedPropertyValue(AutomationElement.NativeWindowHandleProperty);
                    Assert.IsTrue(nativeHwnd != 0);
                } 
            }
        }

        [TestMethod()]
        public void GetClickablePointTest()
        {
            AutomationElement clock = GetClock();
            Point point = clock.GetClickablePoint();
            Assert.IsTrue(point.X > 0);
            Assert.IsTrue(point.Y > 0);
        }

        [TestMethod()]
        public void GetRuntimeIdTest()
        {
            int[] runtimeId = AutomationElement.RootElement.GetRuntimeId();
            Assert.IsNotNull(runtimeId);
            Assert.IsTrue(runtimeId.Length > 0);
        }

        [TestMethod()]
        public void GetUpdatedCacheTest()
        {
            AutomationElement elem = AutomationElement.RootElement;
            try
            {
                string name = elem.Cached.Name;
                Assert.Fail("expected exception");
            }
            catch (ArgumentException)
            {
            }

            CacheRequest req = new CacheRequest();
            req.Add(AutomationElement.NameProperty);
            AutomationElement refreshed = elem.GetUpdatedCache(req);
            string name2 = refreshed.Cached.Name;
        }

        /// <summary>
        /// Simple test to invoke the start menu and test GetCurrentPattern
        /// </summary>
        [TestMethod()]
        public void GetCurrentPatternTest()
        {
            LegacyIAccessiblePattern pattern = (LegacyIAccessiblePattern)GetTaskbar().GetCurrentPattern(LegacyIAccessiblePattern.Pattern);
            Assert.IsNotNull(pattern);
        }

        /// <summary>
        /// Simple test to invoke the start menu and test GetCachedPattern
        /// </summary>
        [TestMethod()]
        public void GetCachedPatternTest()
        {
            CacheRequest req = new CacheRequest();
            req.Add(LegacyIAccessiblePattern.Pattern);
            using (req.Activate())
            {
                LegacyIAccessiblePattern pattern = (LegacyIAccessiblePattern)GetTaskbar().GetCachedPattern(LegacyIAccessiblePattern.Pattern);
                Assert.IsNotNull(pattern);
            }
        }

        [TestMethod()]
        public void GetSupportedTest()
        {
            AutomationProperty[] properties = GetTaskbar().GetSupportedProperties();
            Assert.IsNotNull(properties);
            Assert.IsTrue(properties.Length > 0);
            foreach (AutomationProperty property in properties)
            {
                Assert.IsNotNull(property);
                Assert.IsNotNull(property.ProgrammaticName);
                string programmaticName = Automation.PropertyName(property);
                Assert.IsNotNull(programmaticName);
            }

            AutomationPattern[] patterns = GetTaskbar().GetSupportedPatterns();
            Assert.IsNotNull(patterns);
            Assert.IsTrue(patterns.Length > 0);
            foreach (AutomationPattern pattern in patterns)
            {
                Assert.IsNotNull(pattern);
                Assert.IsNotNull(pattern.ProgrammaticName);
                string programmaticName = Automation.PatternName(pattern);
                Assert.IsNotNull(programmaticName);
            }
        }

        [TestMethod()]
        public void CachedRelationshipTest()
        {
            CacheRequest req = new CacheRequest();
            req.TreeScope = TreeScope.Element | TreeScope.Children;
            using (req.Activate())
            {
                AutomationElement rootElement = AutomationElement.RootElement;
                AutomationElementCollection rootChildren = rootElement.CachedChildren;
                Assert.IsNotNull(rootChildren);
                Assert.IsTrue(rootChildren.Count > 0);

                AutomationElement firstChild = rootChildren[0];
                AutomationElement cachedParent = firstChild.CachedParent;
                Assert.IsNotNull(cachedParent);
                Assert.AreEqual(cachedParent, rootElement);
            }
        }

        [TestMethod()]
        public void NotSupportedValueTest()
        {
            AutomationElement taskbar = GetTaskbar();
            // Pretty sure the start button doesn't support this
            object value = taskbar.GetCurrentPropertyValue(AutomationElement.ItemStatusProperty, true);
            Assert.AreEqual(value, AutomationElement.NotSupported);
        }

        [TestMethod()]
        public void BoundaryRectTest()
        {
            System.Windows.Rect boundingRect = GetTaskbar().Current.BoundingRectangle;
            Assert.IsTrue(boundingRect.Width > 0);
            Assert.IsTrue(boundingRect.Height > 0);
        }

        [TestMethod()]
        public void CompareTest()
        {
            AutomationElement el1 = GetTaskbar();
            AutomationElement el2 = GetTaskbar();
            Assert.IsTrue(Automation.Compare((AutomationElement)null, (AutomationElement)null));
            Assert.IsFalse(Automation.Compare(null, el1));
            Assert.IsFalse(Automation.Compare(el1, null));
            Assert.IsTrue(Automation.Compare(el1, el2));
            Assert.IsTrue(Automation.Compare(el1.GetRuntimeId(), el2.GetRuntimeId()));
        }

        [TestMethod()]
        public void ElementNotAvailableTest()
        {
            AutomationElement element;
            try
            {
                element = AutomationElement.FromHandle((IntPtr)0xF00D);
                Assert.Fail("expected exception");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ElementNotAvailableException));
            }
        }

        [TestMethod()]
        public void ProviderDescriptionTest()
        {
            string description = (string)AutomationElement.RootElement.GetCurrentPropertyValue(AutomationElement.ProviderDescriptionProperty);
            Assert.IsNotNull(description);
            Assert.IsTrue(description.Length > 0);
        }
    }
}
