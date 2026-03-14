using UnityEngine;

public class SOObject : MonoBehaviour
{
    public UnitData data;

    private void Start()
    {
        if (gameObject.name.Contains("(1)"))
        {
            data.MaxMovement = 1;
        }
    }

    public void ShowData()
    {
        Utils.ColorLog($"MaxMovement {data.MaxMovement}", "Green");
    }
}
