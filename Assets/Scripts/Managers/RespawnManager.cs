using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RespawnManager : MonoBehaviourPunCallbacks
{
    public static RespawnManager Instance;
    private PhotonView _view;
    private Transform _respawnPoints;
    private void Awake() {
        Instance = this;
        _view = GetComponent<PhotonView>();
    }

    private void Start() {
        _respawnPoints = GameObject.Find("RespawnPoints").transform;
    }
 
    public void Respawn(int number, float time) {
        StartCoroutine(CoRespawn(number, time));
    }

    IEnumerator CoRespawn(int number, float time) {
        yield return new WaitForSeconds(time);
        RespawnObject(number);
    }

    private void RespawnObject(int number) {
        var units = FindObjectsOfType<UnitBase>();
        int count = 0;
        foreach(Transform t in _respawnPoints.transform) {
            foreach(UnitBase p in units) {
                if(Vector3.Distance(t.position, p.transform.position) > 5f) {
                    count++;
                }
            }
            if (count >= units.Length) {
                GameObject unit;

                if (number < 600) {
                    unit = PhotonNetwork.Instantiate($"Prefabs/Unit/Player", t.position, Quaternion.identity);
                    var Uis = GameObject.FindObjectsOfType(typeof(UI_Player));
                    foreach(var g in Uis) {
                        if(g.GetComponent<UI_Player>()._myIndex == unit.GetComponent<PlayerController>()._myIndexNumber) {
                            g.GetComponent<UI_Player>().SetPlayer(unit.GetComponent<PlayerController>());
                            Debug.Log($"�÷��̾� ��ȣ{number}");
                            break;
                        }
                    }
                } else {
                    if (PhotonNetwork.IsMasterClient) {
                        PhotonNetwork.Instantiate($"Prefabs/Unit/Ai", t.position, Quaternion.identity);
                        Debug.Log($"Ai ��ȣ{number}");
                    }
                }
                return;
            } else
                count = 0;
        }
    }
}
