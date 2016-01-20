using UnityEngine;

public class StairsController : MonoBehaviour
{
    public int steps;
    public PlayerController.PlayerOrientation ascendingDirection;

    [HideInInspector]
    public Bounds bounds;

    void Awake()
    {
        bounds = GetComponent<BoxCollider2D>().bounds;
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerController>().SetStairs(this);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            other.GetComponent<PlayerController>().SetStairs(null);
        }
    }
}
