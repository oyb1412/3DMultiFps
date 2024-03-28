using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginScene : BaseScene {
    public override void Clear() {
    }

    public override void Init() {
        Cursor.lockState = CursorLockMode.Confined;
        base.Init();
    }
}
