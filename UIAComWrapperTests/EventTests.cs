// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.


using System;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIAComWrapperTests
{
    public class EventHandlerBase
    {
        private System.Threading.ManualResetEvent _syncEvent = new System.Threading.ManualResetEvent(false);
        private AutomationElement _eventSource;
        private uint _receivedEventCount;

        protected void OnMatchingEvent(AutomationElement sender)
        {
            _eventSource = sender;
            _receivedEventCount++;
            _syncEvent.Set();
        }

        public EventHandlerBase()
        {
        }

        public void Start()
        {
            _receivedEventCount = 0;
            _eventSource = null;
        }

        public bool Confirm()
        {
            return this._syncEvent.WaitOne(3000 /* ms */);
        }

        public AutomationElement EventSource
        {
            get
            {
                return this._eventSource;
            }
        }

        public uint ReceivedEventCount
        {
            get
            {
                return this._receivedEventCount;
            }
        }
    }

    public class FocusChangeHandler : EventHandlerBase
    {
        public void HandleEvent(object sender, AutomationFocusChangedEventArgs e)
        {
            OnMatchingEvent((AutomationElement)sender);
        }
    }

    public class BasicChangeHandler : EventHandlerBase
    {
        public void HandleEvent(object sender, AutomationEventArgs e)
        {
            OnMatchingEvent((AutomationElement)sender);
        }
    }

    public class PropertyChangeHandler : EventHandlerBase
    {
        public void HandleEvent(object sender, AutomationPropertyChangedEventArgs e)
        {
            OnMatchingEvent((AutomationElement)sender);
        }
    }

    public class StructureChangeHandler : EventHandlerBase
    {
        public void HandleEvent(object sender, StructureChangedEventArgs e)
        {
            OnMatchingEvent((AutomationElement)sender);
        }
    }

    /// <summary>
    /// Summary description for EventTests
    /// </summary>
    [TestClass]
    public class EventTests
    {
        public EventTests()
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
        public void TestFocusChange()
        {
            // Launch a notepad and set focus to it
            using (AppHost host1 = new AppHost("notepad.exe", ""))
            {
                host1.Element.SetFocus();

                FocusChangeHandler handler = new FocusChangeHandler();
                Automation.AddAutomationFocusChangedEventHandler(
                    new AutomationFocusChangedEventHandler(handler.HandleEvent));
                handler.Start();

                // Launch a new notepad and set focus to it
                using (AppHost host2 = new AppHost("notepad.exe", ""))
                {
                    host2.Element.SetFocus();

                    Assert.IsTrue(handler.Confirm());
                    Assert.IsNotNull(handler.EventSource);
                }

                Automation.RemoveAutomationFocusChangedEventHandler(
                    new AutomationFocusChangedEventHandler(handler.HandleEvent));
            }
        }

        [TestMethod]
        [Ignore] 
        // Test is not working on Windows 8 due to the Start Button being removed
        public void TestInvokeEvent()
        {
            AutomationElement startButton = AutomationElementTest.GetStartButton();
            BasicChangeHandler handler = new BasicChangeHandler();
            Automation.AddAutomationEventHandler(
                InvokePattern.InvokedEvent,
                startButton,
                TreeScope.Element,
                new AutomationEventHandler(handler.HandleEvent));
            handler.Start();
            InvokePattern invoke = (InvokePattern)startButton.GetCurrentPattern(InvokePattern.Pattern);
            invoke.Invoke();
            System.Windows.Forms.SendKeys.SendWait("{ESC}");
            Assert.IsTrue(handler.Confirm());
            Assert.IsNotNull(handler.EventSource);
            Automation.RemoveAutomationEventHandler(
                InvokePattern.InvokedEvent,
                startButton,
                new AutomationEventHandler(handler.HandleEvent));
        }

        [TestMethod]
        public void TestPropChangeEvent()
        {
            using (AppHost host = new AppHost("notepad.exe", ""))
            {
                TransformPattern transformPattern = (TransformPattern)host.Element.GetCurrentPattern(TransformPattern.Pattern);

                PropertyChangeHandler handler = new PropertyChangeHandler();
                Automation.AddAutomationPropertyChangedEventHandler(
                    host.Element,
                    TreeScope.Element,
                    new AutomationPropertyChangedEventHandler(handler.HandleEvent),
                    AutomationElement.BoundingRectangleProperty);
                handler.Start();
                System.Threading.Thread.Sleep(100 /* ms */);
                transformPattern.Move(200, 200);
                Assert.IsTrue(handler.Confirm());
                Assert.IsNotNull(handler.EventSource);
                Assert.AreEqual(host.Element, handler.EventSource);
                Automation.RemoveAutomationPropertyChangedEventHandler(
                    host.Element,
                    new AutomationPropertyChangedEventHandler(handler.HandleEvent));
            }
        }

        [TestMethod]
        public void TestStructureChangeEvent()
        {
            StructureChangeHandler handler = new StructureChangeHandler();
            Automation.AddStructureChangedEventHandler(
                AutomationElement.RootElement,
                TreeScope.Subtree,
                new StructureChangedEventHandler(handler.HandleEvent));
            handler.Start();

            // Start Notepad to get a structure change event
            using (AppHost host = new AppHost("notepad.exe", ""))
            {
            }

            Assert.IsTrue(handler.Confirm());
            Assert.IsNotNull(handler.EventSource);
            Automation.RemoveStructureChangedEventHandler(
                AutomationElement.RootElement,
                new StructureChangedEventHandler(handler.HandleEvent));
        }
    }
}
