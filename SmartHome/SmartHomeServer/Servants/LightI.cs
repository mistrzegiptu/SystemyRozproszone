using Ice;
using SmartHome;

namespace SmartHomeServer.Servants
{
    internal class LightI : LightDisp_
    {
        private bool _onState = false;
        private readonly string _name;

        public LightI(string name)
        {
            _name = name;
        }

        public override bool IsOn(Current current) => _onState;

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
