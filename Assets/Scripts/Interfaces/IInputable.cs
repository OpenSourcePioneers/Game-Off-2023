using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputable
{
    bool IsMoving();
    bool IsShiftClicked();
    bool IsShiftHold();
    bool IsSpacePressed();
    bool IsLMouseClick();
    bool IsLMouseHold();
    bool IsRMouseClick();
    bool IsRMouseHold();
}
