using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoordinateAttack : MonoBehaviour
{
    private Vector3 targetPos;
    private CharacterData characterData;
    [SerializeField] Transform EmitterPivot;
    [SerializeField] ParticleSystem PS;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, PS.main.duration);
        StartCoroutine(ShootDelay());
    }

    public void SetTargetPos(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }
    private IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(0.1f);
        AmberESkillArrows eSkillArrows = Instantiate(AssetManager.GetInstance().ESkillArrowsPrefab, EmitterPivot.position, Quaternion.identity).GetComponent<AmberESkillArrows>();
        eSkillArrows.SetCharacterData(characterData);
        eSkillArrows.GetRB().velocity = eSkillArrows.transform.forward * 10f;
        eSkillArrows.GetRB().transform.rotation = Quaternion.LookRotation(eSkillArrows.GetRB().velocity);
        eSkillArrows.SetFocalPointContact(targetPos);
    }

    public void SetCharacterData(CharacterData characterData)
    {
        this.characterData = characterData;
    }
}
