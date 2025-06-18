using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 200f;
    [SerializeField] private float rayDrawLength = 2f;
    [Header("Lifetime Settings")]
    [SerializeField] private float destroyAfterCollisionTime = 5f;
    [SerializeField] private float maxLifetime = 10f;

    private Rigidbody rb;
    private bool hasCollided = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.useGravity = false; // havada süzülsün
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.velocity = transform.forward * speed;
            // rb.AddForce(transform.forward * 200, ForceMode.Force);
        }

        Destroy(gameObject, maxLifetime);
    }

    void Update()
    {
        // Debug draw 
        Debug.DrawRay(transform.position, transform.forward * rayDrawLength, Color.red);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        // Dummy var mý?
        DummyTarget dummy = collision.collider.GetComponentInParent<DummyTarget>();
        if (dummy != null)
            dummy.OnHit(collision.GetContact(0).point);

        if (rb) rb.useGravity = true;
        Destroy(gameObject, destroyAfterCollisionTime);
    }

}
