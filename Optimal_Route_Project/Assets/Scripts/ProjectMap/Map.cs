using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using Assets.Scripts;
using TMPro;


[ExecuteInEditMode]
public class Map : MonoBehaviour
{
    public float allTripLength = 0;

    [SerializeField]
    private TextMeshPro textMeshProComponent;

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
    public GameObject animalShelterPoint;
    [SerializeField]
    private GameObject areasParent;
    [SerializeField]
    [Header("Map additional sprites")]
    private Sprite CatSprite;


    [Space(10f)]

    [SerializeField]
    private Trip trip;
    [Header("Lists of objects")]
    public List<GameObject> points = new List<GameObject>();
    public List<GameObject> vehicles = new List<GameObject>();
    public GameObject startWarehouse;



    private Dictionary<string, string> pointsConnections = new Dictionary<string, string>();

    private const string prefixMarket = "Market_";
    private const string prefixWarehouse = "Warehouse_";
    private const string prefixAnimalShelter = "AnimalShelter";
    private const string prefixCatSpot = "_CatSpot";

    private const string totalDistanceTraveledText = "Total distance traveled by all vehicles - ";

    private void Awake()
    {
        textMeshProComponent = GetComponent<TextMeshPro>();
        areasParent = GameObject.FindGameObjectWithTag("Areas");

    }

    private void Update()
    {
        
        textMeshProComponent.text = totalDistanceTraveledText + allTripLength.ToString() + " km";
        textMeshProComponent.color = Color.white;
        textMeshProComponent.fontSize = 36;
        textMeshProComponent.alignment = TextAlignmentOptions.Center;
        textMeshProComponent.text = textMeshProComponent.text.ToUpper();
        textMeshProComponent.fontStyle = FontStyles.Bold;
    }


    [ContextMenu("Spawn whole map")]
    private void CreateMap()
    {
        ClearAreas();

        allTripLength = 0;

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

        Debug.Log("Animal Shelter " + counterNames + " coordinates: " + points[counterNames].transform.position);


        Debug.Log("Number of points in list: " + points.Count);


    }


