using System.Windows.Automation;
using NUnit.Framework;
using UIAComWrapperInternal;

namespace UIAComWrapperTests
{
    public struct ObjectTestMapping
    {
        public AutomationProperty property;
        public object input;
        public object expected;

        public ObjectTestMapping(AutomationProperty property, object input, object expected)
        {
            this.property = property;
            this.input = input;
            this.expected = expected;
        }
    };

    /// <summary>
    /// Tests for the ObjectConverter system that 'polishes' returned objects
    /// into the right types for this API.
    /// </summary>
    [TestFixture]
    public class ObjectConverterTest
    {
        [Test]
        public void TestObjectConverter()
        {
            ObjectTestMapping[] testMap = new ObjectTestMapping[] {
                new ObjectTestMapping(AutomationElement.BoundingRectangleProperty, new double[] {1, 2, 3, 4}, new System.Windows.Rect(1, 2, 3, 4)), 
                new ObjectTestMapping(AutomationElement.ControlTypeProperty, (int)50000, ControlType.Button), 
                new ObjectTestMapping(AutomationElement.ClickablePointProperty, new double[] {1, 2}, new System.Windows.Point(1, 2)), 
                new ObjectTestMapping(AutomationElement.CultureProperty, 0x0409, new System.Globalization.CultureInfo(0x0409)), 
                new ObjectTestMapping(AutomationElement.CultureProperty, 0, System.Globalization.CultureInfo.InvariantCulture),
                new ObjectTestMapping(AutomationElement.LabeledByProperty, null, null), 
                new ObjectTestMapping(AutomationElement.LabeledByProperty, Automation.Factory.GetRootElement(), AutomationElement.RootElement), 
                new ObjectTestMapping(AutomationElement.OrientationProperty, 2, OrientationType.Vertical),
                new ObjectTestMapping(DockPattern.DockPositionProperty, 4, DockPosition.Fill), 
                new ObjectTestMapping(ExpandCollapsePattern.ExpandCollapseStateProperty, 2, ExpandCollapseState.PartiallyExpanded),
                new ObjectTestMapping(WindowPattern.WindowVisualStateProperty, 2, WindowVisualState.Minimized),
                new ObjectTestMapping(WindowPattern.WindowInteractionStateProperty, 1, WindowInteractionState.Closing),
                new ObjectTestMapping(TablePattern.RowOrColumnMajorProperty, 2, RowOrColumnMajor.Indeterminate),
                new ObjectTestMapping(TogglePattern.ToggleStateProperty, 1, ToggleState.On)
            };

            foreach (ObjectTestMapping mapping in testMap)
            {
                PropertyTypeInfo info;
                Schema.GetPropertyTypeInfo(mapping.property, out info);
                object output = mapping.input;
                if (info != null && info.ObjectConverter != null)
                {
                    output = info.ObjectConverter(mapping.input);
               } 
                else
                {
                    output = Utility.WrapObjectAsProperty(mapping.property, mapping.input);
                }
                Assert.IsTrue(output == null || info == null || output.GetType() == info.Type);
                Assert.AreEqual(output, mapping.expected);
            }
        }
    }
}
