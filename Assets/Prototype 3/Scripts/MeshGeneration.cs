using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneration : MonoBehaviour
{
    private VertexHandler vertexSource;
    public float width = .3f;
    public Vector3[] trianglesr; 
    private void Start()
    {
        vertexSource = GetComponent<VertexHandler>();
        InvokeRepeating("hqhq", 0, 0.01f);

    }



    void hqhq()
    {
        if (Input.GetKey(KeyCode.Space)) MeshCreator();
    }
    void MeshCreator()
    {
        var list = vertexSource.vertexLists[vertexSource.checkCurrentList()];
        vertexSource.VerticesGeneration(vertexSource.checkCurrentList());
        Mesh m = new Mesh();
        m.name = "trailMesh";
        if (list.Count < 4)
        {
            return;
        }
        int listLength = 2 * list.Count - 1;
        Vector3[] vertice = new Vector3[listLength];

        for (int i = 0; i < vertice.Length; i++)
        {
            if (i == vertice.Length - 1 || i == 1)
            {
                vertice[i] = list[(i - 1)/2] + new Vector2(0,width);
            }
            else if(i % 2 == 0)
            {
                vertice[i] = list[i / 2];
            }
            else
            {
                int pI = (i - 1) / 2;
                vertice[i] = list[pI] + new Vector2(0, width);
                // vertice[i] = ParallelePoint(list[pI], list[pI - 1], list[pI + 1]);
            }
        }
        m.vertices = vertice;
        int[] triangles = new int[3 * listLength - 6];
        trianglesr = new Vector3[3 * listLength - 6];
        int j = 0;
        int k = 0;
        int f = 0;
        for (int i = 0; i < triangles.Length; i++)
        {
            if (j == 6)
            {
                j = 0;
                f++;
            }

            if (j <= 3) k = 0;
            else if (j == 4) k = -2;
            else if (j == 5) k = -4;
            j++;

            triangles[i] = i + k  - 4 * f;
            trianglesr[i] = m.vertices[i + k - 4 * f];
        }
        m.triangles = triangles;
        m.MarkDynamic();
        m.Optimize();
        m.OptimizeReorderVertexBuffer();
        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();    
        vertexSource.meshs[vertexSource.checkCurrentList()].mesh = m;
        vertexSource.meshsD[vertexSource.checkCurrentList()].sharedMesh = null;
        vertexSource.meshsD[vertexSource.checkCurrentList()].sharedMesh = m;
        
    }

/*    Vector3 ParallelePoint(Vector3 currentPoint, Vector3 previousPoint, Vector3 nextPoint)
    {
*//*        float angle = Vector3.Angle(currentPoint - previousPoint, currentPoint - nextPoint);
        angle /= 2;
        Vector3 bissectriceVector = Quaternion.AngleAxis(angle, Vector3.forward) * (currentPoint - previousPoint);
        Vector3 parallelePoint = currentPoint + bissectriceVector * width;*//*
        return parallelePoint;
    }*/
}
