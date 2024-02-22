using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance {get; private set;}

    VisualElement healthBar;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        healthBar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1f);
    }

    /// <param name="value">A number between 0 and 1</param>
    public void SetHealthValue(float value) {
        healthBar.style.width = Length.Percent(value * 100f);
    }
}
