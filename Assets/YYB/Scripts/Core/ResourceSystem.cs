using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    public static ResourceSystem Instance { get; private set; }

    [Header("Current")]
    [SerializeField] private int sugar;
    [SerializeField] private int crystal;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // �̺�Ʈ ����: ���� óġ �� �������� �߰�
        EventBus.Subscribe(Events.OnMonsterKilled, (obj) =>
        {
            AddSugar((int)obj);
            EventBus.Publish(Events.OnResourcePing, null);
        });
    }

    // �ʱ�ȭ (�������� ���� �� GameManager�� ȣ��)
    public void Setup(int startSugar, int startCrystal)
    {
        sugar = startSugar;
        crystal = startCrystal;
        EventBus.Publish(Events.OnResourcePing, null);
    }

    public bool TryUseSugar(int amount)
    {
        if (sugar < amount) return false;
        sugar -= amount;
        EventBus.Publish(Events.OnResourcePing, null);
        return true;
    }

    public bool TryUseCrystal(int amount)
    {
        if (crystal < amount) return false;
        crystal -= amount;
        EventBus.Publish(Events.OnResourcePing, null);
        return true;
    }

    public void AddSugar(int amount)
    {
        sugar += amount;
        EventBus.Publish(Events.OnResourcePing, null);
    }

    public int Sugar => sugar;
    public int Crystal => crystal;
}