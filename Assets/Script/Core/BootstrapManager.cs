using System;
using System.Collections;
using UnityEngine;

public class BootstrapManager : MonoBehaviour
{
    public static event Action OnGameReady;

    [Header("Manager References")]
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private UnitManager _unitManager;

    private void Awake()
    {
        StartCoroutine(InitializeGameSystems());
    }

    private IEnumerator InitializeGameSystems()
    {
        Utils.ColorLog("=== GAME INITIALIZATION START ===", "Yellow");

        yield return RegisterServices();

        yield return VerifyServices();

        yield return InitializeGameLogic();
    }

    private IEnumerator RegisterServices()
    {
        Utils.ColorLog("--- Registering Services ---", "Cyan");

        RegisterService(_gameManager, "GameManager");
        yield return null;

        RegisterService(_gridManager, "GridManager");
        yield return null;

        RegisterService(_tileManager, "TileManager");
        yield return null;

        RegisterService(_cameraManager, "CameraManager");
        yield return null;

        RegisterService(_turnManager, "TurnManager");
        yield return null;

        RegisterService(_unitManager, "UnitManager");
        yield return null;

        Utils.ColorLog("--- All Services Registered ---", "Cyan");

    }

    private void RegisterService(IServiceMB service, string serviceName)
    {
        if (service != null)
        {
            service.Register();
            Utils.ColorLog($"✓ Registered : {serviceName}", "Green");
        }
        else
        {
            Utils.ErrorLog($"✗ Failed to register: {serviceName} (null reference)");
        }
    }

    private IEnumerator VerifyServices()
    {
        Utils.ColorLog("--- Verifying Services ---", "Yellow");

        bool allServicesReady = true;

        allServicesReady &= VerifyService<IGameManagerReadService>("IGameManagerReadService");
        allServicesReady &= VerifyService<IGridManagerService>("IGridManagerService");
        allServicesReady &= VerifyService<ITileManagerService>("ITileManagerService");
        allServicesReady &= VerifyService<ICameraManagerService>("ICameraManagerService");
        allServicesReady &= VerifyService<ITurnManagerService>("ITurnManagerService");
        allServicesReady &= VerifyService<IUnitManagerService>("IUnitManagerService");

        if (!allServicesReady)
        {
            Utils.ErrorLog("CRITICAL : Not all services are ready !");
            yield break;
        }

        Utils.ColorLog("--- All Services Ready ---", "Green");
        yield return null;
    }

    private bool VerifyService<T>(string serviceName) where T : class
    {
        T service = ServiceLocator.Get<T>();
        if (service != null)
        {
            Utils.ColorLog($"✓ Verified : {serviceName}", "Green");
            return true;
        }
        else
        {
            Utils.ErrorLog($"✗ Missing : {serviceName}");
            return false;
        }
    }

    private IEnumerator InitializeGameLogic()
    {
        Utils.ColorLog("--- Initializing Game Logic ---", "Magenta");

        Utils.ColorLog("=== GAME READY ===", "Yellow");
        OnGameReady?.Invoke();

        yield return null;

        // Démarrer la génération de la grille
        IGridManagerService gridManager = ServiceLocator.Get<IGridManagerService>();
        if (gridManager != null)
        {
            gridManager.GenerateGrid();
            Utils.ColorLog("Grid generation initiated", "Yellow");
        }

        yield return null;
    }
}
