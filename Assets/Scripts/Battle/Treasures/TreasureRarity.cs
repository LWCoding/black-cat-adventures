using UnityEngine;

public enum TreasureRarity { Starter = 0, Common = 1, Rare = 2, SuperRare = 3, Legendary = 4 }

public static class TreasureRarityInfo
{

    public static Color GetColor(TreasureRarity r) => r switch
    {
        TreasureRarity.Common    => new Color32(0x66, 0xB2, 0xFF, 0xFF),
        TreasureRarity.Rare      => new Color32(0x4C, 0xC9, 0x5F, 0xFF),
        TreasureRarity.SuperRare => new Color32(0xB5, 0x7B, 0xFF, 0xFF),
        TreasureRarity.Legendary => new Color32(0xFF, 0xD2, 0x4C, 0xFF),
        _                        => new Color32(0xC8, 0xC8, 0xC8, 0xFF), // Starter
    };

    public static string GetHexColor(TreasureRarity r) => ColorUtility.ToHtmlStringRGB(GetColor(r));

    public static string GetLabel(TreasureRarity r) => r switch
    {
        TreasureRarity.SuperRare => "Super Rare!",
        TreasureRarity.Legendary => "Legendary!",
        TreasureRarity.Starter   => "Starter",
        _                        => r.ToString(), // "Common" / "Rare"
    };

}
