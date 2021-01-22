using System;
using System.Windows.Automation;
using System.Windows.Automation.Providers;
using NUnit.Framework;
using Interop.UIAutomationClient;
using UIAutomationClient = Interop.UIAutomationClient;

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
    [TestFixture]
    public class ClientSideProvidersTest
    {
        private IntPtr startButtonHwnd;

        [SetUp]
        public void MyTestInitialize()
        {
            // Find the Start button, which will be our target
            AutomationElement trueStartButton = AutomationElementTest.GetTaskbar();
            this.startButtonHwnd = (IntPtr)trueStartButton.Current.NativeWindowHandle;
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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
    }
}
