using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour, IServiceMB,  ICameraManagerService
{
    [SerializeField] private List<Camera> _availableCamera;
    private Camera _camPlayerOne;
    private Camera _camPlayerTwo;

    private void Start()
    {
        BootstrapManager.OnGameReady += OnGameReady;
    }
    private void OnGameReady()
    {
        RandomizeCameraSelection();
    }

    public void Register()
    {
        ServiceLocator.Register<ICameraManagerService>(this);
    }

    public void Unregister()
    {
        ServiceLocator.Unregister<ICameraManagerService>(this);
    }

    public void RandomizeCameraSelection()
    {
        int Cam1Key = Random.Range(0, _availableCamera.Count);
        int Cam2Key = Random.Range(0, _availableCamera.Count);

        int whileFlag = 0;
        while (Cam1Key == Cam2Key || whileFlag < 10)
        {
            whileFlag++;
            Cam2Key = Random.Range(0, _availableCamera.Count);
        }

        _camPlayerOne = _availableCamera[Cam1Key];
        _camPlayerTwo = _availableCamera[Cam2Key];

        Utils.SafeSetActive(_camPlayerOne.gameObject, true);
        Utils.SafeSetActive(_camPlayerTwo.gameObject, true);

        _camPlayerOne.fieldOfView = _camPlayerTwo.fieldOfView = 75;

        _camPlayerOne.gameObject.name += "(player 1)";
        _camPlayerTwo.gameObject.name += "(player 2)";

        _camPlayerOne.rect = new Rect(0, 0, 0.5f, 1f);
        _camPlayerTwo.rect = new Rect(0.5f, 0, 0.5f, 1f);
    }

    public Camera GetCameraAtScreenPosition(Vector3 screenPosition)
    {
        if (_camPlayerOne == null || _camPlayerTwo == null)
        {
            Utils.ErrorLog("Cameras not initialized !");
        }

        float normalizedX = screenPosition.x / Screen.width;

        if (normalizedX < 0.5)
        {
            return _camPlayerOne;
        }
        else
        {
            return _camPlayerTwo;
        }
    }

    public Camera GetPlayerCamera(int playerID)
    {
        return playerID == 1 ? _camPlayerOne : _camPlayerTwo;
    }

    public void SetCameraPositionFromGrid(int gridLenthX, int gridLengthY, float CellSize)
    {
        Vector3 gridCenter = new Vector3(
            (gridLenthX * CellSize) / 2f - 0.5f,
            0,
            (gridLengthY * CellSize) / 2f - 0.5f
        );

        float distance = Mathf.Max(gridLenthX, gridLengthY) * CellSize * 0.6f; // Distance du centre
        float height = distance * 1.5f; // Hauteur proportionnelle
        float[] angles = { 0f, 90f, 180f, 270f }; // Angles autour de la grille (4 côtés)

        for (int i = 0; i < _availableCamera.Count; i++)
        {
            float angleRad = angles[i] * Mathf.Deg2Rad;

            // Position circulaire autour du centre à distance égale
            Vector3 position = new Vector3(
                gridCenter.x + Mathf.Sin(angleRad) * distance,
                height,
                gridCenter.z + Mathf.Cos(angleRad) * distance
            );

            _availableCamera[i].transform.position = position;

            // Orienter la caméra vers le centre de la grille
            _availableCamera[i].transform.LookAt(gridCenter);
        }
    }

    private void OnDestroy()
    {
        BootstrapManager.OnGameReady -= OnGameReady;
        Unregister();
    }
}
