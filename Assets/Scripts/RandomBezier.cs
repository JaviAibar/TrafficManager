using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;
using Level;

public class RandomBezier : MonoBehaviour
{
    [SerializeField] private GameObject bezierContainer;
    private BezierSpline[] splines;
    [SerializeField] private RoadUser roadUser;
    private void Awake()
    {
        splines = FindObjectsOfType<BezierSpline>();
        roadUser = GetComponent<RoadUser>() ?? GetComponentInChildren<RoadUser>();
        if (roadUser)
        {
            roadUser.Spline = GetARandomSpline();
            SpriteRenderer rend = roadUser.GetComponent<SpriteRenderer>();
            rend.sortingOrder = GetSortingOrderFromSpline();
            rend.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            roadUser.NormalSpeed = roadUser.NormalSpeed + GetRandomSpeedVariability();
            roadUser.Bezier.speed = roadUser.NormalSpeed;
        }

    }

    private float GetRandomSpeedVariability()
    {
        return Mathf.Lerp(0.6f, 1.3f, Random.Range(0f, 1f));
    }

    private int GetSortingOrderFromSpline()
    {
        // Spline name has the info of sorting order
        return int.Parse(roadUser.Spline.name.Split(" ")[1]) - 3;
    }

    private BezierSpline GetARandomSpline()
    {
        return splines[Random.Range(0, splines.Length)];
    }

    private void Update()
    {
        
        if (roadUser.Bezier.NormalizedT >= 0.99)
        {
            Destroy(gameObject, 1f);
        }
    }

}
