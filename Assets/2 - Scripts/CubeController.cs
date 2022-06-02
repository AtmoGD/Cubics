using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CubeController : MonoBehaviour, Damagable
{
    public static CubeController instance;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private ParticleSystem dashParticles;
    [SerializeField] private ParticleSystem.EmissionModule dashParticlesEmission;
    [SerializeField] private bool startWithFullMana = false;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField] private float laserOffset = 0.1f;
    [SerializeField] private float dashCosts = 2f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashMaxSpeed = 10f;
    [SerializeField] private float dashDuration = 0.5f;
    public float DashDuration { get { return dashDuration; } }
    [SerializeField] private float wallDieDelayDuringDash = 0.2f;
    [SerializeField] private float dashManaRegeneration = 0f;
    [SerializeField] private float shieldCosts = 1f;
    [SerializeField] private float shieldDuration = 1f;
    public float ShieldDuration { get { return shieldDuration; } }
    public float Mana { get; set; }
    [SerializeField] private float maxMana = 100f;
    [SerializeField] private float manaSectionSize = 40f;
    [SerializeField] private ShieldController shield;
    public float ManaSections { get { return Mana / manaSectionSize; } }
    public Vector2 FlyDirection { get; private set; }
    public Vector3 RotateDirection { get; private set; }
    public Vector3 TargetLookAt { get; private set; }
    private float dashTimeLeft = 0f;
    private float shieldTimeLeft = 0f;
    public float ShieldTimeLeft { get { return shieldTimeLeft; } }
    public float DashCooldown { get { return dashTimeLeft; } }
    public bool IsDashing { get { return dashTimeLeft > 0f; } }
    private float dashEmissionRate = 0f;

    public bool IsShooting { get; private set; }
    [SerializeField] private float shootCooldown = 0.1f;
    private float actualShootCooldown = 0f;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        FlyDirection = Vector2.zero;
        RotateDirection = Vector2.zero;
        TargetLookAt = Vector3.zero;
        Mana = startWithFullMana ? maxMana : 0f;

        dashParticlesEmission = dashParticles.emission;
        dashEmissionRate = dashParticlesEmission.rateOverTime.constant;
    }

    private void Update()
    {
        dashTimeLeft -= Time.deltaTime;
        shieldTimeLeft -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (FlyDirection.magnitude > 0.3)
        {
            float _maxSpeed = maxSpeed;
            float _speed = speed;

            if (dashTimeLeft > 0)
            {
                _maxSpeed = dashMaxSpeed;
                _speed = dashSpeed;

                dashParticlesEmission.rateOverTime = dashEmissionRate;
            }
            else
            {
                dashParticlesEmission.rateOverTime = 0f;
            }

            Vector3 movement = FlyDirection.normalized * _speed * Time.fixedDeltaTime;

            Vector3 newVelocity = Vector3.Lerp(rb.velocity, rb.velocity + movement, Time.fixedDeltaTime);

            rb.velocity = Vector3.ClampMagnitude(newVelocity, _maxSpeed);





            // if (RotateDirection.magnitude > 0.1)
            // {
            //     TargetLookAt = Vector3.Lerp(TargetLookAt, RotateDirection.normalized, rotationSpeed);
            //     TargetLookAt *= 0.999f;
            // } else
            // {
            //     TargetLookAt = Vector3.Lerp(TargetLookAt, Vector3.zero, rotationSpeed);
            //     TargetLookAt *= 0.999f;
            // }
        }

        // TargetLookAt = Vector3.Lerp(transform.forward, FlyDirection.normalized, rotationSpeed);

        if (RotateDirection.magnitude > 0.3)
            TargetLookAt = Vector3.Lerp(TargetLookAt.normalized, RotateDirection.normalized, rotationSpeed);
        else
            TargetLookAt = Vector3.Lerp(TargetLookAt.normalized, FlyDirection, rotationSpeed);

        // Quaternion targetRotation = Quaternion.LookRotation(targetLookAt);
        // if (targetRotation != Quaternion.identity)
        //     rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, rotationSpeed);

        if (TargetLookAt != Vector3.zero)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(TargetLookAt), rotationSpeed);
        // transform.rotation = Quaternion.LookRotation(targetLookAt);
        // transform.rotation = Quaternion.LookRotation(TargetLookAt);

        actualShootCooldown -= Time.fixedDeltaTime;

        if (IsShooting)
        {
            if (actualShootCooldown <= 0)
            {
                Shoot();
                actualShootCooldown = shootCooldown;
            }
        }
    }

    public void TakeDamage(float damage)
    {

    }

    public void Die(Vector2 knockback, float delay, bool addMana, bool addScore, bool removeFromList)
    {
        Destroy(gameObject);
    }

    public void Shoot()
    {
        Vector3 lookAt = Vector3.zero;

        if(RotateDirection.magnitude > 0.1)
            lookAt = transform.position + RotateDirection.normalized * laserOffset;
        else
            lookAt = transform.position + (Vector3)FlyDirection.normalized * laserOffset;


        LaserController laser = Instantiate(laserPrefab, lookAt, Quaternion.identity).GetComponent<LaserController>();
        laser.Sender = gameObject;
        // Vector3 lookAt = laser.transform.position + TargetLookAt.normalized;
        laser.transform.LookAt(transform.position + (lookAt - transform.position).normalized);

        // laser.transform.LookAt(-(laser.transform.position + TargetLookAt));
        laser.StartFlying();
    }

    public void AddMana(float amount)
    {
        if (dashTimeLeft > 0)
            Mana += amount * dashManaRegeneration;
        else
            Mana += amount;

        Mana = Mathf.Clamp(Mana, 0f, maxMana);
    }

    public float GetMaxMana() { return maxMana; }

    public void MovementInput(InputAction.CallbackContext context)
    {
        FlyDirection = context.ReadValue<Vector2>();
    }

    public void ShootInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsShooting = true;
            // Vector3 dir = 

        }
        else if (context.canceled)
        {
            IsShooting = false;
        }
    }
    public void RotateToMouse(InputAction.CallbackContext context)
    {
        if (Camera.main)
        {
            Vector2 mousePos = context.ReadValue<Vector2>();

            Vector3 lookAt = Camera.main.ScreenPointToRay(mousePos).GetPoint(Camera.main.transform.position.z);
            lookAt.z = 0f;
            // Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
            // worldPos.z = 0;

            // Vector3 direction = (lookAt - transform.position).normalized;
            Vector3 direction = transform.position - lookAt;
            RotateDirection = direction;
        }
    }

    public void RoateToDirection(InputAction.CallbackContext context)
    {
        RotateDirection = context.ReadValue<Vector2>();
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        if (context.performed && dashTimeLeft <= 0 && ManaSections >= dashCosts)
        {
            dashTimeLeft = dashDuration;
            Mana -= dashCosts * manaSectionSize;
        }
    }

    public void ShieldInput(InputAction.CallbackContext context)
    {
        if (context.performed && shieldTimeLeft <= 0 && ManaSections >= shieldCosts)
        {
            ShieldController shield = Instantiate(this.shield, transform.position, Quaternion.identity, this.transform).GetComponent<ShieldController>();
            shield.Init(this);
            Mana -= shieldCosts * manaSectionSize;
            shieldTimeLeft = shieldDuration;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (dashTimeLeft > 0f)
            {
                Damagable damagable = collision.gameObject.GetComponent<Damagable>();

                if (damagable != null)
                    damagable.Die(Vector2.zero, wallDieDelayDuringDash, true, true, true);
            }
        }
    }
}
