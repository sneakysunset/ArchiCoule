using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
static public class Anims
{
    //Animation d'un déplacement d'une position à une autre.
    //L'animC est une animation curve permettant de personnaliser la vitesse de l'animation en fonction de son temps de play.
    //encCoroutineEvent est un event à rentrer quand on appel la fonction permettant d'avoir un call back à la fin de l'animation.
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
