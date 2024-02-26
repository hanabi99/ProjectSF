using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
namespace MyGameFrameWork {
    public class SceneLoadManager : Singleton<SceneLoadManager>
    {
        public void LoadTargetScene(string SceneName, Action action = null)
        {
            StartCoroutine(LoadScene(SceneName, action));
        }

        IEnumerator LoadScene(string SceneName, Action action = null)
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            action?.Invoke();
            GC.Collect();
            yield return null;

            //while (!asyncOperation.isDone)
            //{
            //    if (asyncOperation.progress >= 0.9)
            //    {
            //        yield return asyncOperation.allowSceneActivation = true;
            //    }
            //    yield return null;
            //}
        }
    }

    }
