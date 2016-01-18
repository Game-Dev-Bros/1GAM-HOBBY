using UnityEngine;

public class SliderController : MonoBehaviour
{
    public GameObject pointer;
    public float minValue, maxValue;
    public float minAngle, maxAngle;
    public bool invertRotation;
    public float value;

    public void UpdateSlider(float value = 0)
    {
        this.value = Mathf.Min(Mathf.Max(value, minValue), maxValue);
        float angle = (maxAngle - minAngle) * (value / (maxValue - minValue)) + minAngle;

        if(invertRotation)
        {
            angle = -angle;
        }

        pointer.transform.eulerAngles = new Vector3(0, 0, angle);
    }

    void OnValidate()
    {
        UpdateSlider(value);
    }
}
