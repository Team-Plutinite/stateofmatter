using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// delegate for internal debuff actions
public delegate void DebuffAction();

// class to represent an enemy's debuff
public class Debuff
{
    private float maxTime;
    private float debuffTimer;
    private float pauseTimer;
    private DebuffAction initialAction;
    private DebuffAction tickAction;
    private DebuffAction finalAction;

    public Debuff(float time, DebuffAction initialAction, DebuffAction tickAction, DebuffAction finalAction)
    {
        debuffTimer = time;
        maxTime = time;
        pauseTimer = 0.0f;
        this.initialAction = initialAction;
        this.tickAction = tickAction;
        this.finalAction = finalAction;

        // Call the initial action on debuff creation
        this.initialAction?.Invoke();
    }

    /// <summary>
    /// How much time remains for this debuff
    /// </summary>
    public float Timer { get { return debuffTimer; } set { debuffTimer = value; } }

    /// <summary>
    /// The final action to invoke on this enemy before removing the debuff.
    /// Usually used to reset something the initial action did.
    /// </summary>
    /// <returns>If the action was successfully called.</returns>
    public void InvokeFinalAction()
    {
        finalAction?.Invoke();
    }

    /// <summary>
    /// Tick this debuff down. Invokes the associated tick function if there is one.
    /// </summary>
    /// <returns>If the action was successfully called.</returns>
    public void Tick()
    {
        // Timer is 0, so don't tick.
        if (debuffTimer <= 0.0f)
            return;

        // If the timer is paused, do nothing
        if (pauseTimer > 0.0f)
            pauseTimer -= Time.deltaTime;
        else
        {
            debuffTimer -= Time.deltaTime;
            tickAction?.Invoke();
        }
    }

    /// <summary>
    /// Pauses this debuff's timer for a set amount of seconds.
    /// </summary>
    /// <param name="time">The time seconds to pause for.</param>
    public void Pause(float time)
    {
        pauseTimer = time;
    }

    public void ResetTimer()
    {
        debuffTimer = maxTime;
    }
}
