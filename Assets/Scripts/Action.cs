using System;
using UnityEngine;

[Serializable]
public class Action
{
    public bool interactable = true;
    public bool active = true;
    public string text;
    public string tag;
    public int duration;
    public float statModifier;
}
