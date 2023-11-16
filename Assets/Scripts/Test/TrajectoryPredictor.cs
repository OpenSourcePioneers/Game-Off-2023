using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPredictor : MonoBehaviour
{
    [SerializeField] private Transform projectile;
    [SerializeField] private Transform target;
    [SerializeField] private float projHeight;

    Rigidbody projRb;
    // Start is called before the first frame update
    void Start()
    {
        projRb = Instantiate(projectile, transform.position, transform.rotation).GetComponent<Rigidbody>();
        projRb.velocity = Predict();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 Predict()
    {
        float g = -Physics.gravity.magnitude;
        Vector3 s = target.position - transform.position;
        float h = s.y + projHeight;
        if(h < projHeight)
            h = projHeight;
        Vector3 disInXZ = new Vector3(s.x, 0f, s.z);
        Vector3 velY = Vector3.up * Mathf.Sqrt(-2 * g * h);
        Vector3 velXZ = disInXZ / (Mathf.Sqrt(-2 * h/g) + Mathf.Sqrt(2 *(s.y - h)/g));
        return velY + velXZ;
    }
}
