using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    //Animator
    [SerializeField] private Animator animator;

    // [SerializeField] private float startScale = 0f;
    // [SerializeField] private float endScale = 1f;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private float knockback = 0f;
    private float timeSinceStarted = 0f;
    private Vector3 rotationDirection = Vector3.zero;
    private float rotationSpeed = 0f;
    [SerializeField] private float rotationMaxSpeed = 0f;
    [SerializeField] private float destroyDelay = 0f;
    [SerializeField] private CubeController cube;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(CubeController _cube)
    {
        cube = _cube;

    }

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = Random.Range(0f, rotationMaxSpeed);
        rotationDirection = new Vector3(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f));
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate
        transform.Rotate(rotationDirection * rotationSpeed * Time.deltaTime);

        timeSinceStarted += Time.deltaTime;
        if (timeSinceStarted >= lifetime)
        {
            Die();
        }
    }

    //Die
    public void Die()
    {
        animator.SetTrigger("Die");
        Destroy(gameObject, destroyDelay);
    }

    //OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Damagable damagable = other.gameObject.GetComponent<Damagable>();
            if (damagable != null)
                damagable.Die((other.transform.position - cube.transform.position) * knockback, 0f, false);
            // other.gameObject.GetComponent<WallController>()?.Die();
        }
    }

    //OnCollisionEnter
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Damagable damagable = collision.gameObject.GetComponent<Damagable>();
            if (damagable != null)
                damagable.Die(Vector2.zero, 0f, false);
        }
    }

    // private void OnCollisionEnter(Collider other)
    // {
    //     if (other.gameObject.tag == "Enemy")
    //     {
    //         Damagable damagable = other.gameObject.GetComponent<Damagable>();
    //         if (damagable != null)
    //             damagable.Die(0f);
    //         // other.gameObject.GetComponent<WallController>()?.Die();
    //     }
    // }
}
