using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedManager : MonoBehaviourPunCallbacks
{
    public InputField _inputField;
    public Button btn;
    private void Awake() {
        //초당 Send 횟수
        PhotonNetwork.SendRate = 60;

        //초당 패킷 직렬화 횟수
        PhotonNetwork.SerializationRate = 30;
        btn.onClick.AddListener(OnConnected);
    }

    private new void OnConnected() {
        if(_inputField.text.Length > 4) {
            Debug.Log("닉네임은 4자 이하로 표현해주세요");
            return;
        }

        if(_inputField.text.Length == 0) {
            Debug.Log("닉네임을 입력해 주세요");
            return;
        }

        //포톤 클라우드에 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// PhotonNetwork.ConnectUsingSettings()으로 인해 자동으로 호출됨.
    /// </summary>
    public override void OnConnectedToMaster() {
        Debug.Log($"유저 {_inputField.text} 마스터 서버 접속 성공");
        PhotonNetwork.LocalPlayer.NickName = _inputField.text;
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// PhotonNetwork.JoinLobby()함수로 자동으로 호출됨.
    /// </summary>
    public override void OnJoinedLobby() {
        base.OnJoinedLobby();
        PhotonNetwork.LoadLevel("Lobby");
        Debug.Log($"유저 {_inputField.text} 로비 진입 성공");
    }
}
