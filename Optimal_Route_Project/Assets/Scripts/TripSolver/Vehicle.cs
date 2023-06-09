using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;

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
        private float vehicleSpeed = 20.0f;

        public int loadout = 0;
        public int LeftLoadoutSpace { get { return maxCapacity - loadout; } }
        public GoodsData loadoutGoods;
        public bool hasCat = false;
        public float routeLength = 0;

        [Header("Travel info")]
        [SerializeField]
        private Vector2 startPosition;
        [SerializeField]
        private Vector2 currentPosition;
        [SerializeField]
        private float totalDistanceTraveled = 0f;
        [SerializeField]
        public bool displayRouteLength = false;
        [SerializeField]
        public bool allowVehicleMovement = false;
        [SerializeField]
        private bool tripFinished = false;
        public GameObject[] tripPoints;

        [Header("Map Controller Reference")]
        [SerializeField]
        private Map MapController;

        [Header("Text Component")]
        [SerializeField]
        private TextMeshPro vehicleTextMeshProComponent;

        private int nextPointIndex;
        private LineRenderer pathLine;

        private Color color;

        private void Awake()
        {
            Array vehicleTypes = Enum.GetValues(typeof(VehicleType));
            VehicleType = (VehicleType)vehicleTypes.GetValue(UnityEngine.Random.Range(0, vehicleTypes.Length));
            maxCapacity = (int)VehicleType;

            pathLine = GetComponent<LineRenderer>();
            MapController = GameObject.FindGameObjectWithTag("MapController").GetComponent<Map>();

            startPosition = transform.position;
            currentPosition = transform.position;

            loadoutGoods = new GoodsData();

            vehicleTextMeshProComponent = transform.Find("Text_Component").GetComponent<TextMeshPro>();

            SetVehicleColor();
        }

        private void Update()
        {
            if (allowVehicleMovement == true && tripFinished == false)
            {
                PerformTripDrive();
                CalculateTraveledDistance();
            }


            if (displayRouteLength == true)
            {
                vehicleTextMeshProComponent.enabled = true;
                vehicleTextMeshProComponent.text = "Total route lenght: " + totalDistanceTraveled + " km";
                vehicleTextMeshProComponent.color = Color.black;
                vehicleTextMeshProComponent.fontSize = 12;
                vehicleTextMeshProComponent.alignment = TextAlignmentOptions.Center;
                vehicleTextMeshProComponent.text = vehicleTextMeshProComponent.text.ToUpper();
                vehicleTextMeshProComponent.fontStyle = FontStyles.Bold;

            }
            else
            {
                vehicleTextMeshProComponent.enabled = false;
            }

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


        private void PerformTripDrive()
        {

            transform.position = Vector2.MoveTowards(transform.position, tripPoints[nextPointIndex].transform.position, vehicleSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, tripPoints[nextPointIndex].transform.position) < 0.1f)
            {

                if (nextPointIndex < tripPoints.Length - 1)
                {
                    nextPointIndex = nextPointIndex + 1;
                }
                else
                {
                    nextPointIndex = 0;
                    tripFinished = true;
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

        internal void ComputeRouteLenght()
        {
            routeLength = 0;
            for (int i = 0; i < tripPoints.Length - 1; i++)
            {
                routeLength += Vector3.Distance(tripPoints[i].transform.position, tripPoints[i + 1].transform.position);
            }
        }
    }
}
