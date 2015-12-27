using System.Windows.Automation;
using NUnit.Framework;

namespace UIAComWrapperTests
{
    /// <summary>
    ///This is a test class for Conditions and is intended
    ///to contain all Conditions Unit Tests
    ///</summary>
    [TestFixture]
    public class ConditionTests
    {
        /// <summary>
        ///A test for AndCondition
        ///</summary>
        [Test]
        public void AndConditionTest()
        {
            // Positive test
            Condition condition = Condition.TrueCondition;
            Condition condition2 = Condition.FalseCondition;
            AndCondition target = new AndCondition(condition, condition2);
            Condition[] actual;
            actual = target.GetConditions();
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Length, 2);

            // Negative test - include a null
            try
            {
                target = new AndCondition(condition, null);

                Assert.Fail("expected exception");
            }
            catch (System.ArgumentException)
            {
            }
        }

        /// <summary>
        ///A test for GetConditions
        ///</summary>
        [Test]
        public void OrConditionTest()
        {
            Condition condition = Condition.TrueCondition;
            Condition condition2 = OrCondition.FalseCondition;
            OrCondition target = new OrCondition(condition, condition2);
            Condition[] actual;
            actual = target.GetConditions();
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Length, 2);

            // Negative test - include a null
            try
            {
                target = new OrCondition(condition, null);

                Assert.Fail("expected exception");
            }
            catch (System.ArgumentException)
            {
            }
        }

        /// <summary>
        ///A test for NotCondition
        ///</summary>
        [Test]
        public void NotConditionTest()
        {
            Condition condition = Condition.TrueCondition;
            NotCondition target = new NotCondition(condition);
            Assert.IsNotNull(target);
            Condition child = target.Condition;
            Assert.IsNotNull(child);
        }

        /// <summary>
        ///A test for PropertyCondition
        ///</summary>
        [Test]
        public void PropertyConditionTest()
        {
            PropertyCondition cond1 = new PropertyCondition(
                AutomationElement.NameProperty, 
                "foo");
            Assert.IsNotNull(cond1);
            Assert.AreEqual(cond1.Value, "foo");
            Assert.AreEqual(cond1.Property.ProgrammaticName, "AutomationElementIdentifiers.NameProperty");

            System.Windows.Rect rect = new System.Windows.Rect(0, 0, 20, 20);
            PropertyCondition cond2 = new PropertyCondition(
                AutomationElement.BoundingRectangleProperty,
                rect);
            Assert.IsNotNull(cond2);
            object value = cond2.Value;
            Assert.IsInstanceOf<double[]>(value);
            Assert.AreEqual(((double[])value).Length, 4);
            Assert.AreEqual(cond2.Property.ProgrammaticName, "AutomationElementIdentifiers.BoundingRectangleProperty");

            PropertyCondition cond3 = new PropertyCondition(
                AutomationElement.ClickablePointProperty,
                new System.Windows.Point(0, 0));
            Assert.IsNotNull(cond3);
            value = cond3.Value;
            Assert.IsInstanceOf<double[]>(value);
            Assert.AreEqual(((double[])value).Length, 2);
            Assert.AreEqual(cond3.Property.ProgrammaticName, "AutomationElementIdentifiers.ClickablePointProperty");

            // Negative case
            try
            {
                PropertyCondition cond4 = new PropertyCondition(
                    null, null);

                Assert.Fail("expected exception");
            }
            catch (System.ArgumentException)
            {
            }



        }
    }
}
