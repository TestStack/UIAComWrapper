using System.Windows.Automation;
using NUnit.Framework;

namespace UIAComWrapperTests
{
    
    
    /// <summary>
    ///This is a test class for CacheRequestTest and is intended
    ///to contain all CacheRequestTest Unit Tests
    ///</summary>
    [TestFixture]
    public class CacheRequestTest
    {
        /// <summary>
        ///A test for TreeScope
        ///</summary>
        [Test]
        public void TreeScopeTest()
        {
            CacheRequest target = new CacheRequest();
            TreeScope expected = TreeScope.Subtree;
            TreeScope actual;
            target.TreeScope = expected;
            actual = target.TreeScope;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TreeFilter
        ///</summary>
        [Test]
        public void TreeFilterTest()
        {
            CacheRequest target = new CacheRequest();
            PropertyCondition expected = new PropertyCondition(AutomationElement.NameProperty, "foo");
            PropertyCondition actual;
            target.TreeFilter = expected;
            actual = (PropertyCondition)target.TreeFilter;
            Assert.AreEqual(expected.Flags, actual.Flags);
            Assert.AreEqual(expected.Property, actual.Property);
            Assert.AreEqual(expected.Value, actual.Value);
        }

        /// <summary>
        ///A test for Current
        ///</summary>
        [Test]
        public void CurrentTest()
        {
            // We expect the Current one at this point to be the Default one
            CacheRequest actual;
            actual = CacheRequest.Current;
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.AutomationElementMode, AutomationElementMode.Full);
            Assert.AreEqual(actual.TreeScope, TreeScope.Element);
            Assert.IsNotNull(actual.TreeFilter);

            Assert.IsTrue(actual.TreeFilter is NotCondition);
            NotCondition notCond = (NotCondition)actual.TreeFilter;
            Assert.IsTrue(notCond.Condition is PropertyCondition);
            PropertyCondition propCond = (PropertyCondition)notCond.Condition;
            Assert.AreEqual(propCond.Property, AutomationElement.IsControlElementProperty);
            Assert.AreEqual(propCond.Value, false);
        }

        /// <summary>
        ///A test for AutomationElementMode
        ///</summary>
        [Test]
        public void AutomationElementModeTest()
        {
            CacheRequest target = new CacheRequest(); 
            target.AutomationElementMode = AutomationElementMode.Full;
            AutomationElementMode actual = target.AutomationElementMode;
            Assert.AreEqual(AutomationElementMode.Full, actual);
        }

        /// <summary>
        ///A test for Push and Pop
        ///</summary>
        [Test]
        public void PushPopTest()
        {
            CacheRequest defaultCR = CacheRequest.Current;
            CacheRequest target = new CacheRequest();
            target.TreeScope = TreeScope.Children;
            target.Push();
            CacheRequest target2 = new CacheRequest();
            target2.TreeScope = TreeScope.Subtree;
            target2.Push();

            // Try to change target2 - this should fail
            try
            {
                target2.TreeScope = TreeScope.Descendants;

                Assert.Fail("exception expected");
            }
            catch (System.InvalidOperationException)
            {
            }

            target2.Pop();
            target.Pop();
            Assert.AreEqual(CacheRequest.Current, defaultCR);
        }

        /// <summary>
        ///A test for Clone
        ///</summary>
        [Test]
        public void CloneTest()
        {
            CacheRequest target = new CacheRequest();
            target.TreeScope = TreeScope.Subtree;
            CacheRequest actual;
            actual = target.Clone();
            Assert.AreEqual(target.TreeScope, actual.TreeScope);
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [Test]
        public void AddTest()
        {
            CacheRequest target = new CacheRequest(); 
            target.Add(AutomationElement.HelpTextProperty);
            target.Add(ExpandCollapsePatternIdentifiers.Pattern);
        }

        /// <summary>
        ///A test for Activate
        ///</summary>
        [Test]
        public void ActivateTest()
        {
            CacheRequest target = new CacheRequest();
            Assert.AreNotEqual(CacheRequest.Current, target);
            using (target.Activate())
            {
                Assert.AreEqual(CacheRequest.Current, target);
                CacheRequest target2 = new CacheRequest();
                using (target2.Activate())
                {
                    Assert.AreNotEqual(CacheRequest.Current, target);
                }
            }
        }
    }
}
