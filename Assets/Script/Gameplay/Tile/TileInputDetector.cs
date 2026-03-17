using System;
using UnityEngine;

public class TileInputDetector : MonoBehaviour, IServiceMB, ITileInputDetectorService
{
    private ICameraManagerService _iCameraManagerService;

    private LayerMask _tileLayerMask;

    public void Register()
    {
        ServiceLocator.Register<ITileInputDetectorService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<ITileInputDetectorService>(this);
    }

    private void Start()
    {
        _tileLayerMask = LayerMask.GetMask("Tiles");

        BootstrapManager.OnGameReady += OnGameReady;
    }

    private void OnGameReady()
    {
        _iCameraManagerService = ServiceLocator.Get<ICameraManagerService>();

        if (_iCameraManagerService != null)
        {
            Utils.ColorLog("TileInputDetector: Camera service acquired", "Green");
        }
        else
        {
            Utils.ErrorLog("TileInputDetector: CameraManagerService not found!");
        }
    }

    private void Update()
    {
        if (_iCameraManagerService == null) return;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            DetectTileClick();
        }
    }

    private void DetectTileClick()
    {
        Camera activeCamera = _iCameraManagerService.GetCameraAtScreenPosition(Input.mousePosition);

        if (activeCamera == null)
        {
            Utils.ErrorLog("No active camera found at mouse position !");
            return;
        }

        Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, _tileLayerMask))
        {
            TileView tileView = hit.collider.GetComponent<TileView>();

            if (tileView != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    tileView.TriggerLeftClick();
                    Utils.ColorLog($"Raycast ({activeCamera.name}) : LEFT clicked on {tileView.name}", "Cyan");
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    tileView.TriggerRightClick();
                    Utils.ColorLog($"Raycast ({activeCamera.name}) : RIGHT clicked on {tileView.name}", "Cyan");
                }
            }
            else
            {
                Utils.ColorLog($"Raycast hit {hit.collider.name} but no TileView found", "Yellow");
            }
        }
    }

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;
        Unregister();
    }
}