using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class LongPressTrigger : MonoBehaviour, IPointerDownHandler,
                                               IPointerUpHandler,
                                               IPointerExitHandler
{
    [ReadOnly]
    public float IntervalSecond = 1f;
    
    UnityAction _onLongPointerDown;
    float _executeTime;

    void Start()
    {
        //AddLongPressAction(null);
    }

    void Update()
    {
        if (_executeTime > 0f && _executeTime <= Time.realtimeSinceStartup)
        {
            _onLongPointerDown();
            _executeTime = -1f;
        }
    }

    void OnDestroy()
    {
        _onLongPointerDown = null;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _executeTime = Time.realtimeSinceStartup + IntervalSecond;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _executeTime = -1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _executeTime = -1f;
    }

    public void AddLongPressAction(UnityAction action)
    {
        //_onLongPointerDown = action;
        _onLongPointerDown = () => Debug.Log("‚Û‚¿");
    }
}
