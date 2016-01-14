using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpriteRendererController : MonoBehaviour
{
    public int zOrder;
    public bool fixedZOrder;
    public int selectedOption;
    public Material spriteMaterial;

    private bool _hideChildrenInInspector;
    public bool hideChildrenInInspector
    {
        get
        {
            return _hideChildrenInInspector;
        }
        set
        {
            _hideChildrenInInspector = value;
            UpdateVisibilityInInspector();
        }
    }

    private void UpdateVisibilityInInspector()
    {
        foreach (Transform child in transform)
        {
            if(hideChildrenInInspector)
            {
                child.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                child.hideFlags = HideFlags.None;
            }
        }
        #if UNITY_EDITOR
        if(hideChildrenInInspector)
        {
            EditorUtility.SetDirty(gameObject);
        }
        #endif
    }

    public void RecalculateZOrder()
    {
        foreach (Transform child in transform)
        {
            if (!fixedZOrder)
            {
                zOrder = (int)-child.transform.position.y;
            }

            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                child.GetComponent<SpriteRenderer>().sortingOrder = zOrder;
            }
        }
    }

    public void UpdateMaterial()
    {
        foreach (Transform child in transform)
        {
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                child.GetComponent<SpriteRenderer>().material = spriteMaterial;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpriteRendererController))]
public class SpriteOrderEditor : Editor
{
    private string[] zOptions = new string[] { "Dynamic", "Fixed" };

    SpriteRendererController controller;

    public override void OnInspectorGUI()
    {
        if (controller == null)
        {
            controller = (SpriteRendererController)target;
        }

        controller.hideChildrenInInspector = EditorGUILayout.Toggle("Hide In Inspector", controller.hideChildrenInInspector);
        EditorGUILayout.Separator();
        GUI.enabled = !Application.isPlaying;

        controller.selectedOption = EditorGUILayout.Popup("Z-order Type", controller.selectedOption, zOptions);
        controller.fixedZOrder = (controller.selectedOption == 1);

        if (controller.fixedZOrder)
        {
            controller.zOrder = EditorGUILayout.IntField("Z-order Value", controller.zOrder);
        }

        if (GUILayout.Button("Recalculate Z-order"))
        {
            ((SpriteRendererController)target).RecalculateZOrder();
        }

        controller.spriteMaterial = (Material)EditorGUILayout.ObjectField("Sprite", controller.spriteMaterial, typeof(Material), false);

        GUI.enabled = GUI.enabled && (controller.spriteMaterial != null);
        if (GUILayout.Button("Update Material"))
        {
            ((SpriteRendererController)target).UpdateMaterial();
        }
    }
}
#endif
