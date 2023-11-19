using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class ArrowIndicator : MonoBehaviour
{
    private CharacterManager characterManager;
    private GameObject source;
    [SerializeField] Transform Pivot;
    private void Start()
    {
        characterManager = CharacterManager.GetInstance();
    }

    public GameObject GetSource()
    {
        return source;
    }
    public void SetSource(GameObject source)
    {
        this.source = source;
    }

    void Update()
    {
        UpdateRotation();
    }

    private void UpdateRotation()
    {
        if (source == null)
            return;

        Vector3 direction = (source.transform.position - characterManager.GetPlayerController().transform.position);
        direction.Normalize();
        Quaternion sourceRot = Quaternion.LookRotation(direction);
        sourceRot.z = -sourceRot.y;
        sourceRot.x = sourceRot.y = 0;
        Vector3 north = new Vector3(0, 0, Camera.main.transform.eulerAngles.y);

        Pivot.rotation = sourceRot * Quaternion.Euler(north);
    }
}
