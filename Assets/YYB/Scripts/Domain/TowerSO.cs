using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Tower")]
public class TowerSO : ScriptableObject
{
    public string towerName;
    public TowerType towerType;
    public Sprite icon;
    public GameObject prefab;      // Tower.cs�� ���� ������
    public TowerLevel[] levels;    // 1~3�ܰ� ����
}

[System.Serializable]
public struct TowerLevel
{
    public int damage;
    public float attackSpeed;   // �ʴ� ����(���ǻ�)
    public float range;         // ���� �Ÿ� ����(Ÿ�� ���� ȯ��)
    public int upgradeCostSugar;
    public int specialCostCrystal; // Ư�� ��ȭ ������ 0
}
