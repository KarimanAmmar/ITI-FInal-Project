using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    Transform targetPoint;
    [SerializeField] float speed;
    [SerializeField] private Float_event enemy_damage_event;
    [SerializeField] private float damage;
    private void OnTriggerEnter(Collider other)
    {
        this.gameObject.SetActive(false);
        if (other.CompareTag(GameConstant.EnemyTag))
        {
            if(other.gameObject.GetComponent<enemy_health_system>() != null)
            {
                other.gameObject.GetComponent<enemy_health_system>().Take_damage(damage);
            }
        }
    }
    public void Initialize(Transform target, float moveSpeed)
    {
        targetPoint = target;
        speed = moveSpeed;
    }

    private void Update()
    {
        Move();
    }
    void Move()
    {
        if (targetPoint != null)
        {
            Vector3 direction = (targetPoint.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }
}
