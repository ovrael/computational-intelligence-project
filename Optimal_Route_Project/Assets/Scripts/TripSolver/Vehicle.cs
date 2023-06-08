using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [ExecuteInEditMode]
    public enum VehicleType
    {
        Small = 1000, // Green
        Medium = 1500, // Blue
        Large = 2000 // Red
    }

    [ExecuteInEditMode]
    public class Vehicle : MonoBehaviour
    {
        [Header("Vehicle parameters")]
        [SerializeField]
        private VehicleType vehicleType;
        public VehicleType VehicleType { get { return vehicleType; } private set { vehicleType = value; } }
        [SerializeField]
        public int maxCapacity;
        [SerializeField]
        private float vehicleSpeed = 10.0f;

        public int loadout = 0;
        public int LeftLoadoutSpace { get { return maxCapacity - loadout; } }

        [Header("Travel info")]
        [SerializeField]
        private int howManyVisits = 4;
        [SerializeField]
        private List<GameObject> pointsToVisit;
        [SerializeField]
        private Vector2 startPosition;
        [SerializeField]
        private Vector2 currentPosition;
        [SerializeField]
        private float totalDistanceTraveled = 0f;

        [Header("Test run")]
        [SerializeField]
        private bool allowTestRun = false;
        [SerializeField]
        private bool testRunFinished = false;

        [Header("Map Controller Reference")]
        [SerializeField]
        private Map MapController;

        private int nextPointIndex;
        private LineRenderer pathLine;

        private Color color;
        public GameObject[] tripPoints;


        private void Awake()
        {
            Array vehicleTypes = Enum.GetValues(typeof(VehicleType));
            VehicleType = (VehicleType)vehicleTypes.GetValue(UnityEngine.Random.Range(0, vehicleTypes.Length));
            maxCapacity = (int)VehicleType;

            pathLine = GetComponent<LineRenderer>();
            MapController = GameObject.FindGameObjectWithTag("MapController").GetComponent<Map>();

            startPosition = transform.position;
            currentPosition = transform.position;

            SetVehicleColor();
        }

        private void Update()
        {
            //if (allowTestRun == true && testRunFinished == false)
            //{
            //    PerformTestRun();
            //    CalculateTraveledDistance();
            //}
        }

        private void SetVehicleColor()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Config config = GameObject.FindGameObjectWithTag("MapController").GetComponent<Config>();

            switch (vehicleType)
            {
                case VehicleType.Small:
                    spriteRenderer.color = config.GreenVehicleColor;
                    break;
                case VehicleType.Medium:
                    spriteRenderer.color = config.BlueVehicleColor;
                    break;
                case VehicleType.Large:
                    spriteRenderer.color = config.RedVehicleColor;
                    break;

                default:
                    Debug.Log("Can't find vehicle of type: " + vehicleType);
                    break;
            }
        }

        //public void LoadAllFromOther(GoodsData otherGoodsData, bool takeHowMuchYouCan = true)
        //{
        //    // I want to take all goods
        //    if (!takeHowMuchYouCan)
        //    {
        //        // Whoops, too much for me!
        //        if (GoodsData.GetAllWeight() + otherGoodsData.GetAllWeight() > maxCapacity)
        //        {
        //            Debug.LogWarning($"Vehicle cannot take other goods, it's more thant its max capacity." +
        //                $"current capacity:{GoodsData.GetAllWeight()}, " +
        //                $"other weights:{otherGoodsData.GetAllWeight()}, " +
        //                $"max capacity:{maxCapacity}");
        //            return;
        //        }

        //        GoodsData.LoadFromOther(otherGoodsData);
        //    }

        //    // I want to take as much as I have capacity
        //    if (takeHowMuchYouCan)
        //    {

        //    }
        //}

        public void LoadFromOther(Point point)
        {

        }

        public void UnloadToOther(Point point)
        {
            if (point.PointType == PointType.Market)
            {
            }
        }

        [ContextMenu("Add random points to list")]
        private void PickRandomPoints()
        {

            if (pointsToVisit != null)
            {
                pointsToVisit.Clear();
            }

            pointsToVisit.Add(this.gameObject);

            for (int i = 0; i < howManyVisits; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, MapController.points.Count);

                if (!pointsToVisit.Contains(MapController.points[randomIndex]))
                {
                    pointsToVisit.Add(MapController.points[randomIndex]);
                }
            }

            VisualizePath();

        }


        private void VisualizePath()
        {
            Vector3[] pointsPositions = new Vector3[pointsToVisit.Count];

            pointsPositions[0] = transform.position;

            for (int i = 1; i < pointsPositions.Length; i++)
            {
                pointsPositions[i] = pointsToVisit[i].transform.position;
            }

            pathLine.positionCount = pointsPositions.Length;
            pathLine.SetPositions(pointsPositions);

        }

        [ContextMenu("Reset vehicle")]
        private void ResetVehicle()
        {
            transform.position = startPosition;
            allowTestRun = false;
            testRunFinished = false;

            currentPosition = transform.position;
            totalDistanceTraveled = 0;

            pointsToVisit.Clear();
            pathLine.positionCount = 0;
        }

        private void PerformTestRun()
        {

            transform.position = Vector2.MoveTowards(transform.position, pointsToVisit[nextPointIndex].transform.position, vehicleSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, pointsToVisit[nextPointIndex].transform.position) < 0.1f)
            {

                if (nextPointIndex < pointsToVisit.Count - 1)
                {
                    nextPointIndex = nextPointIndex + 1;
                }
                else
                {
                    nextPointIndex = 0;
                    testRunFinished = true;
                }

            }

        }

        private void CalculateTraveledDistance()
        {
            float distanceThisFrame = Vector3.Distance(transform.position, currentPosition);
            totalDistanceTraveled += distanceThisFrame;

            currentPosition = transform.position;
        }
        void OnDrawGizmosSelected()
        {

            if (tripPoints == null || tripPoints.Length == 0)
            {
                Debug.Log("trip is null or has 0 elements");
                return;
            }

            for (int i = 0; i < tripPoints.Length - 1; i++)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

                Vector3 current = tripPoints[i].transform.position;
                Vector3 next = tripPoints[i + 1].transform.position;
                Handles.DrawBezier(current, next, current, next, spriteRenderer.color, null, 5);

                //Gizmos.color = spriteRenderer.color;
                //Gizmos.DrawLine(current, next);
            }
        }
    }
}
