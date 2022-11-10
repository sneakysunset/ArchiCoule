using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class ConstantMeshGeneration : MonoBehaviour
{
    #region Variables
    /*[SerializeField]*/ private float[] pointArray;
    private List<Vector2> pointList = new List<Vector2>();
    private Color col;
    public GameObject meshPrefab;
    private CharacterController2D charC;
    Transform meshFolder;
    private MeshFilter meshF;
    private MeshCollider meshC;
    private MeshRenderer meshR;
    [Space(20)]
    [Range(.001f, 1)] public float lineResolution = .5f;
    public float lineBeginningX = -13f;
    public float lineEndX = 13f;
    public float width = .3f;
    public float lineYOffSet = 0;
    #endregion

    private void Start()
    {
        meshFolder = GameObject.FindGameObjectWithTag("MeshFolder").transform;
        charC = GetComponent<CharacterController2D>();
        pointArray = Utils_Mesh.GeneratePointArray(pointArray, lineBeginningX, lineEndX, lineResolution);
        col = charC.col;
        var firstPoint = new Vector2(Utils_Mesh.closestPoint(pointArray, transform.position.x), transform.position.y);
        pointList.Add(firstPoint);
        InstantiateMesh();
        //InvokeRepeating("MeshCreator", 0, 0.005f);
    }

    private void InstantiateMesh()
    {
        GameObject temp = Instantiate(meshPrefab, meshFolder);
        meshF = temp.GetComponent<MeshFilter>();
        meshC = temp.GetComponent<MeshCollider>();
        meshR = temp.GetComponent<MeshRenderer>();
        meshR.material.color = col;
        temp.name = "Mesh " + charC.playerType.ToString();
    }

    private void FixedUpdate()
    {
        MeshCreator();
    }

    void MeshCreator()
    {
        var list = pointList.OrderBy(v => v.x).ToList();
        UpdatePointList();
        FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCorde/TirageCorde");
        if (list.Count < 4) return;

        Mesh m = new Mesh();
        m.name = "trailMesh";

        Utils_Mesh.UpdateMeshVertices(list, width, m);
        Utils_Mesh.UpdateMeshTriangles(list.Count, m);
        m.MarkDynamic();
        m.Optimize();
        m.OptimizeReorderVertexBuffer();
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();
        meshF.mesh = m;
        meshC.sharedMesh = null;
        meshC.sharedMesh = m;
    }

    public void UpdatePointList()
    {
        int closestVertexIndex = Utils_Mesh.ClosestPointInList(pointList, transform.position.x, pointArray);
        float closestVertexX = pointList[closestVertexIndex].x;
        //Enleve les doublons, il faut trouver une alternative
        for (int i = 1; i < pointList.Count; i++)
        {
            if (Mathf.Abs(pointList[i].x - pointList[i - 1].x) < lineResolution * .9f)
            {
                pointList.RemoveAt(i);
                return;
            }
        }
        bool condition2 = Mathf.Abs(transform.position.x - closestVertexX) > lineResolution;

        if (condition2)
            Utils_Mesh.AddPoints(pointArray, pointList, closestVertexX, transform.position - Vector3.up * lineYOffSet, lineResolution, lineYOffSet);
        else if (!condition2)
            Utils_Mesh.UpdatePointsPos(pointList, closestVertexIndex, transform.position, lineYOffSet);
    }
}
