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
    public KeyCode absorbLineKey;
    [Range(.001f, 1)] public float lineResolution = .5f;
    public float lineBeginningX = -13f;
    public float lineEndX = 13f;
    public float width = .3f;
    public float lineYOffSet = 0;
    public int numberOfPointAbsorbed;
    public float threshHoldToUpdatePoints;
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
    }

    bool absorbing;
    private void Update()
    {
        if (Input.GetKey(absorbLineKey))
        {
            absorbing = true;
        }
        else absorbing = false;
    }

    private void InstantiateMesh()
    {
        GameObject temp = Instantiate(meshPrefab, meshFolder);
        meshF = temp.GetComponent<MeshFilter>();
        meshC = temp.GetComponent<MeshCollider>();
        meshR = temp.GetComponent<MeshRenderer>();
        meshR.material.color = col;
        temp.name = "Mesh " + charC.playerType.ToString();
        charC.meshObj = temp;
    }

    bool absorbFlag = false;
    public float absorbRate;
    private void FixedUpdate()
    {
        if (absorbing && pointList.Count > 1 && !absorbFlag)
        {
            StartCoroutine(MeshAbsorption(absorbRate));
            absorbFlag = true;
        }
        else MeshCreator();
    }

    IEnumerator MeshAbsorption(float timer)
    {
        yield return new WaitForSeconds(timer);
        int i = 0;
        while (i < numberOfPointAbsorbed)
        {
            i++;
            pointList.RemoveAt(0);
            charC.transform.localScale += Vector3.one * charC.movementScaler * Time.deltaTime;
        }

        absorbFlag = false;
        MeshCreator();
    }

    void MeshCreator()
    {
        var list = pointList;
        list = pointList.OrderBy(v => v.x).ToList();

        if (!UpdatePointList()) return;

        if (list.Count < 4 || meshF.gameObject.layer == 10) return;

        
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

    public AnimationCurve anim;
    public float animSpeed;
    public bool UpdatePointList()
    {
        int closestVertexIndex = Utils_Mesh.ClosestPointInList(pointList, transform.position.x, pointArray);
        float closestVertexX = pointList[closestVertexIndex].x;
        
        BandAidRemovePoints();

        closestVertexIndex = Utils_Mesh.ClosestPointInList(pointList, transform.position.x, pointArray);
        closestVertexX = pointList[closestVertexIndex].x;

        bool condition2 = false;
        bool condition1 = Mathf.Abs(transform.position.x - closestVertexX) > lineResolution;
        if(pointList.Count > 4)
        {
            condition2 =  Mathf.Abs(transform.position.y - pointList[closestVertexIndex].y) > threshHoldToUpdatePoints;
        }


        if (condition1)
        {
            Utils_Mesh.AddPoints(pointArray, pointList, closestVertexX, transform.position - Vector3.up * lineYOffSet, lineResolution, lineYOffSet, charC);
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCorde/TirageCorde");
            return true;
        }
        else if (!condition1 && condition2)
        {
            StartCoroutine(AnimationLerp(pointList[closestVertexIndex].y, transform.position.y - lineYOffSet, anim, closestVertexIndex, animSpeed));
            //Remplacer par son de Update de corde;
            //FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCorde/TirageCorde");
            //Utils_Mesh.UpdatePointsPos(pointList, closestVertexIndex, transform.position, lineYOffSet);
            return true;
        }
        else
        {
            return false;
        }
    }
    IEnumerator AnimationLerp(float startPos, float endPos, AnimationCurve anim, int i, float speed, Action onLerpEnd = null)
    {
        float j = 0;
        while (j < 1)
        {
            j += Time.deltaTime * (1 / speed);
            var Vec = pointList[i];
            Vec.y = Mathf.Lerp(startPos, endPos, anim.Evaluate(j));
            pointList[i] = Vec;
            yield return new WaitForEndOfFrame();
        }

        pointList[i] = new Vector2(pointList[i].x, endPos);
        onLerpEnd?.Invoke();
        yield return null;
    }


    //Enleve les doublons, il faut trouver une alternative
    void BandAidRemovePoints()
    {
        for (int i = 1; i < pointList.Count; i++)
        {
            if (Mathf.Abs(pointList[i].x - pointList[i - 1].x) < lineResolution * .9f)
            {
                pointList.RemoveAt(i);
                charC.transform.localScale += Vector3.one * charC.movementScaler / 100;
                return;
            }
        }
    }
}
