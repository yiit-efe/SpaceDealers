using UnityEngine;

public class SpawnRise : MonoBehaviour
{
    public float riseHeight = 1.5f;
    public float riseSpeed = 2f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool rising = true;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + Vector3.up * riseHeight;

        // Ensure physics won't move the spawned object while it rises: make Rigidbody kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    void Update()
    {
        if (!rising) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, riseSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            rising = false;
        }
    }
}
