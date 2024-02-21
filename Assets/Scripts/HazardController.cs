using System.Collections;
using UnityEngine;

public class HazardController : MonoBehaviour
{
    Animator animator;
    public int damage;

    [Header("Animation")]
    public float delayDestroyInSeconds;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Unit"))
            return;

        UnitController controller = other.GetComponent<UnitController>();
        if (controller)
        {
            if (animator) animator.SetTrigger("Boom");
            
            controller.ChangeHealth(-damage);
            Destroy(gameObject, delayDestroyInSeconds);
        }
    }
}