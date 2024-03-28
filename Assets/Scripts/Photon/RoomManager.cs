using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public Button StartBtn;
    // Start is called before the first frame update
    void Start() {
        StartBtn.onClick.AddListener(OnStartButtonPressed);
        StartBtn.onClick.AddListener(() => StartBtn.GetComponentInChildren<Text>().text = "Start!!!");
        StartBtn.onClick.AddListener(() => StartBtn.interactable = false);
    }
    public void OnStartButtonPressed() {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", true } });
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (AllPlayersReady()) {
            // 모든 플레이어가 준비되었다면 게임 씬으로 전환
            PhotonNetwork.LoadLevel("InGame");
        }
    }

    bool AllPlayersReady() {
        foreach (Player player in PhotonNetwork.PlayerList) {
            object isPlayerReady;
            if (player.CustomProperties.TryGetValue("IsReady", out isPlayerReady)) {
                if (!(bool)isPlayerReady)
                    return false;
            } else {
                return false; // 준비 상태가 설정되지 않은 플레이어가 있다면
            }
        }
        return true; // 모든 플레이어가 준비되었음
    }
}
