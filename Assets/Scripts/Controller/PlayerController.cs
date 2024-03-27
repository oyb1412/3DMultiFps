using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System;

public class PlayerController : UnitBase
{
    public float _rotateSpeed = 2f;
    private float vx;
    private float vy;
    public Action<int> PlayerHpEvent;
    public Action<int> PlayerKillEvent;
    public Action<int, int> PlayerBulletEvent;
    protected override void Awake() {
        base.Awake();
    }
    void Start()
    {
        Managers.Input.OnKeyboardEvent += OnMoveUpdate;
        Managers.Input.OnKeyboardEvent += SetReload;
        Managers.Input.OnKeyboardUpEvent += OnPressExitUpdate;
        Managers.Input.OnMouseEvent += OnShotUpdate;
        Managers.Input.OnMouseUpEvent += OnShotExitUpdate;
    }

    void Update()
    {
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
        if (State == Define.UnitState.Dead)
            return;

        _currentBulletNumber--;
        PlayerBulletEvent?.Invoke(_currentBulletNumber, _remainBulletNumber);
        if (_currentBulletNumber == 0) {
            OnReloadUpdate();
            return;
        }

        _fireEffect.Play();
        _cartridgeEffect.Play();
        StartCoroutine(COShake());
        int mask = (1 << (int)Define.LayerList.Unit) | (1 << (int)Define.LayerList.Obstacle);

        Debug.DrawRay(_firePos.position, _firePos.forward * 100f, Color.green, 1f);
        bool isHit = Physics.Raycast(_firePos.position, _firePos.forward , out var hit, float.MaxValue, mask);
        if (!isHit)
            return;

        if (hit.collider.gameObject.layer == (int)Define.LayerList.Obstacle) {
            GameObject effect = Managers.Resources.Instantiate("Effect/BulletEffect", null);
            effect.transform.position = hit.point;
            effect.transform.LookAt(_firePos.position);
            Destroy(effect, 1f);
            return;

        }

        if (hit.collider.gameObject.layer == (int)Define.LayerList.Unit) {
            GameObject effect = Managers.Resources.Instantiate("Effect/BloodEffect", null);
            effect.transform.position = hit.point;
            effect.transform.LookAt(_firePos.position);
            UnitBase player = hit.collider.GetComponentInChildren<UnitBase>();
            Destroy(effect, 1f);
            player.Hit(this);
        }
    }
    public override void Hit(UnitBase attacker) {
        if (State == Define.UnitState.Dead)
            return;

        _hp -= 10;
        PlayerHpEvent?.Invoke(_hp);
        if (_hp <= 0) {
            Dead();
            var ai = attacker as AIController;
            if (ai) {
                ai._targetUnit = null;
            }
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
        base.ReloadEvent();
        if (State == Define.UnitState.Dead)
            return;

        PlayerBulletEvent?.Invoke(_currentBulletNumber, _remainBulletNumber);
    }
}
