using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance { get; private set; }

    [Header("Settings")]
    public float displayDialogueTime = 4.0f;
    VisualElement dialogueBox;
    int dialogueCalls = 0;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UIDocument document = GetComponent<UIDocument>();

        dialogueBox = document.rootVisualElement.Q<VisualElement>("NPCDialogue");
        dialogueBox.style.display = DisplayStyle.None;
    }

    public void DisplayDialogue(string name, string content)
    {
        dialogueBox.style.display = DisplayStyle.Flex;
        dialogueBox.Q<Label>("Name").text = name;
        dialogueBox.Q<Label>("Dialogue").text = content;
        StartCoroutine(HideDialogueBox());
    }

    IEnumerator HideDialogueBox() {
        dialogueCalls++;
        yield return new WaitForSeconds(displayDialogueTime);
        dialogueCalls--;
        if (dialogueCalls == 0) {
            dialogueBox.style.display = DisplayStyle.None;
        }
    }
}