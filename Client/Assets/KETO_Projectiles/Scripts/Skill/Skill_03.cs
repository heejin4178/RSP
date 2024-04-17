using System.Collections;
using System;
using UnityEngine;

public class Skill_03 : MonoBehaviour, ISkill
{
    [SerializeField] private string m_name = "Skill_03_Wind";

    [SerializeField] private float m_coolTime = 1f;

    [Serializable]
    public struct EffectData
    {
        public Transform transform;
        public GameObject prefab;
        public float waitTime;
    }

    public EffectData CastEffect;
    public EffectData ProjectileEffect;

    private float m_coolTimeCounter = 0f;

    private Quaternion m_rotation = Quaternion.identity;

    public void Initialize()
    {
        m_coolTimeCounter = 0f;
    }

    public void StartSkill(Vector3 _position)
    {
        if (m_coolTimeCounter > m_coolTime)
        {
            m_coolTimeCounter = 0f;
            StartCoroutine(SkillCoroutine(_position));
        }
    }

    public void Cancel()
    {
    }
    public string GetName()
    {
        return m_name;
    }

    private void Update()
    {
        m_coolTimeCounter += Time.deltaTime;
    }

    private IEnumerator SkillCoroutine(Vector3 _position)
    {
        m_rotation = Quaternion.LookRotation(_position - transform.position);
        m_rotation.x = 0f;
        m_rotation.z = 0f;

        if (CastEffect.transform != null & CastEffect.prefab != null)
        {
            yield return new WaitForSeconds(CastEffect.waitTime);
            GameObject cast = Instantiate(CastEffect.prefab, CastEffect.transform.position, m_rotation);
            cast.transform.parent = CastEffect.transform;
            ParticleSystem castParticle = cast.GetComponent<ParticleSystem>();
            if (castParticle != null)
                Destroy(cast, castParticle.main.duration);
        }

        if (ProjectileEffect.transform != null & ProjectileEffect.prefab != null)
        {
            yield return new WaitForSeconds(ProjectileEffect.waitTime);

            Camera.main.GetComponent<ShakeCamera>().Shake();

            GameObject projectile = Instantiate(ProjectileEffect.prefab, ProjectileEffect.transform.position, m_rotation);
            ParticleSystem projectileParticle = projectile.GetComponent<ParticleSystem>();
            if (projectileParticle != null)
                Destroy(projectile, projectileParticle.main.duration);
        }
    }
}
