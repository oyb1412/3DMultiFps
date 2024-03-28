using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : BaseScene
{
    private PhotonView _view;
    private Transform _respawnPoints;
    private Text _timeText;
    private GameObject _exitText;
    private float _currentTime;
    private int _exitTime = 3;
    public int index = 999;
    public override void Clear()
    {
    }

    public override void Init() {
        base.Init();
        _view = GetComponent<PhotonView>();
        _currentTime = Time.time;
        Cursor.lockState = CursorLockMode.Locked;

        _respawnPoints = GameObject.Find("RespawnPoints").transform;
        _timeText = GameObject.Find("TimeText").GetComponent<Text>();
        _exitText = GameObject.Find("ExitText");
        _exitText.SetActive(false);
        GameObject player = PhotonNetwork.Instantiate($"Prefabs/Unit/Player", _respawnPoints.GetChild(PhotonNetwork.LocalPlayer.ActorNumber - 1).transform.position, Quaternion.identity);
        GameObject playerui = Managers.Resources.Instantiate("UI/UI_Player", null);
        playerui.GetComponent<UI_Player>().SetPlayer(player.GetComponent<PlayerController>());
        if (PhotonNetwork.IsMasterClient) {
            int playerCount = PhotonNetwork.PlayerList.Length;
            for (int i = playerCount; i < _respawnPoints.childCount; i++) {
                GameObject ai = PhotonNetwork.Instantiate($"Prefabs/Unit/Ai", _respawnPoints.GetChild(i).transform.position, Quaternion.identity);
            }
        }
    }
   
    void TimeText() {
        float sceneTime = Time.time - _currentTime;
        if (sceneTime / 60 >= _exitTime) {
            _timeText.text = $"0{_exitTime} : 00";
            _exitText.SetActive(true);
            Time.timeScale = 0f;
            StartCoroutine(CoGameExit(3f));
            return;
        }
        int time = (int)Mathf.Floor(sceneTime);
        _timeText.text = string.Format("{0:D2} : {1:D2}", time / 60, time % 60);
    }
    public void Update() {
        TimeText();
    }

    IEnumerator CoGameExit(float time) {
        yield return new WaitForSecondsRealtime(time);
        PhotonNetwork.LoadLevel("Score");
    }
}
