using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDesigner : MonoBehaviour
{

    [SerializeField]
    private int ropeCount;

    public int RopeCount { get => ropeCount; set => ropeCount = value; }
}
