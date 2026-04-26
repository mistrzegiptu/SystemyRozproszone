using Ice;
using SmartHome;

namespace SmartHomeServer.Servants
{
    internal class DimmerLightI : DimmerLightDisp_
    {
        private bool _onState = false;
        private short _brightnessLevel = 0;
        private readonly string _name;

        public DimmerLightI(string name)
        {
            _name = name;
        }

        public override short GetBrightness(Current current)
        {
            return _brightnessLevel;
        }

        public override bool IsOn(Current current)
        {
            return _onState;
        }

        public override void SetBrightness(short level, Current current)
        {
            if (level < 0 || level > 100)
                throw new OutOfRangeException("Value must be between 0 and 100");

            _brightnessLevel = level;
            Console.WriteLine($"[{_name}]: Brightness set for {_brightnessLevel}");
        }

        public override void TurnOff(Current current)
        {
            _onState = false;
            Console.WriteLine($"[{_name}]: Turned OFF");

        }

        public override void TurnOn(Current current)
        {
            _onState = true;
            Console.WriteLine($"[{_name}]: Turned ON");
        }
    }
}
