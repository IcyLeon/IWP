using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] Transform SlashPivot;
    public Transform GetSlashPivot()
    {
        return SlashPivot;
    }
}
