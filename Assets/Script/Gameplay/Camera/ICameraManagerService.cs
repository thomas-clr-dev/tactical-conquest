using System.Collections.Generic;
using UnityEngine;

public interface ICameraManagerService
{
    public void SetCameraPositionFromGrid(int gridLenthX, int gridLengthY, float CellSize);
}
