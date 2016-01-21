using System;

[Serializable]
public class Action
{
    public bool interactable = true;
    public bool active = true;
    public string text;
    public string tag;
    public float duration;
    public float statModifier;
}
