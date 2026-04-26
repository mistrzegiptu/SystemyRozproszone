using Ice;
using SmartHome;

namespace SmartHomeServer.Servants
{
    internal class RGBLightI : RGBLightDisp_
    {
        private bool _onState = false;
        private Color _color;
        private readonly string _name;

        public RGBLightI(string name)
        {
            _name = name;
        }

        public override Color GetColor(Current current)
        {
            return _color;
        }

        public override bool IsOn(Current current)
        {
            return _onState;
        }

        public override void SetColor(Color c, Current current)
        {
            var (r, g, b) = c;

            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
                throw new OutOfRangeException("RGB values must be in range 0-255");

            _color = c;
            Console.WriteLine($"[{_name}]: Color set ({r}, {g}, {b})");
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
