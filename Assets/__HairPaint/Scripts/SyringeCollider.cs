using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public enum SyringeStatus { Left, Middle, Right };

//[SerializeField]
//private SyringeStatus syringeStatus;

enum SyringeIndex
{
    Left1,
    Left2,
    Left3,
}

public class SyringeCollider : MonoBehaviour
{
    [SerializeField]
    private SyringeIndex syringeIndex;

    [SerializeField]
    private Color selectedColor;
    [SerializeField]
    private Transform lid;
    [SerializeField]
    private Transform water;

    private int levelValue;
    private List<Color> syringeMat = new List<Color>();
    private void Start()
    {
        levelValue = LevelHelper.Instance.ActiveLevel;

        syringeMat = ObjectManager.Instance.SyringeMat;
        selectedColor = syringeMat[(levelValue - 10) * 2 + levelValue - 10 + (int)syringeIndex];

        transform.GetChild(4).GetComponent<MeshRenderer>().material.color = selectedColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Plugger"))
        {
            other.transform.parent.GetChild(2).GetComponent<RodChecker>().PaintHair(selectedColor);
            DOTween.Kill("BackLid");
            lid.DOLocalMoveY(-0.1f, 2).SetId("GoLid");
            DOTween.Kill("BackWater");
            water.DOScaleY(0, 2).SetId("GoWater");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Plugger"))
        {
            other.transform.parent.GetChild(2).GetComponent<RodChecker>().IsPaintActive = false;
            DOTween.Kill("GoLid");
            lid.DOLocalMoveY(0, 0.1f).SetId("BackLid");
            DOTween.Kill("GoWater");
            water.DOScaleY(1, 0.1f).SetId("BackWater");
        }
    }

}
