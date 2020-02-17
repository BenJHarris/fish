using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public GameObject boidPrefab;

    public BoidGroup group;

    public int numToSpawn;

    public int spawnRadius;

    void Start()
    {
        group = new BoidGroup();

        for (int i = 0; i < numToSpawn; i++)
        {
            GameObject instance = Instantiate(boidPrefab, Random.insideUnitSphere * spawnRadius + transform.position, Random.rotation);
            instance.transform.parent = transform;
            Boid boid = instance.GetComponent<Boid>();
            group.AddBoid(boid);
            boid.Initialize(group);
        }
    }
}
