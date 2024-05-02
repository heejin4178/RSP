using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class UIManager
{
    private UI_Base _sceneUI;
    private Stack<UI_Base> _uiStack = new Stack<UI_Base>();

    public T GetSceneUI<T>() where T : UI_Base
    {
        return _sceneUI as T;
    }
    
    public T ShowSceneUI<T>() where T : UI_Base
    {
        if (_sceneUI != null)
            return GetSceneUI<T>();

        string key = typeof(T).Name + ".prefab";
        T ui = Managers.Resource.Instantiate(key, pooling: false).GetOrAddComponent<T>();
        _sceneUI = ui;

        return ui;
    }
    
    public void CloseSceneUI()
    {
        Managers.Resource.Destroy(_sceneUI.gameObject);
    }

    public T ShowPopup<T>() where T : UI_Base
    {
        string key = typeof(T).Name + ".prefab";
        T ui = Managers.Resource.Instantiate(key, pooling: true).GetOrAddComponent<T>();
        _uiStack.Push(ui);
        RefreshTimeScale();

        return ui;
    }

    public void ClosePopup()
    {
        if (_uiStack.Count == 0)
            return;

        UI_Base ui = _uiStack.Pop();
        Managers.Resource.Destroy(ui.gameObject);
        RefreshTimeScale();
    }

    public void RefreshTimeScale()
    {
        if (_uiStack.Count > 0)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
}
