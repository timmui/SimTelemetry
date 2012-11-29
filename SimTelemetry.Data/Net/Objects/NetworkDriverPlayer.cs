﻿/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Net.Objects
{
    public class NetworkDriverPlayer : IDriverPlayer
    {
        public double Engine_Lifetime_Live { get; set; }
        public double Engine_Lifetime_Typical { get; set; }
        public double Engine_Lifetime_Variation { get; set; }
        public double Engine_Lifetime_Oil_Base { get; set; }
        public double Engine_Lifetime_RPM_Base { get; set; }
        public double Engine_Temperature_Oil { get; set; }
        public double Engine_Temperature_Water { get; set; }
        public int Engine_BoostSetting { get; set; }
        public int Gear { get; set; }
        public int PitStop_Number { get; set; }
        public int PitStop_FuelStop1 { get; set; }
        public int PitStop_FuelStop2 { get; set; }
        public int PitStop_FuelStop3 { get; set; }
        public double RAM_Fuel { get; set; }
        public double RAM_Torque { get; set; }
        public double RAM_Power { get; set; }
        public double Fuel { get; set; }
        public double Suspension_SpendleTorque_LF { get; set; }
        public double Suspension_SpendleTorque_RF { get; set; }
        public double Suspension_RideHeight_LF_G { get; set; }
        public double Suspension_RideHeight_LR_G { get; set; }
        public double Suspension_RideHeight_RF_G { get; set; }
        public double Suspension_RideHeight_RR_G { get; set; }
        public double Suspension_RideHeight_LF { get; set; }
        public double Suspension_RideHeight_LR { get; set; }
        public double Suspension_RideHeight_RF { get; set; }
        public double Suspension_RideHeight_RR { get; set; }
        public string Tyre_Compound_Front { get; set; }
        public string Tyre_Compound_Rear { get; set; }
        public double Tyre_Grip_Forwards_LF { get; set; }
        public double Weight_Rearwheel { get; set; }
        public double DryWeight { get; set; }
        public double Tyre_Pressure_Optimal_LF { get; set; }
        public double Tyre_Pressure_Optimal_LR { get; set; }
        public double Tyre_Pressure_Optimal_RF { get; set; }
        public double Tyre_Pressure_Optimal_RR { get; set; }
        public double Tyre_Pressure_LF { get; set; }
        public double Tyre_Pressure_LR { get; set; }
        public double Tyre_Pressure_RF { get; set; }
        public double Tyre_Pressure_RR { get; set; }
        public double Tyre_Pressure_LF_G { get; set; }
        public double Tyre_Pressure_LR_G { get; set; }
        public double Tyre_Pressure_RF_G { get; set; }
        public double Tyre_Pressure_RR_G { get; set; }
        public double Tyre_Grip_Sidewards_LF { get; set; }
        public double Tyre_Grip_Sidewards_LR { get; set; }
        public double Tyre_Grip_Sidewards_RF { get; set; }
        public double Tyre_Grip_Sidewards_RR { get; set; }
        public double Tyre_Speed_LF { get; set; }
        public double Tyre_Speed_LR { get; set; }
        public double Tyre_Speed_RF { get; set; }
        public double Tyre_Speed_RR { get; set; }
        public double Tyre_Temperature_LF_Inside { get; set; }
        public double Tyre_Temperature_LR_Inside { get; set; }
        public double Tyre_Temperature_RF_Inside { get; set; }
        public double Tyre_Temperature_RR_Inside { get; set; }
        public double Tyre_Temperature_LF_Middle { get; set; }
        public double Tyre_Temperature_LR_Middle { get; set; }
        public double Tyre_Temperature_RF_Middle { get; set; }
        public double Tyre_Temperature_RR_Middle { get; set; }
        public double Tyre_Temperature_LF_Outside { get; set; }
        public double Tyre_Temperature_LR_Outside { get; set; }
        public double Tyre_Temperature_RF_Outside { get; set; }
        public double Tyre_Temperature_RR_Outside { get; set; }
        public double Tyre_Temperature_LF_Optimal { get; set; }
        public double Tyre_Temperature_LR_Optimal { get; set; }
        public double Tyre_Temperature_RF_Optimal { get; set; }
        public double Tyre_Temperature_RR_Optimal { get; set; }
        public double Tyre_Temperature_LF_Fresh { get; set; }
        public double Tyre_Temperature_LR_Fresh { get; set; }
        public double Tyre_Temperature_RF_Fresh { get; set; }
        public double Tyre_Temperature_RR_Fresh { get; set; }
        public double Wheel_Radius_LF { get; set; }
        public double Wheel_Radius_LR { get; set; }
        public double Wheel_Radius_RF { get; set; }
        public double Wheel_Radius_RR { get; set; }
        public int Aerodynamics_FrontWing_Setting { get; set; }
        public double Aerodynamics_FrontWing_Downforce { get; set; }
        public int Aerodynamics_RearWing_Setting { get; set; }
        public double Aerodynamics_RearWing_Downforce { get; set; }
        public double Aerodynamics_FrontWing_Drag_Total { get; set; }
        public double Aerodynamics_FrontWing_Drag_Base { get; set; }
        public double Aerodynamics_RearWing_Drag_Total { get; set; }
        public double Aerodynamics_RearWing_Drag_Base { get; set; }
        public double Aerodynamics_LeftFender_Drag { get; set; }
        public double Aerodynamics_RightFender_Drag { get; set; }
        public double Aerodynamics_Body_Drag { get; set; }
        public double Aerodynamics_Body_DragHeightDiff { get; set; }
        public double Aerodynamics_Body_DragHeightAvg { get; set; }
        public double Aerodynamics_Radiator_Drag { get; set; }
        public int Aerodynamics_Radiator_Setting { get; set; }
        public double Aerodynamics_BrakeDuct_Drag { get; set; }
        public int Aerodynamics_BrakeDuct_Setting { get; set; }
        public double Engine_RPM { get; set; }
        public double Engine_RPM_Max_Live { get; set; }
        public double Engine_RPM_Max_Scale { get; set; }
        public int Engine_RPM_Max_Step { get; set; }
        public double Engine_RPM_Idle_Min { get; set; }
        public double Engine_RPM_Idle_Max { get; set; }
        public double Engine_RPM_Launch_Min { get; set; }
        public double Engine_RPM_Launch_Max { get; set; }
        public double Engine_Idle_ThrottleGain { get; set; }
        public double Engine_Torque_Negative { get; set; }
        public double Engine_Torque { get; set; }
        public PowerTrainDrivenWheels Powertrain_DrivenWheels { get; set; }
        public double Powertrain_DriverDistribution { get; set; }
        public double Brake_TypicalFailure_LF { get; set; }
        public double Brake_TypicalFailure_LR { get; set; }
        public double Brake_TypicalFailure_RF { get; set; }
        public double Brake_TypicalFailure_RR { get; set; }
        public double Brake_Temperature_LF { get; set; }
        public double Brake_Temperature_LR { get; set; }
        public double Brake_Temperature_RF { get; set; }
        public double Brake_Temperature_RR { get; set; }
        public double Brake_Thickness_LF { get; set; }
        public double Brake_Thickness_LR { get; set; }
        public double Brake_Thickness_RF { get; set; }
        public double Brake_Thickness_RR { get; set; }
        public double Brake_OptimalTemperature_LF_Low { get; set; }
        public double Brake_OptimalTemperature_LR_Low { get; set; }
        public double Brake_OptimalTemperature_RF_Low { get; set; }
        public double Brake_OptimalTemperature_RR_Low { get; set; }
        public double Brake_OptimalTemperature_LF_High { get; set; }
        public double Brake_OptimalTemperature_LR_High { get; set; }
        public double Brake_OptimalTemperature_RF_High { get; set; }
        public double Brake_OptimalTemperature_RR_High { get; set; }
        public double Brake_Torque_LF { get; set; }
        public double Brake_Torque_LR { get; set; }
        public double Brake_Torque_RF { get; set; }
        public double Brake_Torque_RR { get; set; }
        public double Clutch_Torque { get; set; }
        public double Clutch_Friction { get; set; }
        public double Pedals_Clutch { get; set; }
        public double Pedals_Throttle { get; set; }
        public double Pedals_Brake { get; set; }
        public double SteeringAngle { get; set; }
        public LevelIndicator DrivingHelp_BrakingHelp { get; set; }
        public LevelIndicator DrivingHelp_SteeringHelp { get; set; }
        public LevelIndicator DrivingHelp_TractionControl { get; set; }
        public bool DrivingHelp_OppositeLock { get; set; }
        public bool DrivingHelp_SpinRecovery { get; set; }
        public bool DrivingHelp_StabilityControl { get; set; }
        public bool DrivingHelp_AutoClutch { get; set; }
        public double Speed { get; set; }
        public double Brake_InitialThickness_LF { get; set; }
        public double Brake_InitialThickness_RF { get; set; }
        public double Brake_InitialThickness_LR { get; set; }
        public double Brake_InitialThickness_RR { get; set; }
        public double SpeedSlipping { get; set; }
    }
}
