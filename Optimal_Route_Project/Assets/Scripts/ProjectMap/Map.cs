using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using Assets.Scripts;


[ExecuteInEditMode]
public class Map : MonoBehaviour
{

    [Header("Map parameters")]
    [SerializeField]
    private int randomSeed = 69420;

    [Space(10f)]

    [SerializeField]
    private int numberOfPoints;
    [SerializeField]
    private int numberOfWarehouses;
    [SerializeField]
    private int numberOfVehicles;

    [Space(10f)]

    [Header("Map objects")]
    [SerializeField]
    private GameObject marketPoint;
    [SerializeField]
    private GameObject warehousePoint;
    [SerializeField]
    private GameObject vehiclePrefab;
    [SerializeField]
    private GameObject animalShelterPoint;
    [SerializeField]
    [Header("Map additional sprites")]
    private Sprite CatSprite;

    [Space(10f)]

    [SerializeField]
    private Trip trip;
    public List<GameObject> points = new List<GameObject>();
    public List<GameObject> vehicles = new List<GameObject>();
    public GameObject startWarehouse;



    private Dictionary<string, string> pointsConnections = new Dictionary<string, string>();

    private const string prefixMarket = "Market_";
    private const string prefixWarehouse = "Warehouse_";
    private const string prefixAnimalShelter = "AnimalShelter";
    private const string prefixCatSpot = "_CatSpot";


    [ContextMenu("Spawn whole map")]
    private void CreateMap()
    {
        if (points != null)
        {
            ClearMap();
        }

       
        int counterNames = 0;


        do
        {

            Vector2 randomPosition = new Vector2(Random.Range(0, 100), Random.Range(0, 100));



            if (points.Count < numberOfWarehouses)
            {

                points.Add(Instantiate(warehousePoint, randomPosition, Quaternion.identity));
                points[counterNames].name = prefixWarehouse + counterNames.ToString();
            }
            else
            {

                points.Add(Instantiate(marketPoint, randomPosition, Quaternion.identity));
                points[counterNames].name = prefixMarket + counterNames.ToString();
            }

            

            Debug.Log("Point " + counterNames + " coordinates: " + points[counterNames].transform.position);

            counterNames++;


        } while (points.Count < numberOfPoints);


        points.Add(Instantiate(animalShelterPoint, new Vector3(points[0].transform.position.x + Random.Range(-6f, 6f), points[0].transform.position.y + Random.Range(-6f, 6f)), Quaternion.identity));
        points[counterNames].name = prefixAnimalShelter.ToString();

        Debug.Log("Point " + counterNames + " coordinates: " + points[counterNames].transform.position);


        Debug.Log("Number of points in list: " + points.Count);


    }


    [ContextMenu("Clear map")]
    private void ClearMap()
    {

        foreach (GameObject go in points)
        {
            DestroyImmediate(go);
        }

        foreach (GameObject vehicleObject in vehicles)
        {
            DestroyImmediate(vehicleObject);
        }

        points.Clear();
        pointsConnections.Clear();

        vehicles.Clear();
    }

    [ContextMenu("Add Points to dictionary")]
    private void AddPointsToDictionary()
    {

        float minDist = 0f;
        Vector2 currentPointPosition = Vector2.zero;
        Vector2 nextPointPosition = Vector2.zero;
        Vector2 closestPointPosition = Vector2.zero;
        GameObject chosenOne = null;


        for (int i = 0; i <= points.Count - 1; i++)
        {

            minDist = Mathf.Infinity;

            currentPointPosition = points[i].transform.position;

            for (int j = 0; j <= points.Count - 1; j++)
            {

                nextPointPosition = points[j].transform.position;


                float dist = Vector2.Distance(currentPointPosition, nextPointPosition);

                if (dist < minDist && dist != 0f)
                {
                    minDist = dist;
                    closestPointPosition = points[j].transform.position;
                    chosenOne = points[j];


                }
            }


            pointsConnections.Add(points[i].name, chosenOne.name);


        }
    }

