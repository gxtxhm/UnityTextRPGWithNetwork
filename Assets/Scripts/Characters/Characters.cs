using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IGameCharacter
{
    public int Hp { get; set; }
    public int AttackPower { get; set; }
    public string Name { get;}

    event UnityAction OnDeadEvent;

    event UnityAction OnAttackEvent;

    
}