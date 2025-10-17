using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Wave")]
public class WaveSO : ScriptableObject
{
    public float prepareTime;      // ���̺� ���� �� �غ� �ð�(Ready ������ ����)
    public SpawnGroup[] spawns;    // ���� ����(����/����/����)
    public Reward reward;          // ���̺� ���� ����(�������� ��)
}

[System.Serializable]
public struct SpawnGroup
{
    public MonsterSO monster;
    public int count;
    public float interval;
    public float startDelay;
}

[System.Serializable]
public struct Reward
{
    public int sugar;          // ���̺� Ŭ���� �� ����
    public int crystalBonus;   // �ɼ�: �ʿ� ������ 0
}
