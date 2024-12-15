using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SUMS
{
    public struct Station
    {
        public string ID;
        public string name;
        public string status;
    }

    public static class Stations
    {
        public static List<Station> stations = new List<Station>();

        public static void InitializeStations()
        {
            NewStation();
            Console.WriteLine("DEBUG : Stations initialized");
        }

        public static void NewStation()
        {
            Station station = new Station();
            station.ID = "FP-332";
            station.name = "Base Terra Alpha";
            station.status = "ONLINE";

            stations.Add(station);
        }
    }
}
