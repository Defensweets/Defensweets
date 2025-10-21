using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// ���̺� �ϳ��� ����������Ŭ�� å������ �ý���.
/// - �غ�ð� ��� �� �׷캰 ���� �� ���� ���� ���� �� �ʵ� Ŭ���� ���� �� ���� ����
/// - ���� ���� �����տ� WaveAgent�� �ٿ���
///   �ı�(���/��ǥ ����) ������ �ڵ����� WaveSystem�� ����ǰ� �Ѵ�.
/// </summary>
public class WaveSystem : MonoBehaviour
{
    public static WaveSystem Instance { get; private set; }

    [Header("Spawn Settings")]
    [Tooltip("���� ��ȯ ��ġ(�ʼ�)")]
    public Transform spawnPoint;

    // �ʵ忡 ����ִ� ���� Ʈ��ŷ(�ߺ� ������ HashSet + ��ȸ�� List)
    private readonly HashSet<GameObject> activeMonsterSet = new HashSet<GameObject>();
    private readonly List<GameObject> activeMonsters = new List<GameObject>();

    // �ܺο��� �����ϴ� ����
    public bool IsWaveActive { get; private set; } = false;
    public bool IsSpawnFinished => spawningGroups <= 0;

    // ���� ���� ����
    private int spawningGroups = 0;            // ���ÿ� ���� �ִ� �׷� �ڷ�ƾ ��
    private Coroutine waveRoutine;

    // �ݹ�(�ɼ�): GameManager���� �Ѱ��� �� ����
    private Action onSpawnedCb;     // ���� 1���� ������ ������
    private Action onOneRemovedCb;  // ���� 1���� ����(���/����)�� ������

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // (����) �̺�Ʈ ����: ���� Monster�� �̺�Ʈ�� �����Ѵٸ� ���⿡ ���� ����
        // EventBus.Subscribe(Events.OnMonsterKilled, _ => {/*�ڿ� ó�� ��*/});
        // EventBus.Subscribe(Events.OnMonsterReachGoal, _ => {/*����Ʈ ó�� ��*/});
    }

    /// <summary>
    /// GameManager���� ȣ��. ���̺� �� �� ����.
    /// </summary>
    public IEnumerator RunWave(WaveSO waveData, Action onSpawned = null, Action onOneRemoved = null)
    {
        if (waveData == null)
        {
            Debug.LogError("[WaveSystem] RunWave: waveData is null");
            yield break;
        }
        if (spawnPoint == null)
        {
            Debug.LogError("[WaveSystem] RunWave: spawnPoint not assigned");
            yield break;
        }

        IsWaveActive = true;
        onSpawnedCb = onSpawned;
        onOneRemovedCb = onOneRemoved;

        // �غ� �ð�
        if (waveData.prepareTime > 0f)
            yield return new WaitForSeconds(waveData.prepareTime);

        // �׷캰 ���� ���� (���� ����)
        spawningGroups = 0;
        foreach (var group in waveData.spawns)
        {
            // struct�� null �� �Ұ� �� �ʵ�� ��ȿ�� �Ǵ�
            if (group.count <= 0) continue;
            if (group.monster == null) continue;

            spawningGroups++;
            StartCoroutine(SpawnGroup(group));
        }

        // ������ ��� ������, �ʵ忡 ���Ͱ� 0���� �� ������ ���
        while (true)
        {
            if (IsSpawnFinished && activeMonsters.Count == 0) break;
            yield return null;
        }

        // ���̺� ���� ����
        var rw = waveData.reward;
        if (rw.sugar > 0)
            ResourceSystem.Instance.AddSugar(rw.sugar);

        if (rw.crystalBonus > 0)
            ResourceSystem.Instance.TryUseCrystal(-rw.crystalBonus);

        // ���� ����
        IsWaveActive = false;
        onSpawnedCb = null;
        onOneRemovedCb = null;

        // (����) ���̺� ���� �̺�Ʈ ���
        // EventBus.Publish(Events.OnWaveCleared, null);
    }

    /// <summary>
    /// �� �׷� ����(���� ���� �� nȸ ��ȯ �� ����)
    /// </summary>
    private IEnumerator SpawnGroup(SpawnGroup group)
    {
        if (group.startDelay > 0f)
            yield return new WaitForSeconds(group.startDelay);

        for (int i = 0; i < group.count; i++)
        {
            SpawnOne(group.monster);
            onSpawnedCb?.Invoke();

            if (group.interval > 0f)
                yield return new WaitForSeconds(group.interval);
            else
                yield return null; // ���� ������ ���� ���� ����
        }

        spawningGroups--;
        if (spawningGroups < 0) spawningGroups = 0; // ������ġ
    }

    /// <summary>
    /// ���� 1�� ��ȯ + WaveAgent �����ؼ� �����ֱ� ����
    /// </summary>
    private void SpawnOne(MonsterSO monsterData)
    {
        var prefab = monsterData.prefab;
        if (prefab == null)
        {
            Debug.LogWarning("[WaveSystem] SpawnOne: Monster prefab is null");
            return;
        }

        var go = GameObject.Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // WaveAgent ����(�ߺ� ����)
        var agent = go.GetComponent<WaveAgent>();
        if (agent == null) agent = go.AddComponent<WaveAgent>();
        agent.Setup(this);

        RegisterMonster(go);

        // (������ Monster.cs���� Initialize�� �����ϸ�, ���⼭ ����)
        // var monster = go.GetComponent<Monster>();
        // if (monster != null) monster.Initialize(monsterData /*, PathSystem �� �ʿ��*/);
    }

    /// <summary>�ʵ� Ʈ��ŷ: ���</summary>
    private void RegisterMonster(GameObject go)
    {
        if (go == null) return;
        if (activeMonsterSet.Add(go))
            activeMonsters.Add(go);
    }

    /// <summary>�ʵ� Ʈ��ŷ: ����(���/��ǥ����/�ı�)</summary>
    private void UnregisterMonster(GameObject go)
    {
        if (go == null) return;

        if (activeMonsterSet.Remove(go))
        {
            // List���� ������ ����(�ڿ��� ������)
            int idx = activeMonsters.IndexOf(go);
            if (idx >= 0)
            {
                int last = activeMonsters.Count - 1;
                activeMonsters[idx] = activeMonsters[last];
                activeMonsters.RemoveAt(last);
            }

            onOneRemovedCb?.Invoke();
        }
    }

    /// <summary>
    /// ���Ͱ� �ı��� �� WaveAgent�� ȣ��
    /// </summary>
    internal void OnMonsterDestroyed(GameObject go)
    {
        UnregisterMonster(go);
    }

    // ��������������������������������������������������������������������������������������������������������������������
    // ���� ����: ������ ���Ϳ� �ٿ��� ���� ����� �����ϴ� ������Ʈ
    [DisallowMultipleComponent]
    private sealed class WaveAgent : MonoBehaviour
    {
        private WaveSystem owner;
        private bool reported = false;

        public void Setup(WaveSystem sys) => owner = sys;

        // � �����ε� �ı��� ��(���/�񵵴�/������) �ڵ� ����
        private void OnDestroy()
        {
            // �� ���� ������ �ߺ� ȣ�� ����
            if (reported || owner == null) return;
            reported = true;
            owner.OnMonsterDestroyed(gameObject);
        }
    }
}