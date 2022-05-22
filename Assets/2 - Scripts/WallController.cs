using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour, Damagable
{
    // wall which has a small amount of health. walls always going to cubeController
    private Rigidbody rb;
    private Animator animator;
    [SerializeField] private int pointsWorth;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeedMin;
    [SerializeField] private float rotationSpeedMax;
    [SerializeField] private float dieDelay = 0f;
    private float health = 100f;
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private GameObject diePrefab;

    private CubeController controller;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        health = maxHealth;

        controller = CubeController.instance;

        rb.angularVelocity = Random.insideUnitSphere * Random.Range(rotationSpeedMin, rotationSpeedMax);
    }

    private void FixedUpdate() {
        if(isDead) return;
        
        Vector3 movement = (controller.transform.position - transform.position) * speed * Time.fixedDeltaTime;
        rb.velocity = movement;
        // rb.MovePosition(Vector3.MoveTowards(transform.position, controller.transform.position, speed * Time.fixedDeltaTime));
    }

    //Set Die Delay
    public void SetDieDelay(float delay) {
        dieDelay = delay;
    }

    public void Die(Vector2 knockback, float delay = 0f, bool addMana = true) {
        if (isDead) return;
        
        isDead = true;

        if(knockback != Vector2.zero) {
            rb.AddForce(knockback, ForceMode.Impulse);
        }

        if (diePrefab) Instantiate(diePrefab, transform.position, Quaternion.identity);
        
        GameController.instance.AddScore(pointsWorth, addMana, 1f, 1f, 0.05f);

        animator.SetTrigger("Die");

        Destroy(gameObject, dieDelay);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        animator.SetTrigger("Hit");

        if (health <= 0)
        {
            Die(Vector2.zero);
        }
    }
}
