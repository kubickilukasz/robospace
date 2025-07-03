using UnityEngine;
using Kubeec.General;
using System.Collections.Generic;
using Kubeec.VR.Interactions;
using Kubeec.VR.Player;
using Kubeec.NPC;
using Kubeec.Network;
using Unity.Netcode;
using Kubeec.NPC.LazyNavigation;

public class ShootSceneController : EnableDisableInitableDisposable{

    [SerializeField] ScreenHUD screen;
    [SerializeField] ZoneController zoneController;
    [SerializeField] Platform platform;
    [SerializeField] Collider triggerColliderButtonUp;
    [SerializeField] InteractionSimpleButton buttonStart;
    [SerializeField] List<Transform> spawnTransforms = new();
    [SerializeField] List<WaveDefinition> waves = new();

    bool isDuringWave => enemies.Count > 0;

    int currentWaveIndex = 0;
    int currentWaveNumber = 1;
    WaveDefinition currentWave;
    LocalPlayerReference player; //todo what if more players
    NetworkObjectPool pool;
    List<PooledNPC> enemies = new List<PooledNPC>();

    public void StartWave() {
        if (isDuringWave) {
            return;
        }
        screen.SetText(currentWaveNumber.ToString());
        triggerColliderButtonUp.enabled = false;
        currentWaveIndex = Mathf.Clamp(currentWaveIndex, 0, waves.Count - 1);
        currentWave = waves[currentWaveIndex];
        OnStartWave(currentWave);
    }

    public void ResetWaves() {
        Debug.Log($"Reset waves");
        ClearEnemies();
        currentWaveIndex = 0;
        currentWaveNumber = 1;
        triggerColliderButtonUp.enabled = true;
        screen.SetText(" ");
    }

    void OnStartWave(WaveDefinition wave) {
        for (int i = 0; i < wave.numberEnemies; i++) {
            int t = i % spawnTransforms.Count;
            Transform tr = spawnTransforms[t];
            NetworkObject go = pool.GetNetworkObject(wave.enemy, tr.position, tr.rotation);
            PooledNPC npc = go.GetComponent<PooledNPC>();
            NonPlayerCharacter nonPlayerCharacter = npc.NonPlayerCharacter;
            nonPlayerCharacter.Behaviour.ZoneController = zoneController;
            nonPlayerCharacter.Init();
            npc.Init(wave.enemy.gameObject);
            npc.onDestruct += OnEnemyDeath;
            enemies.Add(npc);
        }
        Debug.Log($"Start Wave {enemies.Count}");
    }

    void ClearEnemies() {
        foreach (PooledNPC nonPlayerCharacter in enemies) {
            nonPlayerCharacter.onDestruct -= OnEnemyDeath;
            nonPlayerCharacter.Destruct();
        }
        enemies.Clear();
    }

    void OnEnemyDeath(PooledNPC pooledNPC) {
        Debug.Log("Enemy killed");
        pooledNPC.onDestruct -= OnEnemyDeath;
        if (enemies.Contains(pooledNPC)) {
            enemies.Remove(pooledNPC);
        }
        if (enemies.Count == 0) {
            OnEndWave();
        }
    }

    void OnEndWave() {
        Debug.Log("End Wave");
        currentWaveIndex++;
        currentWaveNumber++;
        platform.Up();
    }

    protected override void OnInit(object data) {
        ResetWaves();
        buttonStart.onActive += StartWave;
        pool = NetworkObjectPool.Singleton;
    }

    protected override void OnDispose() {
        ClearEnemies();
        if (buttonStart) {
            buttonStart.onActive -= StartWave;
        }
    }

    [System.Serializable]
    public class WaveDefinition {
        public GameObject enemy;
        public int numberEnemies = 1;
    }

}
