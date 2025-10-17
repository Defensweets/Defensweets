using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Monster")]
public class MonsterSO : ScriptableObject
{
    public string monsterName;
    public MonsterType monsterType;
    public GameObject prefab;  // Monster.cs�� ���� ������
    public int hp;
    public float speed;
    public int rewardSugar;
    public float slowResist;   // 0~1 (���ο� ����), �ʿ� ������ 0
    public bool splitsOnDeath; // ���� ť�� ���� �п���
}