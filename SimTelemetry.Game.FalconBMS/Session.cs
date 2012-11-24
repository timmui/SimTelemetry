﻿using System;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.FalconBMS
{
    public class Session : ISession
    {
        public string GameData_TrackFile
        {
            get { return ""; }
            set { }
        }

        public string GameDirectory
        {
            get { return ""; }
            set { }
        }

        public int RaceLaps
        {
            get { return 0; }
            set { }
        }

        public bool IsRace
        {
            get { return false; }
            set { }
        }

        public string CircuitName
        {
            get { return "Blackwood"; }
            set { }
        }

        public float TrackTemperature
        {
            get { return 0; }
            set { }
        }

        public float AmbientTemperature
        {
            get { return 0; }
            set { }
        }

        public SessionInfo Type
        {
            get { return new SessionInfo(); }
            set { }
        }

        public float Time
        {
            get { return 0; }
            set { }
        }

        public float TimeClock
        {
            get { return 0; }
            set { }
        }

        public int Cars_InPits
        {
            get { return 0; }
            set { }
        }

        public int Cars_OnTrack
        {
            get { return 0; }
            set { }
        }

        public int Cars
        {
            get { return 1; }
            set { }
        }

        public bool Active
        {
            get { return true; }
            set { }
        }

        public bool Flag_YellowFull
        {
            get { return false; }
            set { }
        }

        public bool Flag_Red
        {
            get { return false; }
            set { }
        }

        public bool Flag_Green
        {
            get { return false; }
            set { }
        }

        public bool Flag_Finish
        {
            get { return false; }
            set { }
        }

        public bool IsOffline
        {
            get { return false; }
            set { }
        }
    }
}