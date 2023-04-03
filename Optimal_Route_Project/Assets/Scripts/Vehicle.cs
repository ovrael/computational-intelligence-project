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


        [SerializeField]
        private int maxCapacity;

        private GoodsData goodsData;
        public GoodsData GoodsData { get => goodsData; set => goodsData = value; }


        private void Awake()
        {
            Array vehicleTypes = Enum.GetValues(typeof(VehicleType));
            VehicleType = (VehicleType)vehicleTypes.GetValue(UnityEngine.Random.Range(0, vehicleTypes.Length));
            maxCapacity = (int)VehicleType;
            SetVehicleColor();
        }

        private void SetVehicleColor()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            switch (vehicleType)
            {
                case VehicleType.Small:
                    spriteRenderer.color = Config.GetInstance().GreenVehicleColor;
                    break;
                case VehicleType.Medium:
                    spriteRenderer.color = Config.GetInstance().BlueVehicleColor;
                    break;
                case VehicleType.Large:
                    spriteRenderer.color = Config.GetInstance().RedVehicleColor;
                    break;

                default:
                    Debug.Log("Can't find vehicle of type: " + vehicleType);
                    break;
            }
        }

        public void LoadAllFromOther(GoodsData otherGoodsData, bool takeHowMuchYouCan = true)
        {
            // I want to take all goods
            if (!takeHowMuchYouCan)
            {
                // Whoops, too much for me!
                if (GoodsData.GetAllWeight() + otherGoodsData.GetAllWeight() > maxCapacity)
                {
                    Debug.LogWarning($"Vehicle cannot take other goods, it's more thant its max capacity." +
                        $"current capacity:{GoodsData.GetAllWeight()}, " +
                        $"other weights:{otherGoodsData.GetAllWeight()}, " +
                        $"max capacity:{maxCapacity}");
                    return;
                }

                GoodsData.LoadFromOther(otherGoodsData);
            }

            // I want to take as much as I have capacity
            if (takeHowMuchYouCan)
            {

            }
        }
    }
}
