using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DressColor : MonoBehaviour
{
    [SerializeField]
    private Transform dressTransform;
    [SerializeField]
    private List<Color> dressColor = new List<Color>();
    private int levelValue;
    
    void Start()
    {
        levelValue = LevelHelper.Instance.ActiveLevel -10;
        dressTransform.GetComponent<SkinnedMeshRenderer>().material.color = dressColor[levelValue];
    }
    
}
