using UnityEngine;
using UnityEngine.UIElements;

public class UIHealthIcon : MonoBehaviour {
    public static bool displayHealthBars = true;

    [HideInInspector]
    public int currentHealth;

    private TextMesh text;

    private void Start() {
        text = GetComponentInChildren<TextMesh>();
    }

    /// <param name="value">A value between 0 and 10.</param>
    public void SetHealthValue(int value) {
        currentHealth = value;
        text.text = value.ToString();
        gameObject.SetActive(displayHealthBars && value > 0 && value < 10);
    }

    public void FlipSprite()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}