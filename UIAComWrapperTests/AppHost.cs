// (c) Copyright Microsoft, 2012.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.


using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace UIAComWrapperTests
{
    public class AppHost : IDisposable
    {
        private Process _process;
        private IntPtr _hwnd;
        private AutomationElement _element;
        private WindowPattern _windowPattern;

        public AppHost(string program, string args)
        {
            // Start up the program and find it
            _process = Process.Start(program, args);
            _hwnd = ActiveWaitForHwnd(_process.Id);
            if (_hwnd == IntPtr.Zero)
            {
                throw new InvalidOperationException("app never stabilized");
            }

            // Find it
            _element = AutomationElement.FromHandle(_hwnd);
            if (_element == null)
            {
                throw new InvalidOperationException();
            }

            _windowPattern = (WindowPattern)_element.GetCurrentPattern(WindowPattern.Pattern);
        }

        public AutomationElement Element
        {
            get
            {
                return this._element;
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow( IntPtr hwnd, int dir );

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId( IntPtr hwnd, out int pid );

        [DllImport("user32.dll")]
        private static extern IntPtr PostMessage( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam );

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam );

        [DllImport("user32.dll")]
        private static extern int GetClassName( IntPtr hwnd, System.Text.StringBuilder str, int nMaxCount );

        [DllImport("user32.dll")]
        private static extern int WaitForInputIdle( IntPtr hProcess, int dwMilliseconds );

        [DllImport("user32.dll")]
        private static extern int GetWindowText( IntPtr hwnd, System.Text.StringBuilder str, int nMaxCount );

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible( IntPtr hwnd );


        const int GW_HWNDNEXT = 2;
        const int GW_CHILD = 5;
        const int WM_CLOSE = 0x0010;

        private IntPtr ActiveWaitForHwnd(int pid)
        {
            for( int attempt = 0 ; attempt < 240 ; attempt++ )
            {
                IntPtr hwnd = TryFindWindowNow( pid, null /* className */, null /*windowTitle*/ );
                if( hwnd != IntPtr.Zero )
                {
                    return hwnd;
                }
                System.Threading.Thread.Sleep( 500 );
            }
            return IntPtr.Zero;
        }

        private IntPtr TryFindWindowNow( int pid, string className, string windowTitle )
        {
            // Loop through top-level windows
            IntPtr hwnd = GetDesktopWindow();
            IntPtr hwndChild = GetWindow( hwnd, GW_CHILD );
            for( ; hwndChild != IntPtr.Zero ; hwndChild = GetWindow( hwndChild, GW_HWNDNEXT ) )
            {
                // Screen out non-matching PIDs
                if( pid != 0 )
                {
                    int id;
                    GetWindowThreadProcessId( hwndChild, out id );
                    if( id != pid )
                    {
                        continue;
                    }
                }

                // Gotta be visible
                if( ! IsWindowVisible( hwndChild ) )
                {
                    continue;
                }

                // No consoles need apply
                System.Text.StringBuilder realClassName = new System.Text.StringBuilder( 64 );
                GetClassName(hwndChild, realClassName, 64);
                if (String.Compare(realClassName.ToString(), "ConsoleWindowClass", true) == 0)
                {
                    continue;
                }

                // Check classname, if requested
                if( className != null )
                {
                    if (String.Compare(className, realClassName.ToString(), true, CultureInfo.InvariantCulture) != 0)
                    {
                        continue;
                    }
                }

                // Check title, if requested
                if( windowTitle != null )
                {
                    System.Text.StringBuilder testWindowTitle = new System.Text.StringBuilder( 64 );
                    GetWindowText( hwndChild, testWindowTitle, 64 );
                    if (String.Compare(windowTitle, testWindowTitle.ToString(), true, CultureInfo.InvariantCulture) != 0)
                    {
                        continue;
                    }
                }

                // We have a match!
                return hwndChild;
            }
            return IntPtr.Zero;
        }

        #region IDisposable Members

        void System.IDisposable.Dispose()
        {
            if (_windowPattern != null)
            {
                _windowPattern.Close();
                _windowPattern = null;
            }
            if (_process != null)
            {
                _process.WaitForExit();
                _process = null;
            }
        }

        #endregion
    }
}
