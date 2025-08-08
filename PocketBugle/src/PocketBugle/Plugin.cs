using BepInEx;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

namespace PocketBugle;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    private const ushort BugleItemID = 15;
    private const string BugleItemName = "Bugle";
    private const string DestroyHeldItemMethod = nameof(CharacterItems.DestroyHeldItemRpc);
    private const string SpawnItemInHandMethod = nameof(CharacterItems.RPC_SpawnItemInHandMaster);

    private static KeyCode PocketKey => PocketConfig.PocketKey.Value;

    private void Awake()
    {
        Logger.LogInfo("Plugin waking...");
        PocketConfig.Bind(Config);
        Logger.LogInfo("Plugin awake!");
    }

    private static bool IsBugle(Item? item) => item && item.itemID == BugleItemID;

    private static bool IsHoldingBugle(Character character) =>
        IsBugle(character.data.currentItem);

    private static void DestroyHeldItem(Character character) =>
        character.refs.items.photonView.RPC(DestroyHeldItemMethod, PhotonNetwork.LocalPlayer);
        // character.refs.items.DestroyHeldItemRpc(); // Will not work if PEAKERRpcInfo is installed


    private static bool TryDestroyHeldBugle(Character character)
    {
        if (!IsHoldingBugle(character)) return false;
        DestroyHeldItem(character);

        var slot = character.refs.items.currentSelectedSlot;
        if (slot.IsSome) character.player.EmptySlot(slot);

        return true;
    }

    private static bool TryEquipBugle(Character character)
    {
        if (IsHoldingBugle(character)) return true;

        // Inventory is full - consider this successful so current item isn't dropped
        if (!character.player.tempFullSlot.IsEmpty()) return true;

        var itemSlots = character.player.itemSlots ?? [];
        for (byte i = 0; i < itemSlots.Length; i++)
        {
            // ItemSlot isn't a UnityEngine.Object so null-conditional is safe
            var item = itemSlots[i]?.prefab;
            if (!IsBugle(item)) continue;

            character.refs.items.EquipSlot(Optionable<byte>.Some(i));
            return true;
        }

        return false;
    }

    private static void SpawnBugle(Character character) =>
        character.refs.items.photonView.RPC(SpawnItemInHandMethod, RpcTarget.MasterClient, BugleItemName);
        // character.refs.items.SpawnItemInHand(BugleItemName); // Will not work if PEAKERRpcInfo is installed


    private void Update()
    {
        if (!Input.GetKeyDown(PocketKey)) return;

        var character = Character.localCharacter;
        if (!character) return;

        if (TryDestroyHeldBugle(character)) return;
        if (TryEquipBugle(character)) return;
        SpawnBugle(character);
    }
}
