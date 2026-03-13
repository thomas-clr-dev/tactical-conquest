using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour, IServiceMB,  ICameraManagerService
{
    [SerializeField] private List<Camera> _availableCamera;
    private Camera _camPlayerOne;
    private Camera _camPlayerTwo;


    private void Awake()
    {
        Register();
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

        while (Cam1Key == Cam2Key)
        {
            Cam2Key = Random.Range(0, _availableCamera.Count);
        }

        Camera CamP1 = _availableCamera[Cam1Key];
        Camera CamP2 = _availableCamera[Cam2Key];

        Utils.SafeSetActive(CamP1.gameObject, true);
        Utils.SafeSetActive(CamP2.gameObject, true);

        CamP1.fieldOfView = CamP2.fieldOfView = 75;

        CamP1.gameObject.name += "(player 1)";
        CamP2.gameObject.name += "(player 2)";

        CamP1.rect = new Rect(0, 0, 0.5f, 1f);
        CamP2.rect = new Rect(0.5f, 0, 0.5f, 1f);
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
}