    [ContextMenu("Clear map")]
    private void ClearMap()
    {

        allTripLength = 0;

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

        ClearAreas();
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
        vehicles ??= new List<GameObject>();

        ClearVehicles();
        if (Config.GetInstance() != null)
            numberOfVehicles = Random.Range(Config.GetInstance().MinVehicles, Config.GetInstance().MaxVehicles + 1);
        else
            numberOfVehicles = Random.Range(3, 7);

        if (numberOfVehicles <= 0 || vehiclePrefab == null)
            return;

        List<GameObject> warehouses = points.Where(p => p.GetComponent<Point>().PointType == PointType.Warehouse).ToList();
        int randomWarehouseIndex = Random.Range(0, warehouses.Count);
        startWarehouse = warehouses[randomWarehouseIndex];

        for (int i = 0; i < numberOfVehicles; i++)
        {
            Vector3 positionAroundWarehouse = startWarehouse.transform.position + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), -5);
            GameObject createdVehicle = Instantiate(vehiclePrefab, positionAroundWarehouse, Quaternion.identity);
            createdVehicle.name = $"Vehicle_{i}";
            createdVehicle.GetComponent<Vehicle>().enabled = true;
            vehicles.Add(createdVehicle);
        }

        int carWithCatIndex = Random.Range(0, numberOfVehicles);
        var carWithCat = vehicles[carWithCatIndex];
        carWithCat.GetComponent<Vehicle>().hasCat = true;
        carWithCat.transform.Find("Cat_Spot").GetComponent<SpriteRenderer>().sprite = CatSprite;
    }

    [ContextMenu("Clear vehicles")]
    private void ClearVehicles()
    {
        foreach (GameObject vehicleObject in vehicles)
        {
            DestroyImmediate(vehicleObject);
        }
        vehicles.Clear();

        ClearAreas();
    }
    private void RunTripSolver(bool runWithGoodsData = false)
    {
        trip = new Trip(this.points, this.vehicles, this.startWarehouse);
        trip.Run(runWithGoodsData);

        var tripDatas = trip.GetTripDatas();
        foreach (TripData tripData in tripDatas)
        {
            tripData.Vehicle.tripPoints = tripData.TripPoints.ToArray();
            tripData.Vehicle.ComputeRouteLenght();
            allTripLength += tripData.Vehicle.routeLength;
            Debug.Log($"Vehicle {tripData.Vehicle.name} route equals {tripData.Vehicle.routeLength}");
        }
        Debug.Log($"All route length equals {allTripLength}");

        ClearAreas();

        int counter = 0;
        foreach (var limitation in trip.limitations)
        {
            GameObject rect = new GameObject();
            var sprite = rect.AddComponent<SpriteRenderer>();

            rect.transform.position = new Vector3(limitation.x + limitation.width / 2 + counter * limitation.width, limitation.y + limitation.height / 2 + counter * limitation.height, 0);
            rect.transform.localScale = new Vector3(limitation.width, limitation.height, 1);
            sprite.color = Random.ColorHSV(0, 1, 0.2f, 0.3f, 0.95f, 1f);
            sprite.sprite = GameObject.Find("Background_Sprite").GetComponent<SpriteRenderer>().sprite;

            rect.transform.SetParent(areasParent.transform);
        }

        
    }

    
    private void FindSpotForCat()
    {
        int randomIndex = Random.Range(0, vehicles.Count);
        GameObject randomVehicle = vehicles[randomIndex];

        randomVehicle.name += prefixCatSpot;

        SpriteRenderer catSpot = randomVehicle.transform.Find("Cat_Spot").GetComponent<SpriteRenderer>();

        catSpot.sprite = CatSprite;
    }

    [ContextMenu("Clear Areas")]
    private void ClearAreas()
    {
        List<GameObject> objectsToDestroy = new List<GameObject>();

        foreach (Transform child in areasParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                objectsToDestroy.Add(child.gameObject);
            }
        }

        foreach (GameObject obj in objectsToDestroy)
        {
            DestroyImmediate(obj);
        }
    }

    private void ShowRouteLength()
    {

        foreach (GameObject vehicle in vehicles)
        {
            Vehicle vehicleScript = vehicle.GetComponent<Vehicle>();
            if (vehicleScript != null)
            {
                vehicleScript.displayRouteLength = !vehicleScript.displayRouteLength;
            }
        }

    }

    [ContextMenu("Allow Vehicle Movement")]
    private void AllowVehicleMovement()
    {

        foreach (GameObject vehicle in vehicles)
        {
            Vehicle vehicleScript = vehicle.GetComponent<Vehicle>();
            if (vehicleScript != null)
            {
                vehicleScript.allowVehicleMovement = !vehicleScript.allowVehicleMovement;
            }
        }

    }

    void OnValidate()
    {
        if (Random.seed != randomSeed)
            Random.InitState(randomSeed);
    }

    #region  Map Buttons

    [EditorToolsButtons.Button(name: "Spawn Whole Map", space: 15f)]
    private void SpawnWholeMapButton() => CreateMap();
    [EditorToolsButtons.Button(name: "Clear Map", space: 5f)]
    private void ClearMapButton() => ClearMap();
    [EditorToolsButtons.Button(name: "Gizmo On/Off", space: 5f)]
    private void GizmoActivationButton() => GizmoActivation();
    [EditorToolsButtons.Button(name: "Create Vehicles", space: 5f)]
    private void CreateVehiclesButton() => CreateVehicles();
    [EditorToolsButtons.Button(name: "Run Vehicles", space: 5f)]
    private void RunTripSolverButton() => RunTripSolver();

    [EditorToolsButtons.Button(name: "Run Vehicles With GoodsData", space: 5f)]
    private void RunTripSolverButtonWithGoods() => RunTripSolver(true);

    [EditorToolsButtons.Button(name: "Clear vehicles", space: 5f)]
    private void ClearVehiclesButton() => ClearVehicles();

    [EditorToolsButtons.Button(name: "Show route length", space: 5f)]
    private void ShowRouteLengthButton() => ShowRouteLength();
    [EditorToolsButtons.Button(name: "Allow Vehicle Movement", space: 5f)]
    private void AllowVehicleMovementButton() => AllowVehicleMovement();

    //[EditorToolsButtons.Button(name: "Add Points to Dictionary", space: 5f)]
    //private void AddPointsToDictionaryButton() => AddPointsToDictionary();
    //[EditorToolsButtons.Button(name: "Print Dictionary", space: 5f)]
    //private void PrintDictionaryButton() => PrintDictionary();

    #endregion
}





