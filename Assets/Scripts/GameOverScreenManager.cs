using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GameOverScreenManager: MonoBehaviour
    {
        [SerializeField] private Button replay;
        [SerializeField] private Button quit;

        private void Start()
        {
            quit.onClick.RemoveAllListeners();
            quit.onClick.AddListener(Application.Quit);
            
            replay.onClick.RemoveAllListeners();
            replay.onClick.AddListener((() =>
            {
               LoadLevel();
            }));
        }

        private void LoadLevel(float time = 0)
        {
            StartCoroutine(LoadLevelCoroutine(time));
        }

        private IEnumerator LoadLevelCoroutine(float time = 0)
        {
            yield return new WaitForSeconds(time);
            GameObject tempObject = new GameObject("Temporary_Destroyer");
            DontDestroyOnLoad(tempObject);
            Scene dontDestroyScene = tempObject.scene;
            DestroyImmediate(tempObject);
            GameObject[] ddolObjects = dontDestroyScene.GetRootGameObjects();
            for (int i = ddolObjects.Length - 1; i >= 0; i--)
            {
                DestroyImmediate(ddolObjects[i]);
            }

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            yield return Resources.UnloadUnusedAssets();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }
    }
}