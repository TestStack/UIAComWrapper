// (c) Copyright Michael Bernstein, 2009.
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
    /// ScenarioTests: intended to contain tests that manipulate UI
    /// and are therefore less reliable than pure unit tests.
    ///</summary>
    [TestClass()]
    public class ScenarioTests
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

        [TestMethod()]
        public void ExpandCollapsePatternTest()
        {
            using (AppHost host = new AppHost("rundll32.exe", "shell32.dll,Control_RunDLL intl.cpl"))
            {
                // Find a well-known combo box
                AutomationElement combo = host.Element.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "1021"));
                Assert.IsNotNull(combo);

                ExpandCollapsePattern expando = (ExpandCollapsePattern)combo.GetCurrentPattern(ExpandCollapsePattern.Pattern);
                Assert.AreEqual(expando.Current.ExpandCollapseState, ExpandCollapseState.Collapsed);
                expando.Expand();
                System.Threading.Thread.Sleep(100 /* ms */);
                Assert.AreEqual(expando.Current.ExpandCollapseState, ExpandCollapseState.Expanded);
                expando.Collapse();
                System.Threading.Thread.Sleep(100 /* ms */);
                Assert.AreEqual(expando.Current.ExpandCollapseState, ExpandCollapseState.Collapsed);
            }
        }

        [TestMethod()]
        public void ExpandCollapsePatternCachedTest()
        {
            using (AppHost host = new AppHost("rundll32.exe", "shell32.dll,Control_RunDLL intl.cpl"))
            {
                CacheRequest req = new CacheRequest();
                req.Add(ExpandCollapsePattern.Pattern);
                req.Add(ExpandCollapsePattern.ExpandCollapseStateProperty);
                using (req.Activate())
                {
                    // Find a well-known combo box
                    AutomationElement combo = host.Element.FindFirst(TreeScope.Subtree,
                        new PropertyCondition(AutomationElement.AutomationIdProperty, "1021"));
                    Assert.IsNotNull(combo);

                    ExpandCollapsePattern expando = (ExpandCollapsePattern)combo.GetCachedPattern(ExpandCollapsePattern.Pattern);
                    Assert.AreEqual(expando.Cached.ExpandCollapseState, ExpandCollapseState.Collapsed);
                }
            }
        }

        [TestMethod()]
        public void NoClickablePointTest()
        {
            // Launch a notepad and position it
            using (AppHost host1 = new AppHost("notepad.exe", ""))
            {
                TransformPattern transformPattern1 = (TransformPattern)host1.Element.GetCurrentPattern(TransformPattern.Pattern);
                transformPattern1.Move(0, 0);
                transformPattern1.Resize(400, 300);

                System.Windows.Point pt1 = host1.Element.GetClickablePoint();

                // Launch a second notepad and position it on top
                using (AppHost host2 = new AppHost("notepad.exe", ""))
                {
                    TransformPattern transformPattern2 = (TransformPattern)host2.Element.GetCurrentPattern(TransformPattern.Pattern);
                    transformPattern2.Move(0, 0);
                    transformPattern2.Resize(400, 300);

                    // Now try it again for host1
                    try
                    {
                        System.Windows.Point pt1again = host1.Element.GetClickablePoint();
                        Assert.Fail("expected exception");
                    }
                    catch (NoClickablePointException)
                    {
                    }
                }
            }
        }

        [TestMethod()]
        public void RangeValuePatternTest()
        {
            using (AppHost host = new AppHost("rundll32.exe", "shell32.dll,Control_RunDLL main.cpl ,2"))
            {
                // Find a well-known slider
                AutomationElement slider = host.Element.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "101"));
                Assert.IsNotNull(slider);

                RangeValuePattern range = (RangeValuePattern)slider.GetCurrentPattern(RangeValuePattern.Pattern);
                double originalValue = range.Current.Value;
                try
                {
                    Assert.IsTrue(range.Current.SmallChange >= 0);
                    Assert.IsTrue(range.Current.LargeChange >= 0);
                    Assert.IsTrue(originalValue >= range.Current.Minimum);
                    Assert.IsTrue(originalValue <= range.Current.Maximum);
                    Assert.IsFalse(range.Current.IsReadOnly);
                    range.SetValue(range.Current.Minimum);
                    System.Threading.Thread.Sleep(100 /* ms */);
                    Assert.AreEqual(range.Current.Value, range.Current.Minimum);

                    range.SetValue(range.Current.Maximum);
                    System.Threading.Thread.Sleep(100 /* ms */);
                    Assert.AreEqual(range.Current.Value, range.Current.Maximum);

                    double midpoint = (range.Current.Maximum + range.Current.Minimum) / 2;
                    range.SetValue(midpoint);
                    System.Threading.Thread.Sleep(100 /* ms */);
                    Assert.AreEqual(range.Current.Value, midpoint);
                }
                finally
                {
                    range.SetValue(originalValue);
                }
            }
        }

        public void RangeValuePatternCachedTest()
        {
            using (AppHost host = new AppHost("rundll32.exe", "shell32.dll,Control_RunDLL main.cpl ,2"))
            {
                CacheRequest req = new CacheRequest();
                req.Add(RangeValuePattern.Pattern);
                req.Add(RangeValuePattern.IsReadOnlyProperty);
                req.Add(RangeValuePattern.MaximumProperty);
                req.Add(RangeValuePattern.MinimumProperty);
                req.Add(RangeValuePattern.SmallChangeProperty);
                req.Add(RangeValuePattern.LargeChangeProperty);
                using (req.Activate())
                {
                    // Find a well-known slider
                    AutomationElement slider = host.Element.FindFirst(TreeScope.Subtree,
                        new PropertyCondition(AutomationElement.AutomationIdProperty, "101"));
                    Assert.IsNotNull(slider);

                    RangeValuePattern range = (RangeValuePattern)slider.GetCachedPattern(RangeValuePattern.Pattern);
                    double originalValue = range.Cached.Value;
                    Assert.IsTrue(range.Cached.SmallChange >= 0);
                    Assert.IsTrue(range.Cached.LargeChange >= 0);
                    Assert.IsTrue(originalValue >= range.Cached.Minimum);
                    Assert.IsTrue(originalValue <= range.Cached.Maximum);
                    Assert.IsFalse(range.Cached.IsReadOnly);
                }
            }
        }

        [TestMethod()]
        public void TextPatternTest()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();

            // Fragile -- I'm open to a better way of doing this.
            using (AppHost host = new AppHost("xpsrchvw.exe", "..\\..\\..\\UiaComWrapperTests\\bin\\debug\\test.xps"))
            {
                AutomationElement mainContent = host.Element.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.IsTextPatternAvailableProperty, true));
                TextPattern text = (TextPattern)mainContent.GetCurrentPattern(TextPattern.Pattern);
                Assert.AreEqual(text.SupportedTextSelection, SupportedTextSelection.Single);

                TextPatternRange range1 = text.DocumentRange;
                Assert.IsNotNull(range1);
                Assert.AreEqual(text, range1.TextPattern);
                TextPatternRange range2 = range1.Clone();
                Assert.IsNotNull(range2);
                Assert.IsTrue(range1.Compare(range2));
                Assert.IsTrue(0 == range1.CompareEndpoints(TextPatternRangeEndpoint.Start, range2, TextPatternRangeEndpoint.Start));
                Assert.IsTrue(0 == range1.CompareEndpoints(TextPatternRangeEndpoint.End, range2, TextPatternRangeEndpoint.End));

                string keyString = "Constitution of the United States";
                TextPatternRange range3 = range1.FindText(keyString, false, true);
                Assert.IsNotNull(range3);
                string foundString = range3.GetText(-1);
                Assert.AreEqual(keyString, foundString);
                range3.Select();
                TextPatternRange[] selectedRanges = text.GetSelection();
                Assert.AreEqual(1, selectedRanges.Length);
                TextPatternRange selectedRange = selectedRanges[0];
                Assert.IsTrue(range3.Compare(selectedRange));

                // Test attributes.  Casts will fail if types are wrong
                System.Globalization.CultureInfo culture = (System.Globalization.CultureInfo)range3.GetAttributeValue(TextPattern.CultureAttribute);
                string fontName = (string)range3.GetAttributeValue(TextPattern.FontNameAttribute);
                bool hiddenValue = (bool)range3.GetAttributeValue(TextPattern.IsItalicAttribute);
                Assert.AreEqual(AutomationElement.NotSupported, range3.GetAttributeValue(TextPattern.IsHiddenAttribute));

                TextPatternRange range5 = range1.FindAttribute(TextPattern.IsItalicAttribute, true, false /* backward */);
                Assert.IsNotNull(range5);
                Assert.AreEqual("Note", range5.GetText(-1));

                range5.ExpandToEnclosingUnit(TextUnit.Line);
                string line5 = range5.GetText(-1);
                Assert.AreEqual("Preamble Note ", line5);

                System.Windows.Rect[] rects = range3.GetBoundingRectangles();
                Assert.AreEqual(rects.Length, 1);
                Assert.IsTrue(rects[0].Width > 0);
                Assert.IsTrue(rects[0].Height > 0);
            }
        }

        [TestMethod()]
        public void TogglePatternTest()
        {
            using (AppHost host = new AppHost("rundll32.exe", "shell32.dll,Control_RunDLL main.cpl ,2"))
            {
                // Find a well-known checkbox
                AutomationElement checkbox = host.Element.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "109"));
                Assert.IsNotNull(checkbox);

                TogglePattern toggle = (TogglePattern)checkbox.GetCurrentPattern(TogglePattern.Pattern);
                ToggleState originalState = toggle.Current.ToggleState;
                toggle.Toggle();
                // Slight wait for effect
                System.Threading.Thread.Sleep(100 /* ms */);
                ToggleState currentState = toggle.Current.ToggleState;
                Assert.AreNotEqual(originalState, currentState);

                // Put it back
                while (currentState != originalState)
                {
                    toggle.Toggle();
                    System.Threading.Thread.Sleep(100 /* ms */);
                    currentState = toggle.Current.ToggleState;
                }
            }
        }

        public void TogglePatternCachedTest()
        {
            using (AppHost host = new AppHost("rundll32.exe", "shell32.dll,Control_RunDLL main.cpl"))
            {
                CacheRequest req = new CacheRequest();
                req.Add(TogglePattern.Pattern);
                req.Add(TogglePattern.ToggleStateProperty);
                using (req.Activate())
                {
                    // Find a well-known checkbox
                    AutomationElement checkbox = host.Element.FindFirst(TreeScope.Subtree,
                        new PropertyCondition(AutomationElement.AutomationIdProperty, "114"));
                    Assert.IsNotNull(checkbox);

                    TogglePattern toggle = (TogglePattern)checkbox.GetCachedPattern(TogglePattern.Pattern);
                    ToggleState originalState = toggle.Cached.ToggleState;
                    Assert.IsTrue(originalState == ToggleState.On || originalState == ToggleState.Off);
                }
            }
        }

        [TestMethod()]
        public void TransformPatternTest()
        {
            // Launch a notepad and position it
            using (AppHost host = new AppHost("notepad.exe", ""))
            {
                TransformPattern transformPattern = (TransformPattern)host.Element.GetCurrentPattern(TransformPattern.Pattern);
                // Coded to expectations for an explorer window
                Assert.IsTrue(transformPattern.Current.CanMove);
                Assert.IsTrue(transformPattern.Current.CanResize);
                Assert.IsFalse(transformPattern.Current.CanRotate);
                
                // Little move
                transformPattern.Move(10, 10);

                // Little resize
                transformPattern.Resize(200, 200);
            }
        }

        [TestMethod()]
        public void TransformPatternCachedTest()
        {
            using (AppHost host = new AppHost("notepad.exe", ""))
            {
                CacheRequest req = new CacheRequest();
                req.Add(TransformPattern.Pattern);
                req.Add(TransformPattern.CanMoveProperty);
                req.Add(TransformPattern.CanResizeProperty);
                req.Add(TransformPattern.CanRotateProperty);
                using (req.Activate())
                {
                    AutomationElement cachedEl = host.Element.GetUpdatedCache(req);

                    TransformPattern transformPattern = (TransformPattern)cachedEl.GetCachedPattern(TransformPattern.Pattern);
                    // Coded to expectations for an explorer window
                    Assert.IsTrue(transformPattern.Cached.CanMove);
                    Assert.IsTrue(transformPattern.Cached.CanResize);
                    Assert.IsFalse(transformPattern.Cached.CanRotate);

                    // Little move
                    transformPattern.Move(10, 10);

                    // Little resize
                    transformPattern.Resize(200, 200);
                }
            }
        }

        [TestMethod()]
        public void ValuePatternTest()
        {
            using (AppHost host = new AppHost("rundll32.exe", "shell32.dll,Control_RunDLL intl.cpl"))
            {
                // Find a well-known combo box
                AutomationElement combo = host.Element.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "1021"));
                Assert.IsNotNull(combo);

                ValuePattern value = (ValuePattern)combo.GetCurrentPattern(ValuePattern.Pattern);
                Assert.IsFalse(value.Current.IsReadOnly);
                Assert.IsTrue(value.Current.Value.Length > 0);
            }
        }

        [TestMethod()]
        public void ValuePatternCachedTest()
        {
            using (AppHost host = new AppHost("rundll32.exe", "shell32.dll,Control_RunDLL intl.cpl"))
            {
                CacheRequest req = new CacheRequest();
                req.Add(WindowPattern.Pattern);
                req.Add(WindowPattern.CanMaximizeProperty);
                req.Add(WindowPattern.CanMinimizeProperty);
                req.Add(WindowPattern.IsTopmostProperty);
                req.Add(WindowPattern.WindowInteractionStateProperty);
                req.Add(WindowPattern.WindowVisualStateProperty);
                using (req.Activate())
                {
                    // Find a well-known combo box
                    AutomationElement combo = host.Element.FindFirst(TreeScope.Subtree,
                        new PropertyCondition(AutomationElement.AutomationIdProperty, "1021"));
                    Assert.IsNotNull(combo);

                    ValuePattern value = (ValuePattern)combo.GetCurrentPattern(ValuePattern.Pattern);
                    Assert.IsFalse(value.Current.IsReadOnly);
                    Assert.IsTrue(value.Current.Value.Length > 0);
                }
            }
        }

        [TestMethod()]
        public void WindowPatternTest()
        {
            using (AppHost host = new AppHost("notepad.exe", ""))
            {
                // Window Pattern
                WindowPattern windowPattern = (WindowPattern)host.Element.GetCurrentPattern(WindowPattern.Pattern);
                Assert.IsTrue(windowPattern.Current.CanMaximize);
                Assert.IsTrue(windowPattern.Current.CanMinimize);
                Assert.IsFalse(windowPattern.Current.IsTopmost);
                Assert.AreNotEqual(windowPattern.Current.WindowVisualState, WindowVisualState.Minimized);
                Assert.AreNotEqual(windowPattern.Current.WindowInteractionState, WindowInteractionState.Closing);
            }
        }

        [TestMethod()]
        public void WindowPatternCachedTest()
        {
            using (AppHost host = new AppHost("notepad.exe", ""))
            {
                CacheRequest req = new CacheRequest();
                req.Add(WindowPattern.Pattern);
                req.Add(WindowPattern.CanMaximizeProperty);
                req.Add(WindowPattern.CanMinimizeProperty);
                req.Add(WindowPattern.IsTopmostProperty);
                req.Add(WindowPattern.WindowInteractionStateProperty);
                req.Add(WindowPattern.WindowVisualStateProperty);
                using (req.Activate())
                {
                    AutomationElement cachedEl = host.Element.GetUpdatedCache(req);

                    // Window Pattern
                    WindowPattern windowPattern = (WindowPattern)cachedEl.GetCachedPattern(WindowPattern.Pattern);
                    Assert.IsTrue(windowPattern.Cached.CanMaximize);
                    Assert.IsTrue(windowPattern.Cached.CanMinimize);
                    Assert.IsFalse(windowPattern.Cached.IsTopmost);
                    Assert.AreNotEqual(windowPattern.Cached.WindowVisualState, WindowVisualState.Minimized);
                    Assert.AreNotEqual(windowPattern.Cached.WindowInteractionState, WindowInteractionState.Closing);
                }
            }
        }
    }
}
