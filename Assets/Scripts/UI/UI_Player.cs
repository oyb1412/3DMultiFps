using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    private PlayerController _player;
    private Text _playerHp;
    private Text _playerKill;
    private Text _playerBullet;

    private void Start() {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
        _playerHp = Util.FindChild(gameObject, "PlayerHpText").GetComponent<Text>();
        _playerKill = Util.FindChild(gameObject, "KillNumberText").GetComponent<Text>();
        _playerBullet = Util.FindChild(gameObject, "BulletNumberText").GetComponent<Text>();

        _player.PlayerHpEvent += (currentHp) => _playerHp.text = $"{currentHp} / 100";
        _player.PlayerBulletEvent += (currentBullet, remainBullet) => _playerBullet.text = $"{currentBullet} / {remainBullet}";
        _player.PlayerKillEvent += (killNumber) => _playerKill.text = $"{killNumber}";
    }
}
