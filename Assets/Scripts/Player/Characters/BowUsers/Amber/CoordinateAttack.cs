using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateAttack : MonoBehaviour
{
    private CharacterData characterData;
    [SerializeField] Transform EmitterPivot;
    [SerializeField] ParticleSystem PS;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, PS.main.duration);
        StartCoroutine(ShootDelay());
    }

    private IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(0.1f);
        AmberESkillArrows eSkillArrows = Instantiate(AssetManager.GetInstance().ESkillArrowsPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<AmberESkillArrows>();
        eSkillArrows.SetCharacterData(characterData);
        eSkillArrows.GetRB().velocity = GetShootDirection(EmitterPivot.position) * 10f;
        eSkillArrows.GetRB().transform.rotation = Quaternion.LookRotation(eSkillArrows.GetRB().velocity);
        eSkillArrows.SetFocalPointContact(GetContactPoint(EmitterPivot.position));
    }

    public void SetCharacterData(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    private Vector3 GetShootDirection(Vector3 pos)
    {
        Vector3 dir = AssetManager.RandomVectorInCone(GetDirection(pos), 75f);
        dir.y = Mathf.Abs(dir.y);
        return dir;
    }

    private Vector3 GetDirection(Vector3 pos)
    {
        return (GetContactPoint(pos) - EmitterPivot.position).normalized;
    }

    private Vector3 GetContactPoint(Vector3 pos)
    {
        float range = 10f;
        Characters NearestEnemy = GetNearestCharacters(range);
        Vector3 forward;
        if (NearestEnemy == null)
        {

            forward = transform.forward;
            forward.y = 0;
            Vector3 endPos = transform.position + forward * range;
            if (Physics.Raycast(endPos, Vector3.down, out RaycastHit hit))
            {
                endPos = hit.point;
            }
            return endPos;
        }
        else
        {
            forward = NearestEnemy.transform.position - pos;
            forward.Normalize();
            return NearestEnemy.transform.position;
        }
    }


    private Characters GetNearestCharacters(float range)
    {
        Collider[] colliders = Physics.OverlapSphere(CharacterManager.GetInstance().GetPlayerController().transform.position, range, LayerMask.GetMask("Entity"));
        if (colliders.Length == 0)
            return null;

        Collider CurrentCollider = colliders[0];

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            float Dist1 = (collider.transform.position - CharacterManager.GetInstance().GetPlayerController().transform.position).magnitude;
            float Dist2 = (collider.transform.position - CurrentCollider.transform.position).magnitude;
            if (Dist1 < Dist2)
                CurrentCollider = collider;
        }
        return CurrentCollider.GetComponent<Characters>();
    }
}
