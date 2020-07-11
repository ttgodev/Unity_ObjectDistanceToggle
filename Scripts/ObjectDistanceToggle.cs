using UnityEngine;

public class ObjectDistanceToggle : MonoBehaviour
{
    public float lod0 = 35;
    public int assignedIndex;

    private void Start()
    {
        Register();
    }

    public void Register()
    {
        if (ObjectDistanceToggleManager.Instance != null)
        {
            assignedIndex = ObjectDistanceToggleManager.Instance.RegisterObject(this);
        }
    }

    private void OnDestroy()
    {
        if (ObjectDistanceToggleManager.Instance != null)
        {
            ObjectDistanceToggleManager.Instance.RemoveObject(assignedIndex);
        }
    }

}