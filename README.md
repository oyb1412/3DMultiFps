## **📃핵심 기술**

### ・멀티플레이를 위한 유닛간 동기화

🤔**WHY?**

유닛간의 이동, 회전, 물리처리 및 애니메이션과 같은 표면적인 정보의 동기화 및 점수, 체력등의 내적인 데이터를 동기화해 자연스러운 멀티플레이 경험을 제공

🤔**HOW?**

 관련 코드

- UnitBase
    
    ```csharp
    public abstract class UnitBase : MonoBehaviourPunCallbacks
    {
    	 [PunRPC]
       protected void SetAnimatorRPC(string name) {
           _animator.CrossFade(name, _animationFadeTime);
       }
       protected void SetAnimator(string name) {
        _view.RPC("SetAnimatorRPC", RpcTarget.All, name);
    	}
    }
    ```
    
- GameScene
    
    ```csharp
    public class GameScene : BaseScene
    {
    	public override void Init() {
        base.Init();
        _view = GetComponent<PhotonView>();
       
        GameObject player = PhotonNetwork.Instantiate($"Prefabs/Unit/Player", _respawnPoints.GetChild(PhotonNetwork.LocalPlayer.ActorNumber - 1).transform.position, Quaternion.identity);
        GameObject playerui = Managers.Resources.Instantiate("UI/UI_Player", null);
        playerui.GetComponent<UI_Player>().SetPlayer(player.GetComponent<PlayerController>());
        if (PhotonNetwork.IsMasterClient) {
            int playerCount = PhotonNetwork.PlayerList.Length;
            for (int i = playerCount; i < _respawnPoints.childCount; i++) {
                PhotonNetwork.Instantiate($"Prefabs/Unit/Ai", _respawnPoints.GetChild(i).transform.position, Quaternion.identity);
            }
        }
    
    	}
    }
    ```
    

🤓**Result!**

Photon라이브러리를 모든 AI및 플레이어의 외적, 내적데이터를 동기화해, 사실적인 멀티플레이 구현 성공

### ・멀티플레이를 위한 로그인, 방 생성, 입장, 게임시작 기능

🤔**WHY?**

같이 플레이를 원하는 플레이어와의 플레이를 위해 방 기능을 구현

🤔**HOW?**

 관련 코드

- ConnectedManager
    
    ```csharp
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
    ```
    
- LobbyManager
    
    ```csharp
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
    
    ```
    
- RoomManager
    
    ```csharp
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
    ```
    

🤓**Result!**

중복이 불가능한 닉네임 지정 및 로비 입장, 방 생성, 방 입장, 게임 시작 기능등을 구현해 실제 인게임까지의 과정을 기능적으로 구현

### ・유니티 에디터를 이용한 AI의 시야를 외적으료 표현

🤔**WHY?**

‘적의 시야각 90도’ 라는 단순한 정보만 있었기 때문에, 적의 시야각이 실제 환경에서 어느 정도인지 체감하기가 힘들었음.

🤔**HOW?**

 관련 코드

- AIFov
    
    ```csharp
    public class AIFov : MonoBehaviour
    {
        public float viewRange = 15f;
        [Range(0, 360)]
        public float viewAngle = 120f;
    
        public Vector3 CirclePoint(float angle) {
            angle += transform.eulerAngles.y;
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
        }
    
        public UnitBase isTracePlayer() {
            var colls = Physics.OverlapSphere(transform.position, viewRange, LayerMask.GetMask("Unit"));
            if (colls.Length > 0) {
                foreach (var coll in colls) {
                    if (coll.gameObject == gameObject)
                        continue;
    
                    var dir = coll.transform.position - transform.position;
                    dir = dir.normalized;
                    if (Vector3.Angle(transform.forward, dir) < viewAngle * 0.5f) {
                        int mask = (1 << (int)Define.LayerList.Unit) | (1 << (int)Define.LayerList.Obstacle);
                        bool iru = Physics.Raycast(transform.position, dir, out var target, float.MaxValue, mask);
    
                        if (!iru)
                            continue;
    
                        if(target.collider.gameObject.layer == (int)Define.LayerList.Obstacle)
                            continue;
    
                        else if(target.collider.gameObject.layer == (int)Define.LayerList.Unit)
                            return coll.GetComponent<UnitBase>();
                    }
                }
            }
            return null;
        }
    
    }
    ```
    
