﻿using Generic.Singleton;
using SocketIO;
using UnityEngine;

public sealed class SIO_StartupListener : Listener
{
    public Player Player;
    public void R_GET_RSS(SocketIOEvent obj)
    {
        //Debug.Log(obj);
        SyncData.RSS_Position.LoadTable(obj.data["R_GET_RSS"]);
    }

    public void R_BASE_INFO(SocketIOEvent obj)
    {
        //Debugger.Log(obj);
        SyncData.BaseInfo.LoadTable(obj.data["R_BASE_INFO"]);
        Player.BaseInfo = SyncData.BaseInfo.Rows[0];
    }

    public void R_USER_INFO(SocketIOEvent obj)
    {
        SyncData.UserInfo.LoadTable(obj.data["R_USER_INFO"]);
        Player.Info = SyncData.UserInfo.Rows[0];

        //string data = obj.data["R_USER_INFO"].ToString();
        //Debugger.Log(obj);
    }

    public void R_GET_POSITION(SocketIOEvent obj)
    {
        //Debugger.Log(obj);
        SyncData.Position.LoadTable(obj.data["R_GET_POSITION"]);
    }

    public void R_TRAINNING(SocketIOEvent obj)
    {
        //Debug.Log(obj);
    }

    public void R_BASE_DEFEND(SocketIOEvent obj)
    {
       // Debug.Log(obj);
        SyncData.BaseDefends[0].LoadTable(obj.data["R_BASE_DEFEND"]);
    }

    public void R_UPGRADE(SocketIOEvent obj)
    {
        //Debug.Log(obj);
    }

    public void R_BASE_UPGRADE(SocketIOEvent obj)
    {
        SyncData.CurrentBaseUpgrade.LoadTable(obj.data["R_BASE_UPGRADE"]);
    }

    private void R_UNIT(SocketIOEvent obj)
    {
        //Debug.Log(obj);
        SyncData.UnitTable.LoadTable(obj.data["R_UNIT"]);
    }

    private void R_PLAYER_INFO(SocketIOEvent obj)
    {
        Debug.Log(obj);
        SyncData.UserInfo.LoadTable(obj.data["R_PLAYER_INFO"],false);
        
    }

    private void R_BASE_PLAYER(SocketIOEvent obj)
    {
        Debug.Log(obj);        
        SyncData.BasePlayerTable.LoadTable(obj.data["R_BASE_PLAYER"]);
    }

    public override void RegisterCallback()
    {
        On("R_GET_RSS", R_GET_RSS);
        On("R_BASE_INFO", R_BASE_INFO);
        On("R_USER_INFO", R_USER_INFO);
        On("R_GET_POSITION", R_GET_POSITION);

        On("R_TRAINING", R_TRAINNING);
        On("R_BASE_DEFEND", R_BASE_DEFEND);
        On("R_BASE_UPGRADE", R_BASE_UPGRADE);
        On("R_UPGRADE", R_UPGRADE);
        On("R_UNIT", R_UNIT);
        On("R_PLAYER_INFO", R_PLAYER_INFO);       
        On("R_BASE_PLAYER", R_BASE_PLAYER);
    }
}