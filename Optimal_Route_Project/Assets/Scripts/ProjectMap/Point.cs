using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum PointType
{
    Market,
    Warehouse
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



    [Header("Connected Points info")]
    [SerializeField]
    private Vector2 closestPointPosition = Vector2.zero;

    [Header("Map Controller Reference")]
    [SerializeField]
    private Map MapController;

    private void Awake()
    {
        MapController = GameObject.FindGameObjectWithTag("MapController").GetComponent<Map>();
    }

    private void Update()
    {
        FindClosestPoint();
    }



    private void FindClosestPoint()
    {

        GameObject closestPoint = null;

        float minDist = Mathf.Infinity;

        currentPointPosition = transform.position;
        Vector2 nextPointPosition = Vector2.zero;



        foreach (GameObject point in MapController.points)
        {

            nextPointPosition = point.transform.position;

            float dist = Vector2.Distance(currentPointPosition, nextPointPosition);

            if (dist < minDist && dist != 0f)
            {
                minDist = dist;
                closestPointPosition = point.transform.position;
                closestPoint = point;


            }
        }


    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(currentPointPosition, closestPointPosition);
    }


}
