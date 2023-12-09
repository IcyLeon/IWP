using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRootParent : MonoBehaviour
{
    public GameObject GetOwner()
    {
        return transform.root.gameObject;
    }
}
