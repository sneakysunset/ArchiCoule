using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils_Mesh 
{
    #region points
    public static float[] GeneratePointArray(float[] pointArray, float lineStart, float lineEnd, float lineResolution)
    {
        int pointArrayLength = Mathf.CeilToInt((lineEnd - lineStart) / lineResolution);
        pointArray = new float[pointArrayLength];


        for (int i = 0; i < pointArray.Length; i++)
        {
            pointArray[i] = lineStart + (i * lineResolution);
        }
        pointArray[pointArray.Length - 1] = lineEnd;
        return pointArray;
    }

    public static int CurrentListIndex(float[] pointArray, List<List<Vector2>> lineList, Vector2 currentPosition, float maxDistance, Action AddPointCall = null)
    {
        if(lineList.Count < 1)
        {
            AddLineList(pointArray, lineList, currentPosition, AddPointCall);
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

        AddLineList(pointArray, lineList, currentPosition, AddPointCall);
        return lineList.Count - 1;
    }

    static private void AddLineList(float[] pointArray, List<List<Vector2>> lineList, Vector2 currentPosition, Action AddPointCall = null)
    {
        lineList.Add(new List<Vector2>());
        Vector2 pointPos = new Vector2(closestPoint(pointArray, currentPosition.x), currentPosition.y);
        lineList[lineList.Count - 1].Add(pointPos);
        AddPointCall?.Invoke();
    }

    private static float closestPoint(float[] pointArray, float positionX)
    {
        float pointX = 0;
        float distance = 1000;
        foreach(float point in pointArray)
        {
            float tempDistance = Mathf.Abs(positionX - point);

            if(tempDistance < distance)
            {
                pointX = point;
                distance = tempDistance;
            }
        }
        return pointX;
    }

    public static int ClosestPointInList(List<Vector2> pointList, float currentPositionX, float[] pointArray)
    {
        //int closestVertexIndex = 0;
        int pointX = 0;
        float distance = 1000;
        for (int i = 0; i < pointList.Count; i++)
        {
            float tempDistance = Mathf.Abs(currentPositionX - pointList[i].x);

            if (tempDistance < distance)
            {
                pointX = i;
                distance = tempDistance;
            }
/*            float distancePrev = Mathf.Abs(closestPoint(pointArray, currentPositionX) - pointList[i - 1].x);
            float distanceCurr = Mathf.Abs(closestPoint(pointArray, currentPositionX) - pointList[i].x);
            bool condition = distanceCurr < distancePrev;

            if (condition) closestVertexIndex = i;*/
        }
        //return closestVertexIndex;
        return pointX;
    }


    public static void AddPoints(float[] pointArray, List<Vector2> pointList, float closestVertice, Vector2 currentPosition, float minDistance)
    {
        float playerPoint = closestPoint(pointArray, currentPosition.x);
        float temp = Mathf.Abs(playerPoint - closestVertice);
        float prevY = pointList[pointList.Count-1].y;

        if(playerPoint - closestVertice < 0)
        {
            for (float i = temp - minDistance; i > 0; i -= minDistance)
            {
                float xPos = playerPoint + i;
                float yPos = currentPosition.y - ((currentPosition.y - prevY) * i / temp);
                Vector2 newPoint = new Vector2(xPos, yPos);
                pointList.Add(newPoint);
            }
        }
        else
        {
            for (float i = temp - minDistance; i > 0; i -= minDistance)
            {
                float xPos = playerPoint - i;
                float yPos = currentPosition.y - ((currentPosition.y - prevY) * i / temp);
                Vector2 newPoint = new Vector2(xPos, yPos);
                pointList.Add(newPoint);
            }
        }
        pointList.Add(new Vector2(playerPoint, currentPosition.y));
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
        //vertices = vertices.OrderBy(v => v.x).ToArray();

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
        float angle = Vector3.Angle(currentPoint - previousPoint, currentPoint - nextPoint);
        //Debug.Log(angle);
        angle /= 2;
        Vector3 bissectriceVector = Quaternion.AngleAxis(angle, Vector3.forward) * (currentPoint - previousPoint);
        Vector3 parallelePoint = currentPoint + (bissectriceVector.normalized * lineWidth);
        return parallelePoint;
    }
    #endregion
}
