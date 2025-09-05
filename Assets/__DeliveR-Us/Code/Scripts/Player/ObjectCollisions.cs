using UnityEngine;

public class ObjectCollisions : MonoBehaviour
{
    [SerializeField] private AudioSource _impactSrc;
    [SerializeField] private float minImpactSpeed = 0.1f;
    [SerializeField] private float maxImpactSpeed = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Portal>(out Portal portal))
        {
            portal.Interact();
        }
        else if (other.TryGetComponent(out Checkout checkout))
        {
            if (OrderManager.Instance.OrderCollected) return;

            checkout.OnStartInteraction();
        }
        else if (other.TryGetComponent(out Deliver deliver))
        {
            deliver.OnStartInteraction();
            deliver.UpdateLookTarget(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Checkout checkout))
        {
            checkout.OnStopInteraction();
        }
        else if (other.TryGetComponent(out Deliver deliver))
        {
            deliver.OnStopInteraction();
            deliver.UpdateLookTarget(null);

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_impactSrc == null) return;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) return;

        float impactSpeed = collision.relativeVelocity.magnitude;
        float volume = 0f; // declare at top

        if (impactSpeed >= minImpactSpeed)
        {
            float normalized = Mathf.InverseLerp(minImpactSpeed, maxImpactSpeed, impactSpeed);
            volume = normalized * AudioManager.Instance.GetAudioSettings.SfxVolume;
            _impactSrc.PlayOneShot(_impactSrc.clip, volume);
        }

        // Check if the scooter collided with a car
        CarWheels car = collision.gameObject.GetComponent<CarWheels>();
        if (car == null) return;

        Vector3 pushDir = (transform.position - collision.transform.position).normalized;
        float force = 10f;
        rb.AddForce(pushDir * force, ForceMode.Impulse);

        Destroy(collision.gameObject);

        Debug.Log($"🛵 Scooter collided with car. Impact speed: {impactSpeed:F2}, Volume: {volume:F2}");
    }


}
