using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriggerAnimation
{
    public static void TriggerAnim(Animator animator, string trigger)
    {
          animator.SetTrigger(trigger);
    }
}
