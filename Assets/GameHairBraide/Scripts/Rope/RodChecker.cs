﻿using UnityEngine;
using Unity.Profiling;
using System.Collections.Generic;
using Obi;
using Five.String;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(ObiActor))]
public class RodChecker : MonoBehaviour
{
    [SerializeField]
    private GameObject _box2DCheckPrefab;
    [SerializeField]
    private Rope _hostRope;

    private static ProfilerMarker m_DrawParticlesPerfMarker = new ProfilerMarker("DrawParticles");

    public bool render = true;
    public Shader shader;
    public Color particleColor = Color.white;
    public float radiusScale = 1;

    private Material material;
    private ParticleImpostorRendering impostors;

    public IEnumerable<Mesh> ParticleMeshes => impostors.Meshes;

    private Mesh particleMesh;

    private Renderer ropeRenderer;

    public List<Mesh> listMeshes
    {
        get => impostors.listMeshes;
        set => impostors.listMeshes = value;
    }

    public Material ParticleMaterial => material;

    private bool _isCheckFree;
    public bool isCheckFree
    {
        get => _isCheckFree;
        set => _isCheckFree = value;
    }
    public bool IsPaintActive { get => isPaintActive; set => isPaintActive = value; }

    private bool isFree;

    private List<RodCheckBoxCollider2D> _rodCheckBoxCollider2DList = new List<RodCheckBoxCollider2D>();

    private int count = 0;
    private int _rodCheckBoxCollider2DListCount = 0;
    private Collider2D[] colliders;

    [SerializeField]
    private MeshRenderer _meshRenderer;

    [SerializeField]
    private float fillValue = 0;

    private bool isPaintActive;
    private bool isPaintChange;
    private bool isPainted;
    private bool isPaintedWrong;
    private Color oldColor;
    private float ambientValue = 2;
    private LevelHelper levelHelper;
    private bool isRopeFull;
    private GameManagerHelper gameManager;

    private void Start()
    {
        if (!_meshRenderer) _meshRenderer = GetComponent<MeshRenderer>();
        DMCGameUtilities_OnChangeMaterialRope(DMCGameUtilities.MaterialRopeCurrent);
        CreateListCheckBoxCollider();
        levelHelper = LevelHelper.Instance.GetComponent<LevelHelper>();
        gameManager = GameManagerHelper.Instance;
    }



    public void OnEnable()
    {
        impostors = new ParticleImpostorRendering();
        GetComponent<ObiActor>().OnInterpolate += DrawParticles;
        DMCGameUtilities.OnChangeMaterialRope += DMCGameUtilities_OnChangeMaterialRope;
        StartCoroutine(UpdateColliderPosition());


    }

    public void OnDisable()
    {
        GetComponent<ObiActor>().OnInterpolate -= DrawParticles;
        DMCGameUtilities.OnChangeMaterialRope -= DMCGameUtilities_OnChangeMaterialRope;

        if (impostors != null)
            impostors.ClearMeshes();
        DestroyImmediate(material);


    }


    private void Update()
    {
        if (IsPaintActive)
        {
            fillValue += Time.deltaTime * 2;
            ropeRenderer.sharedMaterial.SetFloat("_FillRate", fillValue);
            if(fillValue > 3f && !isRopeFull)
            {
                isRopeFull = true;
                levelHelper.FullRopeValue++;
                if (levelHelper.FullRopeValue>=3)
                {
                    levelHelper.NextLevelSave();
                    gameManager.GameWin();
                }
            }
        }
        if (isPainted && fillValue >= 0.2)
        {
            ropeRenderer.sharedMaterial.SetFloat("_FillRate", fillValue);
            ropeRenderer.sharedMaterial.SetFloat("_Ambient", ambientValue);
        }

    }

