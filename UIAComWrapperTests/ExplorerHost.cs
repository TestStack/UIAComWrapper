// (c) Copyright Michael Bernstein, 2009.
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
                return this._element;
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
        }

        #endregion
    }
}
