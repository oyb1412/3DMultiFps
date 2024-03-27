using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator _animator;
    private void Awake() {
        _animator = GetComponent<Animator>();
    }
}
