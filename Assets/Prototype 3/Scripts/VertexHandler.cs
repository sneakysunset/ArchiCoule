using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexHandler : MonoBehaviour
{
    public List<List<Vector2>> vertexLists = new List<List<Vector2>>();
    public Transform player;
    public float distanceToAddPoint = .5f;
    public float distanceToCreateNewMesh = 2;
    private int maxVertexListSize = 0;
    public GameObject MeshPrefab;
    public List<MeshFilter> meshs;
    public List<MeshCollider> meshsD;
    public List<Vector2> vertices;
    private void Start()
    {
        vertexLists.Add(new List<Vector2>());
        vertexLists[0].Add(player.position);
        meshs.Add(Instantiate(MeshPrefab).GetComponent<MeshFilter>());
        meshsD.Add(Instantiate(MeshPrefab).GetComponent<MeshCollider>());
        
    }

    private void Update()
    {
        vertices = vertexLists[maxVertexListSize];
    }


    public int checkCurrentList()
    {
        for (int i = 0; i <= maxVertexListSize; i++)
        {
            for (int j = 0; j < vertexLists[i].Count; j++)
            {
                if (Mathf.Abs(vertexLists[i][j].x - transform.position.x) < distanceToCreateNewMesh)
                {
                    //print("No Change :" + i);
                    return i;
                }
            }
        }
        maxVertexListSize++;
        vertexLists.Add(new List<Vector2>());
        vertexLists[maxVertexListSize].Add(player.position);
        meshs.Add(Instantiate(MeshPrefab).GetComponent<MeshFilter>());
        meshsD.Add(Instantiate(MeshPrefab).GetComponent<MeshCollider>());

        return maxVertexListSize;
    }

    public void VerticesGeneration(int vertexListIndex)
    {
        int closestVertex = GetClosestVertexIndex(vertexListIndex);
        //print("ClosestVertex = " + closestVertex);
        bool condition1 = closestVertex != -1;
        bool condition2 = Mathf.Abs(player.position.x - vertexLists[vertexListIndex][closestVertex].x) > distanceToAddPoint;
        // print("Condition2 =" + condition2);
        for (int i = 1; i < vertexLists[vertexListIndex].Count; i++)
        {
            if (Mathf.Abs(vertexLists[vertexListIndex][i].x - vertexLists[vertexListIndex][i - 1].x) < distanceToAddPoint - 0.2)
            {
                vertexLists[vertexListIndex].RemoveAt(i);
                return;
            }
        }

        if (condition1 && condition2)
        {
            AddVertices(vertexListIndex, vertexLists[vertexListIndex][closestVertex].x);
        }
        else if(condition1 && !condition2)
        {
            UpdateVerticePosition(vertexListIndex);
        }
    }

    private void AddVertices(int vertexListIndex, float closestPoint)
    {
        float temp = Mathf.Abs(player.position.x - closestPoint);
        for (float i = temp; i > 0; i -= distanceToAddPoint)
        {
            Vector2 newPoint = new Vector2(player.position.x - i, player.position.y);
            vertexLists[vertexListIndex].Add(newPoint);
        }
    }

    private void UpdateVerticePosition(int vertexListIndex)
    {
        
    }

    private int GetClosestVertexIndex(int vertexListIndex)
    {
        int closestVertexIndex = -1;
            //print(vertexListIndex);
        if (vertexLists[vertexListIndex].Count - 1 < 1) return 0;
        for (int i = 1; i < vertexLists[vertexListIndex].Count; i++)
        {
            float distancePrev = Mathf.Abs(player.position.x - vertexLists[vertexListIndex][i - 1].x);
            float distanceCurr = Mathf.Abs(player.position.x - vertexLists[vertexListIndex][i].x);
            bool condition = distanceCurr < distancePrev;
            if (condition)
            {
                closestVertexIndex = i;
            }
        }
        return closestVertexIndex;
    }
}
