using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Button CreateRoomBtn;
    public Button JoinRoomBtn;

    public InputField RoomNameField;
    public InputField RoomLimitNumberField;
    public GameObject RoomListItem;
    public Transform svContent;

    private Dictionary<string, RoomInfo> _dicRoomList = new Dictionary<string, RoomInfo>();

    private void Awake() {
        CreateRoomBtn.onClick.AddListener(OnClickCreateRoom);
        JoinRoomBtn.onClick.AddListener(JoinRandomRoom);
    }

    //방 목록의 변화가 있을 때 호출되는 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        base.OnRoomListUpdate(roomList);
        //Content에 자식으로 붙어있는 Item을 다 삭제
        DeleteRoomListItem();
        //dicRoomInfo 변수를 roomList를 이용해서 갱신
        UpdateRoomListItem(roomList);
        //dicRoom을 기반으로 roomListItem을 만들자
        CreateRoomListItem();
    }

    private void SelectRoomListItem(string roomName) {
        RoomNameField.text = roomName;
    }

    private void DeleteRoomListItem() {
        foreach (Transform c in svContent) {
            Destroy(c.gameObject);
        }
    }

    private void UpdateRoomListItem(List<RoomInfo> roomList) {
        foreach (var info in roomList) {
            //dicRoomInfo에 info 의 방이름으로 되어있는 key값이 존재하는가
            if (_dicRoomList.ContainsKey(info.Name)) {
                //만약에 방이 삭제되었으면?
                if (info.RemovedFromList) {
                    _dicRoomList.Remove(info.Name); //삭제
                    continue;
                }
            }
            _dicRoomList[info.Name] = info; //추가
        }
    }

    void CreateRoomListItem() {
        foreach (RoomInfo info in _dicRoomList.Values) {
            //방 정보 생성과 동시에 ScrollView-> Content의 자식으로 하자
            GameObject go = Instantiate(RoomListItem, svContent);
            //생성된 item에서 RoomListItem 컴포넌트를 가져온다.
            RoomList item = go.GetComponent<RoomList>();
            //가져온 컴포넌트가 가지고 있는 SetInfo 함수 실행
            item.SetInfo(info.Name, info.PlayerCount, info.MaxPlayers);
            //item 클릭되었을 때 호출되는 함수 등록
            item.onDelegate = SelectRoomListItem;
        }
    }



    // 생성 버튼 클릭시 호출되는 함수
    public void OnClickCreateRoom() {
       if (string.IsNullOrEmpty(RoomNameField.text) || string.IsNullOrEmpty(RoomLimitNumberField.text)) {
            Debug.Log("방 제목이나 인원수를 입력하세요");
            return;
        }

       if(int.Parse(RoomLimitNumberField.text) > 4) {
            Debug.Log("참여 가능 최대 인원은 4명입니다. 다시 입력하세요");
            return;
        }

        //방 옵션
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = int.Parse(RoomLimitNumberField.text);

        //방 목록에 보이게 할것인가?
        options.IsVisible = true;

        //방에 참여 가능 여부
        options.IsOpen = true;

        //방 생성
        PhotonNetwork.CreateRoom(RoomNameField.text, options);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("방 생성 실패" + message);
    }
    public override void OnCreatedRoom() {
        base.OnCreatedRoom();
        Debug.Log("방 생성 성공");
    }
    public void OnClickJoinRoom() {
        // 방 참여
        PhotonNetwork.JoinRoom(RoomNameField.text);
    }
    public override void OnJoinedRoom() {
        base.OnJoinedRoom();
        Debug.Log("방 입장 성공");
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("방 입장 실패" + message);
    }
    void JoinOrCreateRoom() {
        //방 옵션
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = int.Parse(RoomLimitNumberField.text);

        //방 목록에 보이게 할것인가?
        options.IsVisible = true;

        //방에 참여 가능 여부
        options.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom(RoomNameField.text, options, TypedLobby.Default);
    }
    void JoinRandomRoom() {
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("방 입장 성공");
        PhotonNetwork.LoadLevel("Room");
    } // 참여 버튼 클릭시 호출되는 함수
    public override void OnJoinRandomFailed(short returnCode, string message) {
        base.OnJoinRandomFailed(returnCode, message);
    }
}
