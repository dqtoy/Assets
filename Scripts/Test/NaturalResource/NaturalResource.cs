﻿using ManualTable.Row;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Flag
{
    Owner = 4,
    Enemy = 5,
    Guild = 6,
}

public enum RssType
{
    Farm = 1,
    Wood,
    Metal,
    Rock,
}

public class NaturalResource : MonoBehaviour
{
    private GameObject rss;
    private GameObject flag;

    public RSS_PositionRow Data;
    public int Id;

    public Vector3Int CellPos { get; private set; }

    private void Awake()
    {
        LookAt look = GetComponent<LookAt>();
        look.GameObject = transform;
        look.Target = Camera.main.transform;
        look.ProjectionDir = ProjectionDir.Right;
    }

    private void Start()
    {
        Data = ResourceManager.Instance.Datas[Id - 1];        
        ResourceManager.Instance[Id] = this;
        InitData();       
    }

    private void OnMouseUp()
    {
        RssType type = (RssType)Data.RssType;
        string general = string.Format("{0}: Lv {1}", type.ToString(), Data.Level);

        Popup.Instance.Open(general, Data.Quality.ToString(), Data.Position);
        Popup.Instance.SetCursor(CellPos);
    }
    public void InitData()
    {
        if (Data != null)
        {
            rss?.SetActive(false);
            int type = Data.RssType - 1;
            rss = transform.GetChild(type).gameObject;
            rss?.SetActive(true);

            // parse position
            string[] posArr = Data.Position.Split(',');

            CellPos = Vector3Int.zero;
            CellPos = new Vector3Int(int.Parse(posArr[0]),int.Parse(posArr[1]), int.Parse(posArr[2]));

            transform.position = HexMap.Instance.CellToWorld(CellPos + new Vector3Int(5,5,0));
        }
    }    

    
}