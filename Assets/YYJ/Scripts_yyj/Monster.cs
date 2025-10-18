using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public MonsterSO monsterData;

    private int currentHp;
    private List<Vector3> path;
    private int currentWaypointIndex = 0;
    private Transform goal;     // ��������

    // Start is called before the first frame update
    void Start()
    {
        currentHp = monsterData.hp;

        goal = GameObject.FindWithTag("Goal").transform; // �������� Goal �±� ���� ������Ʈ �±״� �ӽ�
        path = Pathfinding.FindPath(transform.position, goal.position);

        if (path == null)
        {
            Debug.LogError("��ΰ� �����ϴ�.", gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandleMovement()
    {
        if (path == null || currentWaypointIndex >= path.Count)
        {
            return;     // ���� �Ǵ� �� ����
        }

        Vector3 targetPosition = path[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, monsterData.speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= path.Count)     // ���� ������ ���� ��
            {
                OnReachGoal();
            }
        }
    }

    private void OnReachGoal()      // ���� �� ���Ϳ��� �߻��� ���� ���� �߰�
    {
        Destroy(gameObject);        
    }

    public void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()      // ��� �� ���Ϳ��� �߻��� ���� ���� �߰�
    {
        Destroy(gameObject);
    }
}
