#pragma once

module SmartHome {

    exception OutOfRangeException {
        string reason;
    };

    exception HardwareErrorException {
        string details;
    };

    interface Light {
        void TurnOn();
        void TurnOff();
        bool IsOn();
    };

    struct Color {
        short r;
        short g;
        short b;
    };

    interface RGBLight extends Light {
        void SetColor(Color c) throws OutOfRangeException;
        Color GetColor();
    };

    interface DimmerLight extends Light {
        void SetBrightness(short level) throws OutOfRangeException;
        short GetBrightness();
    };

    struct ScheduleRule {
        string time;
        float targetTemp;
    };

    sequence<ScheduleRule> DailyPlan;

    interface Thermostat {
        void SetTemperature(float temp) throws OutOfRangeException;
        float GetTemperature();
        
        void SetDailyPlan(DailyPlan plan);
        DailyPlan GetDailyPlan();
    };

    interface RollerBlind {
        void SetPosition(short percentage) throws OutOfRangeException;
        short GetPosition();
    };

    interface VenetianBlind extends RollerBlind {
        void SetTilt(short angle) throws HardwareErrorException; 
        short GetTilt();
    };

};