using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using static System.Console;

// App made by: Per Sverre Anda on a rainy day the 14 of june 2019.
// This program is developed to keep track of rentalbikes in the city of Oslo, consuming official available APIs. 
// The API is here a json-file, updating every 10 minutes.
// This program dont update every 10 minutes, but I could implement it easy using a timer.
// I could also make an UWP app of this, making it available in windows store.

namespace BySykkelSys
{
    class Program
    {
        static void Main()
        {
            string info=GetJson("https://gbfs.urbansharing.com/oslobysykkel.no/station_information.json");     // gets the content from given json file.

            var jObjectStationInfo = JsonConvert.DeserializeObject<RootObjectStationInfo>(info);  //Our aquired json-text is now converted to "StationInfo" objects

            info=GetJson("https://gbfs.urbansharing.com/oslobysykkel.no/station_status.json");  // gets the content from given json file.

            var jObjectStationStatus = JsonConvert.DeserializeObject<RootObjectStationStatus>(info);  //Our aquired json-text is now converted to "StationStatus" objects


            //*****************************************************LINQ******************************************************************
            //*
            //* Using the magic of LINQ we can now make a list containing the information we need by combining our two existing 'lists' of objects

            var query = from sInfo in jObjectStationInfo.data.stations
                        join sStatus in jObjectStationStatus.data.stations
                             on sInfo.station_id equals sStatus.station_id orderby sStatus.num_bikes_available ascending   
                        select new
                        {
                            sInfo.station_id,
                            sInfo.name,
                            sStatus.num_bikes_available,
                            sStatus.num_docks_available
                        };
            //*
            //*****************************************************LINQ******************************************************************



            //*****************************************************PRINOUT******************************************************************
            //*

            WriteLine(String.Format("{0,-10} | {1,-10} | {2,-10} | {3,-10}", "ID", "Bikes" , "Free docks", "Place" )); 

            foreach(var item in query)
            {
                WriteLine(String.Format("{0,-10} | {1,-10} | {2,-10} | {3,-10}", item.station_id, item.num_bikes_available, item.num_docks_available, item.name));

            }

            //*
            //*****************************************************PRINTOUT******************************************************************

        }


        //*****************************************************HTTP Request******************************************************************
        //*
        //* The method below inserts one of the given urls and returns a string for us containing our snagged Json. This method is called multiple times.

        public static string GetJson(string url)
        {
            using(var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("client-name", "per-anda_BySykkelSys");
                var response = httpClient.GetStringAsync(new Uri(url)).Result;

                return response;

            }

        }

        //*
        //*****************************************************HTTP Request******************************************************************

    }




    //*********STOP READING HERE
    //*********Tips: Consider the classes below only as informationholders, and dont overthink them, i did;
    //*********they are made using this webpage: http://json2csharp.com/ by pasting in the two necessary bicycle Jsons ***************************  
    //*

    public class RootObjectStationStatus
        {
            public int last_updated { get; set; }
            public StationStatusData data { get; set; }
        }

        public class StationStatus
        {
            public int is_installed { get; set; }
            public int is_renting { get; set; }
            public int num_bikes_available { get; set; }
            public int num_docks_available { get; set; }
            public int last_reported { get; set; }
            public int is_returning { get; set; }
            public string station_id { get; set; }
        }

        public class StationStatusData
        {
            public List<StationStatus> stations { get; set; }
        }


      

        public class StationInfo
        {
            public string station_id { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
            public int capacity { get; set; }
        }

        public class StationInfoData
        {
            public List<StationInfo> stations { get; set; }
        }

        public class RootObjectStationInfo
        {
            public int last_updated { get; set; }
            public StationInfoData data { get; set; }
        }

    //*
    //*****************************************************************************************************************

    }

