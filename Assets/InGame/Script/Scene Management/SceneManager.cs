using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IronRain.SceneManagement
{
    public static class SceneManager
    {
        private static string OnSceneLoadedMethodName = "OnSceneLoaded";
        private static string ActivationMethodName = "Activation";
        private static string DeactivationMethodName = "Deactivation";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static readonly HashSet<Scene> _loadedScenes = new HashSet<Scene>();
        public static HashSet<Scene> LoadedScenes => _loadedScenes;

        private static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            _loadedScenes.Add(scene);

            PerformActionOnScene(scene, OnSceneLoadedMethodName);
        }

        private static void OnSceneUnloaded(Scene scenes)
        {
            _loadedScenes.Remove(scenes);
        }

        public static void Activation(Scene scene)
        {
            PerformActionOnScene(scene, ActivationMethodName);
        }

        public static void Deactivation(Scene scene)
        {
            PerformActionOnScene(scene, DeactivationMethodName);
        }

        public static void Activation(string sceneName)
        {
            var scene = _loadedScenes.First(s => s.name == sceneName);

            PerformActionOnScene(scene, ActivationMethodName);
        }

        public static void Deactivation(string sceneName)
        {
            var scene = _loadedScenes.First(s => s.name == sceneName);

            PerformActionOnScene(scene, DeactivationMethodName);
        }

        private static void PerformActionOnScene(Scene scene, string action)
        {
            var rootObjects = scene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                TraverseGameObject(rootObject, obj => obj.SendMessage(action, SendMessageOptions.DontRequireReceiver));
            }
        }

        private static void TraverseGameObject(GameObject gameObject, Action<GameObject> action)
        {
            action?.Invoke(gameObject);

            // 子オブジェクトが存在しない場合、このオブジェクトがleafGameObjectです。
            if (gameObject.transform.childCount == 0)
            {
                return;
            }

            // 子オブジェクトが存在する場合、再帰的にそれぞれの子オブジェクトを走査します。
            foreach (Transform childTransform in gameObject.transform)
            {
                TraverseGameObject(childTransform.gameObject, action);
            }
        }
    }
}