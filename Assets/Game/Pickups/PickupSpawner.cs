﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : Spawner {

    [SerializeField]
    [Range(0.1f, 5f)]
    private float _spawnChanceDelay = 1f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _spawnChance = .1f;

    protected override bool CanSpawn(GameObject template)
    {
        return true;
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnChanceDelay * (2.5f - (WibblyWobbly.TimeSpeed)) * .6f);
            if (GameOver.IsEnded())
                yield break;

            if (Random.Range(0f, 1f) < _spawnChance)
            {
                Spawn();
            }
        }
    }
}
