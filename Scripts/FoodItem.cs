using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class FoodItem : MonoBehaviour
{
    [Header("Food Information")]
    public FoodData foodData;

    [Header("Rotation")]
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 60f;

    private Rigidbody rb;
    private Collider foodCollider;
    private bool isStacked;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        foodCollider = GetComponent<Collider>();
    }

    private void Update()
    {
        if (isStacked)
        {
            transform.Rotate(
                rotationAxis.normalized * rotationSpeed * Time.deltaTime,
                Space.Self
            );
        }
    }

    public void Initialize(FoodData data)
    {
        foodData = data;
    }

    public void SetStacked(bool stacked)
    {
        isStacked = stacked;
        rb.isKinematic = stacked;
        foodCollider.enabled = !stacked;

        if (stacked)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
            rb.velocity = Vector3.zero;
#endif
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void Throw(Vector3 direction, float force)
    {
        isStacked = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        foodCollider.enabled = true;

        rb.AddForce(direction * force, ForceMode.Impulse);
        rb.AddTorque(
            Random.insideUnitSphere * 3f,
            ForceMode.Impulse
        );

        Destroy(gameObject, 8f);
    }
}