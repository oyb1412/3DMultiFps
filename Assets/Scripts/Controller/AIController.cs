using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using DG.Tweening;
using Photon.Pun;
using Unity.VisualScripting;
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

    [PunRPC]
    private void SetIndex() {
        _myIndexNumber = Managers.aiIndex--;
    }
    private void Start() {
        _view.RPC("SetIndex", RpcTarget.AllBuffered);

        StartCoroutine(CoMove());
    }

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

        _view.RPC("SetEffectRPC", RpcTarget.All);
        
        float ran = Random.Range(-.3f, .3f);
        int mask = (1 << (int)Define.LayerList.Unit) | (1 << (int)Define.LayerList.Obstacle);
        Debug.DrawRay(_firePos.position, _firePos.forward * 100f, Color.red, 1f);
        bool isHit = Physics.Raycast(_firePos.position, _firePos.forward + new Vector3(ran,ran,0f), out var hit, float.MaxValue, mask);
        if (!isHit)
            return;

        if (hit.collider.gameObject.layer == (int)Define.LayerList.Obstacle) {
            GameObject effect = PhotonNetwork.Instantiate("Prefabs/Effect/BulletEffect", hit.point, Quaternion.identity);
            effect.transform.LookAt(_firePos.position);
            Destroy(effect, 1f);
            return;
        }
        

        if (hit.collider.gameObject.layer == (int)Define.LayerList.Unit) {
            GameObject effect = PhotonNetwork.Instantiate("Prefabs/Effect/BloodEffect", hit.point, Quaternion.identity);
            effect.transform.LookAt(transform.position);
            Destroy(effect, 1f);
            UnitBase unit = hit.collider.GetComponentInChildren<UnitBase>();
            unit._view.RPC("SetHp", RpcTarget.All, 10, _myIndexNumber);
        }
    }

    public IEnumerator CoMove() {
        int ran = Random.Range(0, _movePoint.childCount - 1);
        Transform pos = _movePoint.GetChild(ran);
        _agent.SetDestination(pos.position);
        State = Define.UnitState.Idle;
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

    [PunRPC]
    public void SetHp(int i, int index) {

        if (State == Define.UnitState.Dead)
            return;

        _hp -= 10;

        if(!_targetUnit) {
            var players = GameObject.FindObjectsByType(typeof(UnitBase), FindObjectsSortMode.None);
            foreach (var item in players) {
                if (index == item.GetComponent<UnitBase>()._myIndexNumber) {
                    _targetUnit = item.GetComponent<UnitBase>();
                    break;
                }
            }
        }
        
      if (_hp <= 0) {
            var players = GameObject.FindObjectsByType(typeof(UnitBase), FindObjectsSortMode.None);
            foreach (var item in players) {
                if (index == item.GetComponent<UnitBase>()._myIndexNumber)
                    if(item as PlayerController)
                        item.GetComponent<PlayerController>().PlayerKillEvent?.Invoke();
                break;
            }
            Dead();

        }
    }

    public override void ReloadEvent() {
        base.ReloadEvent();
        if (_targetUnit == null)
            return;

        State = Define.UnitState.Shot;
    }

    public override void DeadEvent() {
       
        RespawnManager.Instance.Respawn(_myIndexNumber, 5f);
        PhotonNetwork.Destroy(gameObject);
    }
}
