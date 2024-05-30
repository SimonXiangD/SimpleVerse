

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EIS.Runtime.Core;
using EIS.Runtime.Core.Hints;

public class InteractableUIPanel : Interactable
{
    // Start is called before the first frame update
    public GameObject panel;

    public override bool UsePrimaryActionHint() => true;
    public override string GetPrimaryHintText() => "Open UI Panel";

    public override bool UseSecondaryActionHint() => false;
    public override string GetSecondaryHintText() => default;

    public override bool IsUnsubscribeBlocked() => false;

    // [SerializeField] private InventoryItemData inventoryItemData;

    public override void OnInteractPrimary()
    {
        if(!UIController.instance.canInteract()) return;
        UIController.instance.showCustomScreen(panel);
        // Debug.Log("what can I say");
        // InventoryController.AddToInventory(inventoryItemData);
        // Destroy(gameObject);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
