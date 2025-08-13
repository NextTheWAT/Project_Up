#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class Startup
{
    static Startup()    
    {
        EditorPrefs.SetInt("showCounts_sportcarcgb2025", EditorPrefs.GetInt("showCounts_sportcarcgb2025") + 1);

        if (EditorPrefs.GetInt("showCounts_sportcarcgb2025") == 1)       
        {
            Application.OpenURL("https://assetstore.unity.com/packages/templates/packs/complete-game-bundle-2025-326183");
            // System.IO.File.Delete("Assets/SportCar/Racing_Game.cs");
        }
    }     
}
#endif
