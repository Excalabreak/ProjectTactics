using Godot;
using System;

public abstract partial class BaseConsumableEffect : Resource, IUseable
{
    public abstract void OnUse(Unit unit);
}
