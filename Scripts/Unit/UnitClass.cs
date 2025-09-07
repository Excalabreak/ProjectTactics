using Godot;
using System;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/07/2025]
 * [script for unit class info]
 */

public partial class UnitClass : Node
{
    [Export] private Unit _unit;
    private UnitClassResource _classResource;

    public override void _Ready()
    {
        _classResource = _unit.unitResource.unitClass;
        //get class from unit resources
    }

    /// <summary>
    /// gets the sprite for the class based on what weapon
    /// </summary>
    /// <param name="weapon">weapon for class</param>
    /// <returns>texture for weapon</returns>
    public Texture2D GetWeaponTexture(WeaponTypeEnum weapon)
    {
        if (!_classResource.weaponSprites.ContainsKey(weapon))
        {
            GD.Print("no weapon texture for " + weapon);
            return _classResource.weaponSprites[WeaponTypeEnum.UNARMED];
        }

        return _classResource.weaponSprites[weapon];
    }

    public UnitClassResource classResource
    {
        get { return _classResource; }
    }
}