- FOVEditor
    
    ```csharp
    using UnityEditor;
    using UnityEngine;
    
    [CustomEditor(typeof(AIFov))]
    public class FOVEditor : Editor {
        private void OnSceneGUI() {
            AIFov fov = (AIFov)target;
    
            Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);
    
            Handles.color = Color.green;
    
            Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.viewRange);
    
            Handles.color = new Color(1f, 1f, 1f, 0.2f);
    
            Handles.DrawSolidArc(fov.transform.position, Vector3.up, fromAnglePos, fov.viewAngle, fov.viewRange);
    
            Handles.Label(fov.transform.position + (fov.transform.forward * 2.0f), fov.viewAngle.ToString());
        }
    }
    ```
    
- AIController
    
    ```csharp
    public class AIController : UnitBase
    {
        private AIFov _fov;
        private void Update() {
            if (State == Define.UnitState.Dead)
                return;
                    
            if (!_targetUnit && _fov.isTracePlayer())
            {
                _targetUnit = _fov.isTracePlayer();
            }
    
            if (_targetUnit && !_fov.isTracePlayer() && State == Define.UnitState.Shot) {
                _targetUnit = null;
                State = Define.UnitState.Idle;
    
                StartCoroutine(CoMove());
            }
        }
    }
    ```
    

🤓**Result!**

적의 시야를 기즈모 형식으로 씬에 표시해, 지정한 각도의 데이터값이 실제 어떻게 적용되고 있는지 시각적으로 쉽게 확인할 수 있게 됨.

### ・옵저버 패턴을 이용한 UI 시스템

🤔**WHY?**

 데이터의 변경이 없음에도 주기적으로 UI에 데이터를 동기화해, 필요 없는 작업이 지속적으로 반복되어 결과적으로 퍼포먼스 하락

🤔**HOW?**

 관련 코드

- PlayerController
    
    ```csharp
    public class PlayerController : UnitBase
    {
    	  public Action<int> PlayerHpEvent;
        public Action PlayerKillEvent;
        public Action<int, int> PlayerBulletEvent;
     }
    ```
    
- UI_Player
    
    ```csharp
    public class UI_Player : MonoBehaviourPunCallbacks
    {
    		public void SetPlayer(PlayerController go) {
        _playerHp = Util.FindChild(gameObject, "PlayerHpText").GetComponent<Text>();
        _playerKill = Util.FindChild(gameObject, "KillNumberText").GetComponent<Text>();
        _playerBullet = Util.FindChild(gameObject, "BulletNumberText").GetComponent<Text>();
        _player = go;
        _myIndex = _player._myIndexNumber;
        _playerHp.text = "100 / 100";
        _playerBullet.text = "30 / 120";
        _playerKill.text = _killNumber.ToString();
        _player.PlayerHpEvent += (currentHp) => _playerHp.text = $"{currentHp} / 100";
        _player.PlayerBulletEvent += (currentBullet, remainBullet) => _playerBullet.text = $"{currentBullet} / {remainBullet}";
        _player.PlayerKillEvent += () => _playerKill.text = $"{++_killNumber}";
    }
    }
    ```
    
- GameScene
    
    ```csharp
    public class GameScene : BaseScene
    {
    		public override void Init() {
        base.Init();
        
        GameObject player = PhotonNetwork.Instantiate($"Prefabs/Unit/Player", _respawnPoints.GetChild(PhotonNetwork.LocalPlayer.ActorNumber - 1).transform.position, Quaternion.identity);
        GameObject playerui = Managers.Resources.Instantiate("UI/UI_Player", null);
        playerui.GetComponent<UI_Player>().SetPlayer(player.GetComponent<PlayerController>());
    
    		}
    }
    ```
    

🤓**Result!**

게임이 시작될 때, 각 플레이어가 각각의 UI를 생성 후 데이터를 연동해, 한 번의 구독으로 반복적인 작업의 호출없이 주기적으로 데이터와 UI를 연동할 수 있게 됨

## 📈보완점

🤔**문제점**

플레이어가 벽에 가까이 다가가면, 무기 오브젝트가 벽에 파묻혀 보이지 않는 문제 발생

🤔**문제의 원인**

무기 오브젝트엔 충돌판정이 없었고, 무기 오브젝트에 충돌판정을 추가하면 플레이어가 벽에 붙지 않아도 충돌로 판정이 되어 더이상 이동이 불가능해짐

🤓**해결방안**

하나만 사용하던 카메라를 무기만을 비추는 카메라와 무기 외 모든것을 비추는 카메라 두 개로 나누고 무기만을 비추는 카메라의 우선순위를 높여 실질적으로 무기 오브젝트가 다른 오브젝트에 파묻혀도 카메라를 통해 출력될 수 있게 수정
