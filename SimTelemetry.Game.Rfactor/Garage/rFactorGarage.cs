﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Game.Rfactor.Garage
{
    public class rFactorGarage : IGarage
    {
        private List<IMod> _mods;
        private List<ITrack> _tracks;

        public string GamedataDirectory
        {
            get { return InstallationDirectory + "GameData\\"; }
        }

        public string InstallationDirectory
        {
            get
            {
                string program_files = Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles);
                if (Directory.Exists(program_files + "\\rfactor\\"))
                    return program_files + "\\rfactor\\";

                program_files = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                if (Directory.Exists(program_files + "\\rfactor\\"))
                    return program_files + "\\rfactor\\";

                return "";

            }
        }

        public string Simulator { get { return "rFactor"; } }
        public bool Available  { get { return true; } }
        public bool Available_Tracks { get { return true; } }
        public bool Available_Mods { get { return true; }  }
        public List<IMod> Mods { get { return _mods; } }
        public List<ITrack> Tracks {  get { return _tracks; } }

        public void Scan()
        {
            ScanTracks();
            ScanCars();
        }

        private void ScanTracks()
        {
            _tracks = new List<ITrack>();
            // rFactor stores data in GDB files.
            // All relevant path data is in stored in AIW files.
            List<string> tracks = GarageTools.SearchFiles(GamedataDirectory, "*.gdb");
            int count = 0;
            foreach(string track in tracks)
            {
                // If AIW file exists
                if(File.Exists(track.Replace(".gdb",".aiw")))
                {
                    _tracks.Add(new rFactorTrack(track));
                    count++;
                }
            }

            Debug.WriteLine(count + " track(s) found");
        }

        private void ScanCars()
        {
            _mods = new List<IMod>();

            // rFactor stores data in GDB files.
            // All relevant path data is in stored in AIW files.
            List<string> vehicles = GarageTools.SearchFiles(InstallationDirectory + "rFM\\", "*.rfm");
            int count = 0;
            foreach (string mod in vehicles)
            {
                if (mod.Contains("ANY_DEV_ONLY")==false)
                {
                    _mods.Add(new rFactorMod(mod));
                    count++;
                }
            }
            Debug.WriteLine(count + " mod(s) found");
        }
    }
}