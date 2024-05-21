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
        _sceneUI = null;
    }

    public T ShowPopup<T>() where T : UI_Base
    {
        string key = typeof(T).Name + ".prefab";
        T ui = Managers.Resource.Instantiate(key, pooling: false).GetOrAddComponent<T>();
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
    
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"{name}.prefab");
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }
}
