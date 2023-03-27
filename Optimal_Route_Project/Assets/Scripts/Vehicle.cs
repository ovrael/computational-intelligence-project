using System;
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
        [SerializeField]
        private VehicleType vehicleType;
        public VehicleType VehicleType { get { return vehicleType; } private set { vehicleType = value; } }

        private int maxCapacity;
        private int currentCapacity;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            currentCapacity = 0;

            Array values = Enum.GetValues(typeof(VehicleType));
            VehicleType = (VehicleType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
            maxCapacity = (int)VehicleType;

            spriteRenderer = GetComponent<SpriteRenderer>();

            switch (vehicleType)
            {
                case VehicleType.Small:
                    spriteRenderer.color = Utils.CreateColor(30, 140, 60);
                    Debug.Log("Change color to green");
                    break;
                case VehicleType.Medium:
                    spriteRenderer.color = Utils.CreateColor(30, 70, 160);
                    Debug.Log("Change color to blue");
                    break;
                case VehicleType.Large:
                    spriteRenderer.color = Utils.CreateColor(150, 30, 30);
                    Debug.Log("Change color to red");
                    break;

                default:
                    Debug.Log("Can't find vehicle of type: " + vehicleType);
                    break;
            }
        }
    }
}
