using Newtonsoft.Json;

namespace PragueParking2._0.DataConfig
{
    class ConfigValues
    {
        [JsonProperty("CarSize")]
        public static int CarSize { get; set; }

        [JsonProperty("MCSize")]
        public static int MCSize { get; set; }

        [JsonProperty("CarPricePerHour")]
        public static int CarPricePerHour { get; set; }

        [JsonProperty("MCPricePerHour")]

        public static int MCPricePerHour { get; set; }

        [JsonProperty("ParkingHouseSize")]

        public static int ParkingHouseSize { get; set; }

        [JsonProperty("ParkingSpotSize")]
        public static int ParkingSpotSize { get; set; }

        [JsonProperty("FreeParkingTime")]
        public static int FreeParkingTime { get; set; }
    }
}
