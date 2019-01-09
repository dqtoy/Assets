﻿using System;
using TMPro;
using UnityEngine;
using UI.Widget;
using System.Collections.Generic;
using static UpgResWdoCtrl;
using ManualTable;
using ManualTable.Row;
using System.Linq;
using EnumCollect;

public class ArmyWindow : BaseWindow
{ 
    [Serializable]
    public struct Element
    {
        public GUIInteractableIcon Icon;
        public GUIProgressSlider LevelBar;
    }

    [Serializable]
    public struct ElementTypeInfo
    {
        public string[] Titles;
        public ListUpgrade[] Types;
        public SoldierTable[] AgentDatabase;
        public ListUpgrade ConstructType;
    }

    [Header("Toggle Group")]
    public GUIToggle Toggle;

    [Header("Illustration Group")]
    private TextMeshProUGUI typeName;
    private GUIInteractableIcon illusImg;
    private GUIInteractableIcon upgradeBtn;
    private GUIProgressSlider levelBar;
    public Transform IllustrationGroup;

    [Header("Element Group")]
    private Element[] elements;

    public Transform[] OrderElements;

    [Header("Army Type's Name")]
    public ElementTypeInfo Infantry;
    public ElementTypeInfo Ranged;
    public ElementTypeInfo Mounted;
    public ElementTypeInfo SeigeEngine;

    private Dictionary<string, ElementTypeInfo> typeDict;

    protected override void Start()
    {
        Toggle.ActiveToggle(0);
    }

    protected override void Init()
    {
        typeDict = new Dictionary<string, ElementTypeInfo>()
        {
            {"Infantry" ,Infantry},
            {"Ranged",Ranged},
            {"Mounted",Mounted},
            {"Siege Engine",SeigeEngine},
        };

        SetupIllustrationGroup();
        SetupOrderElements();
        Toggle.CheckMarkEvents += delegate
        {
            typeName.text = Toggle.ActiveMark.Placeholder.text;
            Load(typeName.text);
        };
        upgradeBtn.OnClickEvents +=
            delegate { OnUpgradeBtn(typeName.text); };
    }

    private void SetupIllustrationGroup()
    {
        typeName = IllustrationGroup.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        illusImg = IllustrationGroup.GetChild(0).GetComponentInChildren<GUIInteractableIcon>();

        upgradeBtn = IllustrationGroup.GetChild(1).GetComponent<GUIInteractableIcon>();

        levelBar = IllustrationGroup.GetChild(2).GetComponent<GUIProgressSlider>();

    }

    private void SetupOrderElements()
    {
        int elementCount = OrderElements.Length;
        elements = new Element[elementCount];

        for (int i = 0; i < elementCount; i++)
        {
            int captureIndex = i;
            elements[i] = new Element()
            {
                Icon = OrderElements[i].GetComponentInChildren<GUIInteractableIcon>(),
                LevelBar = OrderElements[i].GetComponentInChildren<GUIProgressSlider>(),
            };
            elements[i].Icon.OnClickEvents +=
                delegate
                {
                    Controller.Open(UgrResWindow.UpgradeResearch);
                    OnElementBtn(typeName.text,captureIndex);
                };
        }

    }

    private void OnElementBtn(string type, int index)
    {
        SoldierTable table = typeDict[type].AgentDatabase[index];
        ListUpgrade title = typeDict[typeName.text].Types[index];

        int[] need;
        SoldierRow row = table.Rows.FirstOrDefault(x => x.Level == Controller.Sync.Levels.CurrentUpgradeLv);

        if (row != null)
            need = new int[] { row.Food, row.Wood, row.Stone, row.Metal };
        else need = new int[4];

        Controller[UgrResWindow.UpgradeResearch].Load(
            title,
            need,
            row?.MightBonus,
            row?.ResearchTime,
            row?.TimeInt
            );
    }

    private void OnUpgradeBtn(string datatype)
    {
        // open
        Controller.Open(UgrResWindow.UpgradeResearch);
        // server data 
        int level = Controller.Conn.Sync.Levels.CurrentUpgradeLv;

        // get database
        ElementTypeInfo info = typeDict[datatype];
        MainBaseTable table = Controller[info.ConstructType] as MainBaseTable;

        int[] need;
        MainBaseRow row = table.Rows.FirstOrDefault(x => x.Level == level);
        if (row != null)
            need = new int[] { row.FoodCost, row.WoodCost, row.StoneCost, row.MetalCost };
        else need = new int[4];

        Controller[UgrResWindow.UpgradeResearch].Load
            (info.ConstructType,
            need,
            row?.MightBonus,
            row?.TimeMin,
            row?.TimeInt
            );
    }

    public override void Load(params object[] input)
    {
        // data for test
        int mainLevel = Controller.Sync.Levels.MainbaseLevel;
        int curLevel = Controller.Sync.Levels.CurrentUpgradeLv;

        // get database
        ElementTypeInfo armyType = typeDict[input.TryGet<string>(0)];
        MainBaseTable table = Controller[armyType.ConstructType] as MainBaseTable;

        // active button element from 1 -> 4
        for (int i = 0, j = 0; i < table.Rows.Count && j < elements.Length; i++)
        {
            if (table.Rows[i].Unlock != null &&
                table.Rows[i].Unlock != "")
            {
                elements[j].Icon.InteractableChange(curLevel >= table.Rows[i].Level);
                j++;
            }
        }

        // check active or not for upgrade btn
        upgradeBtn.InteractableChange(mainLevel > curLevel);

        // set level bar value and rename element btn
        levelBar.Value = curLevel;
        for (int i = 0; i < armyType.Types.Length; i++)
        {
            elements[i].Icon.Placeholder.text = armyType.Titles[i];
        }
    }



}
