using Ice;
using SmartHome;

namespace SmartHomeServer.Servants
{
    internal class ThermostatI : ThermostatDisp_
    {
        private float _currentTemperature = 20.0f;
        private ScheduleRule[] _dailyPlan = [];
        private readonly string _name;

        public ThermostatI(string name)
        {
            _name = name;
        }

        public override float GetTemperature(Current current)
        {
            return _currentTemperature;
        }

        public override void SetTemperature(float temp, Current current)
        {
            if (temp < 5.0f || temp > 35.0f)
                throw new OutOfRangeException("Temperature must be between 5°C and 35°C");

            _currentTemperature = temp;
            Console.WriteLine($"[{_name}]: Temperature set for {_currentTemperature}°C");
        }

        public override ScheduleRule[] GetDailyPlan(Current current)
        {
            return _dailyPlan;
        }

        public override void SetDailyPlan(ScheduleRule[] plan, Current current)
        {
            if(plan.Any(r => r.targetTemp < 5.0f || r.targetTemp > 35.0f))
                throw new OutOfRangeException("Temperature must be between 5°C and 35°C");

            _dailyPlan = plan;

            Console.WriteLine($"[{_name}]: Daily plan updated");

            foreach (var rule in plan)
            {
                Console.WriteLine($"\t -> Time: {rule.time}, Temp: {rule.targetTemp}°C");
            }
        }
    }
}
