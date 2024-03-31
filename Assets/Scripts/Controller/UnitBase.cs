using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviourPunCallbacks
{
    protected readonly float _animationFadeTime = .2f;
    protected int _remainBulletNumber = 120;
    protected int _currentBulletNumber = 30;
    protected int _maxReloadBulletNumber = 30;
    protected int _hp = 100;
    public int _myIndexNumber;
    protected Transform _parent;

    protected Rigidbody _rigid;
    private Collider _collider;
    private Animator _animator;
    protected Transform _firePos;
    public PhotonView _view;
    protected ParticleSystem _fireEffect;
    protected ParticleSystem _cartridgeEffect;
    protected float _moveSpeed = 3;

    private Define.UnitState _state;

    public Define.UnitState State {
        get { return _state; }
        set {
            if (_state == value)
                return;

            if (!_view.IsMine)
                return;

            switch (value) {
                
                case Define.UnitState.Idle:
                    if (State == Define.UnitState.Dead)
                        return;
                    SetAnimator("Idle");
                    break;
                case Define.UnitState.WalkFront:
                    if (State == Define.UnitState.Shot || State == Define.UnitState.Reload || State == Define.UnitState.Dead)
                        return;
                    SetAnimator("WalkFront");
                    break;
                case Define.UnitState.WalkBack:
                    if (State == Define.UnitState.Shot || State == Define.UnitState.Reload || State == Define.UnitState.Dead)
                        return;
                    SetAnimator("WalkBack");
                    break;
                case Define.UnitState.WalkLeft:
                    if (State == Define.UnitState.Shot || State == Define.UnitState.Reload || State == Define.UnitState.Dead)
                        return;
                    SetAnimator("WalkLeft");
                    break;
                case Define.UnitState.WalkRight:
                    if (State == Define.UnitState.Shot || State == Define.UnitState.Reload || State == Define.UnitState.Dead)
                        return;
                    SetAnimator("WalkRight");
                    break;
                case Define.UnitState.Shot:
                    if (State == Define.UnitState.Reload || State == Define.UnitState.Dead)
                        return;
                    SetAnimator("Shot");
                    break;
                case Define.UnitState.Reload:
                    if (State == Define.UnitState.Dead)
                        return;
                    SetAnimator("Reload");
                    break;
                case Define.UnitState.Dead:
                    SetAnimator("Dead");
                    break;
            }

            _state = value;
        }
    }

    protected virtual void Awake() {
        _view = GetComponent<PhotonView>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _rigid = GetComponent<Rigidbody>();
        _firePos = Util.FindChild(gameObject, "FirePos").transform;
        _fireEffect = Util.FindChild(gameObject, "FireEffect").GetComponent<ParticleSystem>();
        _cartridgeEffect = Util.FindChild(gameObject, "CartridgeEffect").GetComponent<ParticleSystem>();
    }

    [PunRPC]
    protected void SetAnimatorRPC(string name) {
        _animator.CrossFade(name, _animationFadeTime);
    }

    [PunRPC]
    protected void SetEffectRPC() {
        _fireEffect.Play();
        _cartridgeEffect.Play();
    }

    protected void SetAnimator(string name) {
        _view.RPC("SetAnimatorRPC", RpcTarget.All, name);
    }
    public abstract void ShotEvent();


    
    protected IEnumerator CoDestroy(GameObject go, float time) {
        yield return new WaitForSeconds(time);
        PhotonNetwork.Destroy(go);
    }

    protected void Dead() {
        if (!_view.IsMine)
            return;

        State = Define.UnitState.Dead;
        gameObject.layer = 0;
        _collider.enabled = false;
        _rigid.isKinematic = true;
    }


    public abstract void DeadEvent();


    protected void OnReloadUpdate() {
        if (!_view.IsMine)
            return;

        if (State == Define.UnitState.Dead)
            return;

        if (_remainBulletNumber == 0 || _currentBulletNumber == _maxReloadBulletNumber)
            return;

        State = Define.UnitState.Reload;
    }
    public virtual void ReloadEvent() {
        if (!_view.IsMine)
            return;

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

    public void PlayShotSound() {
        SoundManager.Instance.PlayerSfx(SoundManager.Sfx.SHOT);
    }
    public void PlayShotStep() {
        SoundManager.Instance.PlayerSfx(SoundManager.Sfx.STEP);
    }
    public void PlayShotReload() {
        SoundManager.Instance.PlayerSfx(SoundManager.Sfx.RELOAD);
    }
}
