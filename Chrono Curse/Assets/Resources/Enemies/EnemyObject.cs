using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ChronoCurse/Enemy")]

public class EnemyObject : ScriptableObject
{
    [SerializeField] private int _health;
    [SerializeField] private float _speed;
    [SerializeField] private Sprite _sprite;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _attackRate;
    [SerializeField] private float _nextAttackTime;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private RuntimeAnimatorController _animatorController;
    [SerializeField] private Enemy.EnemyType _enemyType;

    public int health { get { return _health; } set { _health = value; } }
    public float speed { get { return _speed; } set { _speed = value; } }

    public float attackRate { get { return _attackRate; } set { _attackRate = value; } }

    public float attackRange { get { return _attackRange; } set { _attackRange = value; } }

    public float nextAttackTime { get { return _nextAttackTime; } set { _nextAttackTime = value; } }
    public RuntimeAnimatorController animatorController { get { return _animatorController; } set { _animatorController = value; } }

    public Sprite sprite { get { return _sprite; } set { _sprite = value; } }

    public Enemy.EnemyType enemyType { get { return _enemyType; } set { _enemyType = value; } }

}