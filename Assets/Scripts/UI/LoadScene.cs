using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadGivenScene(string givenSceneName)
    {
        SceneManager.LoadScene(givenSceneName);
    }
}
