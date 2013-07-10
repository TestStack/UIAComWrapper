// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.


using System;
using System.Windows.Automation;
using System.Windows.Automation.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIAComWrapperTests
{
    /// <summary>
    /// Tests that use Explorer itself as a target
    /// </summary>
    [TestClass]
    public class ExplorerTargetTests
    {
        public ExplorerTargetTests()
        {
        }

        private TestContext testContextInstance;
        private static ExplorerHost explorerHost;

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

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ExplorerTargetTests.explorerHost = new ExplorerHost();
        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            ((IDisposable)ExplorerTargetTests.explorerHost).Dispose();
        }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            ExplorerTargetTests.explorerHost.Element.SetFocus();
        }
        

        #endregion

        [TestMethod()]
        public void GridPatternTest()
        {
            AutomationElement itemsView = ExplorerTargetTests.explorerHost.Element.FindFirst(TreeScope.Subtree,
                new PropertyCondition(AutomationElement.ClassNameProperty, "UIItemsView"));
            Assert.IsNotNull(itemsView);

            // Try out the Grid Pattern
            GridPattern grid = (GridPattern)itemsView.GetCurrentPattern(GridPattern.Pattern);
            Assert.IsTrue(grid.Current.ColumnCount > 0);
            Assert.IsTrue(grid.Current.RowCount > 0);

            // GridItem
            AutomationElement gridItemElement = grid.GetItem(0, 0);
            Assert.IsNotNull(gridItemElement);
            GridItemPattern gridItem = (GridItemPattern)gridItemElement.GetCurrentPattern(GridItemPattern.Pattern);
            Assert.AreEqual(gridItem.Current.Row, 0);
            Assert.AreEqual(gridItem.Current.Column, 0);
            Assert.AreEqual(gridItem.Current.ContainingGrid, itemsView);
        }

        public void GridPatternCachedTest()
        {
            CacheRequest req = new CacheRequest();
            req.Add(GridItemPattern.Pattern);
            req.Add(GridPattern.Pattern);
            req.Add(GridPattern.RowCountProperty);
            req.Add(GridPattern.ColumnCountProperty);
            req.Add(GridItemPattern.RowProperty);
            req.Add(GridItemPattern.ColumnProperty);
            req.Add(GridItemPattern.ContainingGridProperty);

            using (req.Activate())
            {
                AutomationElement itemsView = ExplorerTargetTests.explorerHost.Element.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.ClassNameProperty, "UIItemsView"));
                Assert.IsNotNull(itemsView);

                // Try out the Grid Pattern
                GridPattern grid = (GridPattern)itemsView.GetCachedPattern(GridPattern.Pattern);
                Assert.IsTrue(grid.Cached.ColumnCount > 0);
                Assert.IsTrue(grid.Cached.RowCount > 0);

                // GridItem
                AutomationElement gridItemElement = grid.GetItem(0, 0);
                Assert.IsNotNull(gridItemElement);
                GridItemPattern gridItem = (GridItemPattern)gridItemElement.GetCachedPattern(GridItemPattern.Pattern);
                Assert.AreEqual(gridItem.Cached.Row, 0);
                Assert.AreEqual(gridItem.Cached.Column, 0);
                Assert.AreEqual(gridItem.Cached.ContainingGrid, itemsView);
            }
        }

        [TestMethod()]
        public void MultipleViewPatternTest()
        {
            CacheRequest req = new CacheRequest();
            req.Add(MultipleViewPattern.Pattern);
            req.Add(MultipleViewPattern.CurrentViewProperty);
            req.Add(MultipleViewPattern.SupportedViewsProperty);

            using (req.Activate())
            {
                AutomationElement itemsView = ExplorerTargetTests.explorerHost.Element.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.ClassNameProperty, "UIItemsView"));
                Assert.IsNotNull(itemsView);

                MultipleViewPattern multiView = (MultipleViewPattern)itemsView.GetCachedPattern(MultipleViewPattern.Pattern);
                int[] supportedViews = multiView.Cached.GetSupportedViews();
                Assert.IsNotNull(supportedViews.Length > 0);
                bool inSupportedViews = false;
                foreach (int view in supportedViews)
                {
                    if (view == multiView.Cached.CurrentView)
                    {
                        inSupportedViews = true;
                        break;
                    }
                    string viewName = multiView.GetViewName(view);
                    Assert.IsTrue(viewName.Length > 0);
                }
                Assert.IsTrue(inSupportedViews);
            }
        }

        [TestMethod()]
        public void MultipleViewPatternCachedTest()
        {
            AutomationElement itemsView = ExplorerTargetTests.explorerHost.Element.FindFirst(TreeScope.Subtree,
                new PropertyCondition(AutomationElement.ClassNameProperty, "UIItemsView"));
            Assert.IsNotNull(itemsView);

            MultipleViewPattern multiView = (MultipleViewPattern)itemsView.GetCurrentPattern(MultipleViewPattern.Pattern);
            int[] supportedViews = multiView.Current.GetSupportedViews();
            Assert.IsNotNull(supportedViews.Length > 0);
            bool inSupportedViews = false;
            foreach (int view in supportedViews)
            {
                if (view == multiView.Current.CurrentView)
                {
                    inSupportedViews = true;
                    break;
                }
                string viewName = multiView.GetViewName(view);
                Assert.IsTrue(viewName.Length > 0);
            }
            Assert.IsTrue(inSupportedViews);
        }

        [TestMethod()]
        public void TablePatternTest()
        {
            AutomationElement itemsView = ExplorerTargetTests.explorerHost.Element.FindFirst(TreeScope.Subtree,
                new PropertyCondition(AutomationElement.ClassNameProperty, "UIItemsView"));
            Assert.IsNotNull(itemsView);

            // TablePattern test
            TablePattern table = (TablePattern)itemsView.GetCurrentPattern(TablePattern.Pattern);
            Assert.IsTrue(table.Current.ColumnCount > 0);
            Assert.IsTrue(table.Current.RowCount > 0);
            Assert.IsTrue(table.Current.GetRowHeaders().Length == 0);
            Assert.IsTrue(table.Current.GetColumnHeaders().Length > 0);

            AutomationElement tableItemElement = table.GetItem(0, 0);
            TableItemPattern tableItem = (TableItemPattern)tableItemElement.GetCurrentPattern(TableItemPattern.Pattern);
            Assert.AreEqual(tableItem.Current.Row, 0);
            Assert.AreEqual(tableItem.Current.Column, 0);
            Assert.AreEqual(tableItem.Current.ContainingGrid, itemsView);
            Assert.IsTrue(tableItem.Current.GetColumnHeaderItems().Length == 1);
            Assert.IsTrue(tableItem.Current.GetRowHeaderItems().Length == 0);
        }

        [TestMethod()]
        public void TablePatternCachedTest()
        {
            CacheRequest req = new CacheRequest();
            req.Add(TablePattern.Pattern);
            req.Add(TableItemPattern.Pattern);
            req.Add(GridPattern.Pattern);
            req.Add(GridItemPattern.Pattern);
            req.Add(GridPattern.RowCountProperty);
            req.Add(GridPattern.ColumnCountProperty);
            req.Add(GridItemPattern.RowProperty);
            req.Add(GridItemPattern.ColumnProperty);
            req.Add(GridItemPattern.ContainingGridProperty);
            req.Add(TablePattern.RowHeadersProperty);
            req.Add(TablePattern.ColumnHeadersProperty);
            req.Add(TableItemPattern.RowHeaderItemsProperty);
            req.Add(TableItemPattern.ColumnHeaderItemsProperty);
            using (req.Activate())
            {
                AutomationElement itemsView = ExplorerTargetTests.explorerHost.Element.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.ClassNameProperty, "UIItemsView"));
                Assert.IsNotNull(itemsView);

                // TablePattern test
                TablePattern table = (TablePattern)itemsView.GetCachedPattern(TablePattern.Pattern);
                Assert.IsTrue(table.Cached.ColumnCount > 0);
                Assert.IsTrue(table.Cached.RowCount > 0);
                Assert.IsTrue(table.Cached.GetRowHeaders().Length == 0);
                Assert.IsTrue(table.Cached.GetColumnHeaders().Length > 0);

                AutomationElement tableItemElement = table.GetItem(0, 0);
                TableItemPattern tableItem = (TableItemPattern)tableItemElement.GetCachedPattern(TableItemPattern.Pattern);
                Assert.AreEqual(tableItem.Cached.Row, 0);
                Assert.AreEqual(tableItem.Cached.Column, 0);
                Assert.AreEqual(tableItem.Cached.ContainingGrid, itemsView);
                Assert.IsTrue(tableItem.Cached.GetColumnHeaderItems().Length == 1);
                Assert.IsTrue(tableItem.Cached.GetRowHeaderItems().Length == 0);
            }
        }

        [TestMethod]
        public void TreeIterationTest()
        {
            TreeWalker walker = new TreeWalker(Automation.ControlViewCondition);
            AutomationElement startingElement = ExplorerTargetTests.explorerHost.Element;
            AutomationElement iter = startingElement;
            iter = walker.GetFirstChild(iter);
            iter = walker.GetNextSibling(iter);
            iter = walker.GetParent(iter);
            Assert.AreEqual(startingElement, iter);
            iter = walker.GetLastChild(iter);
            iter = walker.GetPreviousSibling(iter);
            iter = walker.GetParent(iter);
            Assert.AreEqual(startingElement, iter);
        }

        [TestMethod()]
        [Ignore]
        // Test is not working on Windows 8 due to changes in Explorer
        public void VirtualizedPatternTest()
        {
            AutomationElement itemsView = ExplorerTargetTests.explorerHost.Element.FindFirst(TreeScope.Subtree,
                new PropertyCondition(AutomationElement.ClassNameProperty, "UIItemsView"));
            Assert.IsNotNull(itemsView);

            // Get the container
            Assert.IsTrue((bool)itemsView.GetCurrentPropertyValue(AutomationElement.IsItemContainerPatternAvailableProperty));
            ItemContainerPattern container = (ItemContainerPattern)itemsView.GetCurrentPattern(ItemContainerPattern.Pattern);
            
            // Look for something we know is there and is probably below the fold
            AutomationElement item1 = container.FindItemByProperty(null, AutomationElement.NameProperty, "winver");
            Assert.IsNotNull(item1);

            // Let's get another one
            AutomationElement item2 = container.FindItemByProperty(item1, AutomationElement.NameProperty, "xcopy");
            Assert.IsNotNull(item2);

            // Check the bounding rect -- should be empty
            System.Windows.Rect rect1 = item2.Current.BoundingRectangle;
            Assert.AreEqual(0, rect1.Width);
            Assert.AreEqual(0, rect1.Height);

            // Get the virtualized pattern
            Assert.IsTrue((bool)item2.GetCurrentPropertyValue(AutomationElement.IsVirtualizedItemPatternAvailableProperty));
            VirtualizedItemPattern virtItem2 = (VirtualizedItemPattern)item2.GetCurrentPattern(VirtualizedItemPattern.Pattern);
            Assert.IsNotNull(item2);

            // Realize the item and give the window a moment to scroll
            virtItem2.Realize();
            System.Threading.Thread.Sleep(100 /* ms */);

            // Check the bounding rect now - should not be empty
            System.Windows.Rect rect2 = item2.Current.BoundingRectangle;
            Assert.AreNotEqual(0, rect2.Width);
            Assert.AreNotEqual(0, rect2.Height);


        }
    }
}
