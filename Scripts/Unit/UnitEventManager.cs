using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [07/15/2025]
 * [a static event manager for units]
 */

public static class UnitEventManager
{
    public static Action<Unit> UnitDeathEvent;

    /// <summary>
    /// invokes all functions that subscribes to
    /// UnitDeathEvent
    /// </summary>
    /// <param name="unit">the dead unit</param>
    public static void OnUnitDeathEvent(Unit unit)
    {
        UnitDeathEvent?.Invoke(unit);
    }
}
