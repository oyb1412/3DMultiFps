using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class AIController : UnitBase
{
    private NavMeshAgent _agent;
    public UnitBase _targetUnit;
    private Transform _movePoint;
    private AIFov _fov;
    protected override void Awake() {
        base.Awake();
        _fov = GetComponent<AIFov>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;
        _movePoint = GameObject.Find("AiMovePoint").transform;
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
        float ran = Random.Range(-.3f, .3f);
        int mask = (1 << (int)Define.LayerList.Unit) | (1 << (int)Define.LayerList.Obstacle);
        Debug.DrawRay(_firePos.position, _firePos.transform.forward * 100f, Color.red, 1f);
        bool isHit = Physics.Raycast(_firePos.position, _firePos.transform.forward + new Vector3(ran, ran, 0f), out var hit, float.MaxValue, mask);
        if (!isHit)
            return;

        if (hit.collider.gameObject.layer == (int)Define.LayerList.Obstacle)
            return;

        if (hit.collider.gameObject.layer == (int)Define.LayerList.Unit) {
            UnitBase player = hit.collider.GetComponent<UnitBase>();
            player.Hit(this);
        }
    }

    public IEnumerator CoMove() {
        int ran = Random.Range(0, _movePoint.childCount - 1);
        Transform pos = _movePoint.GetChild(ran);
        _agent.SetDestination(pos.position);
        State = Define.UnitState.Idle;
        State = Define.UnitState.WalkFront;
        while (true) {
            float dir = (pos.position - transform.position).magnitude;
            
            if (_targetUnit) {
                _agent.SetDestination(transform.position);
                transform.LookAt(_targetUnit.transform.position);
                State = Define.UnitState.Shot;
                StopAllCoroutines();
                break;
            }
                
            if (dir < 0.1f) {
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
        Debug.Log($"{name}피격당함. 남은체력 {_hp}");

        if (!_targetUnit) {
            _targetUnit = attacker;
        }

        if (_hp <= 0) {
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
