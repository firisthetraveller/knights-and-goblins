using UnityEngine;

public class Projectile : MonoBehaviour {
    Rigidbody2D body2D;
    Animator animator;

    [Header("Projectile settings")]
    public int damage;
    public float timeToLiveInSeconds;

    [HideInInspector]
    public GameObject originShooter;

    private void Awake() {
        body2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        Destroy(gameObject, timeToLiveInSeconds);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject == originShooter) {
            return;
        }

        UnitController controller = other.gameObject.GetComponent<UnitController>();
        
        if (controller) {
            controller.ChangeHealth(-damage);
        }
        body2D.simulated = false;
        animator.SetTrigger("Stuck");
        Destroy(gameObject, 1.5f);
    }

    public void FlipSprite()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void Launch(Vector2 direction, float force) {
        body2D.AddForce(direction * force);
    }
}