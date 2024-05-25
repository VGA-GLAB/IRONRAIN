using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IronRain.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string[] _sceneName;

        private async void Start()
        {
            await LoadScenesAsync(_sceneName);
        }

        #region Core Module

        private readonly List<AsyncOperation> _asyncOperations = new List<AsyncOperation>();

        public float Progress => _asyncOperations.Count == 0 ? 0 : _asyncOperations.Average(o => o.progress);
        public bool IsDone => _asyncOperations.All(o => o.isDone);

        public async Task LoadScenesAsync(string[] sceneNames, Action onCompleted = null, Func<bool> cancelRequest = null)
        {
            foreach (var sceneName in sceneNames)
            {
                var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                _asyncOperations.Add(operation);
            }

            EnableGUI();

            while (!IsDone)
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying) break;
#endif
                if (cancelRequest != null && cancelRequest()) break;

                GUIUpdate();
                await Task.Delay(100);
            }

            GUIUpdate();
            DisableGUI();

            _asyncOperations.Clear();

            onCompleted?.Invoke();
        }
        #endregion

        #region GUI
        [SerializeField]
        private GameObject _loadScreenCanvas;
        [SerializeField]
        private Image _progressBar;

        private void EnableGUI()
        {
            if (_loadScreenCanvas) _loadScreenCanvas.SetActive(true);
            if (_progressBar) _progressBar.enabled = true;
        }

        private void DisableGUI()
        {
            if (_loadScreenCanvas) _loadScreenCanvas.SetActive(false);
            if (_progressBar) _progressBar.enabled = false;
        }

        private void GUIUpdate()
        {
            if (_progressBar) _progressBar.fillAmount = Progress;
        }
        #endregion
    }
}