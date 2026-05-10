using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceSystem.Shared
{
    public class RabbitConfig
    {
        public const string ExchangeName = "space_system";

        public const string RoutingKeyAdminAll = "admin.all";
        public const string RoutingKeyAdminAgencies = "admin.agencies";
        public const string RoutingKeyAdminCarriers = "admin.carriers";

        public const string QueuePeople = "queue_people";
        public const string QueueCargo = "queue_cargo";
        public const string QueueSatellite = "queue_satellite";
    }
}
