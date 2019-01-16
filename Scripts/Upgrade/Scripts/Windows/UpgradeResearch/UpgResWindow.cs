﻿using DB;
using EnumCollect;
using ManualTable;
using ManualTable.Interface;
using ManualTable.Row;
using Network.Data;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Widget;
using UnityEngine;

public class UpgResWindow : BaseWindow
{
    private int[] curMaterials;
    private bool isUpgradeType;
    private BaseUpgradeRow refType;

    public TextMeshProUGUI Title;

    [Header("Progess Bar"), Space]
    public GUISliderWithBtn ProgressSlider;

    [Header("Requirement"), Space]
    public TextMeshProUGUI BuildingLevel;
    public TextMeshProUGUI ResearchLevel;

    [Header("Order Materials"), Space]
    public GUIHorizontalInfo[] OrderMaterialElements;
    public TextMeshProUGUI DurationText;

    [Header("Info Group"), Space]
    public TextMeshProUGUI IllusName;
    public GUIInteractableIcon IllustrateImage;

    [Header("Result Group"), Space]
    public TextMeshProUGUI NumberName;
    public TextMeshProUGUI Amount;

    [Header("Button Group"), Space]
    public GUIInteractableIcon InstantBtn;
    public GUIInteractableIcon LevelUpBtn;
    protected override void Awake()
    {
        base.Awake();
        EventListenersController.Instance.AddEmiter("S_UPGRADE", CreateUpgData);
    }

    protected override void Update()
    {
        base.Update();
        SetTextProgCountdown();
    }

    protected override void Init()
    {
        curMaterials = new int[4];
        LevelUpBtn.OnClickEvents += OnUpgradeBtn;
    }

    /// <summary>
    /// 0: type - ListUpgrade
    /// 1: need material - int[4]
    /// 2: might bonus - int
    /// 3: time min - string
    /// 4: time int - int
    /// </summary>
    /// <param name="data">Params object</param>
    public override void Load(params object[] data)
    {
        ListUpgrade type = data.TryGet<ListUpgrade>(0);
        int[] needMaterials = data.TryGet<int[]>(1);
        int mightBonus = data.TryGet<int>(2);
        string timeMin = data.TryGet<string>(3);
        int timeInt = data.TryGet<int>(4);

        curMaterials[0] = SyncData.CurrentMainBase.Farm;
        curMaterials[1] = SyncData.CurrentMainBase.Wood;
        curMaterials[2] = SyncData.CurrentMainBase.Stone;
        curMaterials[3] = SyncData.CurrentMainBase.Metal;

        refType = SyncData.BaseUpgrade[type];
        isUpgradeType = type.IsUpgrade();

        bool isBuildingRequire = false;
        bool isResearchRequire = false;

        bool activeProgressBar = isUpgradeType ? SyncData.CurrentMainBase.UpgradeWait_ID.IsDefined()
                                                            : SyncData.CurrentMainBase.ResearchWait_ID.IsDefined();

        bool activeBtnGroup = !activeProgressBar;
        activeBtnGroup = activeBtnGroup && IsEnoughtMeterial(needMaterials);

        Title.text = type.ToString().InsertSpace();

        ActiveProgressBar(activeProgressBar && timeInt > 0);
        ActiveBtnGroup(activeBtnGroup);

        ProgressSlider.Slider.MaxValue = timeInt;

        BuildingLevel.transform.parent.gameObject.SetActive(isBuildingRequire);
        ResearchLevel.transform.parent.gameObject.SetActive(isResearchRequire);

        if (curMaterials != null && needMaterials != null)
        {
            for (int i = 0; i < 4; i++)
            {
                int captureInt = i;
                OrderMaterialElements[i].Button.OnClickEvents += delegate
                {
                    SetMaterialRequirement(captureInt, ++curMaterials[captureInt], needMaterials[captureInt]);
                    if (IsEnoughtMeterial(needMaterials))
                    {
                        ActiveBtnGroup(true);
                    }
                };
                SetMaterialRequirement(i, curMaterials[i], needMaterials[i]);
            }
        }

        // ============================ //
        NumberName.text = "Might Bonus";
        Amount.text = string.Format("{0}", mightBonus);

        DurationText.text = "Duration: " + timeMin;
    }

    private void SetMaterialRequirement(int index, int cur, int need)
    {
        GUIHorizontalInfo material = OrderMaterialElements[index];
        material.InteractableChange(cur < need);
        if (cur >= need)
            material.Placeholder.text = string.Format("{0}/{1}", cur, need);
        else
            material.Placeholder.text = string.Format("<color=red>{0}</color>/{1}", cur, need);
    }

    private void ActiveProgressBar(bool value)
    {
        ProgressSlider.gameObject.SetActive(value);
    }

    private void ActiveBtnGroup(bool value)
    {
        InstantBtn.InteractableChange(value);
        LevelUpBtn.InteractableChange(value);
    }

    private void SetTextProgCountdown()
    {
        if (ProgressSlider.gameObject.activeInHierarchy)
        {
            if (isUpgradeType)
            {
                ProgressSlider.Slider.Value = ProgressSlider.Slider.MaxValue - SyncData.CurrentMainBase.UpgradeTime;
                ProgressSlider.Slider.Placeholder.text = SyncData.CurrentMainBase.GetUpgTimeString();

                if (SyncData.CurrentMainBase.UpgIsDone())
                {
                    ActiveProgressBar(false);
                    ActiveBtnGroup(true);
                }
            }
            else
            {
                ProgressSlider.Slider.Value = ProgressSlider.Slider.MaxValue - SyncData.CurrentMainBase.ResearchTime;
                ProgressSlider.Slider.Placeholder.text = SyncData.CurrentMainBase.GetResTimeString();
                if (SyncData.CurrentMainBase.ResIsDone())
                {
                    ActiveProgressBar(false);
                    ActiveBtnGroup(true);
                }
            }
        }
    }

    private JSONObject CreateUpgData()
    {
        Dictionary<string, string> data = new Dictionary<string, string>()
        {
            {"ID_Server"   ,SyncData.UserInfo[0].Server_ID },
            {"ID_User"     ,SyncData.UserInfo[0].ID_User.ToString()},
            {"BaseNumber"  ,SyncData.CurrentMainBase.BaseNumber.ToString() },
            {"ID_Upgrade"  ,((int)refType.ID).ToString()},
            {"UpgradeType" ,refType.UpgradeType.ToString() },
            {"Level"       ,refType.Level.ToString()}
        };
        return new JSONObject(data);
    }

    private void OnUpgradeBtn()
    {

        MainBaseTable t = DBReference.Instance[refType.ID] as MainBaseTable;
        MainBaseRow r = t[refType.Level - 1];

        SyncData.CurrentMainBase.Farm -= t.Rows[refType.Level - 1].FoodCost;
        SyncData.CurrentMainBase.Wood -= t.Rows[refType.Level - 1].WoodCost;
        SyncData.CurrentMainBase.Metal -= t.Rows[refType.Level - 1].MetalCost;
        SyncData.CurrentMainBase.Stone -= t.Rows[refType.Level - 1].StoneCost;

        SyncData.CurrentMainBase.UpgradeWait_ID = refType.ID;
        SyncData.CurrentMainBase.UpgradeTime = t.Rows[refType.Level - 1].TimeInt;

        EventListenersController.Instance.Emit("S_UPGRADE");
        WDOCtrl.Close();
    }

    private bool IsEnoughtMeterial(int[] needMaterials)
    {
        for (int i = 0; i < 4; i++)
        {
            if (curMaterials[i] < needMaterials[i]) return false;
        }
        return true;
    }
}
