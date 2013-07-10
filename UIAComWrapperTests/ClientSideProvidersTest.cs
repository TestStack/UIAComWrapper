// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.


using System;
using System.Windows.Automation;
using System.Windows.Automation.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UIAComWrapperTests
{
    [System.Runtime.InteropServices.ComVisible(true)]
    public class SampleButtonProvider : IRawElementProviderSimple
    {
        private IntPtr _hwnd;
        static public readonly string ButtonName = "TestButton";

        public SampleButtonProvider(IntPtr hwnd)
        {
            _hwnd = hwnd;
        }

        #region IRawElementProviderSimple Members

        public UIAutomationClient.ProviderOptions ProviderOptions
        {
            get
            {
                return UIAutomationClient.ProviderOptions.ProviderOptions_ClientSideProvider;
            }
        }

        public object GetPatternProvider(int patternId)
        {
            return null;
        }

        public object GetPropertyValue(int propertyId)
        {
            if (propertyId == AutomationElement.NameProperty.Id)
            {
                return ButtonName;
            }
            return null;
        }

        public IRawElementProviderSimple HostRawElementProvider
        {
            get
            {
                return AutomationInteropProvider.HostProviderFromHandle(this._hwnd);
            }
        }

        #endregion

        public static IRawElementProviderSimple ButtonFactory(IntPtr hwnd, int idChild, int idObject)
        {
            return new SampleButtonProvider(hwnd);
        }
    }

    /// <summary>
    /// Summary description for ClientSideProvidersTest
    /// </summary>
    [TestClass]
    public class ClientSideProvidersTest
    {
        public ClientSideProvidersTest()
        {
        }

        private TestContext testContextInstance;
        private IntPtr startButtonHwnd;

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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            // Find the Start button, which will be our target
            AutomationElement trueStartButton = AutomationElementTest.GetTaskbar();
            this.startButtonHwnd = (IntPtr)trueStartButton.Current.NativeWindowHandle;
        }
        
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestButtonClientSideProvider()
        {
            // Create the provider
            ClientSideProviderDescription provider = new ClientSideProviderDescription(
                new ClientSideProviderFactoryCallback(SampleButtonProvider.ButtonFactory), "Shell_TrayWnd");
            ClientSideProviderDescription[] providers = new ClientSideProviderDescription[1] { provider };

            try
            {
                // Register it
                ClientSettings.RegisterClientSideProviders(providers);

                // Get the overridden element
                AutomationElement startButton = AutomationElement.FromHandle(this.startButtonHwnd);

                // Validate that it is ours
                Assert.AreEqual(SampleButtonProvider.ButtonName, startButton.Current.Name);

                // Unregister it
                ClientSettings.RegisterClientSideProviders(new ClientSideProviderDescription[0]);

                // Get the overridden element
                startButton = AutomationElement.FromHandle(this.startButtonHwnd);

                // Validate that it is not ours
                Assert.AreNotEqual(SampleButtonProvider.ButtonName, startButton.Current.Name);
            }
            finally
            {
                // Restore the status quo ante
                ClientSettings.RegisterClientSideProviders(new ClientSideProviderDescription[0]);
            }
        }

        [TestMethod]
        public void TestPartialNameMatch()
        {
            // Create the provider
            ClientSideProviderDescription provider = new ClientSideProviderDescription(
                new ClientSideProviderFactoryCallback(
                    SampleButtonProvider.ButtonFactory),
                    "Shell_",
                    null /* image name */,
                    ClientSideProviderMatchIndicator.AllowSubstringMatch);
            ClientSideProviderDescription[] providers = new ClientSideProviderDescription[1] { provider };

            try
            {
                // Register it
                ClientSettings.RegisterClientSideProviders(providers);

                // Get the overridden element
                AutomationElement startButton = AutomationElement.FromHandle(this.startButtonHwnd);

                // Validate that it is ours
                Assert.AreEqual(SampleButtonProvider.ButtonName, startButton.Current.Name);
            }
            finally
            {
                // Restore the status quo ante
                ClientSettings.RegisterClientSideProviders(new ClientSideProviderDescription[0]);
            }
        }

        [TestMethod]
        public void TestPartialMatchNotPermitted()
        {
            // Create the provider
            ClientSideProviderDescription provider = new ClientSideProviderDescription(
                new ClientSideProviderFactoryCallback(
                    SampleButtonProvider.ButtonFactory),
                    "Shell_",
                    null /* image name */,
                    ClientSideProviderMatchIndicator.None);
            ClientSideProviderDescription[] providers = new ClientSideProviderDescription[1] { provider };

            try
            {
                // Register it
                ClientSettings.RegisterClientSideProviders(providers);

                // Get the overridden element
                AutomationElement startButton = AutomationElement.FromHandle(this.startButtonHwnd);

                // Validate that it is not ours
                Assert.AreNotEqual(SampleButtonProvider.ButtonName, startButton.Current.Name);
            }
            finally
            {
                // Restore the status quo ante
                ClientSettings.RegisterClientSideProviders(new ClientSideProviderDescription[0]);
            }
        }

        [TestMethod]
        public void TestImageMatch()
        {
            // Create the provider
            ClientSideProviderDescription provider = new ClientSideProviderDescription(
                new ClientSideProviderFactoryCallback(
                    SampleButtonProvider.ButtonFactory),
                    "Shell_TrayWnd",
                    "EXPLORER.EXE",
                    ClientSideProviderMatchIndicator.None);
            ClientSideProviderDescription[] providers = new ClientSideProviderDescription[1] { provider };

            try
            {
                // Register it
                ClientSettings.RegisterClientSideProviders(providers);

                // Get the overridden element
                AutomationElement startButton = AutomationElement.FromHandle(this.startButtonHwnd);

                // Validate that it is ours
                Assert.AreEqual(SampleButtonProvider.ButtonName, startButton.Current.Name);
            }
            finally
            {
                // Restore the status quo ante
                ClientSettings.RegisterClientSideProviders(new ClientSideProviderDescription[0]);
            }
        }


        [TestMethod]
        public void TestImageMismatch()
        {
            // Create the provider
            ClientSideProviderDescription provider = new ClientSideProviderDescription(
                new ClientSideProviderFactoryCallback(
                    SampleButtonProvider.ButtonFactory),
                    "Shell_TrayWnd",
                    "NOTEPAD.EXE",
                    ClientSideProviderMatchIndicator.None);
            ClientSideProviderDescription[] providers = new ClientSideProviderDescription[1] { provider };

            try
            {
                // Register it
                ClientSettings.RegisterClientSideProviders(providers);

                // Get the overridden element
                AutomationElement startButton = AutomationElement.FromHandle(this.startButtonHwnd);

                // Validate that it is not ours
                Assert.AreNotEqual(SampleButtonProvider.ButtonName, startButton.Current.Name);
            }
            finally
            {
                // Restore the status quo ante
                ClientSettings.RegisterClientSideProviders(new ClientSideProviderDescription[0]);
            }
        }
    }
}
