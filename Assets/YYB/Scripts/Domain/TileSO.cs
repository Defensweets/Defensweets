using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Tile")]
public class TileSO : ScriptableObject
{
    public string tileName;
    public Sprite sprite;
    public bool isWalkable;     // �̵� ��������
    public bool isBuildable;    // Ÿ�� ��ġ ��������
    public TileEffectType effectType; // Ÿ�� ȿ��(������ None)
}

