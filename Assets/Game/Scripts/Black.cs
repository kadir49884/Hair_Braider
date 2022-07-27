using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class Black : MonoBehaviour
{
    public Animator blackAnimator;
    public GameObject loading;
    [SerializeField] private TextMeshProUGUI loadingText;
    private string sceneName;

    public void BlackIn(string sceneN)
    {
        blackAnimator.gameObject.SetActive(true);
        sceneName = sceneN;
        blackAnimator.SetTrigger("IN");
    }

    public void BlackOut()
    {
        blackAnimator.gameObject.SetActive(true);
        blackAnimator.SetTrigger("OUT");
    }

    public void BlackDef()
    {
        blackAnimator.gameObject.SetActive(true);   
    }

    public void OpenScene(string sceneN)
    {
        sceneName = sceneN;
        GotoScene();
    }
    
    public void GotoScene()
    {
        StartCoroutine(LoadScene());
    }
    
    private IEnumerator LoadScene()
    {
        yield return null;
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = true;
        
        while (!asyncOperation.isDone)
        {
            //loadingText.text = "Loading : " + (asyncOperation.progress * 100) + "%";
            yield return null;
        }
    }
}
