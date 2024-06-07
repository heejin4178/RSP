using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiplayerBuildAndRun
{
    [MenuItem("Tools/Run Multiplayer/1 Players")]
    static void PerformMacOsBuild1()
    {
        PerformMacOsBuild(1);
    }
    
    [MenuItem("Tools/Run Multiplayer/2 Players")]
    static void PerformMacOsBuild2()
    {
        PerformMacOsBuild(2);
    }
    
    [MenuItem("Tools/Run Multiplayer/3 Players")]
    static void PerformMacOsBuild3()
    {
        PerformMacOsBuild(3);
    }
    
    [MenuItem("Tools/Run Multiplayer/4 Players")]
    static void PerformMacOsBuild4()
    {
        PerformMacOsBuild(4);
    }
    
    [MenuItem("Tools/Run Multiplayer/8 Players")]
    static void PerformMacOsBuild8()
    {
        PerformMacOsBuild(8);
    }
    
    static void PerformMacOsBuild(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);

        for (int i = 0; i < playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(),
                "/Users/heejinkim/Desktop/" + GetProjectName() + "-" + i.ToString() + "/" + GetProjectName() + i.ToString() + ".app",
                BuildTarget.StandaloneOSX, BuildOptions.AutoRunPlayer);
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}
