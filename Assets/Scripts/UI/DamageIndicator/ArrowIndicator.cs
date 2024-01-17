using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowIndicator : MonoBehaviour
{
    private GameObject target;
    private GameObject source;
    [SerializeField] Image ArrowImage;
    [SerializeField] Transform Pivot;

    public void SetArrowColor(Color32 color)
    {
        ArrowImage.color = color;
    }
    public GameObject GetSource()
    {
        return target;
    }
    public void SetTarget(GameObject t)
    {
        target = t;
    }
    public void SetSource(GameObject source)
    {
        this.source = source;
    }

    void Update()
    {
        if (target == null)
            Destroy(gameObject);

        UpdateRotation();
        UpdateIDamage();
    }

    void UpdateIDamage()
    {
        if (target == null)
            return;

        IDamage dmg = target.GetComponent<IDamage>();
        if (dmg != null)
        {
            if (dmg.IsDead())
                target = null;
        }
    }
    private void UpdateRotation()
    {
        if (source == null || target == null)
            return;

        Vector3 direction = (target.transform.position - source.transform.position);
        direction.Normalize();
        Quaternion sourceRot = Quaternion.LookRotation(direction);
        sourceRot.z = -sourceRot.y;
        sourceRot.x = sourceRot.y = 0;
        Vector3 north = new Vector3(0, 0, Camera.main.transform.eulerAngles.y);

        Pivot.rotation = sourceRot * Quaternion.Euler(north);
    }
}
