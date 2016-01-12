using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public Image background;
    public Text dialogText;
    public Image button;
    public Text buttonText;

    private Action currentAction;

    private bool isVisible;

    void Update()
    {
        if(!isVisible)
        {
            dialogText.text = "";
            currentAction = null;
        }

        Animate();
    }

    public void Hide()
    {
        isVisible = false;
    }

    public void Show(params Action[] actions)
    {
        isVisible = true;

        // hardcoded for a single dialog
        currentAction = actions[0];
        dialogText.text = actions[0].text;
    }

    private void Animate()
    {
        Color color = background.color;
        color.a = isVisible ? 1 : 0;
        background.color = color;

        color = dialogText.color;
        color.a = isVisible ? 1 : 0;
        dialogText.color = color;

        color = button.color;
        color.a = isVisible ? 1 : 0;
        button.color = color;

        color = buttonText.color;
        color.a = isVisible ? 1 : 0;
        buttonText.color = color;
    }
}
