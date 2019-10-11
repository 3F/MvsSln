using System;
using net.r_eg.MvsSln.Core;
using Xunit;

namespace MvsSlnTest.Core
{
    public class SynchSubscribersTest
    {
        [Fact]
        public void ContainerTest1()
        {
            var data = new SynchSubscribers<IHandler>();
            Assert.Equal(0, data.Count);

            var listener1 = new Listener1();
            var listener2 = new Listener2();

            data.Register(listener1);
            data.Register(listener2);
            Assert.Equal(2, data.Count);

            data.Unregister(listener1);
            Assert.Equal(1, data.Count);

            // existence

            Assert.True(data.Contains(listener2));
            Assert.False(data.Contains(listener1));

            Assert.True(data.Exists(listener2.Id));
            Assert.False(data.Exists(listener1.Id));

            // re-adding + duplicates

            data.Register(listener1);
            data.Register(listener1);
            Assert.Equal(2, data.Count);
            Assert.True(data.Contains(listener1));
            Assert.True(data.Exists(listener1.Id));

            // access by id

            Assert.Equal(listener1.Id, data.GetById(listener1.Id).Id);
            Assert.Equal(listener2.Id, data.GetById(listener2.Id).Id);

            var nonexistent = "{8A805C3B-9941-4B82-94D0-E641EBA3881B}";
            Assert.Null(data.GetById(new Guid(nonexistent)));

            data.Reset();
            Assert.Equal(0, data.Count);
        }

        [Fact]
        public void ContainerTest2()
        {
            var data = new SynchSubscribers<IHandler>();

            var listener1 = new Listener1();
            var listener2 = new Listener2();

            data.Register(listener1);
            data.Register(listener2);

            var enumerator = data.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal(listener1.Id, enumerator.Current.Id);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(listener2.Id, enumerator.Current.Id);

            data.Unregister(listener1);
            data.Register(listener1);

            enumerator = data.GetEnumerator();

            Assert.True(enumerator.MoveNext());
            Assert.Equal(listener2.Id, enumerator.Current.Id);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(listener1.Id, enumerator.Current.Id);
        }

        [Fact]
        public void ContainerTest3()
        {
            var data = new SynchSubscribers<IHandler>();

            var listener1 = new Listener1();
            var listener2 = new Listener2();

            data.Register(listener1);
            data.Register(listener2);

            Assert.Equal(listener1.Id, data[0].Id);
            Assert.Equal(listener2.Id, data[listener2.Id].Id);
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
