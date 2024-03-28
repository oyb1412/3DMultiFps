using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomList : MonoBehaviour
{
    private Text roomInfo;

    public Action<string> onDelegate;

    private void Awake() {
        roomInfo = GetComponentInChildren<Text>();
    }

    public void SetInfo(string roomName, int currPlayer, int maxPlayer) {
        name = roomName;
        roomInfo.text = roomName + $"( {currPlayer} / {maxPlayer})";
    }

    public void OnClick() {
        if (onDelegate != null) {
            onDelegate(name);
        }
    }
}
