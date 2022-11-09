using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class MeshGeneration : MonoBehaviour
{
    #region Variables
    [SerializeField]private float[] pointArray;
    private List<List<Vector2>> lineList = new List<List<Vector2>>();
    public List<Vector2> linelists;
    private List<MeshFilter> meshsF = new List<MeshFilter>();
    private List<MeshCollider> meshsC = new List<MeshCollider>();
    private Color col;
    public GameObject meshPrefab;
    private CharacterController2D charC;
    Transform meshFolder;
    [Space(20)]
    [Range(.001f, 1)] public float lineResolution = .5f;
    public float lineBeginningX = -13f;
    public float lineEndX = 13f;
    //public float distanceToAddPoint = .5f;
    public float distanceToCreateNewMesh = 2;
    public float width = .3f;
    public float lineYOffSet = 0;
    public KeyCode renderKey;
    #endregion

    private void Start()
    {
        meshFolder = GameObject.FindGameObjectWithTag("MeshFolder").transform;
        charC = GetComponent<CharacterController2D>();
        pointArray = Utils_Mesh.GeneratePointArray(pointArray, lineBeginningX, lineEndX, lineResolution);
        col = charC.col;
        //InvokeRepeating("Inputer", 0, 0.005f);
    }

    private void Update()
    {
        if (Input.GetKey(renderKey)) input = true;
        else input = false;
    }

    bool input;
    private void FixedUpdate()
    {
        if (input) MeshCreator();
    }

    void MeshCreator()
    {
        int listIndex = Utils_Mesh.CurrentListIndex(pointArray, lineList, transform.position, distanceToCreateNewMesh, InstantiateMesh);
        var list = lineList[listIndex];
        list = list.OrderBy(v => v.x).ToList();
        UpdatePointList(listIndex);

        if (list.Count < 4) return;

        Mesh m = new Mesh();
        m.name = "trailMesh";

        linelists.Clear() ;
        linelists = list;

        Utils_Mesh.UpdateMeshVertices(list, width, m);
        Utils_Mesh.UpdateMeshTriangles(list.Count, m);
        m.MarkDynamic();
        m.Optimize();
        m.OptimizeReorderVertexBuffer();
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();    
        meshsF[listIndex].mesh = m;
        meshsC[listIndex].sharedMesh = null;
        meshsC[listIndex].sharedMesh = m;
        meshsF[listIndex].GetComponent<Renderer>().material.color = col;
    }

    public void UpdatePointList(int listIndex)
    {
        int closestVertexIndex = Utils_Mesh.ClosestPointInList(lineList[listIndex], transform.position.x, pointArray);
        float closestVertexX = lineList[listIndex][closestVertexIndex].x;
        //Enleve les doublons, il faut trouver une alternative
        for (int i = 1; i < lineList[listIndex].Count; i++)
        {
            if (Mathf.Abs(lineList[listIndex][i].x - lineList[listIndex][i - 1].x) < lineResolution * .9f)
            {
                lineList[listIndex].RemoveAt(i);
                return;
            }
        }
        bool condition2 = Mathf.Abs(transform.position.x - closestVertexX) > lineResolution;
      
        if (condition2)
            Utils_Mesh.AddPoints(pointArray ,lineList[listIndex], closestVertexX, transform.position -  Vector3.up * lineYOffSet, lineResolution, lineYOffSet);
        else if (!condition2)
            Utils_Mesh.UpdatePointsPos(lineList[listIndex], closestVertexIndex, transform.position, lineYOffSet);
    }

    void Inputer()
    {
        if (Input.GetKey(KeyCode.Space)) MeshCreator();
    }

    void InstantiateMesh()
    {
        GameObject temp = Instantiate(meshPrefab, meshFolder);
        meshsF.Add(temp.GetComponent<MeshFilter>());
        meshsC.Add(temp.GetComponent<MeshCollider>());
        temp.name = "Mesh " + charC.playerType.ToString() + "N." + (meshsF.Count - 1);
    }



}
