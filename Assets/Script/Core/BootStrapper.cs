using UnityEngine;

public class BootStrapper : MonoBehaviour
{
    private void Awake()
    {
        ITileService tileService = GetComponent<ITileService>();
        ServiceLocator.Register<ITileService>(tileService);

        IGridService gridService = GetComponent<IGridService>();
        ServiceLocator.Register<IGridService>(gridService);

        ICameraService cameraService = GetComponent<ICameraService>();
        ServiceLocator.Register<ICameraService>(cameraService);

    }
}
