using Ice;
using SmartHome;

namespace SmartHomeServer.Servants
{
    internal class VenetianBlindI : VenetianBlindDisp_
    {
        private short _position = 0;
        private short _tiltAngle = 0;
        private readonly string _name;

        public VenetianBlindI(string name)
        {
            _name = name;
        }

        public override short GetPosition(Current current) => _position;

        public override short GetTilt(Current current) => _tiltAngle;

        public override void SetPosition(short percentage, Current current)
        {
            if (percentage < 0 || percentage > 100)
                throw new OutOfRangeException("Position must be between 0 and 100");

            if (_tiltAngle != 90)
                throw new HardwareErrorException("Tilt must be 90 deg in order to change position");

            _position = percentage;
            Console.WriteLine($"[{_name}]: Roller set for {_position}%");
        }

        public override void SetTilt(short angle, Current current)
        {
            if (angle < 0 || angle > 180)
                throw new OutOfRangeException("Position must be between 0 and 100");

            _tiltAngle = angle;
            Console.WriteLine($"[{_name}]: Tilt angle set for {_tiltAngle} deg");
        }
    }
}
