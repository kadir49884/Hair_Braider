using UnityEngine;

public class BlackAnimatorEvent : MonoBehaviour
{
    [SerializeField] private Black black;
    
    public void ActivateLoadingIndicator()
    {
        black.loading.SetActive(true);
    }
    public void DeActivateLoadingIndicator()
    {
        black.loading.SetActive(false);
    }
    
    public void GoToScene()
    {
        black.GotoScene();
    }
}
