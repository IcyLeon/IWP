using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateAttack : MonoBehaviour
{
    private Vector3 targetPos;
    private Vector3 vel;
    private IDamage target;
    private CharacterData characterData;
    [SerializeField] Transform EmitterPivot;
    [SerializeField] ParticleSystem PS;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, PS.main.duration);
        StartCoroutine(ShootDelay());
    }

    public void SetTargetPos(Vector3 targetPos, IDamage target, Vector3 vel)
    {
        this.targetPos = targetPos;
        this.target = target;
        this.vel = vel;
    }
    private IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(0.1f);
        AmberESkillArrows eSkillArrows = Instantiate(AssetManager.GetInstance().ESkillArrowsPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<AmberESkillArrows>();
        eSkillArrows.SetCharacterData(characterData);
        eSkillArrows.GetRB().velocity = vel;
        eSkillArrows.GetRB().transform.rotation = Quaternion.LookRotation(eSkillArrows.GetRB().velocity);
        eSkillArrows.SetFocalPointContact(targetPos, target);
    }

    public void SetCharacterData(CharacterData characterData)
    {
        this.characterData = characterData;
    }
}
