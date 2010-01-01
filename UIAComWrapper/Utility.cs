// (c) Copyright Michael Bernstein, 2009.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
// All other rights reserved.

using System;
using System.Collections;
using System.Windows.Automation;

namespace UIAComWrapperInternal
{
    class Utility
    {
        private static void CheckNonNull(object el1, object el2)
        {
            if (el1 == null)
            {
                throw new ArgumentNullException("el1");
            }
            if (el2 == null)
            {
                throw new ArgumentNullException("el2");
            }
        }

        internal static ControlType ConvertToControlType(int id)
        {
            return ControlType.LookupById(id);
        }

        internal static int ConvertToInt(bool b)
        {
            return (b) ? 1 : 0;
        }

        internal static System.Windows.Rect ConvertToRect(UIAutomationClient.tagRECT rc)
        {
            return new System.Windows.Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
        }

        internal static AutomationElement[] ConvertToElementArray(UIAutomationClient.IUIAutomationElementArray array)
        {
            AutomationElement[] elementArray;
            if (array != null)
            {
                elementArray = new AutomationElement[array.Length];
                for (int i = 0; i < array.Length; i++)
                {
                    elementArray[i] = AutomationElement.Wrap(array.GetElement(i));
                }
            }
            else
            {
                elementArray = null;
            }
            return elementArray;
        }

        internal static bool ConvertException(System.Runtime.InteropServices.COMException e, out Exception uiaException)
        {
            bool handled = true;
            switch (e.ErrorCode)
            {
                case UiaCoreIds.UIA_E_ELEMENTNOTAVAILABLE:
                    uiaException = new ElementNotAvailableException(e);
                    break;

                case UiaCoreIds.UIA_E_ELEMENTNOTENABLED:
                    uiaException = new ElementNotEnabledException(e);
                    break;

                case UiaCoreIds.UIA_E_NOCLICKABLEPOINT:
                    uiaException = new NoClickablePointException(e);
                    break;

                case UiaCoreIds.UIA_E_PROXYASSEMBLYNOTLOADED:
                    uiaException = new ProxyAssemblyNotLoadedException(e);
                    break;

                default:
                    uiaException = null;
                    handled = false;
                    break;
            }
            return handled;
        }

        internal static Array CombineArrays(IEnumerable arrays, Type t)
        {
            int length = 0;
            foreach (Array array in arrays)
            {
                length += array.Length;
            }
            Array destinationArray = Array.CreateInstance(t, length);
            int destinationIndex = 0;
            foreach (Array array3 in arrays)
            {
                int num3 = array3.Length;
                Array.Copy(array3, 0, destinationArray, destinationIndex, num3);
                destinationIndex += num3;
            }
            return destinationArray;
        }

        internal static bool Compare(AutomationElement el1, AutomationElement el2)
        {
            CheckNonNull(el1, el2);
            return Compare(el1.GetRuntimeId(), el2.GetRuntimeId());
        }

        internal static bool Compare(int[] a1, int[] a2)
        {
            if ((a1 == null) != (a2 == null))
            {
                return false;
            }
            if (a1 == null && a2 == null)
            {
                return true;
            }            
            CheckNonNull(a1, a2);
            int length = a1.Length;
            if (length != a2.Length)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return false;
                }
            }
            return true;
        }

        internal static Array RemoveDuplicates(Array a, Type t)
        {
            if (a.Length == 0)
            {
                return a;
            }
            Array.Sort(a);
            int index = 0;
            for (int i = 1; i < a.Length; i++)
            {
                if (!a.GetValue(i).Equals(a.GetValue(index)))
                {
                    index++;
                    a.SetValue(a.GetValue(i), index);
                }
            }
            int length = index + 1;
            if (length == a.Length)
            {
                return a;
            }
            Array destinationArray = Array.CreateInstance(t, length);
            Array.Copy(a, 0, destinationArray, 0, length);
            return destinationArray;
        }

        internal static UIAutomationClient.tagPOINT PointManagedToNative(System.Windows.Point pt)
        {
            UIAutomationClient.tagPOINT nativePoint = new UIAutomationClient.tagPOINT();
            nativePoint.x = (int)pt.X;
            nativePoint.y = (int)pt.Y;
            return nativePoint;
        }

        internal static void ValidateArgument(bool cond, string reason)
        {
            if (!cond)
            {
                throw new ArgumentException(reason);
            }
        }

        internal static void ValidateArgumentNonNull(object obj, string argName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(argName);
            }
        }

        internal static void ValidateCached(bool cached)
        {
            if (!cached)
            {
                throw new InvalidOperationException("Cache Request Needs Cache");
            }
        }

        internal static object WrapObjectAsPattern(AutomationElement el, object nativePattern, AutomationPattern pattern, bool cached)
        {
            PatternTypeInfo info;
            if (!Schema.GetPatternInfo(pattern, out info))
            {
                throw new ArgumentException("Unsupported pattern");
            }
            if (info.ClientSideWrapper == null)
            {
                return null;
            }
            return info.ClientSideWrapper(el, nativePattern, cached);
        }

        internal static object WrapObjectAsProperty(AutomationProperty property, object obj)
        {
            PropertyTypeInfo info;

            // Handle the cases that we know.
            if (obj == AutomationElement.NotSupported)
            {
                // No-op
            }
            else if (obj is UIAutomationClient.IUIAutomationElement)
            {
                obj = AutomationElement.Wrap((UIAutomationClient.IUIAutomationElement)obj);
            }
            else if (obj is UIAutomationClient.IUIAutomationElementArray)
            {
                obj = Utility.ConvertToElementArray((UIAutomationClient.IUIAutomationElementArray)obj);
            }
            else if (Schema.GetPropertyTypeInfo(property, out info))
            {
                // Well known properties
                if ((obj != null) && (info.ObjectConverter != null))
                {
                    obj = info.ObjectConverter(obj);
                }
            }

            return obj;
        }
    }
}
