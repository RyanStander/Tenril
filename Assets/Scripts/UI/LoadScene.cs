using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//loads a scene as well as also displays a loading bar
public class LoadScene : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;
    
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
                
                //if it is a prefab or model prefab, skip
                if (PrefabUtility.GetPrefabType(go) == PrefabType.Prefab ||
                    PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab)
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
