using UnityEngine;

public class PlayerController : MonoBehaviour {
    UnitController controller;

    private void Start() {
        controller = GetComponent<UnitController>();
    }

    private void Update() {
        controller.move = new(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        controller.move.Normalize();
    }
}