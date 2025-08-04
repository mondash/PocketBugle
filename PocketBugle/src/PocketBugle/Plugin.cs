using BepInEx;
using UnityEngine;

namespace PocketBugle;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    private const string BugleItemName = "Bugle";
    private static KeyCode PocketKey => PocketConfig.PocketKey.Value;

    private void Awake()
    {
        Logger.LogInfo("Plugin waking...");
        PocketConfig.Bind(Config);
        Logger.LogInfo("Plugin awake!");
    }

    private void Update()
    {
        if (!Input.GetKeyDown(PocketKey)) return;

        var character = Character.localCharacter;
        if (!character) return;

        var currentItem = character.data.currentItem;
        if (!currentItem)
            character.refs.items.SpawnItemInHand(BugleItemName);
        else if (currentItem.name == BugleItemName)
            character.refs.items.DestroyHeldItemRpc();
    }
}
