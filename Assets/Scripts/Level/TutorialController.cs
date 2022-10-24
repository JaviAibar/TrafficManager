using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public Transform target;
    public Transform mask;
    private Vector3 initialPosition;
    bool moving;

    [ContextMenu("Move to target")]
    public void PointToTarget()
    {
        initialPosition = mask.position;
        moving = true;
    }

    private void Update()
    {
        if (moving)
        {
         /*0   print("mask: " + mask.position);
            print("target: " + target.position);
            print("init: " + initialPosition);

            print("Lerp: " + Vector3.Lerp(mask.position, target.position, 0.1f));
            */mask.Translate(Vector3.Lerp(Vector3.Min(mask.position, target.position), Vector3.Max(mask.position, target.position), Time.deltaTime *  0.000000000000001f));
            print(Vector3.Distance(mask.position, target.position));
            moving = Vector3.Distance(mask.position, target.position) >= 0.5f;
        }
    }
}
