using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : UnitBase
{
    public float _rotateSpeed = 2f;
    private GameObject _mainCamera;
    private GameObject _otherCamera;
    private GameObject _rifle;
    private float vx;
    private float vy;

    public Action<int> PlayerHpEvent;
    public Action PlayerKillEvent;
    public Action<int, int> PlayerBulletEvent;
    protected override void Awake() {
        base.Awake();
    }
    void Start()
    {


        if (!_view.IsMine)
            return;
        _parent = GameObject.Find("@Pool_root").transform;

        _view.RPC("SetIndex", RpcTarget.All);

        _rifle = Util.FindChild(gameObject, "AssaultRifle");
        _mainCamera = Util.FindChild(gameObject, "OtherCamera");
        _otherCamera = Util.FindChild(gameObject, "WeaponCamera");

        _rifle.layer = (int)Define.LayerList.Weapon;
        _mainCamera.SetActive(true);
        _otherCamera.SetActive(true);


        Managers.Input.OnKeyboardEvent += OnMoveUpdate;
        Managers.Input.OnKeyboardEvent += SetReload;
        Managers.Input.OnKeyboardUpEvent += OnPressExitUpdate;
        Managers.Input.OnMouseEvent += OnShotUpdate;
        Managers.Input.OnMouseUpEvent += OnShotExitUpdate;
    }

    [PunRPC]
    private void SetIndex() {
        _myIndexNumber = --Managers.playerIndex;
    }

    void Update()
    {
        if (!_view.IsMine)
            return;

        if (State == Define.UnitState.Dead)
            return;

        OnRotateUpdate();
    }

    private void OnShotExitUpdate(Define.MouseEventType type) {
        if (State == Define.UnitState.Dead)
            return;

        if (State == Define.UnitState.Shot)
            State = Define.UnitState.Idle;
    }
    private void OnPressExitUpdate() {
        if (State == Define.UnitState.Dead)
            return;

        if (State != Define.UnitState.Reload)
            State = Define.UnitState.Idle;
    }
    public override void ShotEvent() {
        if (!_view.IsMine)
            return;

        if (State == Define.UnitState.Dead)
            return;

        _currentBulletNumber--;
        PlayerBulletEvent?.Invoke(_currentBulletNumber, _remainBulletNumber);
        if (_currentBulletNumber == 0) {
            OnReloadUpdate();
            return;
        }

        _view.RPC("SetEffectRPC", RpcTarget.All);

        StartCoroutine(COShake());
        int mask = (1 << (int)Define.LayerList.Unit) | (1 << (int)Define.LayerList.Obstacle);

        Debug.DrawRay(_firePos.position, _firePos.forward * 100f, Color.green, 1f);
        bool isHit = Physics.Raycast(_firePos.position, _firePos.forward , out var hit, float.MaxValue, mask);
        if (!isHit)
            return;

        if (hit.collider.gameObject.layer == (int)Define.LayerList.Obstacle) {
            GameObject effect = PhotonNetwork.Instantiate("Prefabs/Effect/BulletEffect", hit.point, Quaternion.identity);
            effect.transform.parent = _parent;
            effect.transform.LookAt(_firePos.position);
            StartCoroutine(CoDestroy(effect, 0.3f));
            return;

        }

        if (hit.collider.gameObject.layer == (int)Define.LayerList.Unit) {
            GameObject effect = PhotonNetwork.Instantiate("Prefabs/Effect/BloodEffect", hit.point, Quaternion.identity);
            effect.transform.parent = _parent;
            effect.transform.LookAt(_firePos.position);
            UnitBase unit = hit.collider.GetComponentInChildren<UnitBase>();
            StartCoroutine(CoDestroy(effect, 0.3f));

            unit._view.RPC("SetHp", RpcTarget.All, 10, _myIndexNumber);
        }
    }

    [PunRPC]
    public void SetHp(int damage, int index) {
        if (!_view.IsMine)
            return;

        if (State == Define.UnitState.Dead)
            return;

        _hp -= damage;
        PlayerHpEvent?.Invoke(_hp);
       
        if (_hp <= 0) {
            var players = GameObject.FindObjectsByType(typeof(UnitBase), FindObjectsSortMode.None);
            foreach(var item in players) {
                if(index == item.GetComponent<UnitBase>()._myIndexNumber) {
                    if (item as AIController) {
                        item.GetComponent<AIController>()._targetUnit = null;
                        break;
                    }
                    if (item as PlayerController) {
                        item.GetComponent<PlayerController>().PlayerKillEvent?.Invoke();
                        break;
                    }
                }
            }
            Dead();
            
        }
    }

  

    private void SetReload() {
        if (State == Define.UnitState.Dead)
            return;

        if (Input.GetKeyDown(KeyCode.R)) {
            OnReloadUpdate();
        }
    }

    private void OnShotUpdate(Define.MouseEventType type) {
        if (State == Define.UnitState.Dead)
            return;

        if (type != Define.MouseEventType.ButtonLeft)
            return;

        State = Define.UnitState.Shot;
    }

    private void OnMoveUpdate() {
        if (State == Define.UnitState.Dead)
            return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 dirX = moveX * transform.right;
        Vector3 dirZ = moveZ * transform.forward;
        Vector3 dir = dirX + dirZ;

        _rigid.MovePosition(_rigid.position + dir * + _moveSpeed * Time.deltaTime);

        if(moveZ > 0) {
            State = Define.UnitState.WalkFront;
            return;
        }

        if(moveZ < 0) {
            State = Define.UnitState.WalkBack;
            return;
        }

        if (moveX < 0) {
            State = Define.UnitState.WalkLeft;
            return;
        }

        if (moveX > 0) {
            State = Define.UnitState.WalkRight;
            return;
        }
    }

    private void OnRotateUpdate() {
        if (!_view.IsMine)
            return;

        if (State == Define.UnitState.Dead)
            return;

        float MouseX = Input.GetAxis("Mouse X");
        float MouseY = Input.GetAxis("Mouse Y");
        Vector3 dir = new Vector3(MouseY, MouseX, 0);

        vx += dir.x * _rotateSpeed * Time.deltaTime;
        vy += dir.y * _rotateSpeed * Time.deltaTime;

        vx = Mathf.Clamp(vx, -15f, 7f);

        Vector3 lastDir = new Vector3(-vx, vy, 0);
        transform.eulerAngles = lastDir;
    }

    private IEnumerator COShake() {
        if (State != Define.UnitState.Dead) {
            float exitTime = 0;

            while (true) {
                exitTime += Time.deltaTime;
                vx += exitTime * 2f;

                if (exitTime > .1f)
                    break;
                yield return null;
            }
        }
    }

    public override void ReloadEvent() {
        if (!_view.IsMine)
            return;

        base.ReloadEvent();
        if (State == Define.UnitState.Dead)
            return;

        PlayerBulletEvent?.Invoke(_currentBulletNumber, _remainBulletNumber);
    }

    public override void DeadEvent() {
        if (!_view.IsMine)
            return;

        _mainCamera.transform.parent = null;

        RespawnManager.Instance.Respawn(_myIndexNumber, 5f);
        Destroy(_mainCamera.gameObject, 5f);
        PhotonNetwork.Destroy(gameObject);
    }

}
