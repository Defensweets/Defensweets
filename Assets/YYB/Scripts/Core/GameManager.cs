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

    public void StartNextWave()
    {
        if (CurrentState != GameState.Ready) return;

        currentWaveIndex++;
        if (currentWaveIndex >= stage.waves.Length)
        {
            // ��� ���̺� �Ϸ� �� Ŭ����
            OnAllWavesCleared();
            return;
        }

        SetState(GameState.Wave);
        StartCoroutine(WaveSystem.Instance.RunWave(stage.waves[currentWaveIndex],
            onSpawned: () => aliveMonsters++,
            onOneDied: () => { aliveMonsters--; CheckWaveEnd(); }));
    }

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
