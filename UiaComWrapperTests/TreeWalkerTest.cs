using System.Windows.Automation;
using NUnit.Framework;

namespace UIAComWrapperTests
{
    /// <summary>
    /// Summary description for TreeWalker
    /// </summary>
    [TestFixture]
    public class TreeWalkerTest
    {
        [Test]
        public void PreDefinedConditionsTest()
        {
            Condition rawView = Automation.RawViewCondition;
            Assert.IsNotNull(rawView);

            Condition controlView = Automation.ControlViewCondition;
            Assert.IsInstanceOf<NotCondition>(controlView);
            NotCondition notCond = (NotCondition)controlView;
            Assert.IsInstanceOf<PropertyCondition>(notCond.Condition);

            Condition contentView = Automation.ContentViewCondition;
            Assert.IsInstanceOf<NotCondition>(contentView);
            NotCondition notCond2 = (NotCondition)contentView;
            Assert.IsInstanceOf<OrCondition>(notCond2.Condition);
        }

        //
        // TreeIterationTest moved to ExplorerTargetTests.
        //
    }
}
