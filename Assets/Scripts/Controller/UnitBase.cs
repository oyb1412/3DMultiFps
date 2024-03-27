using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected readonly float _animationFadeTime = .2f;
    protected int _remainBulletNumber = 120;
    protected int _currentBulletNumber = 30;
    protected int _maxReloadBulletNumber = 30;
    protected int _hp = 100;
    protected Rigidbody _rigid;
    private Collider _collider;
    private Animator _animator;
    protected ParticleSystem _fireEffect;
    public float _moveSpeed = 5f;

    protected int _killNumber;
    private Define.UnitState _state;
    protected Transform _firePos;

    public Define.UnitState State {
        get { return _state; }
        set {
            if (_state == value)
                return;

            switch (value) {
                case Define.UnitState.Idle:
                    _animator.CrossFade("Idle", _animationFadeTime);
                    break;
                case Define.UnitState.WalkFront:
                    if (State == Define.UnitState.Shot || State == Define.UnitState.Reload)
                        return;
                    _animator.CrossFade("WalkFront", _animationFadeTime);
                    break;
                case Define.UnitState.WalkBack:
                    if (State == Define.UnitState.Shot || State == Define.UnitState.Reload)
                        return;
                    _animator.CrossFade("WalkBack", _animationFadeTime);
                    break;
                case Define.UnitState.WalkLeft:
                    if (State == Define.UnitState.Shot || State == Define.UnitState.Reload)
                        return;
                    _animator.CrossFade("WalkLeft", _animationFadeTime);
                    break;
                case Define.UnitState.WalkRight:
                    if (State == Define.UnitState.Shot || State == Define.UnitState.Reload)
                        return;
                    _animator.CrossFade("WalkRight", _animationFadeTime);
                    break;
                case Define.UnitState.Shot:
                    if (State == Define.UnitState.Reload)
                        return;
                    _animator.CrossFade("Shot", _animationFadeTime);
                    break;
                case Define.UnitState.Reload:

                    _animator.CrossFade("Reload", _animationFadeTime);
                    break;
                case Define.UnitState.Dead:
                    _animator.CrossFade("Dead", _animationFadeTime);
                    break;
            }

            _state = value;
        }
    }

    protected virtual void Awake() {
        _rigid = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _firePos = Util.FindChild(gameObject, "FirePos").transform;
        _fireEffect = Util.FindChild(gameObject, "FireEffect").GetComponent<ParticleSystem>();
    }

    public abstract void ShotEvent();

    public abstract void Hit(UnitBase attacker);

    protected void Dead() {
        if (State == Define.UnitState.Dead)
            return;

        State = Define.UnitState.Dead;
        gameObject.layer = 0;
        _collider.enabled = false;
        _rigid.isKinematic = true;
    }

    public void DeadEvent() {
        //todo
        //시체 삭제 후 카메라 띄우기
        //리스폰 대기
    }


    protected void OnReloadUpdate() {
        if (State == Define.UnitState.Dead)
            return;

        if (_remainBulletNumber == 0 || _currentBulletNumber == _maxReloadBulletNumber)
            return;

        State = Define.UnitState.Reload;
    }
    public virtual void ReloadEvent() {
        if (State == Define.UnitState.Dead)
            return;

        _remainBulletNumber -= (_maxReloadBulletNumber - _currentBulletNumber);

        if (_remainBulletNumber >= _maxReloadBulletNumber) {
            _currentBulletNumber = _maxReloadBulletNumber;
        } else {
            _currentBulletNumber = _remainBulletNumber;
        }

        State = Define.UnitState.Idle;
    }
}
