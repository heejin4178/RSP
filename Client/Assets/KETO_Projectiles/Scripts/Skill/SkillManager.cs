using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour
{

    [SerializeField]
    private int m_totalNumberOfSkills = 0;
    private int m_currentSkillNumber = 0;

    private const string LAYER_DAMAGEABLE = "Default";

    private Ray m_ray;
    private float m_maxDistance = 100f;
    private Vector3 m_hitPosition = Vector3.zero;
    private Quaternion m_rotation = Quaternion.identity;

    private ISkill m_currentSkill = null;

    private Skill_01 m_skill_01 = null;
    private Skill_02 m_skill_02 = null;
    private Skill_03 m_skill_03 = null;
    private Skill_04 m_skill_04 = null;
    private Skill_05 m_skill_05 = null;

    private TextGui m_textGui = null;

    private ISkill GetCurrentSkillByNumber(int _num)
    {
        switch (_num)
        {
            case 0:
                return m_skill_01;
            case 1:
                return m_skill_02;
            case 2:
                return m_skill_03;
            case 3:
                return m_skill_04;
            case 4:
                return m_skill_05;
            default:
                return m_skill_01;
        }
    }
    private void Awake()
    {
        m_currentSkillNumber = 0;

        m_skill_01 = GetComponent<Skill_01>();
        m_skill_02 = GetComponent<Skill_02>();
        m_skill_03 = GetComponent<Skill_03>();
        m_skill_04 = GetComponent<Skill_04>();
        m_skill_05 = GetComponent<Skill_05>();
    }

    private void Start()
    {
        m_textGui = TextGui.Instance;
        m_currentSkill = GetCurrentSkillByNumber(m_currentSkillNumber);
        m_textGui.SetTopRightText(m_currentSkill.GetName());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            m_ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(m_ray.origin, m_ray.direction, out hit, m_maxDistance, 1 << LayerMask.NameToLayer(LAYER_DAMAGEABLE)))
            {
                m_hitPosition = hit.point;
                RotateToMouse(m_hitPosition);
                m_currentSkill.StartSkill(m_hitPosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_currentSkillNumber = --m_currentSkillNumber % m_totalNumberOfSkills;
            if (m_currentSkillNumber < 0)
                m_currentSkillNumber = m_totalNumberOfSkills - 1;
            m_currentSkill = GetCurrentSkillByNumber(m_currentSkillNumber);
            m_textGui.SetTopRightText(m_currentSkill.GetName());
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            m_currentSkillNumber = ++m_currentSkillNumber % m_totalNumberOfSkills;
            m_currentSkill = GetCurrentSkillByNumber(m_currentSkillNumber);
            m_textGui.SetTopRightText(m_currentSkill.GetName());
        }
    }

    public void RotateToMouse(Vector3 _destination)
    {
        m_rotation = Quaternion.LookRotation(_destination - transform.position);
       
        m_rotation.x = 0f;
        m_rotation.z = 0f;
        transform.localRotation = m_rotation;
    }
}