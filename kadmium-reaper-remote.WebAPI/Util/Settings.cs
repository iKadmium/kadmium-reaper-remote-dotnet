using Newtonsoft.Json;
using System.Threading.Tasks;

namespace kadmium_reaper_remote_dotnet.Util
{
    public class Settings
    {
        public string ReaperURI { get; set; }
        public int HttpPort { get; set; }
        public string LightingVenueURI { get; set; }

        [JsonIgnore]
        public static Settings Instance { get; set; }

        public static async Task Initialize()
        {
            Instance = await FileAccess.LoadSettings();
        }

        public Settings()
        {
            ReaperURI = "http://localhost:9080/live.html";
            HttpPort = 80;
            LightingVenueURI = "http://localhost:5000/api/Venue/ActivateByName";
        }
    }
}