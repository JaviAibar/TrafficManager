using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{

    [SerializeField] private GameObject[] vehicles;
    [Range(0,1)]
    [SerializeField] private float probability = 0.025f;
    [SerializeField] private float size = 0.24f;
    private readonly Vector3 PositionOutOfSight = new Vector3(1000, 1000, 0);

    private void Update()
    {
        if (Random.Range(0f, 1f) <= probability)
        {
            GameObject roadUserGO = Instantiate(PickRandomCar(), PositionOutOfSight, Quaternion.identity);
            roadUserGO.transform.localScale = new Vector3(size, size, size);
            roadUserGO.AddComponent(typeof( RandomBezier));
        }
    }

    private GameObject PickRandomCar()
    {
        print($"Rand: {Random.Range(0, vehicles.Length)}");
        return vehicles[Random.Range(0, vehicles.Length)];
    }
}
