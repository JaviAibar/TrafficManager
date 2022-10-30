using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    public GameObject[] vehicles;
    [Range(0,1)]
    public float probability = 0.025f;
    public float size = 0.24f;

    private void Update()
    {
        if (Random.Range(0f, 1f) <= probability)
        {
            GameObject roadUserGO = Instantiate(vehicles[Random.Range(0, vehicles.Length)], new Vector3(1000,1000,1000), Quaternion.identity);
            roadUserGO.transform.localScale = new Vector3(size, size, size);
            roadUserGO.AddComponent(typeof( RandomBezier));
        }
    }
}
