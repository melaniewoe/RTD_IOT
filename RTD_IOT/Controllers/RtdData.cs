﻿using System;
using System.Net;
using System.IO;
using System.Text;
//using ProtoBuf;
//using transit_realtime;

namespace transit_test
{
    class Program
    {
        static void Main(string[] args)
        {
            // You have to use one or the other:
            //Uri myUri = new Uri("http://www.rtd-denver.com/google_sync/TripUpdate.pb");
            Uri myUri = new Uri("http://www.rtd-denver.com/google_sync/VehiclePosition.pb");
            WebRequest myWebRequest = HttpWebRequest.Create(myUri);

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;

            // This username and password is issued for the IWKS 4120 class. Please DO NOT redistribute.
            NetworkCredential myNetworkCredential = new NetworkCredential("RTDgtfsRT", "realT!m3Feed");    // insert credentials here

            CredentialCache myCredentialCache = new CredentialCache();
            myCredentialCache.Add(myUri, "Basic", myNetworkCredential);

            myHttpWebRequest.PreAuthenticate = true;
            myHttpWebRequest.Credentials = myCredentialCache;

            FeedMessage feed = Serializer.Deserialize<FeedMessage>(myWebRequest.GetResponse().GetResponseStream());

            Stop stop_inst = new Stop();
            Trip trip_inst = new Trip();

            foreach (FeedEntity entity in feed.entity)
            {
                if (entity.vehicle != null)
                {
                    if (entity.vehicle.trip != null)
                    {
                        if (entity.vehicle.trip.route_id != null)
                        {
                            Console.WriteLine("Vehicle ID = " + entity.vehicle.vehicle.id);
                            Console.WriteLine("Current Position Information:");
                            Console.WriteLine("Current Latitude = " + entity.vehicle.position.latitude);
                            Console.WriteLine("Current Longitude = " + entity.vehicle.position.longitude);
                            Console.WriteLine("Current Bearing = " + entity.vehicle.position.bearing);
                            Console.WriteLine("Current Status = " + entity.vehicle.current_status + " StopID: " + entity.vehicle.stop_id);
                            if (Stop.stops.ContainsKey(entity.vehicle.stop_id))
                            {
                                Console.WriteLine("The name of this StopID is \"" + Stop.stops[entity.vehicle.stop_id].stop_name + "\"");
                                Console.WriteLine("The Latitude of this StopID is \"" + Stop.stops[entity.vehicle.stop_id].stop_lat + "\"");
                                Console.WriteLine("The Longitude of this StopID is \"" + Stop.stops[entity.vehicle.stop_id].stop_long + "\"");
                                string wheelChairOK = "IS NOT";
                                if (Stop.stops[entity.vehicle.stop_id].wheelchair_access)
                                {
                                    wheelChairOK = "IS";
                                }
                                Console.WriteLine("This stop is " + wheelChairOK + " wheelchair accessible");
                            }

                            Console.WriteLine("Trip ID = " + entity.vehicle.trip.trip_id);
                            if (Trip.trips.ContainsKey(entity.vehicle.trip.trip_id))
                            {
                                if (entity.vehicle.current_status.ToString() == "IN_TRANSIT_TO")
                                {
                                    if (Stop.stops.ContainsKey(entity.vehicle.stop_id))
                                    {
                                        Console.WriteLine("Vehicle in transit to: " + Stop.stops[entity.vehicle.stop_id].stop_name);
                                        Trip.trip_t trip = Trip.trips[entity.vehicle.trip.trip_id];
                                        foreach (Trip.trip_stops_t stop in trip.tripStops)
                                        {
                                            if (stop.stop_id == entity.vehicle.stop_id)
                                            {
                                                Console.WriteLine(".. and is scheduled to arrive there at " + stop.arrive_time);
                                                break;
                                            }
                                        }
                                    }
                                }
                                Console.WriteLine();
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }
    }
}