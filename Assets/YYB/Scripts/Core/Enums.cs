using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Ready, Wave, Result }          // �������� ���� ����
public enum TowerType { Basic, Slow, AoE, Stun }       // Ÿ�� Ÿ�� ����
public enum MonsterType { Normal, Fast, Tank, Split }  // ���� Ÿ�� ����
public enum TileEffectType{ None, Sticky, Explosive, SweetBoost } // Ÿ�� Ÿ�� ����