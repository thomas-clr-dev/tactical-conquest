using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MissingComponentCleanerWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private List<GameObjectInfo> objectsWithMissingScripts = new List<GameObjectInfo>();
    private bool isScanning = false;
    private bool scanCompleted = false;
    private int totalMissingCount = 0;
    private bool includeInactive = true;
    private bool scanPrefabs = false;

    private class GameObjectInfo
    {
        public GameObject gameObject;
        public int missingCount;
        public string path;

        public GameObjectInfo(GameObject go, int count, string objectPath)
        {
            gameObject = go;
            missingCount = count;
            path = objectPath;
        }
    }

    [MenuItem("Window/Vahartzia Tools/Missing Component Cleaner")]
    public static void ShowWindow()
    {
        var window = GetWindow<MissingComponentCleanerWindow>("Component Cleaner");
        window.minSize = new Vector2(450, 300);
    }

    [System.Obsolete]
    private void OnGUI()
    {
        DrawHeader();
        EditorGUILayout.Space();

        DrawScanOptions();
        EditorGUILayout.Space();

        DrawScanButton();
        EditorGUILayout.Space();

        if (scanCompleted)
        {
            DrawResults();
        }
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 14;
        headerStyle.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.LabelField("👻 Missing Component Cleaner", headerStyle);

        GUIStyle descStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
        descStyle.fontSize = 10;
        descStyle.alignment = TextAnchor.MiddleCenter;
        descStyle.normal.textColor = Color.gray;

        EditorGUILayout.LabelField("Détecte et supprime les composants manquants (Missing Scripts) dans la scène", descStyle);

        EditorGUILayout.EndVertical();
    }

    private void DrawScanOptions()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Options de scan", EditorStyles.boldLabel);

        includeInactive = EditorGUILayout.Toggle("Inclure les objets inactifs", includeInactive);
        scanPrefabs = EditorGUILayout.Toggle("Scanner aussi les prefabs", scanPrefabs);

        EditorGUILayout.EndVertical();
    }

    [System.Obsolete]
    private void DrawScanButton()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = !isScanning;
        GUI.color = new Color(0.5f, 0.8f, 1f);
        if (GUILayout.Button(isScanning ? "⏳ Scan en cours..." : "🔍 Scanner la scène", GUILayout.Height(40)))
        {
            ScanScene();
        }
        GUI.color = Color.white;
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();
    }

    [System.Obsolete]
    private void DrawResults()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Statistiques
        EditorGUILayout.BeginHorizontal();

        if (objectsWithMissingScripts.Count == 0)
        {
            GUI.color = new Color(0.3f, 0.9f, 0.3f);
            EditorGUILayout.LabelField("✓ Aucun composant manquant trouvé !", EditorStyles.boldLabel);
            GUI.color = Color.white;
        }
        else
        {
            GUI.color = new Color(1f, 0.6f, 0.2f);
            EditorGUILayout.LabelField($"⚠️ {objectsWithMissingScripts.Count} objet(s) avec {totalMissingCount} composant(s) manquant(s)", EditorStyles.boldLabel);
            GUI.color = Color.white;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if (objectsWithMissingScripts.Count > 0)
        {
            EditorGUILayout.Space();
            DrawCleanAllButton();
            EditorGUILayout.Space();
            DrawObjectList();
        }
    }

    [System.Obsolete]
    private void DrawCleanAllButton()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.color = new Color(1f, 0.3f, 0.3f);
        if (GUILayout.Button($"🧹 Nettoyer tous les composants manquants ({totalMissingCount})", GUILayout.Height(35)))
        {
            if (EditorUtility.DisplayDialog(
                "Nettoyer tous les composants manquants",
                $"Voulez-vous vraiment supprimer {totalMissingCount} composant(s) manquant(s) de {objectsWithMissingScripts.Count} objet(s) ?\n\nCette action peut être annulée avec Ctrl+Z.",
                "Oui, nettoyer",
                "Annuler"))
            {
                CleanAllMissingScripts();
            }
        }
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();
    }

    [System.Obsolete]
    private void DrawObjectList()
    {
        EditorGUILayout.LabelField("Objets affectés:", EditorStyles.boldLabel);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        foreach (var objInfo in objectsWithMissingScripts)
        {
            if (objInfo.gameObject == null)
                continue;

            DrawObjectItem(objInfo);
        }

        EditorGUILayout.EndScrollView();
    }

    [System.Obsolete]
    private void DrawObjectItem(GameObjectInfo objInfo)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();

        // Icône et nom
        GUILayout.Label("🎮", GUILayout.Width(20));

        GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel);
        if (!objInfo.gameObject.activeInHierarchy)
        {
            nameStyle.normal.textColor = Color.gray;
        }

        EditorGUILayout.LabelField(objInfo.gameObject.name, nameStyle);

        GUILayout.FlexibleSpace();

        // Nombre de scripts manquants
        GUI.color = new Color(1f, 0.5f, 0.2f);
        GUILayout.Label($"{objInfo.missingCount} manquant(s)", EditorStyles.miniLabel, GUILayout.Width(90));
        GUI.color = Color.white;

        // Bouton pour sélectionner
        if (GUILayout.Button("🎯", GUILayout.Width(30)))
        {
            Selection.activeGameObject = objInfo.gameObject;
            EditorGUIUtility.PingObject(objInfo.gameObject);
        }

        // Bouton pour nettoyer cet objet
        GUI.color = new Color(1f, 0.5f, 0.5f);
        if (GUILayout.Button("🧹", GUILayout.Width(30)))
        {
            if (EditorUtility.DisplayDialog(
                "Nettoyer cet objet",
                $"Supprimer {objInfo.missingCount} composant(s) manquant(s) de '{objInfo.gameObject.name}' ?",
                "Oui",
                "Non"))
            {
                CleanGameObject(objInfo.gameObject);
                ScanScene(); // Re-scanner après nettoyage
            }
        }
        GUI.color = Color.white;

        EditorGUILayout.EndHorizontal();

        // Chemin dans la hiérarchie
        GUIStyle pathStyle = new GUIStyle(EditorStyles.miniLabel);
        pathStyle.normal.textColor = Color.gray;
        pathStyle.fontSize = 9;
        EditorGUILayout.LabelField($"📁 {objInfo.path}", pathStyle);

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(2);
    }

    [System.Obsolete]
    private void ScanScene()
    {
        isScanning = true;
        objectsWithMissingScripts.Clear();
        totalMissingCount = 0;

        try
        {
            // Récupérer tous les GameObjects de la scène
            GameObject[] allObjects;

            if (includeInactive)
            {
                allObjects = Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where(go => go.scene.isLoaded) // Seulement les objets de la scène active
                    .ToArray();
            }
            else
            {
                allObjects = FindObjectsOfType<GameObject>();
            }

            // Scanner chaque objet
            foreach (GameObject go in allObjects)
            {
                // Ne pas inclure les prefabs sauf si demandé
                if (!scanPrefabs && PrefabUtility.IsPartOfPrefabAsset(go))
                    continue;

                int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);

                if (count > 0)
                {
                    string path = GetGameObjectPath(go);
                    objectsWithMissingScripts.Add(new GameObjectInfo(go, count, path));
                    totalMissingCount += count;
                }
            }

            // Trier par nombre de composants manquants (décroissant)
            objectsWithMissingScripts = objectsWithMissingScripts
                .OrderByDescending(obj => obj.missingCount)
                .ToList();

            scanCompleted = true;

            Debug.Log($"[Missing Component Cleaner] Scan terminé : {objectsWithMissingScripts.Count} objet(s) avec {totalMissingCount} composant(s) manquant(s)");
        }
        finally
        {
            isScanning = false;
        }

        Repaint();
    }

    [System.Obsolete]
    private void CleanAllMissingScripts()
    {
        if (objectsWithMissingScripts.Count == 0)
            return;

        Undo.RegisterCompleteObjectUndo(
            objectsWithMissingScripts.Select(o => o.gameObject).ToArray(),
            "Clean All Missing Scripts");

        int cleanedCount = 0;
        int cleanedObjects = 0;

        foreach (var objInfo in objectsWithMissingScripts)
        {
            if (objInfo.gameObject != null)
            {
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(objInfo.gameObject);
                cleanedCount += removed;
                if (removed > 0)
                    cleanedObjects++;
            }
        }

        Debug.Log($"[Missing Component Cleaner] ✓ Nettoyage terminé : {cleanedCount} composant(s) manquant(s) supprimé(s) de {cleanedObjects} objet(s)");

        EditorUtility.SetDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0]);

        // Re-scanner
        ScanScene();
    }

    private void CleanGameObject(GameObject go)
    {
        if (go == null)
            return;

        Undo.RegisterCompleteObjectUndo(go, "Clean Missing Scripts");

        int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

        if (removed > 0)
        {
            Debug.Log($"[Missing Component Cleaner] ✓ {removed} composant(s) manquant(s) supprimé(s) de '{go.name}'");
            EditorUtility.SetDirty(go);
        }
    }

    private string GetGameObjectPath(GameObject go)
    {
        if (go == null)
            return "";

        string path = go.name;
        Transform current = go.transform.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}