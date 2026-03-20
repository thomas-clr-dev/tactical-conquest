using System;
using UnityEngine;

public class UnitView : MonoBehaviour
{
    public int OwnerPlayerId { get; private set; }
    public UnitData UnitData { get; private set; }

    public event Action<UnitView> OnUnitDestroyed;

    private Renderer _unitRenderer;
    private Material _originalMaterial;

    private void Awake()
    {
        _unitRenderer = GetComponentInChildren<Renderer>();
        if (_unitRenderer != null)
        {
            _originalMaterial = _unitRenderer.material;
        }
    }

    public void Initialize(int playerId, UnitData unitData)
    {
        OwnerPlayerId = playerId;
        UnitData = unitData;

        gameObject.name = $"{unitData.UnitName}_Player{playerId}";
    }

    public void SetPlayerColor(Color playerColor)
    {
        if (_unitRenderer != null)
        {
            _unitRenderer.material.color = playerColor;
        }
    }

    public bool IsOwnedBy(int playerId)
    {
        return OwnerPlayerId == playerId;
    }

    private void OnDestroy()
    {
        OnUnitDestroyed?.Invoke(this);
    }
}