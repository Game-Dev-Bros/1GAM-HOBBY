using UnityEngine;

public class SliderController : MonoBehaviour
{
    public GameObject pointer;
    public float moveSpeed;
    public float minValue, maxValue;
    public float minAngle, maxAngle;
    public bool invertRotation;
    public float value;

    void Awake()
    {
        value = PlayerPrefs.GetFloat("Pointer");
        UpdateSlider(value);
    }

    void Update()
    {
        float distanceToMiddle = (value - (maxValue - minValue) / 2);
        int moveDirection = distanceToMiddle > 0 ? 1 : -1;
        
        value += moveSpeed * moveDirection * Time.deltaTime;
        UpdateSlider(value);
    }

    public void UpdateSlider(float value = 0)
    {
        this.value = Mathf.Min(Mathf.Max(value, minValue), maxValue);

        float angle = (maxAngle - minAngle) * (value / (maxValue - minValue)) + minAngle;

        if(invertRotation)
        {
            angle = -angle;
        }

        if(pointer != null)
        {
            pointer.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    void OnValidate()
    {
        UpdateSlider(value);
    }
}
