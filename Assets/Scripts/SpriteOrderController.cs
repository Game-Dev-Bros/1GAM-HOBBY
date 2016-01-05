using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpriteOrderController : MonoBehaviour
{
    public int zOrder;
    public bool fixedZOrder;
    public int selectedOption;

    public bool running;

    public void RecalculateZOrder()
    {
        running = true;

        foreach(Transform child in transform)
        {
            if(!fixedZOrder)
            {
                zOrder = (int) -child.transform.position.y;
            }

            child.GetComponent<SpriteRenderer>().sortingOrder = zOrder;
        }

        running = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpriteOrderController))]
public class SpriteOrderEditor : Editor
{
    private string[] zOptions = new string[] {"Dynamic", "Fixed" };

    SpriteOrderController controller;

    public override void OnInspectorGUI()
    {
        if(controller == null)
        {
            controller = (SpriteOrderController) target;
        }

        GUI.enabled = !Application.isPlaying && !controller.running;

        controller.selectedOption = EditorGUILayout.Popup("Z-order Type", controller.selectedOption, zOptions);
        controller.fixedZOrder = (controller.selectedOption == 1);

        if(controller.fixedZOrder)
        {
            controller.zOrder = EditorGUILayout.IntField("Z-order Value", controller.zOrder);
        }

        if (GUILayout.Button("Recalculate Z-order"))
        {
            ((SpriteOrderController)target).RecalculateZOrder();
        }
    }
}
#endif
