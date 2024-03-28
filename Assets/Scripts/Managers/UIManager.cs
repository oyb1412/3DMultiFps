using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private GameObject _playerUis;
   

    public void SetPlayerUis(bool trigger) {
        if(_playerUis == null)
            _playerUis = GameObject.Find("UI_Player");

        _playerUis.SetActive(trigger);
    }
}
