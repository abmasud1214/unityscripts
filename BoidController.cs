using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidController : MonoBehaviour
{
    [SerializeField] float viewRadius = 5f;
    [SerializeField] float viewAngle = 80f;

    // private GameObject[] boids;
    private List<GameObject> boids;
    private Vector3 velocity;

    private Vector3[] fibonacci_dir;

    private float maxSpeed = .2f;
    private float maxForce = .5f;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 5);
        velocity = rb.velocity;

        fibonacci_dir = fibonacci_sphere();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        LayerMask m = LayerMask.GetMask("Boid");
        boids = new List<GameObject>();
        Collider[] boidColliders = Physics.OverlapSphere(transform.position, viewRadius, m);
        foreach (Collider c in boidColliders){
            GameObject b = c.gameObject;
            b = b.transform.parent.gameObject;
            float ang = Vector3.Angle(transform.forward, b.transform.position - transform.position);
            if (ang < viewAngle) {
                boids.Add(b);
            }
        }
        if (boids.Count > 0) {

            Vector3 acc = steer();

            if (collisionCourse()){
                Debug.Log("Collision Course");
                Vector3 collisionDir = viableDir();
                Vector3 avoidAcc = Vector3.Normalize(collisionDir) * maxSpeed;
                avoidAcc -= rb.velocity;
                avoidAcc = Vector3.ClampMagnitude(avoidAcc, maxForce) * 5;
                acc += avoidAcc;
            }

            rb.AddForce(acc, ForceMode.Acceleration);
            // rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
            transform.LookAt(transform.position + rb.velocity);
        }
    }

    private Vector3 steer(){
        Vector3 acc = Vector3.zero;

        Vector3 align = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        Vector3 seperation = Vector3.zero;
        for (int i = 0; i < boids.Count; i++){
            GameObject b = boids[i];
            Rigidbody rbo = b.GetComponent<Rigidbody>();

            // Align
            Vector3 v = rbo.velocity;
            align += v;

            // Cohesion
            Vector3 p = rbo.position;
            cohesion += p;

            // Seperation
            Vector3 diff = rb.position - rbo.position;
            diff = Vector3.Normalize(diff);
            cohesion += diff;
        }

        align = align / boids.Count;
        align = Vector3.Normalize(align) * maxSpeed;
        align -= rb.velocity;
        align = Vector3.ClampMagnitude(acc, maxForce);

        cohesion = cohesion / boids.Count;
        cohesion -= rb.position;
        cohesion = Vector3.Normalize(cohesion) * maxSpeed;
        cohesion -= rb.velocity;
        cohesion = Vector3.ClampMagnitude(cohesion, maxForce);

        seperation /= 4;
        seperation = Vector3.Normalize(acc) * 4;
        seperation -= rb.velocity;
        seperation = Vector3.ClampMagnitude(acc, maxForce);

        acc = align + cohesion + seperation;
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
