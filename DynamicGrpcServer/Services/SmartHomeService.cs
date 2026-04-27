using Grpc.Core;

namespace DynamicGrpcServer.Services
{
    public class SmartHomeService(ILogger<SmartHomeService> logger) : SmartHome.SmartHomeBase
    {
        public override Task<LightsResponse> ToggleAllLights(LightsRequest request, ServerCallContext context)
        {
            string newState = request.TurnOn ? "ON" : "OFF";

            Console.WriteLine($"All lights are now {newState}");

            return Task.FromResult(new LightsResponse { StatusCode = 200 });
        }

        public override Task<RoomStatusResponse> GetDevicesStatusInRoom(RoomStatusRequest request, ServerCallContext context)
        {
            Console.WriteLine("Getting devices state");

            var response = new RoomStatusResponse();

            response.Devices.Add(new DeviceStatus { DeviceId = 1, DeviceName = "Alarm", IsHealthy = true });
            response.Devices.Add(new DeviceStatus { DeviceId = 2, DeviceName = "IP camera", IsHealthy = true });
            response.Devices.Add(new DeviceStatus { DeviceId = 3, DeviceName = "Security shutter", IsHealthy = false });

            return Task.FromResult(response);
        }

        public override async Task MonitorTemperatureSensor(TemperatureSensorRequest request, IServerStreamWriter<TemperatureSensorReading> responseStream, ServerCallContext context)
        {
            Console.WriteLine("Listening from temperature sensor");

            var random = new Random();

            for(int i = 0; i < 5; i++)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    break;

                var randomTemperature = random.NextDouble() * 50.0f - 20.0f;
                var reading = new TemperatureSensorReading { Timestamp = DateTime.UtcNow.ToString(), Temperature = (float)Math.Round(randomTemperature, 2) };

                await responseStream.WriteAsync(reading);

                await Task.Delay(500);
            }

            Console.WriteLine("Finished listening from temperature sensor");
        }
    }
}
