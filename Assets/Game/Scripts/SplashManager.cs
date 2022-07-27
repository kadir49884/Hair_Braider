using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SplashManager : MonoBehaviour
{
    public GameObject blackOut;
    [SerializeField] private TextMeshProUGUI loadingText;
    private AsyncOperation asyncOperation;
    private bool canActivateScene;
    

    private void Awake()
    {
        //Application.targetFrameRate = 60;
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
        
//        gameConfig.models.SetTexture("_BaseMap", gameConfig.modelTextures[0]);
//        gameConfig.road.SetTexture("_BaseMap", gameConfig.roadTextures[0]);
        
        blackOut.SetActive(false);
        //Instantiate(Resources.Load("SplashEnv"));

        
        canActivateScene = false;
        StartCoroutine(LoadScene());
        
//        GraphicsSettings.renderPipelineAsset = gameConfig.splashRPs[gameData.qualityInd];
//        UnityEngine.QualitySettings.masterTextureLimit = 2 - gameData.qualityInd;
        
        UnityEngine.QualitySettings.masterTextureLimit = 0;
        
    }
    public void LoadMenu()
    {
        canActivateScene = true;
    }
    
    private IEnumerator LoadScene()
    {
        yield return null;
        
        asyncOperation = SceneManager.LoadSceneAsync(1);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            loadingText.text = "Fun is loading..."; //" : " + (asyncOperation.progress * 100) + "%";
            if (asyncOperation.progress >= .9f)
                loadingText.text = "Game is ready...";//"Loading : 100%";
            
            yield return null;
        }
    }
    public void ActivateScene()
    {
        asyncOperation.allowSceneActivation = true;
    }

    private void Update()
    {
        if (canActivateScene && !blackOut.activeSelf)
            blackOut.SetActive(true);
    }
}
