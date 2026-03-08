using UnityEngine;
using System.Collections;

public static class Utils
{

    /// <summary>Log dans la console</summary>
    public static void ConsoleLog(string text = "")
    {
        Debug.Log($"{text}");
    }

    /// <summary>Log error dans la console</summary>
    public static void ErrorLog(string text = "")
    {
        Debug.LogError($"{text}");
    }

    /// <summary>Log warning dans la console</summary>
    public static void WarningLog(string text = "")
    {
        Debug.LogWarning($"{text}");
    }

    /// <summary>Log avec couleur dans la console</summary>
    public static void ColorLog(string message, string color = "white")
    {
        Debug.Log($"<color={color}>{message}</color>");
    }

    /// <summary>Reset la vélocité linéaire et angulaire d'un Rigidbody</summary>
    public static void ResetVelocity(this Rigidbody rb)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    /// <summary>Reset la vélocité angulaire d'un Rigidbody</summary>
    public static void ResetAngularVelocity(this Rigidbody rb)
    {
        rb.angularVelocity = Vector3.zero;
    }

    /// <summary>Reset la vélocité linéaire d'un Rigidbody</summary>
    public static void ResetLinearVelocity(this Rigidbody rb)
    {
        rb.linearVelocity = Vector3.zero;
    }

    /// <summary>Active/désactive un GameObject avec vérification null</summary>
    public static void SafeSetActive(this GameObject gameObject, bool active)
    {
        if (gameObject != null)
            gameObject.SetActive(active);
    }

    /// <summary>Anime l'alpha d'un CanvasGroup</summary>
    public static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        if (canvasGroup == null) yield break;
        
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

        /// <summary>Vérifie si un point monde est visible par la caméra</summary>
    public static bool IsPointVisible(Vector3 worldPosition, Camera camera = null)
    {
        if (camera == null) camera = Camera.main;
        Vector3 screenPoint = camera.WorldToViewportPoint(worldPosition);
        return screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1 && screenPoint.z > 0;
    }

        /// <summary>Joue un son avec pitch aléatoire</summary>
    public static void PlaySoundWithRandomPitch(AudioSource audioSource, AudioClip clip, float minPitch = 0.8f, float maxPitch = 1.2f)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(clip);
        }
    }
    
        // ========== MISC UTILITIES ==========
    
    /// <summary>Redémarre la scène actuelle</summary>
    public static void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    /// <summary>Met le jeu en pause ou le reprend</summary>
    public static void TogglePause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    /// <summary>Quitte l'application proprement</summary>
    public static void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