    [ContextMenu("Print dictionary")]
    private void PrintDictionary()
    {
        foreach (var kvp in pointsConnections)
        {
            Debug.Log("Key: " + kvp.Key + " Value: " + kvp.Value);
        }
    }

    [ContextMenu("Activate Gizmos")]
    private void GizmoActivation()
    {
        SceneView.lastActiveSceneView.drawGizmos = !SceneView.lastActiveSceneView.drawGizmos;
    }

    [ContextMenu("Create vehicles")]
    private void CreateVehicles()
    {

        if (vehicles != null)
        {
            ClearVehicles();
        if (Config.GetInstance() != null)
            numberOfVehicles = Random.Range(Config.GetInstance().MinVehicles, Config.GetInstance().MaxVehicles + 1);
        else
            numberOfVehicles = Random.Range(3, 7);





        if (numberOfVehicles <= 0 || vehiclePrefab == null)
            return;

        List<GameObject> warehouses = points.Where(p => p.GetComponent<Point>().PointType == PointType.Warehouse).ToList();
        int randomWarehouseIndex = Random.Range(0, warehouses.Count);


            Vector3 positionAroundWarehouse = startWarehouse.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), -5);
            GameObject createdVehicle = Instantiate(vehiclePrefab, positionAroundWarehouse, Quaternion.identity);
            createdVehicle.name = $"Vehicle_{i}";
            createdVehicle.GetComponent<Vehicle>().enabled = true;
            GameObject createdVehicle = Instantiate(vehiclePrefab, randomWarehouse, Quaternion.identity);
            createdVehicle.name = $"Vehicle_{i} ({createdVehicle.GetComponent<Vehicle>().VehicleType})";

            vehicles.Add(createdVehicle);
        }
    }

    [ContextMenu("Clear vehicles")]
    private void ClearVehicles()
    {
        foreach (GameObject vehicleObject in vehicles)
        {
            DestroyImmediate(vehicleObject);
        }

        vehicles.Clear();
    private void RunTripSolver()
    {
        trip = new Trip(this.points, this.vehicles, this.startWarehouse);
        trip.Run();

        var tripDatas = trip.GetTripDatas();
        foreach (TripData tripData in tripDatas)
        {
            tripData.Vehicle.tripPoints = tripData.TripPoints.ToArray();
        }
    }

    [ContextMenu("Find spot for cat")]
    private void FindSpotForCat()
    {
        int randomIndex = Random.Range(0, vehicles.Count);
        GameObject randomVehicle= vehicles[randomIndex];

        randomVehicle.name += prefixCatSpot;

        SpriteRenderer catSpot = randomVehicle.transform.Find("Cat_Spot").GetComponent<SpriteRenderer>();

        catSpot.sprite = CatSprite;
    }

    }

    void OnValidate()
    {
        if (Random.seed != randomSeed)
            Random.InitState(randomSeed);
    }


    #region  Map Buttons

    [EditorToolsButtons.Button(name: "Spawn Whole Map", space: 5f)]
    private void SpawnWholeMapButton() => CreateMap();
    [EditorToolsButtons.Button(name: "Clear Map", space: 5f)]
    private void ClearMapButton() => ClearMap();
    [EditorToolsButtons.Button(name: "Add Points to Dictionary", space: 5f)]
    private void AddPointsToDictionaryButton() => AddPointsToDictionary();
    [EditorToolsButtons.Button(name: "Print Dictionary", space: 5f)]
    private void PrintDictionaryButton() => PrintDictionary();
    [EditorToolsButtons.Button(name: "Gizmo On/Off", space: 5f)]
    private void GizmoActivationButton() => GizmoActivation();
    [EditorToolsButtons.Button(name: "Create Vehicles", space: 5f)]
    private void CreateVehiclesButton() => CreateVehicles();
    [EditorToolsButtons.Button(name: "Clear vehicles", space: 5f)]
    [EditorToolsButtons.Button(name: "Run Vehicles", space: 5f)]
    private void RunTripSolverButton() => RunTripSolver();
    private void ClearVehiclesButton() => ClearVehicles();

    #endregion
}





