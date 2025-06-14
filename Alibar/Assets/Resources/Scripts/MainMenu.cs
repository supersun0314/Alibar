using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainMenu : UIBase
{
    [Header("UI Elements")]
    [SerializeField] private Image titleBanner;
    [SerializeField] private Button closeButton;

    // 定义事件名称常量
    private const string EVENT_CLOSE_BUTTON_CLICK = "OnCloseButtonClick";
    private const string EVENT_MENU_SHOW = "OnMenuShow";
    private const string EVENT_MENU_HIDE = "OnMenuHide";

    protected override void Awake()
    {
        base.Awake();
        Type = UIType.MainMenu;
        Layer = UILayer.MIDDLE;
    }

    protected override void RegisterEvents()
    {
        base.RegisterEvents();
        
        // 注册UI事件
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            AddListener(EVENT_CLOSE_BUTTON_CLICK, new Action(OnCloseButtonClicked));
        }

        // 注册菜单显示/隐藏事件
        AddListener(EVENT_MENU_SHOW, new Action(OnMenuShow));
        AddListener(EVENT_MENU_HIDE, new Action(OnMenuHide));
    }

    protected override void UnregisterEvents()
    {
        base.UnregisterEvents();
        
        // 反注册UI事件
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            RemoveListener(EVENT_CLOSE_BUTTON_CLICK, new Action(OnCloseButtonClicked));
        }

        // 反注册菜单显示/隐藏事件
        RemoveListener(EVENT_MENU_SHOW, new Action(OnMenuShow));
        RemoveListener(EVENT_MENU_HIDE, new Action(OnMenuHide));
    }

    private void OnCloseButtonClicked()
    {
        // 触发关闭按钮点击事件
        TriggerEvent(EVENT_CLOSE_BUTTON_CLICK);
        UIManager.Instance.CloseUI(Type);
    }

    private void OnMenuShow()
    {
        // 处理菜单显示逻辑
        Debug.Log($"MainMenu {Type} is showing");
    }

    private void OnMenuHide()
    {
        // 处理菜单隐藏逻辑
        Debug.Log($"MainMenu {Type} is hiding");
    }

    public override void OnShow()
    {
        base.OnShow();
        // 触发显示事件
        TriggerEvent(EVENT_MENU_SHOW);
    }

    public override void OnHide()
    {
        base.OnHide();
        // 触发隐藏事件
        TriggerEvent(EVENT_MENU_HIDE);
    }
}
