using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSound : MonoBehaviour
{
    private LineCreator lineC;
    private FMOD.Studio.EventInstance sound;
    public float soundUpdateTimer;
    WaitForSeconds waiter;
    private float maxHeight;
    private float minHeight;
    public GameObject visualPrefab;
    private Transform currentVisual;
    IEnumerator soundEnum;

    private void Start()
    {
        waiter = new WaitForSeconds(soundUpdateTimer);
    }

    public void PlaySound()
    {
        if (IsPlaying())
        {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            sound.release();
            currentVisual.position = lineC.pointList[0];
            StopCoroutine(soundEnum);
            soundEnum = null;
        }
        else
        {
            currentVisual = Instantiate(visualPrefab, lineC.pointList[0], Quaternion.identity).transform;

        }

        soundEnum = soundControl();
        
        lineC = FindObjectOfType<LineCreator>();
        sound = FMODUnity.RuntimeManager.CreateInstance("event:/");
        sound.start();
        StartCoroutine(soundEnum);
    }

    bool IsPlaying()
    {
        FMOD.Studio.PLAYBACK_STATE state;
        sound.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    void GetMaxHeight()
    {
        RaycastHit2D hitTop = Physics2D.Raycast(lineC.transform.position, Vector2.up, Mathf.Infinity, 15); 
        RaycastHit2D hitBottom = Physics2D.Raycast(lineC.transform.position, Vector2.down, Mathf.Infinity, 15);
        maxHeight = hitTop.point.y - hitBottom.point.y;
        minHeight = hitBottom.point.y;
    }

    IEnumerator soundControl()
    {
        int i = 0;
        while(i < lineC.pointList.Count - 1)
        {
            i++;
            sound.setParameterByName("parameterName", (lineC.pointList[i].y - minHeight)/ maxHeight);
            currentVisual.position = lineC.pointList[i];
            yield return waiter;
        }
        sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        sound.release();
        Destroy(currentVisual.gameObject);
        StopCoroutine(soundEnum);
        soundEnum = null;
    }
}
