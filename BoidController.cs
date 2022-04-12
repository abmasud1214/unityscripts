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

    private Vector3[] fibonacci_dir;

    private float maxSpeed = 5f;
    private float maxForce = 1f;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        boids = new GameObject[] {boid1, boid2, boid3, boid4};
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 5);

        fibonacci_dir = fibonacci_sphere();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        Vector3 acc = steer();

        if (collisionCourse()){
            Debug.Log("Collision Course");
            Vector3 collisionDir = viableDir();
            Vector3 avoidAcc = Vector3.Normalize(acc) * maxSpeed;
            avoidAcc -= rb.velocity;
            avoidAcc = Vector3.ClampMagnitude(avoidAcc, maxForce) * 5;
            acc += avoidAcc;
        }

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

    private bool collisionCourse() {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        return Physics.SphereCast(ray, 0.5f, 3f);
    }

    private Vector3 viableDir() {
        Vector3[] dir = fibonacci_dir;

        Transform t = this.transform;
        for (int i = 0; i < dir.Length; i++) {
            Vector3 d = t.TransformDirection(dir[i]);
            Ray ray = new Ray(t.position, d);
            if (!(Physics.SphereCast(ray, 0.5f, 3f))){ // TODO: Add parameters for hit radius and maxdistance and add layermask
                return d;
            }
        }

        return t.forward;
    }

    private Vector3[] fibonacci_sphere(){
        Vector3[] dir = new Vector3[300];
        int SAMPLES = 300;

        float phi = Mathf.PI * (3f - Mathf.Sqrt(5f));

        for (int i = 0; i < SAMPLES; i++){
            float y = 1 - (i / (SAMPLES - 1)) * 2;
            float radius = Mathf.Sqrt(1 - y * y);

            float theta = phi * i;

            float x = Mathf.Cos(theta) * radius;
            float z = Mathf.Sin(theta) * radius;

            Vector3 d = new Vector3(x, y, z);
            dir[i] = d;
        }

        return dir;
    }
}
