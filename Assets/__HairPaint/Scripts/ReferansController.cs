using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferansController : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> refImage = new List<Sprite>();
    private int count;

    void Start()
    {
        transform.GetComponent<Image>().sprite = refImage[LevelHelper.Instance.ActiveLevel - 10];
        ChangeScale();
    }
    private void ChangeScale()
    {
        transform.DOScale(new Vector3(0.77f, 1.1f, 1.1f), 0.3f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
        {
            transform.DOScale(new Vector3(0.7f, 1, 1), 0.3f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() =>
               {
                   count++;
                   if (count > 5)
                   {
                       return;
                   }
                   ChangeScale();
               });
        });

    }


}
