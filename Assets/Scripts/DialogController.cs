using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public Image background;
    public Text dialogText;
    public Image button;
    public Text buttonText;

    public Action currentAction;

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

    IEnumerator WriteDialogText(Action action)
    {
        dialogText.text = "";

        if(action != null)
        {
            if(action.interactable)
            {
                dialogText.alignment = TextAnchor.UpperCenter;
                dialogText.text = action.text;
                yield return null;
            }
            else
            {
                dialogText.alignment = TextAnchor.UpperLeft;

                for(int i = 0; i < action.text.Length && isVisible; i++)
                {
                    dialogText.text += action.text[i];
                    yield return new WaitForSeconds(0.025f);
                }
            }
        }
    }

    public bool Show(params Action[] actions)
    {
        isVisible = false;

        foreach(Action action in actions)
        {
            if(action.active)
            {
                isVisible = true;
                if(currentAction != action)
                {
                    currentAction = action;
                    StartCoroutine(WriteDialogText(action));
                }

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
