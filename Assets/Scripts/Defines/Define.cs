using Define.DesignPatterns;
using Define.Dirrect.States;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEditorInternal;
using UnityEngine;
namespace Define
{
	namespace DesignPatterns
	{
		public class SingleTon<T> where T : new()
        {
			private T instance;

			public T GetInstance
			{
				get 
				{
					if (instance == null)
                    {
                        Init();
                        instance = new T();
                    }
					return instance;
				}
			}

            public virtual void Init()
            {

            }

			public void Release()
			{
				instance = default(T);
			}
		}

		public interface IState
		{
			void Enter();
			void Execute();
			void Exit();
        }

    }
	namespace Data
	{
		public class Charactors
		{
            public string name;
            private string dataName;//datapool에서 필요한 이름
			public string GetDataName { get { return dataName; } }
            public sbyte hp;
			public sbyte mana;
			public sbyte skillPoints;//
			public sbyte ad;//attackDamage
			public sbyte ap;//abilityPower
			public Charactors(string name, string dataName, sbyte hp, sbyte mana, sbyte skillPoints, sbyte ad, sbyte ap)
            {
                this.name = name;
                this.dataName = dataName;
                this.hp = hp;
                this.mana = mana;
                this.skillPoints = skillPoints;
                this.ad = ad;
                this.ap = ap;
            }
        }
		public enum charactorJob
		{
			warriror,archer,oracle
		}
    }
	namespace Dirrect
	{

        public class Actors
        {
			private SpriteRenderer sr;
			private Animator anim;
            private string srName;
            StateParent[] states;
            StateParent curr;

            public Actors(string srName)
            {
                this.srName = srName;
                // TODO : anim,sr 등은 리소스메니저 구현 후 만들어야함
                states = new StateParent[5]
                {
                    new IdleState(this,0),new MoveState(this,0),new AttackState(this,0),new DamagedState(this,0),new DieState(this,0)
                };
                curr = states[0];
            }

			public void StateUpdate()
			{
                curr.Execute();
			}
            public void ChangeAnim(StateType sName)
            {
                anim.Play($"{srName}_{sName.ToString()}");
            }
            public void ChangeState(StateType nextState)
            {
                curr.Exit();
                if (curr.type == StateType.Die) return;
                curr = states.Where(x => x.type == nextState).First();
                curr.Enter();
            }

        }
		namespace States
		{
            public enum StateType { Idle,Move,Attack, Damage, Die}

            public class StateParent : IState
            {

                public StateType type;
                protected Actors actor;
                protected float currTime;
                protected float goal;
                public StateParent(Actors actor, float goal)
                {
                    this.actor = actor;
                    this.currTime = 0;
                    this.goal = goal;
                }

                public virtual void Enter()
                {
                    actor.ChangeAnim(type);
                }
                public virtual void Execute()
                {
                    currTime += Time.deltaTime;
                    if (goal <= currTime) actor.ChangeState(StateType.Idle);
                }
                public virtual void Exit()
                {
                    currTime = 0f;
                }
            }
            public class IdleState : StateParent
            {
                public IdleState(Actors actor, float goal) : base(actor, goal)
                {
                    this.type = StateType.Idle;
                    this.actor = actor;
                    this.goal = goal;
                }
            }
            public class MoveState : StateParent
            {
                public MoveState(Actors actor, float goal) : base(actor, goal)
                {
                    this.type = StateType.Move;
                    this.actor = actor;
                    this.goal = goal;
                }
            }
            public class AttackState : StateParent
            {
                public AttackState(Actors actor, float goal) : base(actor, goal)
                {
                    this.type = StateType.Attack;
                    this.actor = actor;
                    this.goal = goal;
                }
            }
            public class DamagedState : StateParent
            {
                public DamagedState(Actors actor, float goal) : base(actor, goal)
                {
                    this.type = StateType.Damage;
                    this.actor = actor;
                    this.goal = goal;
                }
            }

            public class DieState : StateParent
            {
                public DieState(Actors actor, float goal) : base(actor, goal)
                {
                    this.type = StateType.Die;
                    this.actor = actor;
                    this.goal = goal;
                }
                public override void Execute()
                {
                    
                }
            }
        }

    }
}