    public void PaintHair(Color selectedColor)
    {
        isRopeFull = false;
        DOTween.To(value => ambientValue = value, ambientValue, 3, 2).SetEase(Ease.Linear);

        if (isPainted && oldColor != selectedColor)
        {
            DOTween.To(value => fillValue = value, fillValue, 0.2f, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                ropeRenderer.sharedMaterial.SetColor("_FillColor", selectedColor);
                IsPaintActive = true;
                isPainted = false;
            });
        }
        else
        {
            isPainted = true;
            IsPaintActive = true;
            oldColor = selectedColor;
            ropeRenderer.sharedMaterial.SetColor("_FillColor", selectedColor);
        }
    }


    private void DMCGameUtilities_OnChangeMaterialRope(int index)
    {
        var mat = new Material(MenuTheme.instance._materialRope[index]);
        _meshRenderer.materials = new Material[1];
        _meshRenderer.material = mat;
        ropeRenderer = _meshRenderer.gameObject.GetComponent<Renderer>();
        ropeRenderer.sharedMaterial.SetFloat("_FillRate", fillValue);
    }




    private void CreateMaterialIfNeeded()
    {
        if (shader != null)
        {
            if (!shader.isSupported)
                Debug.LogWarning("Particle rendering shader not suported.");

            if (material == null || material.shader != shader)
            {
                DestroyImmediate(material);
                material = new Material(shader);
                material.hideFlags = HideFlags.HideAndDontSave;
            }
        }
    }

    private void DrawParticles(ObiActor actor)
    {
        using (m_DrawParticlesPerfMarker.Auto())
        {
            if (!isActiveAndEnabled || !actor.isActiveAndEnabled || actor.solver == null)
            {
                impostors.ClearMeshes();
                return;
            }

            CreateMaterialIfNeeded();

            impostors.UpdateMeshes(actor);

            DrawParticles();
        }
    }

    private void DrawParticles()
    {
        if (material != null)
        {
            material.SetFloat("_RadiusScale", radiusScale);
            material.SetColor("_Color", particleColor);

            // Send the meshes to be drawn:
            if (render)
            {
                foreach (Mesh mesh in impostors.Meshes)
                {
                    Graphics.DrawMesh(mesh, Matrix4x4.identity, material, gameObject.layer);
                }
            }
        }
    }

    private void CreateListCheckBoxCollider()
    {
        for (int i = 0; i < _hostRope.obiRope.activeParticleCount; i++)
        {
            Debug.Log("create particle colider");

            CreateBoxCollider2D().transform.position = _hostRope.obiRope.GetParticlePosition(i) + new Vector3(0, 0, 5);
        }
    }


    IEnumerator UpdateColliderPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            foreach (Mesh mesh in ParticleMeshes)
            {
                count = 0;
                _rodCheckBoxCollider2DListCount = _rodCheckBoxCollider2DList.Count;
                for (int i = 0; i < mesh.vertices.Length; i += 4)
                {

                    if (count < _rodCheckBoxCollider2DListCount)
                    {

                        _rodCheckBoxCollider2DList[count].transform.position = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 5f);
                    }
                    else
                    {
                        CreateBoxCollider2D().transform.position = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 5f);
                    }
                    count += 1;
                }
            }
            if (!isCheckFree) continue;
            //if (!_hostRope.isPluggerBusy && !_hostRope.isFree && _hostRope.curRodState != RopeState.unplugged)
            //{
            //    if (CheckFree() && isFree)
            //    {
            //        Debug.LogError("ERORR");
            //        _hostRope.IsFree();
            //        break;
            //    }
            //}
        }
    }


    public void StartCheck()
    {
    }


    //private void FixedUpdate()
    //{
    //    Debug.Log("CAI nay o ngoai" + _hostRod.obiRod.activeParticleCount);

    //    //if (!TangleMasterGame.instance.isPlayable) return;
    //    foreach (Mesh mesh in ParticleMeshes)
    //    {
    //        Debug.Log("CAI nay lag lam");
    //        count = 0;
    //        _rodCheckBoxCollider2DListCount = _rodCheckBoxCollider2DList.Count;
    //        for (int i = 0; i < mesh.vertices.Length; i += 4)
    //        {
    //            Debug.Log("CAI nay lag hon lam" + gameObject.GetInstanceID());

    //            if (count < _rodCheckBoxCollider2DListCount)
    //            {
    //                Debug.Log("in if" + gameObject.GetInstanceID());

    //                _rodCheckBoxCollider2DList[count].transform.position = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 5f);
    //            }
    //            else
    //            {
    //                CreateBoxCollider2D().transform.position = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 5f);
    //            }
    //            count += 1;
    //        }

    //        if (count < _rodCheckBoxCollider2DListCount)
    //        {
    //            for (int i = count; i < _rodCheckBoxCollider2DListCount; i++)
    //            {
    //                _rodCheckBoxCollider2DList[i].gameObject.SetActive(false);
    //            }
    //        }
    //    }
    //}
    //private void FixedUpdate()
    //{
    //    Debug.Log("CAI nay o ngoai");

    //    //if (!TangleMasterGame.instance.isPlayable) return;
    //    foreach (Mesh mesh in ParticleMeshes)
    //    {
    //        Debug.Log("CAI nay lag lam");
    //        count = 0;
    //        _rodCheckBoxCollider2DListCount = _rodCheckBoxCollider2DList.Count;
    //        //  for (int i = 0; i < mesh.vertices.Length; i += 4)
    //        {
    //            Debug.Log("CAI nay lag hon lam" + gameObject.GetInstanceID());


    //                CreateBoxCollider2D().transform.position = new Vector3(mesh.vertices[0].x, mesh.vertices[0].y, 5f);

    //        }

    //        //if (count < _rodCheckBoxCollider2DListCount)
    //        //{
    //        //    for (int i = count; i < _rodCheckBoxCollider2DListCount; i++)
    //        //    {
    //        //        _rodCheckBoxCollider2DList[i].gameObject.SetActive(false);
    //        //    }
    //        //}
    //    }
    //}
    private void LateUpdate()
    {

    }

    private bool CheckFree()
    {
        foreach (var rc in _rodCheckBoxCollider2DList)
        {
            if (!rc.gameObject.activeInHierarchy) continue;
            colliders = Physics2D.OverlapBoxAll(rc.transform.position, rc.bc2D.size, 0f);
            if (colliders.Length > 1)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject.GetComponent<RodCheckBoxCollider2D>().hostRope != _hostRope)
                    {
                        //FiveDebug.Log(_hostRod.name + " - Not Free At: " + i);
                        return false;
                    }
                }
            }
        }
        StartCoroutine(CheckFreeDelay(0.3f));
        //  FiveDebug.Log(_hostRope.name + " - Free!");
        return true;
    }

    IEnumerator CheckFreeDelay(float amout)
    {
        yield return new WaitForSeconds(amout);
        int flag = 0;
        foreach (var rc in _rodCheckBoxCollider2DList)
        {
            if (!rc.gameObject.activeInHierarchy) continue;
            colliders = Physics2D.OverlapBoxAll(rc.transform.position, rc.bc2D.size, 0f);
            if (colliders.Length > 1)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject.GetComponent<RodCheckBoxCollider2D>().hostRope != _hostRope)
                    {
                        //FiveDebug.Log(_hostRod.name + " - Not Free At: " + i);
                        isFree = false;
                        flag++;
                    }
                }
            }
        }
        //  FiveDebug.Log(_hostRope.name + " - Free!");
        if (flag == 0)
        {

            isFree = true;
        }
    }

    private Vector3 shadownSize = new Vector3(0.1f, 0.1f, 0.1f);

    private RodCheckBoxCollider2D CreateBoxCollider2D()
    {
        //FiveDebug.Log(_hostRod.name + " - CreateBoxCollider2D");
        RodCheckBoxCollider2D rodCheckBoxCollider2D = Instantiate(_box2DCheckPrefab, transform).GetComponent<RodCheckBoxCollider2D>();
        rodCheckBoxCollider2D.hostRope = _hostRope;
        rodCheckBoxCollider2D.bc2D.size = shadownSize;
        _rodCheckBoxCollider2DList.Add(rodCheckBoxCollider2D);
        return rodCheckBoxCollider2D;
    }

    //#if UNITY_EDITOR
    //    private void OnDrawGizmos()
    //    {
    //        if (!Application.isPlaying) return;
    //        foreach (Mesh mesh in ParticleMeshes)
    //        {
    //            for (int i = 1; i < mesh.vertices.Length; i += 4)
    //            {
    //                Gizmos.color = Color.cyan;
    //                Vector3 tarPos = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y, 5f);
    //                Gizmos.DrawLine(mesh.vertices[i], tarPos);
    //                Gizmos.DrawCube(tarPos, shadownSize);
    //            }
    //        }
    //    }
    //#endif
}