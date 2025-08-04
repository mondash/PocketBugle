using BepInEx.Configuration;
using UnityEngine;

namespace PocketBugle;

internal static class PocketConfig
{
    public static ConfigEntry<KeyCode> PocketKey = null!;

    public static void Bind(ConfigFile config)
    {
        PocketKey = config.Bind(
            "Control",
            "PocketKey",
            KeyCode.B,
            "Keyboard key used to spawn or destroy held bugle"
        );
    }
}
