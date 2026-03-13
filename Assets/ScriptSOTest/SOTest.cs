using UnityEngine;

public class SOTest : MonoBehaviour
{
    public SOObject SOObject1;
    public SOObject SOObject2;

    private void Update()
    {
        SOObject1.ShowData();
        SOObject2.ShowData();
    }
}
