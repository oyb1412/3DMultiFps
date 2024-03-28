using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreScene : BaseScene
{
    public override void Clear() {
    }

    public override void Init() {
        base.Init();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }
}
