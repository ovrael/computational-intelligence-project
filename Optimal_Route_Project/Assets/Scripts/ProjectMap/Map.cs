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

    [Space(10f)]

    public List<GameObject> points = new List<GameObject>();
    public List<GameObject> vehicles = new List<GameObject>();

    


    private Dictionary<string, string> pointsConnections = new Dictionary<string, string>();

    private const string prefixMarket = "Market_";
    private const string prefixWarehouse = "Warehouse_";


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
        }


        if (numberOfVehicles <= 0 || vehiclePrefab == null)
            return;

        List<Vector3> warehousesLocalizations = points.Where(p => p.GetComponent<Point>().PointType == PointType.Warehouse).Select(p => p.transform.position).ToList();
        int randomWarehouseIndex = Random.Range(0, warehousesLocalizations.Count);


        for (int i = 0; i < numberOfVehicles; i++)
        {
            Vector3 randomWarehouse = warehousesLocalizations[randomWarehouseIndex] + new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), -5);
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
    private void ClearVehiclesButton() => ClearVehicles();

    #endregion
}





