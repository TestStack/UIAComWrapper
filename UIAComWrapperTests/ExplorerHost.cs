// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.


using System;
using System.Windows.Automation;

namespace UIAComWrapperTests
{
    public class ExplorerHost : IDisposable
    {
        private AutomationElement _element;
        private WindowPattern _windowPattern;
        private int _hwnd;

        public ExplorerHost()
        {
            // Start up Explorer and find it
            System.Diagnostics.Process.Start("cmd.exe", "/c start %SystemDrive%\\windows\\system32");

            // Wait briefly
            System.Threading.Thread.Sleep(2000 /* ms */);

            // Find it
            _element = AutomationElement.RootElement.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, "system32"));
            if (_element == null)
            {
                throw new InvalidOperationException();
            }

            _hwnd = _element.Current.NativeWindowHandle;

            _windowPattern = (WindowPattern)_element.GetCurrentPattern(WindowPattern.Pattern);
        }

        ~ExplorerHost()
        {
            if (_windowPattern != null)
            {
                _windowPattern.Close();
                _windowPattern = null;
            }
        }

        public AutomationElement Element
        {
            get
            {
                AutomationElement element = AutomationElement.FromHandle((IntPtr)this._hwnd);
                if (element == null)
                {
                    throw new InvalidOperationException();
                }
                return element;
            }
        }

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {
            if (_windowPattern != null)
            {
                _windowPattern.Close();
                _windowPattern = null;
            }
            _element = null;
            _hwnd = 0;
        }

        #endregion
    }
}
