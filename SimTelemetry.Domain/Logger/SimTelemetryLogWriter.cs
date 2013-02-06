﻿using System;
using System.Collections.Generic;
using SimTelemetry.Domain.Events;
using SimTelemetry.Domain.Memory;
using Triton.Maths;

namespace SimTelemetry.Domain.Logger
{
    public class SimTelemetryLogWriter : ITelemetryLogger
    {
        public Aggregates.Telemetry Telemetry { get; protected set; }
        protected MemoryProvider Memory;

        private LogFile _log;

        private int Samples = 0;
        private int lastSize = 0;
        private int sizePerSec = 0;
        private float lt = 0.0f;
        private Filter averageTime = new Filter(100);
        private float LastTime = 0;
        public void Update()
        {
            if (_log == null) return;
            // Log all interesting variables to disk.
            // Log the following instances:
            // Session, Simulator + All drivers

            // Get time first
            var SampleTime = Memory.Get("Session").ReadAs<float>("Time");
            if (SampleTime < LastTime) LastTime = 0;
            if (SampleTime - LastTime < 1.0f / 50.0f)
                return;
            LastTime = SampleTime;
            Samples++;

            //Console.Clear();))
            foreach(var pool in Memory.Pools)
            {
                if (pool.IsTemplate) continue;
                foreach(var fields in pool.Fields)
                {
                    if (fields.Value.IsConstant) continue;
                    if (fields.Value.HasChanged())
                    {
                        //Console.WriteLine(fields.Value.GetType().Name + " <> " + pool.Name + "." + fields.Value.Name);
                        _log.Write(pool.Name, fields.Value.Name, MemoryDataConverter.Rawify(fields.Value.Read));
                    }
                }

            }
            if (Samples % 50 == 0)
            {
                 sizePerSec = _log.dataSize - lastSize;
                lastSize = _log.dataSize;

            }

            _log.Flush(SampleTime);

            float ft = Memory.Get("Session").ReadAs<float>("Time");
            if(lt != 0.0 && ft-lt != 0.0)
            averageTime.Add(ft - lt);
            Console.WriteLine((ft-lt).ToString("00.000") + ","+ averageTime.Average.ToString("0.00000") + " - " + Math.Round(_log.dataSize/1024.0/1024.0,3)+"MB (" + Math.Round(sizePerSec/1024.0,3)+"kB/s - " + Math.Round(sizePerSec*3600/1024.0/1024,3)+"MB 1 hour)");
            lt = ft;
        }

        public void Initialize(Aggregates.Telemetry telemetry, MemoryProvider memory)
        {
            Telemetry = telemetry;
            Memory = memory;

            GlobalEvents.Hook<DrivingStarted>(Handle_StartDriving, true);
            GlobalEvents.Hook<DrivingStopped>(Handle_StopDriving, true);
            GlobalEvents.Hook<SessionStarted>(Handle_StartLogfile, true);
            GlobalEvents.Hook<SessionStopped>(Handle_StopLogfile, true);
            GlobalEvents.Hook<LoadingStarted>((x) => Console.WriteLine("loading started [" + Samples + "]"), true);
            GlobalEvents.Hook<LoadingFinished>((x) => Console.WriteLine("loading finished [" + Samples + "]"), true);
        }

        private void Handle_StopLogfile(SessionStopped obj)
        {
            Console.WriteLine("session stopped [" + Samples + "]");
            _log.Finish();
            _log = null;

        }

        protected void CreateLogStructure(IEnumerable<MemoryPool> pools, ILogNode node)
        {
            foreach(var pool in pools)
            {
                if (pool.IsTemplate) continue;

                var group = node.CreateGroup(pool.Name);
                foreach (var field in pool.Fields)
                {
                    group.CreateField(field.Value.Name, field.Value.ValueType);
                }
                if(pool.Pools.Count > 0)
                    CreateLogStructure(pool.Pools.Values, group);
            }
        }

        protected void Handle_StartLogfile(SessionStarted e)
        {
            Console.WriteLine("session started [" + Samples + "]");
            _log = new LogFile();
            CreateLogStructure(Memory.Pools, _log);
        }

        protected void Handle_StartDriving(DrivingStarted e)
        {
            Console.WriteLine("Starting log [" + Samples + "]");
        }

        protected void Handle_StopDriving(DrivingStopped e)
        {

            Console.WriteLine("Stopping log [" + Samples + "]");
        }

        public void Deinitialize()
        {

        }
    }
}
