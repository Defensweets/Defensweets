using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Analytics;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Data")]
    public StageSO stage;         // �� ������ ����� �������� ������

    public GameState CurrentState { get; private set; }
    private int currentWaveIndex = -1;   // ���� ���� ��

    private int gateHp;           // ���� ü��(�Ǵ� ħ�� ���ġ)
    private int aliveMonsters = 0; // �ʵ忡 ����ִ� ���� ��

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // �������� �ʱ� ����
        gateHp = stage.gateHp;
        ResourceSystem.Instance.Setup(stage.initialSugar, stage.initialCrystal);

        // �̺�Ʈ ����
        EventBus.Subscribe(Events.OnMonsterKilled, (obj) => OnMonsterKilled());
        EventBus.Subscribe(Events.OnWaveCleared, (obj) => OnWaveCleared((int)obj));

        SetState(GameState.Ready);
        // �غ� ������ StartNextWave()�� ��ư/Ÿ�̸ӷ� ȣ��
    }
    /*
    public Transform spawnTransform;  // ���� ����(���� ��ǥ)
    public Transform goalTransform;   // ��ǥ ����(���� ��ǥ)
    public TileSystem tileSystem;

    public void StartNextWave()
    {
        if (CurrentState != GameState.Ready) return;

        // 1) ���� ����� Ÿ�� ���·� �ִܰ�� ��� + ���
        var startCell = PathSystem.Instance.WorldToCell(spawnTransform.position);
        var goalCell = PathSystem.Instance.WorldToCell(goalTransform.position);
        var ok = PathSystem.Instance.ComputeAndLockPath(
            tileSystem.GetWalkableCells(), startCell, goalCell);

        if (!ok)
        {
            // ��ΰ� ������ ���̺� ���� �Ұ�(���/���̵�)
            Debug.LogWarning("��ΰ� ���� ���̺긦 ������ �� �����ϴ�.");
            return;
        }

        // 2) ���̺� ����
        SetState(GameState.Wave);
        StartCoroutine(WaveSystem.Instance.RunWave(
            stage.waves[++currentWaveIndex],
            onSpawned: () => aliveMonsters++,
            onOneDied: () => { aliveMonsters--; CheckWaveEnd(); }));
    }
    */
    private void CheckWaveEnd()
    {
        // ���� �Ϸ� & �ʵ� ���� 0 �� ���̺� ����
        if (aliveMonsters <= 0 && WaveSystem.Instance.IsSpawnFinished)
        {
            // ���� ����
            var reward = stage.waves[currentWaveIndex].reward;
            if (reward.sugar > 0) ResourceSystem.Instance.AddSugar(reward.sugar);
            // (�ɼ�) ũ����Ż ���ʽ��� ���⼭

            EventBus.Publish(Events.OnWaveCleared, currentWaveIndex);
            SetState(GameState.Ready);
        }
    }

    private void OnWaveEnded()
    {
        // ���� ��ȯ + ��� ����
        PathSystem.Instance.Unlock();
        SetState(GameState.Ready);

        // UI/����� �̺�Ʈ ��ε�ĳ��Ʈ
        OnWaveCleared(currentWaveIndex);
    }

    public void OnMonsterReachGoal()
    {
        gateHp--;
        if (gateHp <= 0) OnFailed();
    }

    private void OnMonsterKilled()
    {
        // �ʿ� �� �۷ι� ī��Ʈ/�޺� �� ó��
    }

    private void OnWaveCleared(int waveIndex)
    {
        // UI ����/���� ��
    }

    private void OnAllWavesCleared()
    {
        SetState(GameState.Result);
        // ��� UI(Ŭ����) ȣ��
    }

    private void OnFailed()
    {
        SetState(GameState.Result);
        // ��� UI(����) ȣ��
    }

    private void SetState(GameState next)
    {
        CurrentState = next;
        EventBus.Publish(Events.OnStateChanged, next);
        // Ready: ��/Ÿ�� ���� ���, Wave: �Ϻ� ����, Result: UI ǥ��
    }
}
