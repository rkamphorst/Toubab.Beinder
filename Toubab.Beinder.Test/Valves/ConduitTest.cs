namespace Toubab.Beinder.Valves
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Bindables;
    using Mocks;
    using Paths;


    [TestFixture]
    public class ConduitTest
    {
        [Test, TestCaseSource("CreateWithParametersTestCase")]
        public void CreateWithParameters(Path basePath, Fragment mockSyllables, Path expectAbsolutePath, int family, int expectFamily, int generation, int expectGeneration)
        {
            var mockBindable = new MockBindable(mockSyllables);
            var obj = new object();

            var conduit = Conduit.Create(mockBindable, obj, basePath, family, generation);

            Assert.IsFalse(ReferenceEquals(mockBindable, conduit.Bindable));
            Assert.IsTrue(conduit.Bindable is MockBindable);
            Assert.AreEqual(mockSyllables, conduit.Bindable.NameSyllables);
            Assert.AreEqual(expectFamily, conduit.Family);
            Assert.AreEqual(expectGeneration, conduit.Generation);
            Assert.AreEqual(expectAbsolutePath, conduit.AbsolutePath);
        }

        public IEnumerable<TestCaseData> CreateWithParametersTestCase()
        {
            yield return new TestCaseData(null, new Fragment("a"), new Path(new Fragment("a")), 1, 1, 1, 1);
            yield return new TestCaseData(new Path(new Fragment("a")), new Fragment("b"), new Path(new Path(new Fragment("a")), new Fragment("b")), 2, 2, 3, 3);
            yield return new TestCaseData(new Path(new Fragment("a", "b")), new Fragment("c"), new Path(new Path(new Fragment("a", "b")), new Fragment("c")), -1, -1, -1, -1);
        }




        [Test]
        public void AttachAndDisposeAttachment()
        {
            var mockBindable = new MockBindable(new Fragment("a"));
            var container = new ObjectContainer();
            container.SetObject();
            var conduit = container.CreateConduitWithObject(mockBindable);

            // after creation, conduit should be in detached state
            Assert.IsNull(conduit.Bindable.Object);

            // attach object
            var attachment = conduit.Attach();

            // now, attachment should be non-null, and bindable.object should be set
            Assert.IsNotNull(attachment);
            container.AssertReferenceEqualToObject(conduit.Bindable.Object);

            // act: dispose attachment
            attachment.Dispose();

            // bindable.object should be null again.
            Assert.IsNull(conduit.Bindable.Object);
        }


        [Test]
        public void ObjectIsCollectedIfConduitIsDetached()
        {
            var mockBindable = new MockBindable(new Fragment("a"));
            var container = new ObjectContainer();
            container.SetObject();
            var conduit = container.CreateConduitWithObject(mockBindable);

            // act: first do a gc collect and then attach
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            var attachment = conduit.Attach();

            // assert: attachment should not be null, because obj holds a reference 
            Assert.IsNotNull(attachment);

            // arrange: dispose of attachment, set to null, *and* set obj to null
            attachment.Dispose();
            container.ClearObject();

            // act: do a gc collect again, try to attach
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            attachment = conduit.Attach();

            // now, the attachment should be null because conduit should hold a 
            // *weak* reference
            Assert.IsNull(attachment);
        }

        [Test]
        public void ObjectIsNotCollectedIfConduitIsAttached()
        {
            var mockBindable = new MockBindable(new Fragment("a"));
            var container = new ObjectContainer();
            container.SetObject();
            var conduit = container.CreateConduitWithObject(mockBindable);

            // now, do a forced GC collect

            // act: first do a gc collect and then attach
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            var attachment = conduit.Attach();

            // assert: attachment shoult not be null, because obj holds a reference 
            Assert.IsNotNull(attachment);

            // set obj and attachment to null. note: we don't dispose attachment, just set it to null
            container.ClearObject();

            // act: do a gc collect again, try to attach
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            attachment = conduit.Attach();

            // now, the attachment should also be not null, because the conduit was
            // attached during gc collect
            Assert.IsNotNull(attachment);
        }

        /// <summary>
        /// Class ObjectContainer, needed to make sure no references to _obj appear in
        /// stack frames. A conservative GC will not collect objects that linger in stack frames,
        /// apparently: http://stackoverflow.com/questions/11417283/strange-weakreference-behavior-on-mono
        /// </summary>
        class ObjectContainer
        {
            object _obj;

            public void SetObject()
            {
                _obj = new object();
            }

            public void ClearObject()
            {
                _obj = null;
            }

            public Conduit CreateConduitWithObject(IBindable mockBindable, int family = -1, int generation = -1)
            {
                return Conduit.Create(mockBindable, _obj, null, family, generation);
            }

            public void AssertReferenceEqualToObject(object obj)
            {
                Assert.IsTrue(ReferenceEquals(obj, _obj));
            }
        }


    }
}

