using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration : MonoBehaviour
{
    #region Variables
    private List<List<Vector2>> lineList = new List<List<Vector2>>();
    public List<Vector2> linelists;
    private List<MeshFilter> meshsF = new List<MeshFilter>();
    private List<MeshCollider> meshsC = new List<MeshCollider>();
    public GameObject meshPrefab;
    [Space(20)]
    public float distanceToAddPoint = .5f;
    public float distanceToCreateNewMesh = 2;
    public float width = .3f;
    public float lineYOffSet = 0;
    #endregion

    private void Start()
    {
        //InvokeRepeating("Inputer", 0, 0.01f);
    }

    private void Update()
    {

        if (lineList.Count > 0)
            linelists = lineList[0];
        if (Input.GetKey(KeyCode.Space)) MeshCreator();
    }


    void MeshCreator()
    {
        int listIndex = Utils_MeshFunctions.CurrentListIndex(lineList, transform.position, distanceToCreateNewMesh, InstantiateMesh);
        var list = lineList[listIndex];
        UpdatePointList(listIndex);

        if (list.Count < 4) return;

        Mesh m = new Mesh();
        m.name = "trailMesh";
        Utils_MeshFunctions.UpdateMeshVertices(list, width, m);
        Utils_MeshFunctions.UpdateMeshTriangles(list.Count, m);
        m.MarkDynamic();
        m.Optimize();
        m.OptimizeReorderVertexBuffer();
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();    
        meshsF[listIndex].mesh = m;
        meshsC[listIndex].sharedMesh = null;
        meshsC[listIndex].sharedMesh = m;
    }

    public void UpdatePointList(int listIndex)
    {
        int closestVertexIndex = Utils_MeshFunctions.ClosestPointInList(lineList[listIndex], transform.position.x);
        float closestVertexX = lineList[listIndex][closestVertexIndex].x;
/*        bool condition1 = closestVertexIndex != 0;*/
        bool condition2 = Mathf.Abs(transform.position.x - closestVertexX) > distanceToAddPoint;
        print(closestVertexX);
        for (int i = 1; i < lineList[listIndex].Count; i++)
        {
            if (Mathf.Abs(lineList[listIndex][i].x - lineList[listIndex][i - 1].x) < distanceToAddPoint - 0.2)
            {
                lineList[listIndex].RemoveAt(i);
                return;
            }
        }

        if (/*condition1 && */condition2)
            Utils_MeshFunctions.AddPoints(lineList[listIndex], closestVertexX, transform.position, distanceToAddPoint);
        else if (/*condition1 &&*/ !condition2)
            Utils_MeshFunctions.UpdatePointsPos(lineList[listIndex], closestVertexIndex, transform.position, lineYOffSet);
    }

    void Inputer()
    {
        if (lineList.Count > 0)
            linelists = lineList[0];
        if (Input.GetKey(KeyCode.Space)) MeshCreator();
    }

    void InstantiateMesh()
    {
        meshsF.Add(Instantiate(meshPrefab).GetComponent<MeshFilter>());
        meshsC.Add(Instantiate(meshPrefab).GetComponent<MeshCollider>());
    }



}
