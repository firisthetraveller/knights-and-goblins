using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    UnitController controller;
    bool moveLeft = true;
    public float patrolTime = 1.0f;

    private void Start()
    {
        controller = GetComponent<UnitController>();
        StartCoroutine(ToggleDirection());
    }

    private IEnumerator ToggleDirection()
    {        
        Debug.Log($"Starting patrol...");
        while (controller.IsAlive())
        {            
            controller.move = moveLeft ? Vector2.left : Vector2.right;
            yield return new WaitForSeconds(patrolTime);
            moveLeft = !moveLeft;
        }
    }
}
