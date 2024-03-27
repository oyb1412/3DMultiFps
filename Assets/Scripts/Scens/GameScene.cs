using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    private Transform _respawnPoints;
    public override void Clear()
    {
    }

    public override void Init() {
        base.Init();
        _respawnPoints = GameObject.Find("RespawnPoints").transform;
        GameObject player = Managers.Resources.Instantiate("Unit/Player", null);
        player.transform.position = _respawnPoints.GetChild(0).transform.position;
        for (int i = 1; i < _respawnPoints.childCount; i++) {
            GameObject unit = Managers.Resources.Instantiate("Unit/Ai", null);
            unit.transform.position = _respawnPoints.GetChild(i).transform.position;
        }
        
    }
}
