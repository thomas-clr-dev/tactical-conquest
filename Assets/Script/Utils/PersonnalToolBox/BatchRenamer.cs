using UnityEditor;
using UnityEngine;

public class BatchRenamer : EditorWindow
{
    // Modes de renommage
    private enum RenameMode
    {
        Sequential,      // Numérotation séquentielle
        PrefixSuffix,    // Ajouter préfixe/suffixe
        SearchReplace    // Chercher/Remplacer
    }

    private RenameMode currentMode = RenameMode.Sequential;

    // Mode Sequential
    private string baseName = "NewName";
    private int startIndex = 0;
    private int numberPadding = 2; // 01, 02 ou 001, 002
    private bool keepOriginalName = false;
    private bool addUnderscore = true;

    // Mode Prefix/Suffix
    private string prefix = "";
    private string suffix = "";
    private bool addNumbering = false;

    // Mode Search/Replace
    private string searchText = "";
    private string replaceText = "";
    private bool caseSensitive = false;
    //private bool useRegex = false;

    private Vector2 scrollPosition;

    // Clés pour EditorPrefs
    private const string PREF_MODE = "BatchRenamer_Mode";
    private const string PREF_BASE_NAME = "BatchRenamer_BaseName";
    private const string PREF_START_INDEX = "BatchRenamer_StartIndex";
    private const string PREF_PADDING = "BatchRenamer_Padding";
    private const string PREF_KEEP_NAME = "BatchRenamer_KeepName";
    private const string PREF_ADD_UNDERSCORE = "BatchRenamer_AddUnderscore";
    private const string PREF_PREFIX = "BatchRenamer_Prefix";
    private const string PREF_SUFFIX = "BatchRenamer_Suffix";
    private const string PREF_ADD_NUMBERING = "BatchRenamer_AddNumbering";

    [MenuItem("Window/Vahartzia Tools/Batch Renamer Pro")]
    public static void ShowWindow()
    {
        var window = GetWindow<BatchRenamer>("Batch Renamer Pro");
        window.minSize = new Vector2(450, 400);
    }

    private void OnEnable()
    {
        LoadPreferences();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawHeader();
        EditorGUILayout.Space(10);

        DrawModeSelector();
        EditorGUILayout.Space(5);

        // Afficher l'interface selon le mode
        switch (currentMode)
        {
            case RenameMode.Sequential:
                DrawSequentialMode();
                break;
            case RenameMode.PrefixSuffix:
                DrawPrefixSuffixMode();
                break;
            case RenameMode.SearchReplace:
                DrawSearchReplaceMode();
                break;
        }

        EditorGUILayout.Space(10);
        DrawPreview();
        EditorGUILayout.Space(10);
        DrawActionButtons();
        EditorGUILayout.Space(10);
        DrawHelpBox();

        EditorGUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 14;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.LabelField("✏️ Batch Renamer Pro", headerStyle);
        
        GUIStyle descStyle = new GUIStyle(EditorStyles.miniLabel);
        descStyle.alignment = TextAnchor.MiddleCenter;
        descStyle.normal.textColor = Color.gray;
        EditorGUILayout.LabelField("Renommage en masse avec prévisualisation", descStyle);
        
        EditorGUILayout.EndVertical();
    }

    private void DrawModeSelector()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Mode de renommage", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Toggle(currentMode == RenameMode.Sequential, "🔢 Numérotation", EditorStyles.miniButtonLeft))
            currentMode = RenameMode.Sequential;
        
        if (GUILayout.Toggle(currentMode == RenameMode.PrefixSuffix, "➕ Préfixe/Suffixe", EditorStyles.miniButtonMid))
            currentMode = RenameMode.PrefixSuffix;
        
