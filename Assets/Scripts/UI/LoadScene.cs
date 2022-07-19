using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//loads a scene as well as also displays a loading bar
namespace UI
{
    public class LoadScene : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider slider;
    
#if (UNITY_EDITOR) 
        private void OnValidate()
        {
            if (loadingScreen==null)
            {
                //Get all objects
                foreach (var go in (GameObject[]) Resources.FindObjectsOfTypeAll(typeof(GameObject)))
                {
                    //if it is enabled, skip
                    if (go.hideFlags != HideFlags.None)
                        continue;
                    
                    //YES, THIS GIVES A WARNING, I CANNOT HIDE IT
                    //if it is a prefab, go to next check
                    if (PrefabUtility.GetPrefabAssetType(go)!=PrefabAssetType.NotAPrefab||
                        PrefabUtility.GetPrefabInstanceStatus(go)!=PrefabInstanceStatus.NotAPrefab)
                        continue;
              
                    //if its name is not loading screen, skip
                    if (go.name != "LoadingScreen")
                        continue;
                
                    //set the first result to the loading screen and exit
                    loadingScreen = go;
                    break;
                }
            }

            if (loadingScreen!=null && slider == null)
            {
                slider = loadingScreen.GetComponentInChildren<Slider>();
            }
        }
#endif
        public void LoadGivenScene(string givenSceneName)
        {
            StartCoroutine(LoadAsynchronously(givenSceneName));
        }

        public void LoadGivenScene(int index)
        {
            StartCoroutine(LoadAsynchronously(index));
        }

        private IEnumerator LoadAsynchronously(string givenSceneName)
        {
            var operation = SceneManager.LoadSceneAsync(givenSceneName);

            loadingScreen.SetActive(true);
        
            while (!operation.isDone)
            {
                var progress = Mathf.Clamp01(operation.progress / .9f);

                slider.value = progress;
            
                yield return null;
            }
        }
    
        private IEnumerator LoadAsynchronously(int givenSceneIndex)
        {
            var operation = SceneManager.LoadSceneAsync(givenSceneIndex);

            loadingScreen.SetActive(true);
        
            while (!operation.isDone)
            {
                var progress = Mathf.Clamp01(operation.progress / .9f);

                slider.value = progress;
            
                yield return null;
            }
        }
    }
}
