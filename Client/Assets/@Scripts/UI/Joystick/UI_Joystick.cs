using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Joystick : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _handler;

    private float _joystickRadius;
    private Vector2 _touchPosition;
    private Vector2 moveDir;
    
    void Start()
    {
        _joystickRadius = _background.gameObject.GetComponent<RectTransform>().sizeDelta.y / 2;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("OnPointerClick");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        _background.transform.position = eventData.position;
        _handler.transform.position = eventData.position;
        _touchPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        _handler.transform.position = _touchPosition;
        moveDir = Vector2.zero;

        // Managers.Game.MoveDir = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag");
        Vector2 touchDir = eventData.position - _touchPosition;
        
        float moveDist = Mathf.Min(touchDir.magnitude, _joystickRadius);
        moveDir = touchDir.normalized;
        Vector2 newPosition = _touchPosition + moveDir * moveDist;
        _handler.transform.position = newPosition;

        // Managers.Game.MoveDir = moveDir;
    }
}