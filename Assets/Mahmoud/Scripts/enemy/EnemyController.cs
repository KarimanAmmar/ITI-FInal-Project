using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	[SerializeField] private WanderingState wanderingState;
	[SerializeField] private ChasingState chasingState;
	[SerializeField] private AttackingState attackingState;
	[SerializeField] private float attackDistance;
	[SerializeField] private float chaseDistance;
	private Transform playerTransform;
	private IEnemyState currentState;
	private Rigidbody rb;

	public event Action OnDefeated;

	[SerializeField] private EnemyType enemyType;

	private void OnDisable()
	{
		OnDefeated?.Invoke();
	}

	public void Defeat()
	{
		gameObject.SetActive(false);
		OnDefeated?.Invoke();
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();

		// Initialize the attack behavior based on the enemy type
		switch (enemyType)
		{
			case EnemyType.Exploding:
				attackingState = new AttackingState(new ExplodesAttack());
				break;
			case EnemyType.Blowing:
				attackingState = new AttackingState(new BlowsAttack());
				break;
			case EnemyType.Aiming:
				attackingState = new AttackingState(new AimsAttack());
				break;
			default:
				Debug.LogError("Unknown enemy type!");
				break;
		}
	}

	private void Start()
	{
		// Find player transform (assuming there is only one player)
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			playerTransform = player.transform;
		}
		else
		{
			Debug.LogError("Player not found! Make sure the player object has the 'Player' tag.");
		}

		TransitionToState(wanderingState);
	}

	private void Update()
	{
		if (playerTransform != null)
		{
			float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
			TransitionStateBasedOnDistance(distanceToPlayer);
			currentState.UpdateState(this, playerTransform.position);
		}
	}

	private void TransitionStateBasedOnDistance(float distanceToPlayer)
	{
		if (currentState != attackingState && distanceToPlayer <= attackDistance)
		{
			TransitionToState(attackingState);
		}
		else if (currentState != chasingState && distanceToPlayer > attackDistance && distanceToPlayer <= chaseDistance)
		{
			TransitionToState(chasingState);
		}
		else if (currentState != wanderingState && distanceToPlayer > chaseDistance)
		{
			TransitionToState(wanderingState);
		}
	}

	public void TransitionToState(IEnemyState nextState)
	{
		if (currentState != null)
		{
			currentState.ExitState(this);
		}

		currentState = nextState;
		currentState.EnterState(this);
	}

	public Rigidbody GetRigidbody()
	{
		return rb;
	}

	public void SetPlayerTransform(Transform player)
	{
		playerTransform = player;
	}
}
