using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils_MeshFunctions 
{
    #region points
    public static int CurrentListIndex(List<List<Vector2>> lineList, Vector2 currentPosition, float maxDistance, Action AddPointCall = null)
    {
        if(lineList.Count < 1)
        {
            AddLineList(lineList, currentPosition, AddPointCall);
            return lineList.Count - 1;
        }

        for (int i = 0; i < lineList.Count; i++)
        {
            for (int j = 0; j < lineList[i].Count; j++)
            {
                float distance = Mathf.Abs(lineList[i][j].x - currentPosition.x);
                if (distance < maxDistance) return i;
            }
        }

        AddLineList(lineList, currentPosition, AddPointCall);
        return lineList.Count - 1;
    }
    static private void AddLineList(List<List<Vector2>> lineList, Vector2 currentPosition, Action AddPointCall = null)
    {
        lineList.Add(new List<Vector2>());
        lineList[lineList.Count - 1].Add(currentPosition);
        AddPointCall?.Invoke();
    }

    public static int ClosestPointInList(List<Vector2> pointList, float currentPositionX)
    {
        int closestVertexIndex = 0;
        for (int i = 1; i < pointList.Count; i++)
        {
            float distancePrev = Mathf.Abs(currentPositionX - pointList[i - 1].x);
            float distanceCurr = Mathf.Abs(currentPositionX - pointList[i].x);
            bool condition = distanceCurr < distancePrev;

            if (condition) closestVertexIndex = i;
        }
        return closestVertexIndex;
    }


    public static void AddPoints(List<Vector2> pointList, float closestPoint, Vector2 currentPosition, float minDistance)
    {
            Debug.Log(pointList.Count);
        float temp = Mathf.Abs(currentPosition.x - closestPoint);
        for (float i = temp; i > 0; i -= minDistance)
        {
            Vector2 newPoint = new Vector2(currentPosition.x - i, currentPosition.y);
            pointList.Add(newPoint);
        }
    }

    public static void UpdatePointsPos(List<Vector2> pointList, int closestPointIndex, Vector2 currentPosition, float offSetY)
    {
        pointList[closestPointIndex] = new Vector2(pointList[closestPointIndex].x, currentPosition.y - offSetY);
    }

    
    #endregion

    #region mesh
    public static void UpdateMeshVertices(List<Vector2> pointList, float lineWidth, Mesh m)
    {
        int listLength = 2 * pointList.Count - 1;
        Vector3[] vertices = new Vector3[listLength];

        for (int i = 0; i < vertices.Length; i++)
        {
            if (i == vertices.Length - 1 || i == 1)
            {
                vertices[i] = pointList[(i - 1) / 2] + new Vector2(0, lineWidth);
            }
            else if (i % 2 == 0)
            {
                vertices[i] = pointList[i / 2];
            }
            else
            {
                int pI = (i - 1) / 2;
                vertices[i] = pointList[pI] + new Vector2(0, lineWidth);
                //vertices[i] = ParallelePoint(pointList[pI], pointList[pI - 1], pointList[pI + 1], lineWidth);
            }
        }
        m.vertices = vertices;
    }

    public static void UpdateMeshTriangles(int pointListLength, Mesh m)
    {
        int[] triangles = new int[6 * pointListLength - 9];
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

            triangles[i] = i + k - 4 * f;
        }
        m.triangles = triangles;
    }

    private static Vector3 ParallelePoint(Vector3 currentPoint, Vector3 previousPoint, Vector3 nextPoint, float lineWidth)
    {
        Debug.Log("angle1 :" + Vector3.Angle(currentPoint - previousPoint, currentPoint - nextPoint));
        Debug.Log("angle2 :" + Vector3.Angle(currentPoint - nextPoint, currentPoint - previousPoint));
        Debug.Log("angle3 :" + Vector3.Angle(previousPoint - currentPoint, nextPoint - currentPoint));
        Debug.Log("angle4 :" + Vector3.Angle(nextPoint - currentPoint, previousPoint - currentPoint));

        float angle = Vector3.Angle(currentPoint - previousPoint, currentPoint - nextPoint);
        angle /= 2;
        Vector3 bissectriceVector = Quaternion.AngleAxis(angle, Vector3.forward) * (currentPoint - previousPoint);
        Vector3 parallelePoint = currentPoint + bissectriceVector * lineWidth;
        return parallelePoint;
    }
    #endregion
}
