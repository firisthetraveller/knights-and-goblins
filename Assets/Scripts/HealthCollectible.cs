using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public int amountRestored;

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.gameObject.CompareTag("Unit"))
            return;
        UnitController controller = other.GetComponent<UnitController>();
        if (controller && controller.Health < controller.maxHealth) {
            controller.ChangeHealth(amountRestored);
            Destroy(gameObject);
        }
    }
}
