using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class UnitSelectionPanel : MonoBehaviour
{
    private UIDocument _uiDocument;
    private VisualElement _panel;
    private VisualElement _unitsContainer;
    private Label _totalLabel;
    
    private ITileManagerService _tileManager;
    private TileView _currentTile;
    private TileView _currentDestination;
    
    private List<UnitSelectionData> _currentSelections;

    private void Awake() // ✅ Utiliser Awake au lieu de Start
    {
        _uiDocument = GetComponent<UIDocument>();
        
        // ✅ Vérifier que le UIDocument existe
        if (_uiDocument == null)
        {
            Utils.ErrorLog("UIDocument component not found!");
            return;
        }
    }

    private void OnEnable() // ✅ Utiliser OnEnable pour accéder au rootVisualElement
    {
        // ✅ Attendre que le rootVisualElement soit disponible
        if (_uiDocument == null) return;
        
        if (_uiDocument.rootVisualElement == null)
        {
            Utils.ErrorLog("UIDocument rootVisualElement is null! Make sure UXML is assigned.");
            return;
        }

        // ✅ Maintenant on peut accéder au rootVisualElement en toute sécurité
        _panel = _uiDocument.rootVisualElement.Q<VisualElement>("UnitSelectionPanel");
        
        if (_panel == null)
        {
            Utils.ErrorLog("UnitSelectionPanel element not found in UXML!");
            return;
        }

        _unitsContainer = _panel.Q<VisualElement>("UnitsContainer");
        _totalLabel = _panel.Q<Label>("TotalLabel");
        
        _panel.style.display = DisplayStyle.None;

        // ✅ S'abonner à OnGameReady si pas déjà fait
        BootstrapManager.OnGameReady += OnGameReady;
    }

    private void OnGameReady()
    {
        _tileManager = ServiceLocator.Get<ITileManagerService>();

        var tileManager = _tileManager as TileManager;
        if (tileManager != null)
        {
            tileManager.OnUnitSelectionRequested += ShowSelectionPanel;
            tileManager.OnUnitSelectionCancelled += HideSelectionPanel;
        }

        // ✅ Vérifier que les éléments existent avant de s'abonner
        Button confirmBtn = _panel?.Q<Button>("ConfirmButton");
        Button cancelBtn = _panel?.Q<Button>("CancelButton");

        if (confirmBtn != null)
        {
            confirmBtn.clicked += OnConfirmClicked;
        }
        else
        {
            Utils.ErrorLog("ConfirmButton not found in UXML!");
        }

        if (cancelBtn != null)
        {
            cancelBtn.clicked += OnCancelClicked;
        }
        else
        {
            Utils.ErrorLog("CancelButton not found in UXML!");
        }

        Utils.ColorLog("UnitSelectionPanel initialized", "Green");
    }

    private void ShowSelectionPanel(List<UnitSelectionData> selections, TileView tile)
    {
        _currentTile = tile;
        _currentSelections = selections;
        _panel.style.display = DisplayStyle.Flex;

        GenerateUnitControls(selections);
        UpdateTotalLabel();
        
        Utils.ColorLog("Unit selection panel shown", "Cyan");
    }

    private void GenerateUnitControls(List<UnitSelectionData> selections)
    {
        _unitsContainer.Clear();

        foreach (var selection in selections)
        {
            VisualElement unitRow = new VisualElement();
            unitRow.AddToClassList("unit-row");

            Label nameLabel = new Label($"{selection.UnitType}");
            nameLabel.AddToClassList("unit-name");
            unitRow.Add(nameLabel);

            Label countLabel = new Label($": {selection.TotalAvailable}");
            countLabel.AddToClassList("unit-count");
            unitRow.Add(countLabel);

            VisualElement controls = new VisualElement();
            controls.AddToClassList("unit-controls");

            Button decrementBtn = new Button(() => OnDecrementClicked(selection));
            decrementBtn.text = "-";
            decrementBtn.AddToClassList("btn-decrement");
            controls.Add(decrementBtn);

            Label selectedLabel = new Label($"{selection.SelectedToMove}");
            selectedLabel.name = $"Selected_{selection.UnitType}";
            selectedLabel.AddToClassList("unit-selected");
            controls.Add(selectedLabel);

            Button incrementBtn = new Button(() => OnIncrementClicked(selection));
            incrementBtn.text = "+";
            incrementBtn.AddToClassList("btn-increment");
            controls.Add(incrementBtn);

            unitRow.Add(controls);
            _unitsContainer.Add(unitRow);
        }
    }

    private void OnIncrementClicked(UnitSelectionData selection)
    {
        if (selection.CanSelectMore())
        {
            selection.IncrementSelection();
            UpdateSelectedLabel(selection);
            UpdateTotalLabel();
            
            Utils.ColorLog($"Incremented {selection.UnitType}: {selection.SelectedToMove}/{selection.TotalAvailable}", "Green");
        }
        else
        {
            Utils.ColorLog($"Cannot select more {selection.UnitType} (must leave at least 1)", "Red");
        }
    }

    private void OnDecrementClicked(UnitSelectionData selection)
    {
        selection.DecrementSelection();
        UpdateSelectedLabel(selection);
        UpdateTotalLabel();
        
        Utils.ColorLog($"Decremented {selection.UnitType}: {selection.SelectedToMove}/{selection.TotalAvailable}", "Yellow");
    }

    private void UpdateSelectedLabel(UnitSelectionData selection)
    {
        Label selectedLabel = _unitsContainer.Q<Label>($"Selected_{selection.UnitType}");
        if (selectedLabel != null)
        {
            selectedLabel.text = $"{selection.SelectedToMove}";
        }
    }

    private void UpdateTotalLabel()
    {
        if (_currentSelections == null) return;

        int totalUnits = _currentSelections.Sum(s => s.TotalAvailable);
        int totalRemaining = _currentSelections.Sum(s => s.RemainingOnTile);
        int totalSelected = _currentSelections.Sum(s => s.SelectedToMove);

        _totalLabel.text = $"Total remaining: {totalRemaining}/{totalUnits} (Moving: {totalSelected})";
    }

    private void HideSelectionPanel()
    {
        _panel.style.display = DisplayStyle.None;
        _unitsContainer.Clear();
        _currentSelections = null;
    }

    private void OnConfirmClicked()
    {
        int totalSelected = _currentSelections?.Sum(s => s.SelectedToMove) ?? 0;
        
        if (totalSelected == 0)
        {
            Utils.ErrorLog("No units selected to move!");
            return;
        }

        Utils.ColorLog($"Confirmed selection: {totalSelected} unit(s) to move", "Green");

        var tileManager = _tileManager as TileManager;
        tileManager?.ConfirmUnitSelection(_currentDestination);
        HideSelectionPanel();
    }

    private void OnCancelClicked()
    {
        Utils.ColorLog("Selection cancelled", "Yellow");
        
        var tileManager = _tileManager as TileManager;
        tileManager?.CancelUnitSelection();
        HideSelectionPanel();
    }

    private void OnDisable()
    {
        BootstrapManager.OnGameReady -= OnGameReady;

        var tileManager = _tileManager as TileManager;
        if (tileManager != null)
        {
            tileManager.OnUnitSelectionRequested -= ShowSelectionPanel;
            tileManager.OnUnitSelectionCancelled -= HideSelectionPanel;
        }
    }
}