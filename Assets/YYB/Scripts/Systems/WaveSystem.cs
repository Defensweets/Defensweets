using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    public static WaveSystem Instance { get; private set; }

    private bool isSpawning = false;
    public bool IsSpawnFinished => !isSpawning;

    private List<GameObject> activeMonsters = new List<GameObject>(); // �ʵ忡 �����ϴ� ���� ����
    private Coroutine waveRoutine;

    [Header("Spawn Point")]
    public Transform spawnPoint; // �ν����Ϳ� ���� (���� ��ġ)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    
    /// ���̺� ���� (GameManager���� ȣ��)
    
    public IEnumerator RunWave(WaveSO waveData, System.Action onSpawned, System.Action onOneDied)
    {
        Debug.Log($"[WaveSystem] ���̺� ����: {waveData.name}");

        isSpawning = true;

        // ���̺� �غ� �ð�
        yield return new WaitForSeconds(waveData.prepareTime);

        // ���� ����
        foreach (var group in waveData.spawns)
        {
            yield return new WaitForSeconds(group.startDelay);
            StartCoroutine(SpawnGroup(group, onSpawned));
        }

        // ��� �׷��� ���� ������ ���
        while (isSpawning)
        {
            if (activeMonsters.Count == 0) break;
            yield return null;
        }

        // ���̺� ���� ���� ó��
        var reward = waveData.reward;
        if (reward.sugar > 0) ResourceSystem.Instance.AddSugar(reward.sugar);
        if (reward.crystalBonus > 0) ResourceSystem.Instance.TryUseCrystal(-reward.crystalBonus); // ���� �� �߰� ����

        Debug.Log($"[WaveSystem] ���̺� ����: {waveData.name}");
    }

    
    /// �ϳ��� ���� �׷� ����
    
    private IEnumerator SpawnGroup(SpawnGroup group, System.Action onSpawned)
    {
        for (int i = 0; i < group.count; i++)
        {
            SpawnMonster(group.monster);
            onSpawned?.Invoke();
            yield return new WaitForSeconds(group.interval);
        }

        // ��� ���� �Ϸ� �� ��� (Ȥ�� ���� �׷��� ������ ���� isSpawning ����)
        yield return new WaitForSeconds(0.5f);
        isSpawning = false;
    }

    
    /// ���� �ν��Ͻ� ����
    
    private void SpawnMonster(MonsterSO monsterData)
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning("[WaveSystem] Spawn Point�� �������� �ʾҽ��ϴ�.");
            return;
        }

        GameObject monster = Instantiate(monsterData.prefab, spawnPoint.position, Quaternion.identity);
        /// Monster monsterComp = monster.GetComponent<Monster>();
        /// monsterComp.Initialize(monsterData, this);

        activeMonsters.Add(monster);
    }

    
    /// ���Ͱ� ����ϰų� �ʵ忡�� ���ŵ� �� ȣ��
    
    public void OnMonsterRemoved(GameObject monster)
    {
        if (activeMonsters.Contains(monster))
            activeMonsters.Remove(monster);
    }
}
