using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UiScalingMigration
{
    private const float ReferenceWidth = 1280f;
    private const float ReferenceHeight = 720f;
    private const float AuditOffsetXThreshold = 200f;
    private const float AuditOffsetYThreshold = 150f;

    [MenuItem("Tools/UI/Apply CanvasScaler Settings")]
    public static void ApplyCanvasScalerSettings()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.Log("[UiScalingMigration] Apply cancelled — unsaved scene changes.");
            return;
        }

        int scenesTouched = 0;
        int scalersUpdated = 0;

        foreach (EditorBuildSettingsScene entry in EditorBuildSettings.scenes)
        {
            if (!entry.enabled)
            {
                continue;
            }

            Scene scene = EditorSceneManager.OpenScene(entry.path, OpenSceneMode.Single);
            int updatedInScene = ApplyToScene(scene);

            if (updatedInScene > 0)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                scenesTouched++;
                scalersUpdated += updatedInScene;
                Debug.Log($"[UiScalingMigration] Updated {updatedInScene} CanvasScaler(s) in {entry.path}.");
            }
        }

        Debug.Log($"[UiScalingMigration] Apply complete. {scenesTouched} scene(s) touched, {scalersUpdated} CanvasScaler(s) updated total.");
    }

    private static int ApplyToScene(Scene scene)
    {
        int count = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (CanvasScaler scaler in root.GetComponentsInChildren<CanvasScaler>(includeInactive: true))
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                scaler.matchWidthOrHeight = 0f;
                EditorUtility.SetDirty(scaler);
                count++;
            }
        }
        return count;
    }

    [MenuItem("Tools/UI/Audit Anchors (Dry Run)")]
    public static void AuditAnchors()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            Debug.Log("[UiScalingMigration] Audit cancelled — unsaved scene changes.");
            return;
        }

        int totalFlagged = 0;

        foreach (EditorBuildSettingsScene entry in EditorBuildSettings.scenes)
        {
            if (!entry.enabled)
            {
                continue;
            }

            Scene scene = EditorSceneManager.OpenScene(entry.path, OpenSceneMode.Single);
            int flaggedInScene = AuditScene(scene);
            totalFlagged += flaggedInScene;
            if (flaggedInScene > 0)
            {
                Debug.Log($"[UiScalingMigration] {scene.name}: {flaggedInScene} RectTransform(s) flagged.");
            }
        }

        Debug.Log($"[UiScalingMigration] Audit complete. {totalFlagged} RectTransform(s) flagged across all scenes. Threshold: |x|>{AuditOffsetXThreshold} or |y|>{AuditOffsetYThreshold}, center-anchored only.");
    }

    private static int AuditScene(Scene scene)
    {
        int flagged = 0;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (RectTransform rt in root.GetComponentsInChildren<RectTransform>(includeInactive: true))
            {
                if (!IsCenterAnchored(rt))
                {
                    continue;
                }

                Vector2 pos = rt.anchoredPosition;
                if (Mathf.Abs(pos.x) <= AuditOffsetXThreshold && Mathf.Abs(pos.y) <= AuditOffsetYThreshold)
                {
                    continue;
                }

                string path = GetHierarchyPath(rt.transform);
                Debug.Log($"[UiScalingMigration]   {scene.name} :: {path}  anchoredPosition=({pos.x:F1}, {pos.y:F1})");
                flagged++;
            }
        }
        return flagged;
    }

    private static bool IsCenterAnchored(RectTransform rt)
    {
        return Mathf.Approximately(rt.anchorMin.x, 0.5f)
            && Mathf.Approximately(rt.anchorMin.y, 0.5f)
            && Mathf.Approximately(rt.anchorMax.x, 0.5f)
            && Mathf.Approximately(rt.anchorMax.y, 0.5f);
    }

    private static string GetHierarchyPath(Transform t)
    {
        List<string> parts = new List<string>();
        while (t != null)
        {
            parts.Add(t.name);
            t = t.parent;
        }
        parts.Reverse();
        return string.Join("/", parts);
    }
}
