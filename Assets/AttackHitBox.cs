using System;
using System.Collections;
using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    //Stupid silly unity collision detection like why does it have be MOVING / HAVE MOVED TO CALL ON TRIGGER ENTER AGAIN IF IT IS ENABLED AND DISABLED LIKE AIUGHKJHGKJHKSDJGHLSKDJGHLSKDJ GOD DAMN 
    private void OnEnable() 
    {
        StartCoroutine(NudgeSlightly());
    }

    IEnumerator NudgeSlightly()
    {
        Vector3 originalPos = transform.localPosition;
        transform.localPosition += Vector3.up * 0.001f;
        yield return new WaitForFixedUpdate();
        transform.localPosition = originalPos;
    }
}
