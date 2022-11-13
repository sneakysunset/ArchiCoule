using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Utils_Points
{
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
        if (lineList.Count < 1)
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

    public static float closestPoint(float[] pointArray, float positionX)
    {
        float pointX = 0;
        float distance = 1000;
        foreach (float point in pointArray)
        {
            float tempDistance = Mathf.Abs(positionX - point);

            if (tempDistance < distance)
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

    public static void AddPoints(float[] pointArray, List<Vector2> pointList, float closestVertice, Vector2 currentPosition, float minDistance, float offSetY, CharacterController2D charC)
    {
        float playerPointX = closestPoint(pointArray, currentPosition.x);
        float temp = Mathf.Abs(playerPointX - closestVertice);
        float prevY = pointList[pointList.Count - 1].y;

        if (playerPointX - closestVertice < 0)
        {
            for (float i = temp - minDistance; i > 0; i -= minDistance)
            {
                float xPos = playerPointX + i;
                float yPos = currentPosition.y - ((currentPosition.y - prevY) * i / temp);
                Vector2 newPoint = new Vector2(xPos, yPos);
                pointList.Add(newPoint);
                charC.transform.localScale -= Vector3.one * charC.movementScaler / 100;

            }
        }
        else
        {
            for (float i = temp - minDistance; i > 0; i -= minDistance)
            {
                float xPos = playerPointX - i;
                float yPos = currentPosition.y - ((currentPosition.y - prevY) * i / temp);
                Vector2 newPoint = new Vector2(xPos, yPos);
                pointList.Add(newPoint);
                charC.transform.localScale -= Vector3.one * charC.movementScaler / 100;
            }
        }
        pointList.Add(new Vector2(playerPointX, currentPosition.y));
        charC.transform.localScale -= Vector3.one * charC.movementScaler / 100;

    }

    public static void UpdatePointsPos(List<Vector2> pointList, int closestPointIndex, Vector2 currentPosition, float offSetY)
    {
        pointList[closestPointIndex] = new Vector2(pointList[closestPointIndex].x, currentPosition.y - offSetY);
    }

    public static Vector3 GetParallelePoint(Vector3 currentPoint, Vector3 previousPoint, Vector3 nextPoint, float lineWidth)
    {
        float angle = Vector3.Angle(previousPoint - currentPoint, nextPoint - currentPoint);
        //Debug.Log(angle);
        angle /= 2;
        Vector3 bissectriceVector = Quaternion.AngleAxis(angle, Vector3.forward) * (nextPoint - currentPoint);
        /*if (bissectriceVector.y <= 0)
            Debug.Log("< " + angle);*/
        //bissectriceVector = Quaternion.AngleAxis(angle, Vector3.forward) * (nextPoint - currentPoint);
        Vector3 parallelePoint = currentPoint + (bissectriceVector.normalized * lineWidth);
        return parallelePoint;
    }
}
