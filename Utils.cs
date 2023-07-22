using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;

namespace VehicleGarage;

public class Utils
{
    public static JObject GetVehicleConfig()
    {
        string data = API.LoadResourceFile("fivepd", "/config/vehicles.json");
        return JObject.Parse(data);
    }

    public static JArray GetGarageConfig()
    {
        string data = API.LoadResourceFile("fivepd", "/config/garage.json");
        return JArray.Parse(data);
    }
}