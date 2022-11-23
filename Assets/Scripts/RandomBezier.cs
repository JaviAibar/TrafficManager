using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BezierSolution;
using Level;

public class RandomBezier : MonoBehaviour
{
    public GameObject bezierContainer;
    BezierSpline[] splines;
    public RoadUser roadUser;
    private void Awake()
    {
        splines = FindObjectsOfType<BezierSpline>();
        roadUser = GetComponent<RoadUser>() ?? GetComponentInChildren<RoadUser>();
        if (roadUser)
        {
            int random = Random.Range(0, splines.Length);
            roadUser.Spline = splines[random];
            SpriteRenderer rend = roadUser.GetComponent<SpriteRenderer>();
            rend.sortingOrder = int.Parse(splines[random].name.Split(" ")[1]) - 3;
            rend.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            roadUser.normalSpeed = roadUser.normalSpeed + Mathf.Lerp(0.6f, 1.3f, Random.Range(0f, 1f));
            roadUser.TimeToLoop = -1;
        }

    }



    private void Update()
    {
        if (roadUser.bezier.NormalizedT >= 1) Destroy(gameObject, 1f);
    }

}
