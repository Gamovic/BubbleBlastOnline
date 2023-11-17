using UnityEngine;
using UnityEditor;
using System.Linq;

public class BuildGame
{
    [MenuItem("Build/BuildTheGame")]
    public static void BuildTheGame()
    {
        // Get the first scene
        string scenePath = EditorBuildSettings.scenes[0].path;

        // Build settings
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = "Builds/BubbleBlastOnline",
            target = BuildTarget.StandaloneWindows,
            options = BuildOptions.None
        };

        // Perform build
        BuildPipeline.BuildPlayer(buildOptions);

        Debug.Log("Build completed!");
    }
}
