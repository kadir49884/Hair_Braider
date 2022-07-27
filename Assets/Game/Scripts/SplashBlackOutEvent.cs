using UnityEngine;

public class SplashBlackOutEvent : MonoBehaviour
{
    public void ActivateScene()
    {
        FindObjectOfType<SplashManager>().ActivateScene();
    }
}
