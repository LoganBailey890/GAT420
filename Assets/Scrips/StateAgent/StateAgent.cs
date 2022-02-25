using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAgent : Agent
{
    [SerializeField] public Perception perception;

    public PathFollower path;
    public StateMachine stateMachine = new StateMachine();
     

    public BoolRef enemySeen;
    public FloatRef enemeyDistance;
    public FloatRef health;
    public FloatRef timer;
    public BoolRef atDestination;


    public GameObject enemy { get; set; }

    void Start()
    {

        stateMachine.AddState(new IdelState(this, typeof(IdelState).Name));
        stateMachine.AddState(new PatrolState(this, typeof(PatrolState).Name));
        stateMachine.AddState(new ChaseState(this, typeof(ChaseState).Name));
        stateMachine.AddState(new DeathState(this, typeof(DeathState).Name));
        stateMachine.AddState(new AttackState(this, typeof(AttackState).Name));
        stateMachine.AddState(new EvadeState(this, typeof(EvadeState).Name));
        stateMachine.AddState(new RoamState(this, typeof(RoamState).Name));

        stateMachine.AddTransition(typeof(IdelState).Name, new Transition(new Condition[] { new FloatCondition(timer, Condition.Predicate.LESS, 0) }), typeof(PatrolState).Name);
        stateMachine.AddTransition(typeof(IdelState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);
        stateMachine.AddTransition(typeof(IdelState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 30), new BoolCondition(enemySeen,true) }), typeof(EvadeState).Name);
        stateMachine.AddTransition(typeof(IdelState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.GREATER, 30), new BoolCondition(enemySeen,true) }), typeof(ChaseState).Name);

        stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);
        stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, true) }), typeof(ChaseState).Name);
        stateMachine.AddTransition(typeof(PatrolState).Name, new Transition(new Condition[] { new FloatCondition(timer, Condition.Predicate.LESS_EQUAL, 0) }), typeof(RoamState).Name);

        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen, false) }), typeof(IdelState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new FloatCondition(enemeyDistance, Condition.Predicate.LESS_EQUAL, 2) }), typeof(AttackState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.GREATER, 30) }), typeof(ChaseState).Name);
        stateMachine.AddTransition(typeof(ChaseState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 20), new BoolCondition(enemySeen, true) }), typeof(EvadeState).Name);


        stateMachine.AddTransition(typeof(AttackState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 30) }), typeof(EvadeState).Name);
        stateMachine.AddTransition(typeof(AttackState).Name, new Transition(new Condition[] { new FloatCondition(timer, Condition.Predicate.LESS_EQUAL,0) }), typeof(ChaseState).Name);

        stateMachine.AddTransition(typeof(EvadeState).Name, new Transition(new Condition[] { new BoolCondition(enemySeen,false) }), typeof(IdelState).Name);
        stateMachine.AddTransition(typeof(EvadeState).Name, new Transition(new Condition[] { new FloatCondition(health, Condition.Predicate.LESS_EQUAL, 0) }), typeof(DeathState).Name);

        stateMachine.AddTransition(typeof(RoamState).Name, new Transition(new Condition[] { new BoolCondition(atDestination, true) }), typeof(IdelState).Name);

        stateMachine.SetState(stateMachine.StateFromName(typeof(IdelState).Name));
    }


    void Update()
    {
        var enemies = perception.GetGameObjects();
        enemySeen.value = (enemies.Length != 0);
        enemy = (enemies.Length != 0) ? enemies[0] : null;
        enemeyDistance.value = (enemy != null) ? (Vector3.Distance(transform.position, enemy.transform.position)) : float.MaxValue;

        timer.value -= Time.deltaTime;

        stateMachine.Update();

        animator.SetFloat("Speed", movement.velocity.magnitude);
        
    }

    private void OnGui()
    {
        Vector2 screen = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Label(new Rect(screen.x, Screen.height - screen.y, 300, 20), stateMachine.GetStateName());
    }

}
