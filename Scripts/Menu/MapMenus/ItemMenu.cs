using Godot;
using System.Collections.Generic;

/*
 * Author: [Lam, Justin]
 * Last Updated: [09/09/2025]
 * [script for the menu]
 */

public partial class ItemMenu : BaseMenu
{
    [ExportGroup("Item Buttons")]
    [Export] private Button _equippedWeaponButton;
    [Export] private Button[] _itemSlotButtons;

    [ExportGroup("Subactions")]
    [Export] private Button _useButton;
    [Export] private Button _equipButton;
    [Export] private Button _dequipButton;

    [ExportGroup("Description")]
    [Export] private Label _descriptionLabel;

    private UnitInventory _unitInventory;
    private UnitActionEconomy _unitActionEconomy;
    private Dictionary<Button, IInventoryItem> _inventorySlots = new Dictionary<Button, IInventoryItem>();

    private IInventoryItem _currentlySelectedItem;

    /// <summary>
    /// calls to set the labels for buttons
    /// </summary>
    /// <param name="inventory">inventory for unit</param>
    public void SetUpItemSlots(UnitInventory inventory, UnitActionEconomy actionEconomy)
    {
        _unitInventory = inventory;
        _unitActionEconomy = actionEconomy;

        _equippedWeaponButton.Text = "E: " + inventory.equiptWeapon.itemName;
        _inventorySlots.Add(_equippedWeaponButton, inventory.equiptWeapon);

        for (int i = 0; i < _itemSlotButtons.Length; i++)
        {
            if (i >= inventory.inventoryItems.Length)
            {
                _itemSlotButtons[i].Text = "---";
                continue;
            }

            _itemSlotButtons[i].Text = inventory.inventoryItems[i].itemName;
            _inventorySlots.Add(_itemSlotButtons[i], inventory.inventoryItems[i]);
        }
    }

    /// <summary>
    /// resets the item slots when a change happens
    /// </summary>
    private void ResetItemSlots()
    {
        _useButton.Visible = false;
        _equipButton.Visible = false;
        _dequipButton.Visible = false;

        _inventorySlots = new Dictionary<Button, IInventoryItem>();

        _equippedWeaponButton.Text = "E: " + _unitInventory.equiptWeapon.itemName;
        _inventorySlots.Add(_equippedWeaponButton, _unitInventory.equiptWeapon);

        for (int i = 0; i < _itemSlotButtons.Length; i++)
        {
            if (i >= _unitInventory.inventoryItems.Length)
            {
                _itemSlotButtons[i].Text = "---";
                continue;
            }

            _itemSlotButtons[i].Text = _unitInventory.inventoryItems[i].itemName;
            _inventorySlots.Add(_itemSlotButtons[i], _unitInventory.inventoryItems[i]);
        }
    }

    /// <summary>
    /// shows what unit can do with item when toggled true
    /// </summary>
    /// <param name="toggle">true if the button is pressed</param>
    /// <param name="button">which button was toggled</param>
    private void OnItemButtonToggled(bool toggle, NodePath button)
    {
        if (!toggle)
        {
            _useButton.Visible = false;
            _equipButton.Visible = false;
            _dequipButton.Visible = false;
        }

        Button toggledButton = GetNode(button) as Button;

        if (!_inventorySlots.ContainsKey(toggledButton))
        {
            return;
        }

        _currentlySelectedItem = _inventorySlots[toggledButton];

        if (_currentlySelectedItem is IEquipable)
        {
            if (toggledButton == _equippedWeaponButton)
            {
                if(_unitInventory.HasEquippedWeapon())
                {
                    _dequipButton.Visible = true;
                }
            }
            else
            {
                _equipButton.Visible = true;
            }
        }
        if (_currentlySelectedItem is IUseable && _unitActionEconomy.HasActions())
        {
            _useButton.Visible = true;
        }
    }

    /// <summary>
    /// calls to equip 
    /// </summary>
    private void OnEquipPressed()
    {
        //check for weapon, armor, or accessory
        IEquipable item = _currentlySelectedItem as IEquipable;

        if (item == null)
        {
            return;
        }

        switch (item.equipableSlot)
        {
            case EquipableSlotEnum.WEAPON:
                _unitInventory.EquipWeapon(_currentlySelectedItem);
                break;
            case EquipableSlotEnum.ARMOR:
                break;
            case EquipableSlotEnum.ACCESSORY:
                break;
            default:
                return;
        }
        ResetItemSlots();
    }

    /// <summary>
    /// calls to dequipt a the currenly equipt item
    /// </summary>
    private void OnDequipPressed()
    {
        //check for weapon, armor, or accessory
        IEquipable item = _currentlySelectedItem as IEquipable;

        if (item == null)
        {
            return;
        }

        switch (item.equipableSlot)
        {
            case EquipableSlotEnum.WEAPON:
                _unitInventory.DequipWeaponToInventory();
                break;
            case EquipableSlotEnum.ARMOR:
                break;
            case EquipableSlotEnum.ACCESSORY:
                break;
            default:
                return;
        }
        ResetItemSlots();
    }

    /// <summary>
    /// calls to use item
    /// </summary>
    private void OnUsePressed()
    {
        IUseable item = _currentlySelectedItem as IUseable;

        if (item == null)
        {
            return;
        }

        _unitActionEconomy.UseAction();
        _unitInventory.UseItem(_currentlySelectedItem);

        ResetItemSlots();
    }

    private void UpdateDescriptionPannel(NodePath buttonPath)
    {
        Button button = GetNode(buttonPath) as Button;

        if (!_inventorySlots.ContainsKey(button))
        {
            _descriptionLabel.Text = "Description:";
            return;
        }

        _descriptionLabel.Text = "Description: \n" + _inventorySlots[button].description; 
    }

    /// <summary>
    /// going to put this here for now, but will likely just move it to the
    /// decline buttion
    /// </summary>
    public void OnCancelButtonPress()
    {
        _gameBoard.ResetMenu();

        HideMenu();
    }
}
