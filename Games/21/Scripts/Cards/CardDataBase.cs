using System.Collections.Generic;
using Godot;

namespace cardgames.Games._21.Scripts.Cards;

public static class CardDatabase
{
    public static readonly List<(int Value, int ScoreValue, Texture2D Texture)> CardDatas = new();

    private static readonly List<(int Value, int ScoreValue, string Path)> CardValuesPaths = new()
    {
        (1, 2, "res://Assets/Cards/zold_also.png"),
        (2, 3, "res://Assets/Cards/zold_felso.png"),
        (3, 4, "res://Assets/Cards/zold_kiraly.png"),
        (4, 11, "res://Assets/Cards/zold_asz.png"),
        (5, 7, "res://Assets/Cards/zold_7.png"),
        (6, 8, "res://Assets/Cards/zold_8.png"),
        (7, 9, "res://Assets/Cards/zold_9.png"),
        (8, 10, "res://Assets/Cards/zold_10.png"),

        (11, 2, "res://Assets/Cards/piros_also.png"),
        (12, 3, "res://Assets/Cards/piros_felso.png"),
        (13, 4, "res://Assets/Cards/piros_kiraly.png"),
        (14, 11, "res://Assets/Cards/piros_asz.png"),
        (15, 7, "res://Assets/Cards/piros_7.png"),
        (16, 8, "res://Assets/Cards/piros_8.png"),
        (17, 9, "res://Assets/Cards/piros_9.png"),
        (18, 10, "res://Assets/Cards/piros_10.png"),

        (21, 2, "res://Assets/Cards/makk_also.png"),
        (22, 3, "res://Assets/Cards/makk_felso.png"),
        (23, 4, "res://Assets/Cards/makk_kiraly.png"),
        (24, 11, "res://Assets/Cards/makk_asz.png"),
        (25, 7, "res://Assets/Cards/makk_7.png"),
        (26, 8, "res://Assets/Cards/makk_8.png"),
        (27, 9, "res://Assets/Cards/makk_9.png"),
        (28, 10, "res://Assets/Cards/makk_10.png"),

        (31, 2, "res://Assets/Cards/tok_also.png"),
        (32, 3, "res://Assets/Cards/tok_felso.png"),
        (33, 4, "res://Assets/Cards/tok_kiraly.png"),
        (34, 11, "res://Assets/Cards/tok_asz.png"),
        (35, 7, "res://Assets/Cards/tok_7.png"),
        (36, 8, "res://Assets/Cards/tok_8.png"),
        (37, 9, "res://Assets/Cards/tok_9.png"),
        (38, 10, "res://Assets/Cards/tok_10.png")
    };

    public static void loadTextures()
    {
        CardDatas.Clear();
        foreach (var card in CardValuesPaths)
        {
            Texture2D texture = GD.Load<Texture2D>(card.Path);
            CardDatas.Add((card.Value,card.ScoreValue ,texture));
        }
    }

    public static void GetTexture(int value)
    {
    }
}