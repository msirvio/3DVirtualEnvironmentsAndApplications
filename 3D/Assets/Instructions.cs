using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Instructions : MonoBehaviour
{
    private TextMeshProUGUI text;
    private bool inputIsController = false;

    public string keyBoardControls = "K_RENAME_THIS";
    public string controllerControls = "C_RENAME_THIS";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        inputIsController = (Gamepad.current != null);
        
        if (inputIsController) {
            text.text = controllerControls;
        } else {
            text.text = keyBoardControls;
        }
    }
}