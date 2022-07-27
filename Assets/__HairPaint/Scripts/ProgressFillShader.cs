
using UnityEngine;

public class ProgressFillShader : MonoBehaviour
{

    Material objectMaterial;

    void Start()
    {
        objectMaterial = GetComponent<Renderer>().material;
        float progressBorder = GetComponent<MeshFilter>().mesh.bounds.size.x / 2f;
        objectMaterial.SetFloat("_ProgresssBorder", progressBorder);
    }

}
