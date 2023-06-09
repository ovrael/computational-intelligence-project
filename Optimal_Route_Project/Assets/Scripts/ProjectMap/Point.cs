using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Assets.Scripts;

public enum PointType
{
    Market,
    Warehouse,
    Animal_Shelter
}

[ExecuteInEditMode]
public class Point : MonoBehaviour
{
    [Header("Point parameters")]
    [SerializeField]
    private PointType pointType;
    public PointType PointType { get { return pointType; } private set { pointType = value; } }
    [SerializeField]
    private int maxNumberOfConnections;
    [SerializeField]
    private Vector2 currentPointPosition = Vector2.zero;

    [Header("Map Controller Reference")]
    [SerializeField]
    private Map MapController;

    private Dictionary<GameObject, float> connectedPointsDistance = new Dictionary<GameObject, float>();
    //private Vector2 closestPointPosition = Vector2.zero;

    public int goods;
    public bool skip;

    public GoodsData goodsData;
    public bool IsDone { get { return goodsData.GetAllWeight() == 0; } }

    private void Awake()
    {
        MapController = GameObject.FindGameObjectWithTag("MapController").GetComponent<Map>();

        int wantsOrHave = Random.Range(0, 100);
        goods = Random.Range(100, 201);
        goods *= wantsOrHave < 50 ? -1 : 1;

        goodsData = GoodsData.RandomPointGoods();
    }

    private void Update()
    {

        //FindClosestPointsTest();

        //if (VisitedByCars.Length <= 0)
        //{
        //    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        //    spriteRenderer.color = new Color(1, 1, 1, 1);
        //    return;
        //}


        //if (VisitedByCars.Any(v => v == true))
        //{
        //    SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        //    float red = VisitedByCars.Count(v => v) / (float)(VisitedByCars.Length);
        //    spriteRenderer.color = new Color(red, 0, 0, 1);
        //}
    }

    //private void FindClosestPoints()
    //{

    //    GameObject closestPoint = null;

    //    float minDist = Mathf.Infinity;

    //    currentPointPosition = transform.position;
    //    Vector2 nextPointPosition = Vector2.zero;



    //    foreach (GameObject point in MapController.points)
    //    {

    //        nextPointPosition = point.transform.position;

    //        float dist = Vector2.Distance(currentPointPosition, nextPointPosition);

    //        if (dist < minDist && dist != 0f)
    //        {
    //            minDist = dist;
    //            closestPointPosition = point.transform.position;
    //            closestPoint = point;


    //        }
    //    }



    //}

    private void FindClosestPointsTest()
    {
        Vector2 nextPointPosition = Vector2.zero;

        currentPointPosition = transform.position;


        foreach (GameObject point in MapController.points)
        {
            nextPointPosition = point.transform.position;
            float dist = Vector2.Distance(currentPointPosition, nextPointPosition);


            if (!connectedPointsDistance.ContainsKey(point))
            {
                connectedPointsDistance.Add(point, dist);
            }

        }

        connectedPointsDistance = connectedPointsDistance.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

    }

    [ContextMenu("Print closest points data")]
    private void PrintPointConnections()
    {
        EditorUtils.ClearLog();

        for (int i = 1; i <= maxNumberOfConnections; i++)
        {

            KeyValuePair<GameObject, float> kvp = connectedPointsDistance.ElementAt(i);

            Debug.Log("Distance to: " + kvp.Key + "  from this point: " + kvp.Value);
        }
    }

    [ContextMenu("Print whole connection dictionary")]
    private void PrintWholeConnectionDictionary()
    {
        EditorUtils.ClearLog();

        for (int i = 1; i <= connectedPointsDistance.Count - 1; i++)
        {

            KeyValuePair<GameObject, float> kvp = connectedPointsDistance.ElementAt(i);

            Debug.Log("Distance to: " + kvp.Key + "  from this point: " + kvp.Value);
        }
    }


    void OnDrawGizmosSelected()
    {

        //for (int i = 1; i <= maxNumberOfConnections; i++)
        //{

        //    KeyValuePair<GameObject, float> kvp = connectedPointsDistance.ElementAt(i);

        //    Gizmos.DrawLine(currentPointPosition, kvp.Key.transform.position);


        //}


    }


}
