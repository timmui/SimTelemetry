﻿using System.Linq;
using NUnit.Framework;
using SimTelemetry.Domain;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Logger;
using SimTelemetry.Domain.Memory;

namespace SimTelemetry.Tests.Logger
{
    [TestFixture]
    public class LogGroupTests
    {
        [Test]
        public void Create()
        {
            int counter = 0;
            var source = new MemoryPool("test", MemoryAddress.StaticAbsolute, 0x123456, 0, 0x1234);

            source.Add(new MemoryFieldFunc<int>("testInt", (pool) => counter++));
            source.Add(new MemoryFieldFunc<string>("test", (pool) => "test"));

            var group = new LogGroup(null, "test", source);

            Assert.AreEqual("test", group.Name);
            Assert.AreEqual(2, group.Fields.Count());
            Assert.AreEqual("testInt", group.Fields.FirstOrDefault().Name);
            Assert.AreEqual("test", group.Fields.Skip(1).FirstOrDefault().Name);

            Assert.True(group.Subscribed);

        }

        [Test]
        public void DataIsWrittenIn16MBChunks()
        {
            int dataWrites = 0;
            int timeWrites = 0;
            GlobalEvents.Hook<LogFileWriteAction>((x) =>
                                                      {
                                                          Assert.AreEqual(null, x.File);
                                                          Assert.AreEqual("test", x.Group);
                                                          // Count the write actions););
                                                          if (x.FileType == LogFileType.Data)
                                                              dataWrites++;
                                                          if (x.FileType == LogFileType.Time)
                                                              timeWrites++;
                                                      }, false);

            int counter = 0;
            var source = new MemoryPool("test", MemoryAddress.StaticAbsolute, 0x123456, 0, 0x1234);

            source.Add(new MemoryFieldFunc<int>("testInt", (pool) => counter++));
            source.Add(new MemoryFieldFunc<string>("test", (pool) => "test"));

            var group = new LogGroup(null, "test", source);

            // Fill up  a data file
            for (int i = 0; i < 1441792; i++) // 28 * 1441792 = 38.5MiB
                group.Update(i); // +28 bytes

            Assert.AreEqual(38, dataWrites); // 38*1MiB
            Assert.AreEqual(10, timeWrites);

            group.Close();

            Assert.AreEqual(39, dataWrites); // last 0.5MiB
            Assert.AreEqual(11, timeWrites);
        }
    }
}