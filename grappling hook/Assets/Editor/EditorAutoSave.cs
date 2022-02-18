using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace EditorNamespace
{
    /// <summary>
    /// Unity has probably discussed an auto-save feature countless times over the years
    /// and decided not to implement... so take that information as you'd like. I personally
    /// like the idea and it's worked well for me during my limited testing. If you find any bugs
    /// please report them on the repo: https://github.com/Matthew-J-Spencer/Unity-AutoSave
    /// 
    /// Love your friendly neighborhood Tarodev
    /// </summary>
    [CustomEditor(typeof(AutoSaveConfig))]
    public class EditorAutoSave : Editor 
    {
        private static AutoSaveConfig config;
        private static CancellationTokenSource tokenSource;
        private static Task task;
        
        [InitializeOnLoadMethod] private static void OnEnable() 
        {
            FetchConfig();
            CancelTask();

            tokenSource = new CancellationTokenSource();
            task = SaveInterval(tokenSource.Token);
        }

        private static void FetchConfig() 
        {
            while (true) 
            {
                if (config != null) return;

                string[] configGuids = GetConfigIds();

                if (configGuids.Length == 0) 
                {
                    AssetDatabase.CreateAsset(CreateInstance<AutoSaveConfig>(), $"Assets/{nameof(AutoSaveConfig)}.asset");
                    Debug.Log("A config file has been created at the root of your project.<b> You can move this anywhere you'd like.</b>");
                    continue;
                }

                string path = AssetDatabase.GUIDToAssetPath(configGuids[0]);
                config = AssetDatabase.LoadAssetAtPath<AutoSaveConfig>(path);

                break;
            }
        }

        private static string[] GetConfigIds() 
        {
            string[] configGuids = AssetDatabase.FindAssets(nameof(AutoSaveConfig));
            if (configGuids.Length > 1) Debug.LogWarning("Multiple auto save config assets found. Delete one.");
            return configGuids;
        }

        private static void CancelTask() 
        {
            if (task == null) return;
            tokenSource.Cancel();
            task.Wait();
        }

        private static async Task SaveInterval(CancellationToken token) 
        {
            while (!token.IsCancellationRequested) 
            {
                if (config == null)
                {
                    FetchConfig();
                }
                await Task.Delay(config.Frequency * 1000 * 60, token);
                
                if (!config.Enabled || Application.isPlaying || BuildPipeline.isBuildingPlayer || EditorApplication.isCompiling) continue;
                if (!UnityEditorInternal.InternalEditorUtility.isApplicationActive) continue;

                EditorSceneManager.SaveOpenScenes();
                if (config.Logging) Debug.Log($"Auto-saved at {DateTime.Now:h:mm:ss tt}");
            }
        }

        [MenuItem("Window/Auto save/Find config")]
        public static void ShowConfig() 
        {
            FetchConfig();

            string[] configGuids = GetConfigIds();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<AutoSaveConfig>(AssetDatabase.GUIDToAssetPath(configGuids[0])).GetInstanceID());
        }

        public override void OnInspectorGUI() 
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("You can move this asset where ever you'd like.\nWith ❤, Tarodev.", MessageType.Info);
        }
    }

    public class AutoSaveConfig : ScriptableObject 
    {
        [Tooltip("Enable auto save functionality")]
        public bool Enabled;

        [Tooltip("The frequency in minutes auto save will activate\ni.e, saves every x minutes."), Min(1)]
        public int Frequency = 1;

        [Tooltip("Log a message every time the scene is auto saved")]
        public bool Logging;
    }
}