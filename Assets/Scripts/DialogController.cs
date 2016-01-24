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

    public bool Show(params Action[] actions)
    {
        isVisible = false;

        foreach(Action action in actions)
        {
            if(action.active)
            {
                isVisible = true;
                currentAction = action;
                dialogText.text = action.text;
                break;
            }
        }

        return isVisible;
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
        color.a = (currentAction != null && currentAction.interactable) ? color.a : 0;
        button.color = color;

        color = buttonText.color;
        color.a = isVisible ? 1 : 0;
        color.a = (currentAction != null && currentAction.interactable) ? color.a : 0;
        buttonText.color = color;
    }
}
