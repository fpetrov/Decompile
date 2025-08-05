using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

// Token: 0x020001E0 RID: 480
public class CastleWraithController : NetworkBehaviour, IHitableMonster
{
	// Token: 0x06001374 RID: 4980 RVA: 0x00051F7A File Offset: 0x0005017A
	private void Start()
	{
		this.WraithAgent = base.transform.GetComponent<NavMeshAgent>();
	}

	// Token: 0x06001375 RID: 4981 RVA: 0x00051F90 File Offset: 0x00050190
	public override void OnStartClient()
	{
		base.OnStartClient();
		if (base.HasAuthority)
		{
			this.fsm = new StateMachine(false, false, false);
			this.fsm.AddState("Patrol", new State(null, delegate(State<string, string> state)
			{
				this.PatrolLogic();
			}, null, null, false, false));
			this.fsm.AddState("PatrolAggrod", new State(null, delegate(State<string, string> state)
			{
				this.PatrolWithTargetsLogic();
			}, null, null, false, false));
			this.fsm.AddState("Aggro", new State(null, delegate(State<string, string> state)
			{
				this.AggroLogic();
			}, null, null, false, false));
			this.fsm.AddState("Attack", new State(null, delegate(State<string, string> state)
			{
				this.AttackLogic();
			}, null, (State<string, string> state) => state.timer.Elapsed > 1f, true, false));
			this.fsm.AddTransition("Patrol", "PatrolAggrod", (Transition<string> transition) => this.targets.Count > 0, null, null, false);
			this.fsm.AddTransition("PatrolAggrod", "Patrol", (Transition<string> transition) => this.targets.Count < 1, null, null, false);
			this.fsm.AddTransition("PatrolAggrod", "Aggro", (Transition<string> transition) => this.canSeePlayer, null, null, false);
			this.fsm.AddTransition("Aggro", "Patrol", (Transition<string> transition) => this.targets.Count < 1, null, null, false);
			this.fsm.AddTransition("Aggro", "Attack", (Transition<string> transition) => this.attack, null, null, false);
			this.fsm.AddTransition("Attack", "Aggro", (Transition<string> transition) => !this.attack, null, null, false);
			this.fsm.AddTransition("Attack", "Patrol", (Transition<string> transition) => this.targets.Count < 1, null, null, false);
			this.fsm.Init();
			this.fsminit = true;
		}
	}

	// Token: 0x06001376 RID: 4982 RVA: 0x00052180 File Offset: 0x00050380
	private void Update()
	{
		if (this.fsminit && this.Alive)
		{
			this.fsm.OnLogic();
		}
		if (this.WraithAgent.velocity.magnitude < 0.2f)
		{
			this.WraithAni.SetBool("walk", false);
			this.WraithAni.SetBool("run", false);
			return;
		}
		if (this.WraithAgent.velocity.magnitude < 1.2f)
		{
			this.WraithAni.SetBool("run", false);
			this.WraithAni.SetBool("walk", true);
			return;
		}
		if (this.WraithAgent.velocity.magnitude > 1f)
		{
			this.WraithAni.SetBool("run", true);
			this.WraithAni.SetBool("walk", false);
		}
	}

	// Token: 0x06001377 RID: 4983 RVA: 0x0005225E File Offset: 0x0005045E
	public void SetInitialNodeIndex(int ind)
	{
		this.NodeIndex = ind;
	}

