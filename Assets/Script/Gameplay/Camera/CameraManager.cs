using UnityEngine;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour, ICameraService
{
    private Camera _camPlayerOne;
    private Camera _camPlayerTwo;

    public void RandomizeCameraSelection(List<Camera> availableCamera)
    {
        int Cam1Key = Random.Range(0, availableCamera.Count);
        int Cam2Key = Random.Range(0, availableCamera.Count);

        while (Cam1Key == Cam2Key)
        {
            Cam2Key = Random.Range(0, availableCamera.Count);
        }

        Camera CamP1 = availableCamera[Cam1Key];
        Camera CamP2 = availableCamera[Cam2Key];

        Utils.SafeSetActive(CamP1.gameObject, true);
        Utils.SafeSetActive(CamP2.gameObject, true);

        CamP1.fieldOfView = CamP2.fieldOfView = 75;

        CamP1.gameObject.name += "(player 1)";
        CamP2.gameObject.name += "(player 2)";

        CamP1.rect = new Rect(0, 0, 0.5f, 1f);
        CamP2.rect = new Rect(0.5f, 0, 0.5f, 1f);
    }

    //Vector3[] camPosition =
    //{
    //    new Vector3(1, 1, 2),
    //    new Vector3(1, 1, 0),
    //    new Vector3(2, 1, 1),
    //    new Vector3(0, 1, 1)
    //};

    //Vector3[] camRotation =
    //{
    //    new Vector3(60, -180, 0),
    //    new Vector3(60, 0, 0),
    //    new Vector3(60, -90, 0),
    //    new Vector3(60, 90, 0),
    //};

    //public void SetCameraPositionFromGrid(Vector3Int GridSize, float CellSize, List<Camera> availableCamera)
    //{
    //    float MidGridWidth = (GridSize.x * CellSize) / 2;
    //    float MidGridLenght = (GridSize.z * CellSize) / 2;

    //    for (int i = 0; i < availableCamera.Count; i++)
    //    {
    //        Vector3 position = new Vector3(
    //            camPosition[i].x * MidGridWidth - 0.5f,
    //            camPosition[i].y * 7.5f,
    //            camPosition[i].z * MidGridLenght - 0.5f
    //        );

    //        availableCamera[i].transform.position = position;
    //        availableCamera[i].transform.rotation = Quaternion.Euler(camRotation[i]);
    //    }
    //}

    public void SetCameraPositionFromGrid(Vector3Int GridSize, float CellSize, List<Camera> availableCamera)
    {
        Vector3 gridCenter = new Vector3(
            (GridSize.x * CellSize) / 2f -0.5f,
            0,
            (GridSize.z * CellSize) / 2f -0.5f
        );

        float distance = Mathf.Max(GridSize.x, GridSize.z) * CellSize * 0.6f; // Distance du centre
        float height = distance * 1.5f; // Hauteur proportionnelle
        float[] angles = { 0f, 90f, 180f, 270f }; // Angles autour de la grille (4 côtés)

        for (int i = 0; i < availableCamera.Count; i++)
        {
            float angleRad = angles[i] * Mathf.Deg2Rad;

            // Position circulaire autour du centre à distance égale
            Vector3 position = new Vector3(
                gridCenter.x + Mathf.Sin(angleRad) * distance,
                height,
                gridCenter.z + Mathf.Cos(angleRad) * distance
            );

            availableCamera[i].transform.position = position;

            // Orienter la caméra vers le centre de la grille
            availableCamera[i].transform.LookAt(gridCenter);
        }
    }

}
