using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using net.r_eg.MvsSln.Core;

namespace net.r_eg.MvsSlnTest.Core
{
    [TestClass]
    public class SynchSubscribersTest
    {
        [TestMethod]
        public void ContainerTest1()
        {
            var data = new SynchSubscribers<IHandler>();
            Assert.AreEqual(0, data.Count);

            var listener1 = new Listener1();
            var listener2 = new Listener2();

            data.register(listener1);
            data.register(listener2);
            Assert.AreEqual(2, data.Count);

            data.unregister(listener1);
            Assert.AreEqual(1, data.Count);

            // existence

            Assert.AreEqual(true, data.contains(listener2));
            Assert.AreEqual(false, data.contains(listener1));

            Assert.AreEqual(true, data.exists(listener2.Id));
            Assert.AreEqual(false, data.exists(listener1.Id));

            // re-adding + duplicates

            data.register(listener1);
            data.register(listener1);
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(true, data.contains(listener1));
            Assert.AreEqual(true, data.exists(listener1.Id));

            // access by id

            Assert.AreEqual(listener1.Id, data.getById(listener1.Id).Id);
            Assert.AreEqual(listener2.Id, data.getById(listener2.Id).Id);

            var nonexistent = "{8A805C3B-9941-4B82-94D0-E641EBA3881B}";
            Assert.AreEqual(null, data.getById(new Guid(nonexistent)));

            data.reset();
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void ContainerTest2()
        {
            var data = new SynchSubscribers<IHandler>();

            var listener1 = new Listener1();
            var listener2 = new Listener2();

            data.register(listener1);
            data.register(listener2);

            var enumerator = data.GetEnumerator();

            Assert.AreEqual(true, enumerator.MoveNext());
            Assert.AreEqual(listener1.Id, enumerator.Current.Id);
            Assert.AreEqual(true, enumerator.MoveNext());
            Assert.AreEqual(listener2.Id, enumerator.Current.Id);

            data.unregister(listener1);
            data.register(listener1);

            enumerator = data.GetEnumerator();

            Assert.AreEqual(true, enumerator.MoveNext());
            Assert.AreEqual(listener2.Id, enumerator.Current.Id);
            Assert.AreEqual(true, enumerator.MoveNext());
            Assert.AreEqual(listener1.Id, enumerator.Current.Id);
        }

        [TestMethod]
        public void ContainerTest3()
        {
            var data = new SynchSubscribers<IHandler>();

            var listener1 = new Listener1();
            var listener2 = new Listener2();

            data.register(listener1);
            data.register(listener2);

            Assert.AreEqual(listener1.Id, data[0].Id);
            Assert.AreEqual(listener2.Id, data[listener2.Id].Id);
        }

        private class Listener1: IHandler
        {
            public Guid Id
            {
                get;
                private set;
            } = new Guid("{E2201272-33E1-4336-9BAE-0C77EF90B66C}");
        }

        private class Listener2: IHandler
        {
            public Guid Id
            {
                get;
                private set;
            } = new Guid("{E361DC9E-E45B-45CC-9800-CE80B6C16BDB}");
        }
    }
}
