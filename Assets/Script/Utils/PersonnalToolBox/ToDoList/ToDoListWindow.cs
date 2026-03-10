using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class TodoListWindow : EditorWindow
{
    private TodoListData todoData;
    private Vector2 scrollPosition;
    //private string newTodoTitle = "";
    private string searchText = "";
    
    // Filtres
    private enum FilterMode { All, Active, Completed }
    private FilterMode currentFilter = FilterMode.All;
    private TodoPriorityFilter priorityFilter = TodoPriorityFilter.All;
    private TodoCategoryFilter categoryFilter = TodoCategoryFilter.All;
    
    // Edition
    private string editingItemId = null;
    private bool showAddPanel = false;
    private TodoItem newItem;
    private string itemToDelete = null; // Pour gérer la suppression proprement

    private const string DATA_PATH = "Assets/TodoListData.asset";

    [MenuItem("Window/Vahartzia Tools/Todo List Manager")]
    public static void ShowWindow()
    {
        var window = GetWindow<TodoListWindow>("Todo List");
        window.minSize = new Vector2(400, 300);
    }

    private void OnEnable()
    {
        LoadOrCreateData();
    }

    private void LoadOrCreateData()
    {
        todoData = AssetDatabase.LoadAssetAtPath<TodoListData>(DATA_PATH);
        
        if (todoData == null)
        {
            todoData = CreateInstance<TodoListData>();
            AssetDatabase.CreateAsset(todoData, DATA_PATH);
            AssetDatabase.SaveAssets();
        }
    }

    private void OnGUI()
    {
        if (todoData == null)
        {
            LoadOrCreateData();
            return;
        }

        DrawHeader();
        DrawToolbar();
        DrawStats();
        EditorGUILayout.Space();
        
        DrawTodoList();
        
        EditorGUILayout.Space();
        DrawAddPanel();
        
        // Gérer la suppression après le rendu
        if (!string.IsNullOrEmpty(itemToDelete))
        {
            todoData.RemoveItem(itemToDelete);
            SaveData();
            itemToDelete = null;
            Repaint();
        }
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("📋 Todo List Manager", EditorStyles.boldLabel);
        
        GUILayout.FlexibleSpace();
        
        // Bouton pour effacer les tâches complétées
        if (todoData.GetCompletedCount() > 0)
        {
            if (GUILayout.Button("🗑️ Nettoyer", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                if (EditorUtility.DisplayDialog("Nettoyer les tâches complétées", 
                    $"Supprimer {todoData.GetCompletedCount()} tâche(s) complétée(s) ?", "Oui", "Non"))
                {
                    todoData.ClearCompleted();
                    SaveData();
                }
            }
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Filtres principaux
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Afficher:", GUILayout.Width(60));
        
        if (GUILayout.Toggle(currentFilter == FilterMode.All, "Tous", EditorStyles.miniButtonLeft))
            currentFilter = FilterMode.All;
        if (GUILayout.Toggle(currentFilter == FilterMode.Active, "Actifs", EditorStyles.miniButtonMid))
            currentFilter = FilterMode.Active;
        if (GUILayout.Toggle(currentFilter == FilterMode.Completed, "Complétés", EditorStyles.miniButtonRight))
            currentFilter = FilterMode.Completed;
        
        EditorGUILayout.EndHorizontal();
        
        // Filtres secondaires
        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField("Priorité:", GUILayout.Width(60));
        priorityFilter = (TodoPriorityFilter)EditorGUILayout.EnumPopup(priorityFilter, GUILayout.Width(120));
        
        GUILayout.Space(10);
        
        EditorGUILayout.LabelField("Catégorie:", GUILayout.Width(70));
        categoryFilter = (TodoCategoryFilter)EditorGUILayout.EnumPopup(categoryFilter, GUILayout.Width(120));
        
        EditorGUILayout.EndHorizontal();
        
        // Barre de recherche
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("🔍", GUILayout.Width(20));
        searchText = EditorGUILayout.TextField(searchText);
        if (!string.IsNullOrEmpty(searchText) && GUILayout.Button("✖", GUILayout.Width(20)))
            searchText = "";
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
    }

    private void DrawStats()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        
        int total = todoData.items.Count;
        int active = todoData.GetActiveCount();
        int completed = todoData.GetCompletedCount();
        
        EditorGUILayout.LabelField($"Total: {total}", GUILayout.Width(70));
        
        GUI.color = new Color(0.3f, 0.7f, 1f);
        EditorGUILayout.LabelField($"✓ Actifs: {active}", GUILayout.Width(80));
        
        GUI.color = new Color(0.3f, 0.9f, 0.3f);
        EditorGUILayout.LabelField($"✓ Complétés: {completed}", GUILayout.Width(100));
        
        GUI.color = Color.white;
        
        if (total > 0)
        {
            float progress = (float)completed / total;
            EditorGUI.ProgressBar(GUILayoutUtility.GetRect(100, 18), progress, $"{Mathf.RoundToInt(progress * 100)}%");
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawTodoList()
    {
        var filteredItems = GetFilteredItems();
        
        if (filteredItems.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucune tâche à afficher. Créez-en une !", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        foreach (var item in filteredItems)
        {
            DrawTodoItem(item);
        }
        
        EditorGUILayout.EndScrollView();
    }

    private List<TodoItem> GetFilteredItems()
    {
        var items = todoData.items.AsEnumerable();
        
        // Filtre principal
        if (currentFilter == FilterMode.Active)
            items = items.Where(i => !i.isCompleted);
        else if (currentFilter == FilterMode.Completed)
            items = items.Where(i => i.isCompleted);
        
        // Filtre priorité (sauf si "All")
        if (priorityFilter != TodoPriorityFilter.All)
        {
            TodoPriority targetPriority = (TodoPriority)(priorityFilter - 1); // -1 car "All" est en premier
            items = items.Where(i => i.priority == targetPriority);
        }
        
        // Filtre catégorie (sauf si "All")
        if (categoryFilter != TodoCategoryFilter.All)
        {
            TodoCategory targetCategory = (TodoCategory)(categoryFilter - 1); // -1 car "All" est en premier
            items = items.Where(i => i.category == targetCategory);
        }
        
        // Recherche
        if (!string.IsNullOrEmpty(searchText))
        {
            string search = searchText.ToLower();
            items = items.Where(i => 
                i.title.ToLower().Contains(search) || 
                i.description.ToLower().Contains(search));
        }
        
        // Tri : Urgent > High > Normal > Low, puis non complétés en premier
        return items
            .OrderBy(i => i.isCompleted)
            .ThenByDescending(i => i.priority)
            .ToList();
    }

    private void DrawTodoItem(TodoItem item)
    {
        bool isEditing = editingItemId == item.id;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        // Header
        EditorGUILayout.BeginHorizontal();
        
        // Checkbox
        EditorGUI.BeginChangeCheck();
        bool newCompleted = EditorGUILayout.Toggle(item.isCompleted, GUILayout.Width(20));
        if (EditorGUI.EndChangeCheck())
        {
            item.isCompleted = newCompleted;
            SaveData();
        }
        
        // Barre de priorité
        Color priorityColor = GetPriorityColor(item.priority);
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(4, 20), priorityColor);
        
        // Titre
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        if (item.isCompleted)
        {
            titleStyle.normal.textColor = Color.gray;
        }
        
        if (isEditing)
        {
            item.title = EditorGUILayout.TextField(item.title);
        }
        else
        {
            EditorGUILayout.LabelField(item.title, titleStyle);
        }
        
        GUILayout.FlexibleSpace();
        
        // Badge catégorie
        GUI.color = GetCategoryColor(item.category);
        GUILayout.Label($"[{item.category}]", EditorStyles.miniLabel, GUILayout.Width(80));
        GUI.color = Color.white;
        
        // Bouton éditer
        if (GUILayout.Button(isEditing ? "💾" : "✏️", GUILayout.Width(25)))
        {
            if (isEditing)
            {
                editingItemId = null;
                SaveData();
            }
            else
            {
                editingItemId = item.id;
            }
        }
        
        // Bouton supprimer
        GUI.color = new Color(1f, 0.5f, 0.5f);
        if (GUILayout.Button("✖", GUILayout.Width(25)))
        {
            if (EditorUtility.DisplayDialog("Supprimer la tâche", 
                $"Supprimer '{item.title}' ?", "Oui", "Non"))
            {
                // Marquer pour suppression au lieu de supprimer immédiatement
                itemToDelete = item.id;
            }
        }
        GUI.color = Color.white;
        
        EditorGUILayout.EndHorizontal();
        
        // Description et détails
        if (isEditing || !string.IsNullOrEmpty(item.description))
        {
            EditorGUILayout.Space(2);
            if (isEditing)
            {
                item.description = EditorGUILayout.TextArea(item.description, GUILayout.Height(40));
            }
            else if (!string.IsNullOrEmpty(item.description))
            {
                GUIStyle descStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                descStyle.fontSize = 10;
                descStyle.normal.textColor = item.isCompleted ? Color.gray : Color.white;
                EditorGUILayout.LabelField(item.description, descStyle);
            }
        }
        
        // Détails en édition
        if (isEditing)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Priorité:", GUILayout.Width(60));
            item.priority = (TodoPriority)EditorGUILayout.EnumPopup(item.priority);
            EditorGUILayout.LabelField("Catégorie:", GUILayout.Width(70));
            item.category = (TodoCategory)EditorGUILayout.EnumPopup(item.category);
            EditorGUILayout.EndHorizontal();
        }
        
        // Footer - Date
        EditorGUILayout.BeginHorizontal();
        GUIStyle dateStyle = new GUIStyle(EditorStyles.miniLabel);
        dateStyle.normal.textColor = Color.gray;
        EditorGUILayout.LabelField($"Créé: {item.createdDate}", dateStyle, GUILayout.Width(150));
        
        if (!string.IsNullOrEmpty(item.dueDate))
        {
            EditorGUILayout.LabelField($"Échéance: {item.dueDate}", dateStyle);
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(3);
    }

    private void DrawAddPanel()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        EditorGUILayout.BeginHorizontal();
        
        if (!showAddPanel)
        {
            if (GUILayout.Button("➕ Nouvelle tâche", GUILayout.Height(30)))
            {
                showAddPanel = true;
                newItem = new TodoItem("Nouvelle tâche");
            }
        }
        else
        {
            EditorGUILayout.LabelField("➕ Nouvelle tâche", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            GUI.color = new Color(0.5f, 1f, 0.5f);
            if (GUILayout.Button("✓ Ajouter", GUILayout.Width(80), GUILayout.Height(25)))
            {
                if (!string.IsNullOrEmpty(newItem.title))
                {
                    todoData.AddItem(newItem);
                    SaveData();
                    showAddPanel = false;
                    newItem = null;
                }
            }
            GUI.color = Color.white;
            
            if (GUILayout.Button("✖", GUILayout.Width(25), GUILayout.Height(25)))
            {
                showAddPanel = false;
                newItem = null;
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        if (showAddPanel && newItem != null)
        {
            EditorGUILayout.Space();
            newItem.title = EditorGUILayout.TextField("Titre:", newItem.title);
            newItem.description = EditorGUILayout.TextArea(newItem.description, GUILayout.Height(50));
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Priorité:", GUILayout.Width(60));
            newItem.priority = (TodoPriority)EditorGUILayout.EnumPopup(newItem.priority);
            EditorGUILayout.LabelField("Catégorie:", GUILayout.Width(70));
            newItem.category = (TodoCategory)EditorGUILayout.EnumPopup(newItem.category);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
    }

    private Color GetPriorityColor(TodoPriority priority)
    {
        return priority switch
        {
            TodoPriority.Urgent => new Color(1f, 0.2f, 0.2f),
            TodoPriority.High => new Color(1f, 0.6f, 0.2f),
            TodoPriority.Normal => new Color(0.3f, 0.7f, 1f),
            TodoPriority.Low => new Color(0.5f, 0.5f, 0.5f),
            _ => Color.white
        };
    }

    private Color GetCategoryColor(TodoCategory category)
    {
        return category switch
        {
            TodoCategory.Bug => new Color(1f, 0.3f, 0.3f),
            TodoCategory.Feature => new Color(0.3f, 0.8f, 0.3f),
            TodoCategory.Optimization => new Color(1f, 0.8f, 0.2f),
            TodoCategory.Documentation => new Color(0.6f, 0.6f, 1f),
            TodoCategory.Art => new Color(1f, 0.5f, 0.8f),
            TodoCategory.Audio => new Color(0.8f, 0.3f, 1f),
            TodoCategory.Design => new Color(0.3f, 0.9f, 0.9f),
            _ => Color.gray
        };
    }

    private void SaveData()
    {
        EditorUtility.SetDirty(todoData);
        AssetDatabase.SaveAssets();
    }

    private void OnDestroy()
    {
        if (todoData != null)
        {
            SaveData();
        }
    }
}