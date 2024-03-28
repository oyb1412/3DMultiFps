using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScene : BaseScene
{
    public override void Clear() {
    }

    public override void Init() {
        base.Init();
        Cursor.lockState = CursorLockMode.Confined;

    }
}
