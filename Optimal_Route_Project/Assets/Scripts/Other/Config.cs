using Assets.Scripts;
using UnityEngine;

[ExecuteAlways]
public class Config : MonoBehaviour
{
    private static Config instance;

    [Header("Vehicles data")]
    [SerializeField]
    private int minVehicles = 3;
    public int MinVehicles { get => minVehicles; }

    [SerializeField]
    private int maxVehicles = 6;
    public int MaxVehicles { get => maxVehicles; }

    [SerializeField]
    private Color redVehicleColor = Utils.CreateColor(150, 30, 30);
    public Color RedVehicleColor { get => redVehicleColor; }

    [SerializeField]
    private Color greenVehicleColor = Utils.CreateColor(30, 140, 60);
    public Color GreenVehicleColor { get => greenVehicleColor; }

    [SerializeField]
    private Color blueVehicleColor = Utils.CreateColor(30, 70, 160);
    public Color BlueVehicleColor { get => blueVehicleColor; }

    [Header("Goods data")]

    [SerializeField]
    private int minGoodsWeight = 100;
    public int MinGoodsWeight
    {
        get { return minGoodsWeight; }
        private set { if (value >= 0 && value <= MaxGoodsWeight) minGoodsWeight = value; }
    }

    [SerializeField]
    private int maxGoodsWeight = 200;
    public int MaxGoodsWeight
    {
        get => maxGoodsWeight;
        set { if (value >= 0 && value >= MinGoodsWeight) maxGoodsWeight = value; }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            DestroyImmediate(instance);
    }

    public static Config GetInstance()
    {
        return instance;
    }
}