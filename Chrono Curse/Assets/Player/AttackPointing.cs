using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPointing : MonoBehaviour
{

    public Transform AttackPoint;

    public void FlipPoint(bool right)
    {
        if (right)
        {
            AttackPoint.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (!right)
        {
            AttackPoint.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