        if (GUILayout.Toggle(currentMode == RenameMode.SearchReplace, "🔍 Chercher/Remplacer", EditorStyles.miniButtonRight))
            currentMode = RenameMode.SearchReplace;
        
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetInt(PREF_MODE, (int)currentMode);
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawSequentialMode()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("⚙️ Paramètres de numérotation", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        
        keepOriginalName = EditorGUILayout.Toggle("Garder le nom original", keepOriginalName);
        
        if (!keepOriginalName)
        {
            baseName = EditorGUILayout.TextField("Nom de base", baseName);
        }
        
        addUnderscore = EditorGUILayout.Toggle("Ajouter un underscore (_)", addUnderscore);
        startIndex = EditorGUILayout.IntField("Index de départ", startIndex);
        numberPadding = EditorGUILayout.IntSlider("Zéros de remplissage", numberPadding, 0, 4);
        
        // Exemple de formatage
        EditorGUILayout.Space(5);
        string exampleNumber = startIndex.ToString(GetNumberFormat());
        string exampleName = keepOriginalName ? "ObjectName" : baseName;
        string separator = addUnderscore ? "_" : "";
        EditorGUILayout.LabelField("Exemple :", $"{exampleName}{separator}{exampleNumber}", EditorStyles.miniLabel);
        
        if (EditorGUI.EndChangeCheck())
        {
            SaveSequentialPrefs();
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawPrefixSuffixMode()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("⚙️ Paramètres de préfixe/suffixe", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        
        prefix = EditorGUILayout.TextField("Préfixe", prefix);
        suffix = EditorGUILayout.TextField("Suffixe", suffix);
        
        EditorGUILayout.Space(5);
        addNumbering = EditorGUILayout.Toggle("Ajouter numérotation", addNumbering);
        
        if (addNumbering)
        {
            EditorGUI.indentLevel++;
            startIndex = EditorGUILayout.IntField("Index de départ", startIndex);
            numberPadding = EditorGUILayout.IntSlider("Zéros de remplissage", numberPadding, 0, 4);
            EditorGUI.indentLevel--;
        }
        
        // Exemple
        EditorGUILayout.Space(5);
        string exampleName = "ObjectName";
        string exampleNumber = addNumbering ? ("_" + startIndex.ToString(GetNumberFormat())) : "";
        EditorGUILayout.LabelField("Exemple :", $"{prefix}{exampleName}{suffix}{exampleNumber}", EditorStyles.miniLabel);
        
        if (EditorGUI.EndChangeCheck())
        {
            SavePrefixSuffixPrefs();
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawSearchReplaceMode()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("⚙️ Paramètres de recherche/remplacement", EditorStyles.boldLabel);
        
        searchText = EditorGUILayout.TextField("Rechercher", searchText);
        replaceText = EditorGUILayout.TextField("Remplacer par", replaceText);
        
        EditorGUILayout.Space(5);
        caseSensitive = EditorGUILayout.Toggle("Sensible à la casse", caseSensitive);
        
        // Exemple avec le premier objet sélectionné
        if (Selection.gameObjects.Length > 0)
        {
            EditorGUILayout.Space(5);
            string originalName = Selection.gameObjects[0].name;
            string newName = PerformSearchReplace(originalName);
            
            EditorGUILayout.LabelField("Exemple :", EditorStyles.miniLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(originalName, GUILayout.MaxWidth(150));
            EditorGUILayout.LabelField("→", GUILayout.Width(20));
            EditorGUILayout.LabelField(newName, EditorStyles.boldLabel, GUILayout.MaxWidth(150));
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawPreview()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("👁️ Aperçu", EditorStyles.boldLabel);
        
        if (Selection.gameObjects.Length > 0)
        {
            EditorGUILayout.HelpBox($"{Selection.gameObjects.Length} objet(s) sélectionné(s)", MessageType.Info);
            
            int previewCount = Mathf.Min(5, Selection.gameObjects.Length);
            
            for (int i = 0; i < previewCount; i++)
            {
                string currentName = Selection.gameObjects[i].name;
                string newName = GetNewName(Selection.gameObjects[i], i);
                
                EditorGUILayout.BeginHorizontal();
                
                // Nom actuel
                EditorGUILayout.LabelField(currentName, GUILayout.MaxWidth(180));
                EditorGUILayout.LabelField("→", GUILayout.Width(20));
                
                // Nouveau nom
                GUI.color = currentName != newName ? new Color(0.5f, 1f, 0.5f) : Color.yellow;
                EditorGUILayout.LabelField(newName, EditorStyles.boldLabel, GUILayout.MaxWidth(180));
                GUI.color = Color.white;
                
                EditorGUILayout.EndHorizontal();
            }
            
            if (Selection.gameObjects.Length > 5)
            {
                EditorGUILayout.LabelField($"... et {Selection.gameObjects.Length - 5} autre(s)", EditorStyles.miniLabel);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("⚠️ Aucun objet sélectionné. Sélectionnez des GameObjects dans la hiérarchie.", MessageType.Warning);
        }
        
        EditorGUILayout.EndVertical();
    }

    private void DrawActionButtons()
    {
        EditorGUILayout.BeginHorizontal();
        
        // Bouton Renommer
        GUI.enabled = CanRename();
        GUI.color = new Color(0.5f, 1f, 0.5f);
        if (GUILayout.Button("✓ Renommer", GUILayout.Height(35)))
        {
            RenameSelectedObjects();
        }
        GUI.color = Color.white;
        GUI.enabled = true;
        
        // Bouton Reset
        if (GUILayout.Button("🔄 Réinitialiser", GUILayout.Height(35), GUILayout.Width(120)))
        {
            ResetPreferences();
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawHelpBox()
    {
        string helpText = currentMode switch
        {
            RenameMode.Sequential => "🔢 Mode Numérotation : Renomme les objets avec un nom de base et un numéro séquentiel (ex: Rock_01, Rock_02).",
            RenameMode.PrefixSuffix => "➕ Mode Préfixe/Suffixe : Ajoute du texte au début et/ou à la fin des noms existants (ex: Env_Rock_LOD0).",
            RenameMode.SearchReplace => "🔍 Mode Chercher/Remplacer : Remplace du texte dans les noms existants (ex: 'Rock (1)' → 'Rock_01').",
            _ => ""
        };
        
        EditorGUILayout.HelpBox(helpText + "\n\n💡 Utilisez Ctrl+Z pour annuler le renommage.", MessageType.Info);
    }

    private bool CanRename()
    {
        if (Selection.gameObjects.Length == 0)
            return false;

        switch (currentMode)
        {
            case RenameMode.Sequential:
                return !string.IsNullOrWhiteSpace(baseName) || keepOriginalName;
            
            case RenameMode.PrefixSuffix:
                return !string.IsNullOrWhiteSpace(prefix) || !string.IsNullOrWhiteSpace(suffix) || addNumbering;
            
            case RenameMode.SearchReplace:
                return !string.IsNullOrWhiteSpace(searchText);
            
            default:
                return false;
        }
    }

    private string GetNewName(GameObject go, int index)
    {
        switch (currentMode)
        {
            case RenameMode.Sequential:
                return GetSequentialName(go, index);
            
            case RenameMode.PrefixSuffix:
                return GetPrefixSuffixName(go, index);
            
            case RenameMode.SearchReplace:
                return PerformSearchReplace(go.name);
            
            default:
                return go.name;
        }
    }

    private string GetSequentialName(GameObject go, int index)
    {
        string name = keepOriginalName ? go.name : baseName;
        string separator = addUnderscore ? "_" : "";
        string number = (startIndex + index).ToString(GetNumberFormat());
        return $"{name}{separator}{number}";
    }

    private string GetPrefixSuffixName(GameObject go, int index)
    {
        string name = go.name;
        string result = prefix + name + suffix;
        
        if (addNumbering)
        {
            string number = (startIndex + index).ToString(GetNumberFormat());
            result += "_" + number;
        }
        
        return result;
    }

    private string PerformSearchReplace(string originalName)
    {
        if (string.IsNullOrEmpty(searchText))
            return originalName;

        System.StringComparison comparison = caseSensitive 
            ? System.StringComparison.Ordinal 
            : System.StringComparison.OrdinalIgnoreCase;

        if (originalName.IndexOf(searchText, comparison) >= 0)
        {
            // Créer un regex pattern si nécessaire pour remplacer avec respect de la casse
            if (!caseSensitive)
            {
                return System.Text.RegularExpressions.Regex.Replace(
                    originalName, 
                    System.Text.RegularExpressions.Regex.Escape(searchText), 
                    replaceText, 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            else
            {
                return originalName.Replace(searchText, replaceText);
            }
        }
        
        return originalName;
    }

    private string GetNumberFormat()
    {
        return numberPadding > 0 ? new string('0', numberPadding) : "";
    }

    private void RenameSelectedObjects()
    {
        if (!CanRename())
        {
            EditorUtility.DisplayDialog("Erreur", "Paramètres invalides.", "OK");
            return;
        }

        Undo.RecordObjects(Selection.gameObjects, "Batch Rename");

        int renamedCount = 0;

        for (int i = 0; i < Selection.gameObjects.Length; i++)
        {
            GameObject go = Selection.gameObjects[i];
            string newName = GetNewName(go, i);
            
            if (go.name != newName)
            {
                go.name = newName;
                renamedCount++;
            }
        }

        Debug.Log($"✅ {renamedCount}/{Selection.gameObjects.Length} objet(s) renommé(s) avec succès !");
        ShowNotification(new GUIContent($"✅ {renamedCount} objet(s) renommé(s)"));
    }

    private void LoadPreferences()
    {
        currentMode = (RenameMode)EditorPrefs.GetInt(PREF_MODE, 0);
        baseName = EditorPrefs.GetString(PREF_BASE_NAME, "NewName");
        startIndex = EditorPrefs.GetInt(PREF_START_INDEX, 0);
        numberPadding = EditorPrefs.GetInt(PREF_PADDING, 2);
        keepOriginalName = EditorPrefs.GetBool(PREF_KEEP_NAME, false);
        addUnderscore = EditorPrefs.GetBool(PREF_ADD_UNDERSCORE, true);
        prefix = EditorPrefs.GetString(PREF_PREFIX, "");
        suffix = EditorPrefs.GetString(PREF_SUFFIX, "");
        addNumbering = EditorPrefs.GetBool(PREF_ADD_NUMBERING, false);
    }

    private void SaveSequentialPrefs()
    {
        EditorPrefs.SetString(PREF_BASE_NAME, baseName);
        EditorPrefs.SetInt(PREF_START_INDEX, startIndex);
        EditorPrefs.SetInt(PREF_PADDING, numberPadding);
        EditorPrefs.SetBool(PREF_KEEP_NAME, keepOriginalName);
        EditorPrefs.SetBool(PREF_ADD_UNDERSCORE, addUnderscore);
    }

    private void SavePrefixSuffixPrefs()
    {
        EditorPrefs.SetString(PREF_PREFIX, prefix);
        EditorPrefs.SetString(PREF_SUFFIX, suffix);
        EditorPrefs.SetBool(PREF_ADD_NUMBERING, addNumbering);
        EditorPrefs.SetInt(PREF_START_INDEX, startIndex);
        EditorPrefs.SetInt(PREF_PADDING, numberPadding);
    }

    private void ResetPreferences()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Réinitialiser les Préférences",
            "Voulez-vous réinitialiser tous les paramètres à leurs valeurs par défaut ?",
            "Oui",
            "Annuler"
        );

        if (confirm)
        {
            baseName = "NewName";
            startIndex = 0;
            numberPadding = 2;
            keepOriginalName = false;
            addUnderscore = true;
            prefix = "";
            suffix = "";
            addNumbering = false;
            searchText = "";
            replaceText = "";
            caseSensitive = false;

            SaveSequentialPrefs();
            SavePrefixSuffixPrefs();

            Debug.Log("🔄 Préférences réinitialisées");
            ShowNotification(new GUIContent("✅ Préférences réinitialisées"));
            Repaint();
        }
    }
}
