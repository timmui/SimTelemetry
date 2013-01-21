﻿using System;
using NUnit.Framework;
using SimTelemetry.Domain;

namespace SimTelemetry.Tests.Core
{
#if DEBUG
    public class TestEvent1Handler
    {
        public string Message1 { get; set; }
        public TestEvent1Handler(string message)
        {
            Message1 = message;
        }
    }

    public class TestEvent2Handler
    {
        public string Message2 { get; set; }
        public TestEvent2Handler(string message)
        {
            Message2 = message;
        }
    }

    [TestFixture]
    class EventTests : IDisposable
    {
        public void Initialize()
        {
            GlobalEvents.Reset();
            //
            _event1HandleCount = 0;
            _event2HandleCount = 0;
        }

        public void Dispose()
        {
            GlobalEvents.Reset();
            Assert.AreEqual(0, GlobalEvents.Count);
        }

        private int _event1HandleCount;
        private int _event2HandleCount;

        public void handleEvent1(TestEvent1Handler data)
        {
            Assert.AreEqual(data.Message1, "Hello event 1");
            _event1HandleCount++;
        }

        public void handleEvent2(TestEvent2Handler data)
        {
            Assert.AreEqual(data.Message2, "Hello event 2");
            _event2HandleCount++;
        }
        
        [Test]
        public void Events()
        {
            Initialize();
            Assert.AreEqual(0, GlobalEvents.Count);

            // Add two simple events:
            GlobalEvents.Hook<TestEvent1Handler>(handleEvent1, true);
            GlobalEvents.Hook<TestEvent2Handler>(handleEvent2, false);

            Assert.AreEqual(2, GlobalEvents.Count);

            // Cannot handle 1 event twice, even though network listening is different:7
            GlobalEvents.Hook<TestEvent2Handler>(handleEvent2, true);
            Assert.AreEqual(2, GlobalEvents.Count);

            // Fire event 1 for network:
            GlobalEvents.Fire(new TestEvent1Handler("Hello event 1"), true);
            Assert.AreEqual(1, _event1HandleCount);

            // Fire event 2 for network:
            GlobalEvents.Fire(new TestEvent2Handler("Hello event 2"), true);
            Assert.AreEqual(1, _event2HandleCount);

            // Fire event 1 for local:
            GlobalEvents.Fire(new TestEvent1Handler("Hello event 1"), false);
            Assert.AreEqual(1, _event1HandleCount);

            // Fire event 2 for local:
            GlobalEvents.Fire(new TestEvent2Handler("Hello event 2"), false);
            Assert.AreEqual(2, _event2HandleCount);

            // Unhook
            GlobalEvents.Unhook<TestEvent1Handler>(handleEvent1);
            GlobalEvents.Unhook<TestEvent2Handler>(handleEvent2);

            // Count
            Assert.AreEqual(0, GlobalEvents.Count);

            // Fire event 1 for network, must remain unchanged
            GlobalEvents.Fire(new TestEvent1Handler("Hello event 1"), true);
            Assert.AreEqual(1, _event1HandleCount);

            // Fire event 2 for network, must remain unchanged
            GlobalEvents.Fire(new TestEvent2Handler("Hello event 2"), true);
            Assert.AreEqual(2, _event2HandleCount);
        }
    }
#endif
}
