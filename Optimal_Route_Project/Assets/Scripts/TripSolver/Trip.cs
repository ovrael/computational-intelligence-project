using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class TripData
    {
        public Vehicle Vehicle { get; set; }
        public Rect AreaLimitations { get; set; }
        public List<GameObject> TripPoints { get; set; }

        public TripData()
        {
            Vehicle = new Vehicle();
            AreaLimitations = new Rect();
            TripPoints = new List<GameObject>();
        }

        public void ShowDebug()
        {
            Debug.Log($"Vehicle name: {Vehicle.name}");
            Debug.Log($"Limitations: x:{AreaLimitations.x} width:{AreaLimitations.width} y:{AreaLimitations.y} height:{AreaLimitations.height}");
            for (int i = 0; i < TripPoints.Count; i++)
            {
                Debug.Log($"Trip point {i}: {TripPoints[i].name}");
            }
        }

        public override string ToString()
        {
            return $"Vehicle name: {Vehicle.name}";
        }
    }


    [ExecuteInEditMode]
    public class Trip
    {
        private List<GameObject> points;
        private GameObject[] warehouses;
        private Vehicle[] vehicles;
        private GameObject startWarehouse;
        private TripData[] tripDatas;
        public Rect[] limitations;

        public Trip(List<GameObject> points, List<GameObject> vehicles, GameObject startWarehouse)
        {
            this.points = points;
            this.warehouses = this.points.Where(p => p.GetComponent<Point>().PointType == PointType.Warehouse).ToArray();
            this.vehicles = vehicles.Select(p => p.GetComponent<Vehicle>()).ToArray();
            this.startWarehouse = startWarehouse;
        }

        private void GoTrip(TripData tripData)
        {
            GameObject[] searchMarkets = GetAvailableMarkets(tripData);

            int whileCounter = 0;
            int whileMax = 200;

            if (searchMarkets.Length == 0)
            {
                Debug.Log("No search markets in area, vehicle has no route");
                return;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var sm in searchMarkets)
            {
                sb.Append($"{sm.name}, ");
            }
            Debug.Log("THERE ARE " + searchMarkets.Length + " markets in the area, named: " + sb.ToString());

            while (searchMarkets.Any(p => p.GetComponent<Point>().goods != 0))
            {
                if (whileCounter > whileMax) break;
                Debug.Log($"While... trip length: {tripData.TripPoints.Count} Counter:{whileCounter}");
                whileCounter++;

                int zerosCounter = 0;
                for (int i = 0; i < searchMarkets.Length; i++)
                    if (searchMarkets[i].GetComponent<Point>().goods == 0) zerosCounter++;

                if (zerosCounter == searchMarkets.Length)
                {
                    Debug.Log("All markets have 0 goods");
                    break;
                }

                Debug.Log("Check available markets");
                // There are no available markets -> go to warehouse to reset loadout
                if (searchMarkets.All(p => p.GetComponent<Point>().skip == false))
                {
                    Debug.Log("There are no available markets -> go to warehouse to reset loadout");
                    int leftMarketsGoods = searchMarkets.Sum(p => p.GetComponent<Point>().goods);
                    GoToWarehouse(tripData, leftMarketsGoods);

                    // Clear skip status
                    foreach (var m in searchMarkets)
                    {
                        m.GetComponent<Point>().skip = false;
                    }
                }

                Debug.Log("Find nearest market");
                var undoneMarket = tripData.TripPoints.Last().FindNearestUndoneMarket(searchMarkets);
                if (undoneMarket == null)
                {
                    Debug.Log("undoneMarket is null");

                    tripData.TripPoints.Add(startWarehouse);
                    break;
                }

                Point market = undoneMarket.GetComponent<Point>();
                Debug.Log("Nearest market is: " + undoneMarket.name + " with " + market.goods + " goods.");

                bool visitResult = false;
                if (market.goods < 0) // Market wants goods
                {
                    Debug.Log("Unload vehicle");
                    visitResult = tripData.Vehicle.Unload(market);
                }
                else // Market has goods
                {
                    Debug.Log("Load vehicle");
                    visitResult = tripData.Vehicle.Load(market);
                }


                Debug.Log("Check visit result: " + visitResult);
                // Something went wrong...
                if (!visitResult)
                {
                    Debug.Log("Skip market");
                    market.skip = true;
                }
                else // It's fine, car changed loadout so skipped markets are will be longer skipped
                {
                    Debug.Log("Car changed loadout, reset skips");
                    tripData.TripPoints.Add(undoneMarket);
                    foreach (var m in searchMarkets)
                    {
                        m.GetComponent<Point>().skip = false;
                    }
                }
            }

            if (tripData.TripPoints.Last().name != startWarehouse.name)
            {
                tripData.TripPoints.Add(startWarehouse);
            }

            if (tripData.TripPoints.Count > 2 && tripData.TripPoints[1].name == startWarehouse.name)
                tripData.TripPoints.RemoveAt(0);

            // Found best trip
            Debug.Log("End of trip");
        }

        private void GoToWarehouse(TripData tripData, int leftMarketsGoods)
        {
            bool alreadyInWarehouse = tripData.TripPoints.Last().GetComponent<Point>().PointType == PointType.Market;
            GameObject nearestWarehouse = alreadyInWarehouse ? tripData.TripPoints.Last() : tripData.TripPoints.Last().FindNearestWarehouse(warehouses);

            Debug.Log("Left goods: " + leftMarketsGoods);
            // There are more goods than needed - unload all vehicle
            if (leftMarketsGoods >= 0)
            {
                Debug.Log("Vehicles loadout is 0");
                tripData.Vehicle.loadout = 0;
            }
            else // There are more needed markets
            {
                Debug.Log("Vehicles has maximum load");
                tripData.Vehicle.loadout = tripData.Vehicle.maxCapacity;
            }

            if (!alreadyInWarehouse)
                tripData.TripPoints.Add(nearestWarehouse);
        }

        private GameObject[] GetAvailableMarkets(TripData tripData)
        {
            List<GameObject> availablePoints = new List<GameObject>();

            foreach (var point in points)
            {
                if (point.GetComponent<Point>().PointType == PointType.Warehouse)
                    continue;

                if (tripData.AreaLimitations.Contains(point.transform.position))
                {
                    availablePoints.Add(point);
                }
            }

            return availablePoints.ToArray();
        }

        public void Run()
        {
            tripDatas = new TripData[vehicles.Length];
            limitations = ComputeLimitations(vehicles.Length);

            for (int i = 0; i < tripDatas.Length; i++)
            {
                tripDatas[i] = new TripData
                {
                    Vehicle = vehicles[i],
                    TripPoints = new List<GameObject>()
                    {
                        startWarehouse
                    },
                    AreaLimitations = limitations[i]
                };

                //tripDatas[i].ShowDebug();
            }

            for (int i = 0; i < tripDatas.Length; i++)
            {
                Debug.Log($"-------------------------------------------------------------------------------------------- Car {i} goes on trip!");
                GoTrip(tripDatas[i]);
                Debug.Log($"Car {i} ended trip!");
            }
        }

        private Rect[] ComputeLimitations(int vehiclesCount)
        {
            // Example for 5 in comments

            Rect[] rects = new Rect[vehiclesCount];
            int topPartsCount = vehiclesCount / 2;          // will be 2
            int botPartsCount = vehiclesCount / 2 + vehiclesCount % 2;      // will be 3
            float fieldSize = 100;

            float topPartWidth = fieldSize / topPartsCount; // will be 50.0
            float botPartWidth = fieldSize / botPartsCount; // will be 33.3

            float topPartHeight = (botPartWidth * fieldSize) / (topPartWidth + botPartWidth); // will be 40
            float botPartHeight = fieldSize - topPartHeight; // will be 60

            //Debug.Log($"For {vehiclesCount} vehicles ther will be top: {topPartsCount} and bot: {botPartsCount} parts. TopW:{topPartWidth} BotW: {botPartWidth}. TopH:{topPartHeight} BotH:{botPartHeight}");

            // Add top rects
            for (int i = 0; i < topPartsCount; i++)
            {
                rects[i] = new Rect()
                {
                    x = i * topPartWidth,
                    width = topPartWidth,
                    y = fieldSize - topPartHeight,
                    height = topPartHeight
                };
            }

            // Add bot rects
            for (int i = topPartsCount; i < rects.Length; i++)
            {
                rects[i] = new Rect()
                {
                    x = (i - topPartsCount) * botPartWidth,
                    width = botPartWidth,
                    y = 0,
                    height = botPartHeight
                };
            }

            return rects;
        }

        public TripData[] GetTripDatas()
        {
            return tripDatas;
        }
    }

    [ExecuteInEditMode]
    public static class ExtendTrip
    {
        public static GameObject FindNearestUndoneMarket(this GameObject point, GameObject[] markets)
        {
            GameObject[] undoneMarkets = markets.Where(m => m.GetComponent<Point>().goods != 0 && m.GetComponent<Point>().skip == false).ToArray();

            if (undoneMarkets.Length == 0)
                return null;

            GameObject nearestMarket = undoneMarkets[0];
            float minDistance = float.MaxValue;

            foreach (var market in undoneMarkets)
            {
                if (market.GetComponent<Point>().goods == 0 || market == point)
                    continue;

                float dist = Vector3.Distance(market.transform.position, point.transform.position);
                if (dist < minDistance)
                {
                    nearestMarket = market;
                    minDistance = dist;
                }
            }

            return nearestMarket;
        }

        public static GameObject FindNearestWarehouse(this GameObject point, GameObject[] warehouses)
        {
            if (warehouses.Length == 0)
            {
                Debug.Log("NO WAREHOUSES???????");
                throw new Exception("Something went terribly wrong!");
            }

            GameObject nearestWarehouse = warehouses[0];
            float minDistance = float.MaxValue;

            foreach (var warehouse in warehouses)
            {
                float dist = Vector3.Distance(warehouse.transform.position, point.transform.position);
                if (dist < minDistance)
                {
                    nearestWarehouse = warehouse;
                    minDistance = dist;
                }
            }

            return nearestWarehouse;
        }

        //public static GameObject FindPointsInDistance(this GameObject point, GameObject[] searchPoints, PointType pointType)
        //{
        //    GameObject[] points = searchPoints.Where(p => p.GetComponent<Point>().PointType == pointType).ToArray();
        //    GameObject nearestPoint = points[0];

        //    float minDistance = float.MaxValue;
        //    foreach (var p in points)
        //    {
        //        float dist = Vector3.Distance(p.transform.position, point.transform.position);
        //        if (dist < minDistance)
        //        {
        //            nearestPoint = p;
        //            minDistance = dist;
        //        }
        //    }

        //    return nearestPoint;
        //}

        public static bool Load(this Vehicle vehicle, Point point)
        {
            if (vehicle.LeftLoadoutSpace == 0) return false;

            // Vehicle can take all goods
            if (vehicle.LeftLoadoutSpace > point.goods)
            {
                vehicle.loadout += point.goods;
                point.goods = 0;
                return true;
            }

            // Vehicle take as many goods as it can
            point.goods -= vehicle.LeftLoadoutSpace;
            vehicle.loadout = vehicle.maxCapacity;
            return true;
        }

        public static bool Unload(this Vehicle vehicle, Point point)
        {
            if (vehicle.loadout <= 0) return false;

            // Vehicle can give all goods
            if (vehicle.loadout + point.goods >= 0)
            {
                vehicle.loadout += point.goods;
                point.goods = 0;
                return true;
            }

            // Vehicle take as many goods as it can
            point.goods += vehicle.loadout;
            vehicle.loadout = 0;
            return true;
        }
    }
}
