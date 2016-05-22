﻿namespace Toubab.Beinder.Valves
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Bindables;
    using Paths;


    [TestFixture]
    public class ConduitTest
    {
        [Test, TestCaseSource("CreateWithParametersTestCase")]
        public void CreateWithParameters(Path basePath, Path mockPath, Path expectAbsolutePath, int tag, int expectTag)
        {
            var mockBindable = new MockBindable(mockPath);
            var obj = new object();

            var conduit = Conduit.Create(mockBindable, obj, basePath, tag);

            Assert.IsFalse(ReferenceEquals(mockBindable, conduit.Bindable));
            Assert.IsTrue(conduit.Bindable is MockBindable);
            Assert.AreEqual(mockPath, conduit.Bindable.Path);
            Assert.AreEqual(expectTag, conduit.Tag);
            Assert.AreEqual(expectAbsolutePath, conduit.AbsolutePath);
        }

        public IEnumerable<TestCaseData> CreateWithParametersTestCase()
        {
            yield return new TestCaseData(null, new Path("a"), new Path("a"), 1, 1);
            yield return new TestCaseData(new Path("a"), new Path("b"), new Path("a", "b"), 2, 2);
            yield return new TestCaseData(new Path("a", "b"), new Path("c"), new Path("a", "b", "c"), -1, -1);
        }

        [Test]
        public void AttachAndDisposeAttachment()
        {
            var mockBindable = new MockBindable(new Path("a"));
            var obj = new object();
            var conduit = Conduit.Create(mockBindable, obj, null, -1);

            // after creation, conduit should be in detached state
            Assert.IsNull(conduit.Bindable.Object);

            // attach object
            var attachment = conduit.Attach();

            // now, attachment should be non-null, and bindable.object should be set
            Assert.IsNotNull(attachment);
            Assert.IsTrue(ReferenceEquals(obj, conduit.Bindable.Object));

            // act: dispose attachment
            attachment.Dispose();

            // bindable.object should be null again.
            Assert.IsNull(conduit.Bindable.Object);
        }


        [Test]
        public void ObjectIsCollectedIfConduitIsDetached()
        {
            var mockBindable = new MockBindable(new Path("a"));
            var obj = new object();
            var conduit = Conduit.Create(mockBindable, obj, null, -1);

            // act: first do a gc collect and then attach
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            var attachment = conduit.Attach();

            // assert: attachment should not be null, because obj holds a reference 
            Assert.IsNotNull(attachment);

            // arrange: dispose of attachment, set to null, *and* set obj to null
            attachment.Dispose();
            attachment = null;
            obj = null;

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
            var obj = new object();
            var conduit = Conduit.Create(mockBindable, obj, null, -1);

            // now, do a forced GC collect

            // act: first do a gc collect and then attach
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            var attachment = conduit.Attach();

            // assert: attachment shoult not be null, because obj holds a reference 
            Assert.IsNotNull(attachment);

            // set obj and attachment to null. note: we don't dispose attachment, just set it to null
            attachment = null;
            obj = null;

            // act: do a gc collect again, try to attach
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            attachment = conduit.Attach();

            // now, the attachment should also be not null, because the conduit was
            // attached during gc collect
            Assert.IsNotNull(attachment);
        }



        class MockBindable : IBindable
        {

            public MockBindable(Path path)
            {
                Path = path;
            }

            #region IMixin implementation

            public void SetObject(object newObject)
            {
                Object = newObject;
            }

            public Toubab.Beinder.Mixins.IMixin CloneWithoutObject()
            {
                return new MockBindable(Path);
            }

            #endregion

            #region IBindable implementation

            public System.Type[] ValueTypes
            {
                get
                {
                    throw new System.NotImplementedException();
                }
            }

            public Toubab.Beinder.Paths.Path Path
            {
                get;
                private set;
            }

            public object Object
            {
                get;
                private set;
            }

            #endregion
        }
    }
}

