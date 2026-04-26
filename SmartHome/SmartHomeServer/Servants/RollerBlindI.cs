using Ice;
using SmartHome;

namespace SmartHomeServer.Servants
{
    internal class RollerBlindI : RollerBlindDisp_
    {
        private short _position = 0;
        private readonly string _name;

        public RollerBlindI(string name)
        {
            _name = name;
        }

        public override short GetPosition(Current current) => _position;

        public override void SetPosition(short percentage, Current current)
        {
            if (percentage < 0 || percentage > 100)
                throw new OutOfRangeException("Position must be between 0 and 100");

            _position = percentage;
            Console.WriteLine($"[{_name}]: Roller set for {_position}%");
        }
    }
}
