using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class UIBase : MonoBehaviour
{
    public UIType Type { get; protected set; }
    public UILayer Layer { get; protected set; }

    // 事件注册表
    protected Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    protected virtual void Awake()
    {
        // 子类需要在这里设置Type和Layer
    }

    protected virtual void OnEnable()
    {
        RegisterEvents();
    }

    protected virtual void OnDisable()
    {
        UnregisterEvents();
    }

    public virtual void OnShow()
    {
        gameObject.SetActive(true);
    }

    public virtual void OnHide()
    {
        gameObject.SetActive(false);
    }

    // 注册事件
    protected virtual void RegisterEvents()
    {
        // 子类可以在这里注册需要的事件
    }

    // 反注册事件
    protected virtual void UnregisterEvents()
    {
        // 子类可以在这里反注册事件
        eventTable.Clear();
    }

    // 添加事件监听
    protected void AddListener(string eventName, Delegate handler)
    {
        if (!eventTable.ContainsKey(eventName))
        {
            eventTable[eventName] = handler;
        }
        else
        {
            eventTable[eventName] = Delegate.Combine(eventTable[eventName], handler);
        }
    }

    // 移除事件监听
    protected void RemoveListener(string eventName, Delegate handler)
    {
        if (eventTable.ContainsKey(eventName))
        {
            eventTable[eventName] = Delegate.Remove(eventTable[eventName], handler);
            if (eventTable[eventName] == null)
            {
                eventTable.Remove(eventName);
            }
        }
    }

    // 触发事件
    protected void TriggerEvent(string eventName, params object[] args)
    {
        if (eventTable.ContainsKey(eventName))
        {
            eventTable[eventName]?.DynamicInvoke(args);
        }
    }

    protected virtual void OnDestroy()
    {
        UnregisterEvents();
    }
} 