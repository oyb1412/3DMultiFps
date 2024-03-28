using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviourPunCallbacks
{
    public PlayerController _player;
    public int _myIndex;
    private Text _playerHp;
    private Text _playerKill;
    private Text _playerBullet;
    public int _killNumber;
    private void Start() {
    }

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
