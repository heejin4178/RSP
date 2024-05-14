using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ProjectileJoystick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _handler;
    [SerializeField] private Image _coolTimeBar;

    private float _joystickRadius;
    private Vector2 _touchPosition;
    private Vector2 _moveDir;
    private Vector3 _endPoint;
    
    void Start()
    {
        _joystickRadius = _background.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2;
                
        Managers.Game.OnCoolTimeValueChanged -= OnCoolTimeValueChanged;
        Managers.Game.OnCoolTimeValueChanged += OnCoolTimeValueChanged;
    }
    
    private void OnCoolTimeValueChanged(float value)
    {
        _coolTimeBar.fillAmount = value / 100;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _background.transform.position = eventData.position;
        _handler.transform.position = eventData.position;
        _coolTimeBar.transform.position = eventData.position;
        _touchPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _handler.transform.position = _touchPosition;
        _coolTimeBar.transform.position = _touchPosition;
        _moveDir = Vector2.zero;
        
        if (Managers.Object.MyPlayer.State == CreatureState.Skill)
        {
            Managers.Object.MyPlayer.StopAttackIndicator(); // 투사체 경로 표시 작업 취소
            return;
        }
        
        Vector3 direction = (_endPoint - Managers.Object.MyPlayer.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        float desiredRotationY = lookRotation.eulerAngles.y;

        if (Managers.Object.MyPlayer.LineRenderer.enabled)
            Managers.Object.MyPlayer.Rotation = desiredRotationY; // rotation 갱신

        Debug.Log("pro direction :" + desiredRotationY);

        Managers.Game.SkillType = SkillType.SkillProjectile; // 스킬 타입 설정
        Managers.Game.AttackKeyPressed = true;
        
        Managers.Object.MyPlayer.StopAttackIndicator(); // 투사체 경로 표시 작업 취소
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 touchDir = eventData.position - _touchPosition;
        
        float moveDist = Mathf.Min(touchDir.magnitude, _joystickRadius);
        _moveDir = touchDir.normalized;
        Vector2 newPosition = _touchPosition + _moveDir * moveDist;
        _handler.transform.position = newPosition;
        _coolTimeBar.transform.position = _touchPosition;

        var startPoint = Managers.Object.MyPlayer.transform.position;
        var fireDir = new Vector3(_moveDir.x, 0, _moveDir.y);
        _endPoint = startPoint + fireDir * 10.0f;
        startPoint.y = 0.1f;
        _endPoint.y = 0.1f;
        
        // 투사체 경로 표시 작업
        Managers.Object.MyPlayer.PlayAttackIndicator(Color.yellow, startPoint, _endPoint);
    }
}
