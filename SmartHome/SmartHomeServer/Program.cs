using Ice;
using SmartHomeServer.Servants;
using Exception = System.Exception;

namespace SmartHomeApp
{
    class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                using (var communicator = Util.initialize(ref args))
                {
                    string port = args.Length > 0 ? args[0] : "10000";

                    var adapter = communicator.createObjectAdapterWithEndpoints("SmartHomeAdapter", $"tcp -h localhost -p {port}");

                    Console.WriteLine($"Starting server on port {port}...");

                    switch (port)
                    {
                        case "10000":
                            var livingRoomBlind = new RollerBlindI("LivingRoom RollerBlind");
                            adapter.add(livingRoomBlind, Util.stringToIdentity("RollerBlind_LivingRoom"));

                            var livingRoomLight = new RGBLightI("Living Room RGBLight");
                            adapter.add(livingRoomLight, Util.stringToIdentity("RGBLight_LivingRoom"));

                            var kitchenBlind = new RollerBlindI("Kitchen RollerBlind");
                            adapter.add(kitchenBlind, Util.stringToIdentity("RollerBlind_Kitchen"));

                            var kitchenLight = new LightI("Kitchen Room Light");
                            adapter.add(kitchenLight, Util.stringToIdentity("Light_Kitchen"));

                            var mainThermostat = new ThermostatI("Main Thermostat");
                            adapter.add(mainThermostat, Util.stringToIdentity("Thermostat_Main"));
                            break;

                        case "10001":
                            var bedroomLight = new LightI("Bedroom Light");
                            adapter.add(bedroomLight, Util.stringToIdentity("Light_Bedroom"));

                            var bedroomDimmerLight = new DimmerLightI("Bedroom Dimmer Light");
                            adapter.add(bedroomDimmerLight, Util.stringToIdentity("DimmerLight_Bedroom"));
                            
                            var bedroomBlind = new RollerBlindI("Bedroom RollerBlind");
                            adapter.add(bedroomBlind, Util.stringToIdentity("RollerBlind_Bedroom"));

                            var bathroomBlind = new VenetianBlindI("Bathroom VenetianBlind");
                            adapter.add(bathroomBlind, Util.stringToIdentity("VenetianBlind_Bathroom"));

                            var bathroomThermostat = new ThermostatI("Bathroom Thermostat");
                            adapter.add(bathroomThermostat, Util.stringToIdentity("Thermostat_Bathroom"));

                            break;

                        default:
                            Console.WriteLine("Unknown port, no devives");
                            break;
                    }

                    adapter.activate();
                    Console.WriteLine("Server ready (CTRL + C to finish)");
                    communicator.waitForShutdown();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }
            return 0;
        }
    }
}