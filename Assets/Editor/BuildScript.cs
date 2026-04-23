using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    private const string WebGLOutputPath = "Build/WebGL";

    [MenuItem("Build/WebGL (Itch)")]
    public static void BuildWebGL()
    {
        string[] scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            LogError("No enabled scenes in Build Settings. Open File > Build Profiles and enable at least one scene.");
            ExitIfBatch(1);
            return;
        }

        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            Debug.Log("[BuildScript] Switching active build target to WebGL...");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        }

        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;

        string outputPath = Path.GetFullPath(WebGLOutputPath);
        if (Directory.Exists(outputPath))
        {
            Directory.Delete(outputPath, true);
        }
        Directory.CreateDirectory(outputPath);

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None,
        };

        Debug.Log($"[BuildScript] Building WebGL to {outputPath} with {scenes.Length} scene(s).");
        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"[BuildScript] Build succeeded in {summary.totalTime}. Size: {summary.totalSize} bytes. Output: {outputPath}");
            ExitIfBatch(0);
        }
        else
        {
            LogError($"[BuildScript] Build failed: {summary.result}. Errors: {summary.totalErrors}.");
            ExitIfBatch(1);
        }
    }

    private static string[] GetEnabledScenes()
    {
        List<string> paths = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                paths.Add(scene.path);
            }
        }
        return paths.ToArray();
    }

    private static void LogError(string message)
    {
        Debug.LogError(message);
    }

    private static void ExitIfBatch(int code)
    {
        if (Application.isBatchMode)
        {
            EditorApplication.Exit(code);
        }
    }
}
