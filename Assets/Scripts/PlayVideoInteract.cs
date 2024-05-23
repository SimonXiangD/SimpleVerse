using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EIS.Runtime.Core;
using EIS.Runtime.Core.Hints;
using UnityEngine.Video;

public class PlayVideoInteract : Interactable
{
    // Start is called before the first frame update


    public override bool UsePrimaryActionHint() => true;
    public override string GetPrimaryHintText() => "Play This Video";

    public override bool UseSecondaryActionHint() => false;
    public override string GetSecondaryHintText() => default;

    public override bool IsUnsubscribeBlocked() => false;

    public VidPlayer vidPlayer;

    public VideoClip clip;

    // [SerializeField] private InventoryItemData inventoryItemData;

    public override void OnInteractPrimary()
    {
        vidPlayer.playClip(clip);
        // InventoryController.AddToInventory(inventoryItemData);
        // Destroy(gameObject);
    }

}
