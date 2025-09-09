
/*
 * Author: [Lam, Justin]
 * Last Updated: [08/22/2025]
 * [an interface for all items 
 * that can go into the inventory]
 */

public interface IInventoryItem
{
    string devName { get; }
    string itemName { get; }

    int size { get; }
}
