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
        //�ʴ� Send Ƚ��
        PhotonNetwork.SendRate = 60;

        //�ʴ� ��Ŷ ����ȭ Ƚ��
        PhotonNetwork.SerializationRate = 30;
        btn.onClick.AddListener(OnConnected);
    }

    private new void OnConnected() {
        if(_inputField.text.Length > 4) {
            Debug.Log("�г����� 4�� ���Ϸ� ǥ�����ּ���");
            return;
        }

        if(_inputField.text.Length == 0) {
            Debug.Log("�г����� �Է��� �ּ���");
            return;
        }

        //���� Ŭ���忡 ����
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// PhotonNetwork.ConnectUsingSettings()���� ���� �ڵ����� ȣ���.
    /// </summary>
    public override void OnConnectedToMaster() {
        Debug.Log($"���� {_inputField.text} ������ ���� ���� ����");
        PhotonNetwork.LocalPlayer.NickName = _inputField.text;
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// PhotonNetwork.JoinLobby()�Լ��� �ڵ����� ȣ���.
    /// </summary>
    public override void OnJoinedLobby() {
        base.OnJoinedLobby();
        PhotonNetwork.LoadLevel("Lobby");
        Debug.Log($"���� {_inputField.text} �κ� ���� ����");
    }
}
