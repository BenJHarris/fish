using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidGroup
{

    public List<Boid> members;
    public int Size
    {
        get => members.Count;
    }

    public BoidGroup(List<Boid> startingFlock)
    {
        members = startingFlock;
    }

    public BoidGroup()
    {
        members = new List<Boid>();
    }

    public void AddBoid(Boid boid)
    {
        members.Add(boid);
    }

    public void RemoveBoid(Boid boid)
    {
        members.Remove(boid);
    }

    public BoidGroup FindBoidsInRange(Boid boid, float range)
    {
        return new BoidGroup(members.FindAll(
            b =>
                boid != b &&
                Vector3.Distance(boid.transform.position, b.transform.position) < range
            ));
    }

    public Vector3 CalculateAveragePosition()
    {
        Vector3 total = Vector3.zero;
        foreach (Boid boid in members)
        {
            total += boid.transform.position;
        }
        return total / members.Count;
    }

    public Vector3 CalculateAverageHeading()
    {
        Vector3 total = Vector3.zero;
        foreach (Boid boid in members)
        {
            total += boid.transform.forward;
        }
        return total / members.Count;
    }

    public Vector3 CalculateAvoidance(Boid boid)
    {
        Vector3 total = Vector3.zero;
        foreach (Boid b in members)
        {
            Vector3 difference = boid.transform.position - b.transform.position;
            total += difference.normalized / difference.magnitude;
        }

        return total / members.Count;
    }
}
