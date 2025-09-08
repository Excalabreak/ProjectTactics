using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/08/2025]
 * [script to use potion]
 */

[GlobalClass]
public partial class HealthPotionEffect : BaseConsumableEffect
{
    [Export] private int _healAmount;

    public override void OnUse(Unit unit)
    {
        unit.unitStats.HealUnit(_healAmount);
    }
}
