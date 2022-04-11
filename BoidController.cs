using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    [SerializeField] GameObject boid1;
    [SerializeField] GameObject boid2;
    [SerializeField] GameObject boid3;
    [SerializeField] GameObject boid4;

    private GameObject[] boids;
    private Vector3 velocity;

    private float maxSpeed = 5f;
    private float maxForce = 1f;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        boids = new GameObject[] {boid1, boid2, boid3, boid4};
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        Vector3 acc = steer();
        rb.AddForce(acc, ForceMode.Acceleration);
        transform.LookAt(transform.position + rb.velocity);
    }

    private Vector3 steer(){
        Vector3 acc = Vector3.zero;

        acc = align() + cohesion() + seperation();
        return acc;
    }

    private Vector3 align(){
        Vector3 acc = new Vector3(0, 0, 0);
        for(int i = 0; i < boids.Length; i++) {
            GameObject b = boids[i];
            Rigidbody rbo = b.GetComponent<Rigidbody>();
            Vector3 v = rbo.velocity;
            acc = acc + v;
        }

        acc = acc / 4;
        acc = Vector3.Normalize(acc) * maxSpeed;
        acc = acc - rb.velocity;
        acc = Vector3.ClampMagnitude(acc, maxForce);

        return acc;
    }

    private Vector3 cohesion(){
        Vector3 pos = new Vector3(0, 0, 0);
        for(int i = 0; i < boids.Length; i++) {
            GameObject b = boids[i];
            Rigidbody rbo = b.GetComponent<Rigidbody>();
            Vector3 p = rbo.position;
            pos += p;
        }

        pos = pos / 4;
        pos -= rb.position;
        pos = Vector3.Normalize(pos) * maxSpeed;
        pos -= rb.velocity;
        pos = Vector3.ClampMagnitude(pos, maxForce);

        return pos;
    }

    private Vector3 seperation(){
        Vector3 acc = Vector3.zero;
        for(int i = 0; i < boids.Length; i++) {
            GameObject b = boids[i];
            Rigidbody rbo = b.GetComponent<Rigidbody>();
            Vector3 diff = rb.position - rbo.position;
            diff = Vector3.Normalize(diff);
            acc += diff;
        }
        
        acc /= 4;
        acc = Vector3.Normalize(acc) * 4;
        acc -= rb.velocity;
        acc = Vector3.ClampMagnitude(acc, maxForce);

        return acc;
    }
}
