using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define 
{
    public enum UnitState {
        Idle,
        WalkFront,
        WalkBack,
        WalkLeft,
        WalkRight,
        Run,
        Jump,
        Shot,
        Reload,
        Dead,
    }
    public enum MouseEventType
    {
        None,
        ButtonDownLeft,
        ButtonDownRight,
        ButtonUpLeft,
        ButtonUpRight,
        ButtonLeft,
        ButtonRight,
        Enter,
    }

    public enum SceneType
    {
        None,
        InGame,
    }

    public enum LayerList {
        Unit = 6,
        Obstacle,
    }
}
