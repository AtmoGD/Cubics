using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject diePrefab;
    [SerializeField] private float speed;
    [SerializeField] private float maxLifetime;
    [SerializeField] private float damage;
    [SerializeField] private float timeLeft = 0f;
    public GameObject Sender { get; set; }

    private void Start()
    {
        Sender = null;
    }

    public void StartFlying()
    {
        timeLeft = maxLifetime;

        rb.velocity = transform.forward * -speed;
    }

    private void FixedUpdate()
    {
        timeLeft -= Time.fixedDeltaTime;

        if (timeLeft <= 0)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == Sender)
            return;

        Damagable damagable = other.gameObject.GetComponent<Damagable>();

        if (damagable != null)
        {
            damagable?.TakeDamage(damage);
            if (diePrefab)
                Instantiate(diePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
