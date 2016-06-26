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
        public void CreateWithParameters(Path basePath, Path mockPath, Path expectAbsolutePath, int family, int expectFamily)
        {
            var mockBindable = new MockBindable(mockPath);
            var obj = new object();

            var conduit = Conduit.Create(mockBindable, obj, basePath, family);

            Assert.IsFalse(ReferenceEquals(mockBindable, conduit.Bindable));
            Assert.IsTrue(conduit.Bindable is MockBindable);
            Assert.AreEqual(mockPath, conduit.Bindable.Path);
            Assert.AreEqual(expectFamily, conduit.Family);
            Assert.AreEqual(expectAbsolutePath, conduit.AbsolutePath);
        }

        public IEnumerable<TestCaseData> CreateWithParametersTestCase()
        {
            yield return new TestCaseData(null, new Path("a"), new Path("a"), 1, 1);
            yield return new TestCaseData(new Path("a"), new Path("b"), new Path("a", "b"), 2, 2);
            yield return new TestCaseData(new Path("a", "b"), new Path("c"), new Path("a", "b", "c"), -1, -1);
        }

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

            public Conduit CreateConduitWithObject(IBindable mockBindable, int family = -1)
            {
                return Conduit.Create(mockBindable, _obj, null, family);
            }

            public void AssertReferenceEqualToObject(object obj)
            {
                Assert.IsTrue(ReferenceEquals(obj, _obj));
            }
        }



        [Test]
        public void AttachAndDisposeAttachment()
        {
            var mockBindable = new MockBindable(new Path("a"));
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
            var mockBindable = new MockBindable(new Path("a"));
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
            attachment = null;
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
            var mockBindable = new MockBindable(new Path("a"));
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
            attachment = null;
            container.ClearObject();

            // act: do a gc collect again, try to attach
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            attachment = conduit.Attach();

            // now, the attachment should also be not null, because the conduit was
            // attached during gc collect
            Assert.IsNotNull(attachment);
        }




    }
}

