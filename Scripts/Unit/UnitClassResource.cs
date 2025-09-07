using Godot;
using Godot.Collections;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/07/2025]
 * [resource to hold information about a class]
 */

[GlobalClass]
public partial class UnitClassResource : Resource
{
    //a very temp mesure so that playtesters know what weapon is equipt
    [Export] private Dictionary<WeaponTypeEnum, Texture2D> _weaponSprites = new Dictionary<WeaponTypeEnum, Texture2D>();

    public Dictionary<WeaponTypeEnum, Texture2D> weaponSprites
    {
        get { return _weaponSprites; }
    }
}
