using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private static ObjectManager instance;

    public static ObjectManager Instance { get => instance; set => instance = value; }
    public GameObject LevelDesigner { get => levelDesigner; set => levelDesigner = value; }
    public List<Color> SyringeMat { get => syringeMat; set => syringeMat = value; }

    [SerializeField]
    private GameObject levelDesigner;

    [SerializeField]
    private List<Color> syringeMat = new List<Color>();


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

    }
}
