using UnityEngine;
using System.Collections.Generic;

public interface ICameraService
{
    public void RandomizeCameraSelection(List<Camera> availableCamera);

    public void SetCameraPositionFromGrid(Vector3Int GridSize, float CellSize, List<Camera> availableCamera);
}
