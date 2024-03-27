using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using DG.Tweening;
public class AIController : UnitBase
{
    private NavMeshAgent _agent;
    [HideInInspector]public UnitBase _targetUnit;
    private Transform _movePoint;
    private AIFov _fov;
    protected override void Awake() {
        base.Awake();
        _rigid = GetComponentInParent<Rigidbody>();
        _fov = GetComponent<AIFov>();
        _agent = GetComponentInParent<NavMeshAgent>();
        _agent.speed = _moveSpeed;
        _movePoint = GameObject.Find("AiMovePoints").transform;
    }

    private void Start() {
        StartCoroutine(CoMove());
    }

    private void Update() {
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

    public override void ShotEvent() {
        if (State == Define.UnitState.Dead)
            return;

        if(_targetUnit == null) {
            StartCoroutine(CoMove());
            return;
        }
        _currentBulletNumber--;
        transform.LookAt(_targetUnit.transform.position);
        if (_currentBulletNumber == 0) {
            OnReloadUpdate();
            return;
        }

        _fireEffect.Play();
        _cartridgeEffect.Play();
        float ran = Random.Range(-.25f, .25f);
        int mask = (1 << (int)Define.LayerList.Unit) | (1 << (int)Define.LayerList.Obstacle);
        Debug.DrawRay(_firePos.position, _firePos.forward * 100f, Color.red, 1f);
        bool isHit = Physics.Raycast(_firePos.position, _firePos.forward + new Vector3(ran,ran,0f), out var hit, float.MaxValue, mask);
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
            effect.transform.LookAt(transform.position);
            Destroy(effect, 1f);
            UnitBase player = hit.collider.GetComponentInChildren<UnitBase>();
            player.Hit(this);
        }
    }

    public IEnumerator CoMove() {
        int ran = Random.Range(0, _movePoint.childCount - 1);
        Transform pos = _movePoint.GetChild(ran);
        _agent.SetDestination(pos.position);
        State = Define.UnitState.WalkFront;
        while (true) {
            float dir = (new Vector3(pos.position.x,0f,pos.position.z)
                - new Vector3(transform.position.x, 0f, transform.position.z)).magnitude;
            
            if (_targetUnit) {
                _agent.SetDestination(transform.position);
                transform.LookAt(_targetUnit.transform.position);
                State = Define.UnitState.Shot;
                StopAllCoroutines();
                break;
            }
                
            if (dir < 0.2f) {
                StartCoroutine(CoMove());
                break;
            }
            yield return null;
        }
    }

    public override void Hit(UnitBase attacker) {
        if (State == Define.UnitState.Dead)
            return;

        _hp -= 10;

        if (!_targetUnit) {
            _targetUnit = attacker;
        }

        if (_hp <= 0) {
            var player = attacker as PlayerController;
            if (player)
                player.PlayerKillEvent?.Invoke(++_killNumber);
            Dead();
        }

        
    }

    public override void ReloadEvent() {
        base.ReloadEvent();
        if (_targetUnit == null)
            return;

        State = Define.UnitState.Shot;
    }
}