	// Token: 0x06001378 RID: 4984 RVA: 0x00052268 File Offset: 0x00050468
	private void PatrolLogic()
	{
		this.WraithAgent.speed = 1f;
		if (Vector3.Distance(base.transform.position, this.NodeList[this.NodeIndex].position) < 4f)
		{
			if (this.NodeIndex < this.NodeList.Length - 1)
			{
				this.NodeIndex++;
			}
			else
			{
				this.NodeIndex = 0;
			}
		}
		this.playAgroSound = true;
		this.WraithAgent.SetDestination(this.NodeList[this.NodeIndex].position);
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x000522FC File Offset: 0x000504FC
	private void PatrolWithTargetsLogic()
	{
		this.WraithAgent.speed = 1.5f;
		this.canSeePlayer = false;
		foreach (Collider collider in this.targets)
		{
			this.Target = collider.gameObject;
			Vector3 normalized = (this.Target.transform.position - base.transform.position).normalized;
			float num = Vector3.Distance(this.Target.transform.position, base.transform.position);
			if (!Physics.Raycast(base.transform.position, normalized, num, 8))
			{
				this.canSeePlayer = true;
				break;
			}
		}
		if (!this.canSeePlayer)
		{
			if (Vector3.Distance(base.transform.position, this.NodeList[this.NodeIndex].position) < 4f)
			{
				if (this.NodeIndex < this.NodeList.Length - 1)
				{
					this.NodeIndex++;
				}
				else
				{
					this.NodeIndex = 0;
				}
			}
			this.WraithAgent.SetDestination(this.NodeList[this.NodeIndex].position);
		}
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x00052450 File Offset: 0x00050650
	private void AggroLogic()
	{
		this.WraithAgent.speed = 6f;
		this.WraithAgent.SetDestination(this.Target.transform.position);
		if (Vector3.Distance(this.Target.transform.position, base.transform.position) < 2.5f)
		{
			this.WraithAgent.SetDestination(base.transform.position);
			if (Time.time - this.CDTimer > 2f)
			{
				this.attack = true;
				this.CDTimer = Time.time;
			}
		}
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x000524EC File Offset: 0x000506EC
	private void AttackLogic()
	{
		this.WraithAgent.speed = 0f;
		Vector3 vector = this.Target.transform.position - base.transform.position;
		vector.y = 0f;
		Quaternion quaternion = Quaternion.LookRotation(vector);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, Time.deltaTime * 5f);
		if (this.attack)
		{
			this.attack = false;
			base.StartCoroutine(this.SwordSwing());
		}
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x00052580 File Offset: 0x00050780
	private IEnumerator SwordSwing()
	{
		this.WraithAni.SetBool("attack", true);
		yield return null;
		this.WraithAni.SetBool("attack", false);
		yield break;
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x0005258F File Offset: 0x0005078F
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("poop");
			this.targets.Add(other);
		}
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x000525B4 File Offset: 0x000507B4
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("abba");
			this.targets.Remove(other);
		}
	}

	// Token: 0x0600137F RID: 4991 RVA: 0x000525DA File Offset: 0x000507DA
	public void HitMonster(float Damage)
	{
		Debug.Log(Damage);
		this.Health -= Damage;
		float health = this.Health;
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x00052689 File Offset: 0x00050889
	public virtual void NetworkInitialize___Early()
	{
		if (this.NetworkInitialize___EarlyCastleWraithControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize___EarlyCastleWraithControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600138D RID: 5005 RVA: 0x0005269C File Offset: 0x0005089C
	public virtual void NetworkInitialize__Late()
	{
		if (this.NetworkInitialize__LateCastleWraithControllerAssembly-CSharp.dll_Excuted)
		{
			return;
		}
		this.NetworkInitialize__LateCastleWraithControllerAssembly-CSharp.dll_Excuted = true;
	}

	// Token: 0x0600138E RID: 5006 RVA: 0x000526AF File Offset: 0x000508AF
	public override void NetworkInitializeIfDisabled()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x0600138F RID: 5007 RVA: 0x000526AF File Offset: 0x000508AF
	public virtual void Awake()
	{
		this.NetworkInitialize___Early();
		this.NetworkInitialize__Late();
	}

	// Token: 0x04000B66 RID: 2918
	private NavMeshAgent WraithAgent;

	// Token: 0x04000B67 RID: 2919
	public Animator WraithAni;

	// Token: 0x04000B68 RID: 2920
	private List<Collider> targets = new List<Collider>();

	// Token: 0x04000B69 RID: 2921
	private StateMachine fsm;

	// Token: 0x04000B6A RID: 2922
	public Transform[] NodeList;

	// Token: 0x04000B6B RID: 2923
	private int NodeIndex;

	// Token: 0x04000B6C RID: 2924
	private bool fsminit;

	// Token: 0x04000B6D RID: 2925
	private float CDTimer;

	// Token: 0x04000B6E RID: 2926
	private GameObject Target;

	// Token: 0x04000B6F RID: 2927
	private bool playAgroSound = true;

	// Token: 0x04000B70 RID: 2928
	private float Health = 15f;

	// Token: 0x04000B71 RID: 2929
	private bool Alive = true;

	// Token: 0x04000B72 RID: 2930
	private bool attack;

	// Token: 0x04000B73 RID: 2931
	private bool canSeePlayer;

	// Token: 0x04000B74 RID: 2932
	private bool NetworkInitialize___EarlyCastleWraithControllerAssembly-CSharp.dll_Excuted;

	// Token: 0x04000B75 RID: 2933
	private bool NetworkInitialize__LateCastleWraithControllerAssembly-CSharp.dll_Excuted;
}
