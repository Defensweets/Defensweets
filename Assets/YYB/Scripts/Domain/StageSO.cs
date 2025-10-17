using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Defensweet/Stage")]
public class StageSO : ScriptableObject
{
    public string stageId;
    public int initialSugar;    // ���� ����
    public int initialCrystal;  // ���� ũ����Ż(������������ �ʱ�ȭ)
    public int gateHp;          // ���� ü��(�Ǵ� ħ�� ���ġ)
    public WaveSO[] waves;      // �� ���������� ���̺�(3~6��)
}
