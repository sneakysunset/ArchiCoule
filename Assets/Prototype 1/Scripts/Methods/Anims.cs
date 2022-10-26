using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
static public class Anims
{
    static public IEnumerator MovementAnim(float complitionTime, Vector3 startPos, Vector3 endPos, Transform objT, AnimationCurve animC, UnityEvent endCoroutineEvent = null)
    {
        float timer = 0;

        while(timer < 1)
        {
            timer += Time.deltaTime * (1 / complitionTime);
            objT.position = Vector3.Lerp(startPos, endPos, animC.Evaluate(timer));
            yield return null;
        }
        objT.position = endPos;
        endCoroutineEvent?.Invoke();
        yield return null;
    }
}
