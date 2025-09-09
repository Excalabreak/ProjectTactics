
/*
 * Author: [Lam, Justin]
 * Last Updated: [09/09/2025]
 * [an interface for all items 
 * that can go into the inventory]
 */

public interface IInventoryItem
{
    string devName { get; }
    string itemName { get; }
    string description { get; }

    int size { get; }
}
