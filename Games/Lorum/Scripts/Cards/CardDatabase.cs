using System.Collections.Generic;
using Godot;

namespace cardgames.Lorum.Scripts.Cards;

public static class CardDatabase
{
    public static readonly List<(int Value, Texture2D Texture)> CardDatas = new();
    private static readonly List<(int Value, string Path)> CardValuesPaths = new()
    {

        (1, "res://Assets/Cards/zold_also.png"),
        (2, "res://Assets/Cards/zold_felso.png"),
        (3, "res://Assets/Cards/zold_kiraly.png"),
        (4, "res://Assets/Cards/zold_asz.png"),
        (5, "res://Assets/Cards/zold_7.png"),
        (6, "res://Assets/Cards/zold_8.png"),
        (7, "res://Assets/Cards/zold_9.png"),
        (8, "res://Assets/Cards/zold_10.png"),

        (11, "res://Assets/Cards/piros_also.png"),
        (12, "res://Assets/Cards/piros_felso.png"),
        (13, "res://Assets/Cards/piros_kiraly.png"),
        (14, "res://Assets/Cards/piros_asz.png"),
        (15, "res://Assets/Cards/piros_7.png"),
        (16, "res://Assets/Cards/piros_8.png"),
        (17, "res://Assets/Cards/piros_9.png"),
        (18, "res://Assets/Cards/piros_10.png"),

        (21, "res://Assets/Cards/makk_also.png"),
        (22, "res://Assets/Cards/makk_felso.png"),
        (23, "res://Assets/Cards/makk_kiraly.png"),
        (24, "res://Assets/Cards/makk_asz.png"),
        (25, "res://Assets/Cards/makk_7.png"),
        (26, "res://Assets/Cards/makk_8.png"),
        (27, "res://Assets/Cards/makk_9.png"),
        (28, "res://Assets/Cards/makk_10.png"),

        (31, "res://Assets/Cards/tok_also.png"),
        (32, "res://Assets/Cards/tok_felso.png"),
        (33, "res://Assets/Cards/tok_kiraly.png"),
        (34, "res://Assets/Cards/tok_asz.png"),
        (35, "res://Assets/Cards/tok_7.png"),
        (36, "res://Assets/Cards/tok_8.png"),
        (37, "res://Assets/Cards/tok_9.png"),
        (38, "res://Assets/Cards/tok_10.png")

    };
    public static void loadTextures()
    {
        CardDatas.Clear();
        foreach (var card in CardValuesPaths)
        {
            Texture2D texture = GD.Load<Texture2D>(card.Path);
            CardDatas.Add((card.Value, texture));
        }
    }
}