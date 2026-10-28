using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PitManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] PitConfig config;
    [SerializeField] Match3Config rarityColors;
    [SerializeField] int[] debugReelCounts = new int[4];   // [White, Blue, Purple, Legendary], only when pressing Play directly in this scene

    [Header("Scene")]
    [SerializeField] PitBullet bulletPrefab;
    [SerializeField] BoxCollider spawnBox;
    [SerializeField] Transform spawnParent;
    [SerializeField] PitPlayer player;
    [SerializeField] PitLoadout loadout;
    [SerializeField] CylinderView cylinderView;

    [Header("UI")]
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text statusText;
    [SerializeField] InputActionReference confirm;

    [Header("Rules")]
    [Tooltip("Off: the clock keeps running while the cylinder screen is open. (The right-click details page always pauses.)")]
    [SerializeField] bool pauseTimerWhileCylinderOpen = false;

    public bool Active { get; private set; }

    float timeLeft;
    System.Random rng;

    void OnEnable() { confirm.action.Enable(); confirm.action.performed += OnConfirm; }
    void OnDisable() { confirm.action.performed -= OnConfirm; }
    void OnConfirm(InputAction.CallbackContext _) { if (Active && !cylinderView.DetailOpen) Finish(false); }

    void Start()
    {
        rng = new System.Random();
        var run = GameManager.Instance.Run;

        if (!config.HasAny)
        {
            statusText.text = "No Bullet Definitions in the Pit Config!";
            Debug.LogError("PitManager: the Pit Config's Bullets list is empty. Create Bullet Definitions and add them to that list.");
            return;
        }

        // Roll the pit once per loop from the slot odds.
        if (run.PitPool == null || run.PitBullets == null)
        {
            float[] pct;
            int size;
            if (run.PitPercentages != null) { pct = run.PitPercentages; size = run.PitSize; }
            else { pct = rarityColors.PitChances(debugReelCounts); size = config.fallbackPitSize; }

            run.PitPool = PitOdds.RollPool(pct, size, rng);
            run.PitBullets = new List<BulletInstance>();

            var substituted = new HashSet<Rarity>();
            foreach (var r in run.PitPool)
            {
                var def = config.PickWithFallback(r, rng, out var used);
                if (def == null) continue;
                if (used != r) substituted.Add(r);
                run.PitBullets.Add(BulletInstance.FromDefinition(config, def));
            }

            foreach (var r in substituted)
                Debug.LogWarning($"PitManager: no usable {r} Bullet Definition (check the list and Spawn Weight). " +
                                 $"Those bullets were replaced by the nearest rarity that has one.");
        }

        run.Loadout = new List<BulletInstance>();
        loadout.Refresh();
        timerText.text = Mathf.CeilToInt(config.timeLimit).ToString();
        StartCoroutine(Flow(run));
    }

    IEnumerator Flow(RunState run)
    {
        statusText.text = "Filling the pit...";
        var list = run.PitBullets;
        for (int i = 0; i < list.Count; i += config.bulletsPerBatch)
        {
            int end = Mathf.Min(list.Count, i + config.bulletsPerBatch);
            for (int k = i; k < end; k++) SpawnOne(list[k]);
            yield return new WaitForSeconds(config.batchInterval);
        }
        yield return new WaitForSeconds(config.settleTime);

        timeLeft = config.timeLimit;
        Active = true;
        statusText.text = $"Pick up to {config.chambers} bullets   (TAB: cylinder)";
    }

    void SpawnOne(BulletInstance data)
    {
        var b = spawnBox.bounds;
        var pos = new Vector3(Random.Range(b.min.x, b.max.x), Random.Range(b.min.y, b.max.y), Random.Range(b.min.z, b.max.z));
        var view = Instantiate(bulletPrefab, pos, Random.rotation, spawnParent);
        view.Setup(data, rarityColors.Tint(data.Rarity));
    }

    void Update()
    {
        if (!Active) return;
        if (pauseTimerWhileCylinderOpen && cylinderView.IsOpen) return;

        timeLeft -= Time.deltaTime;   // already frozen by Time.timeScale = 0 during the detail page
        timerText.text = Mathf.Max(0, Mathf.CeilToInt(timeLeft)).ToString();
        timerText.color = timeLeft <= 5f ? new Color(1f, 0.35f, 0.3f) : Color.white;
        if (timeLeft <= 0f) Finish(true);
    }

    void Finish(bool timeUp)
    {
        Active = false;
        if (cylinderView.IsOpen) cylinderView.Close();   // also restores Time.timeScale
        player.Locked = true;

        var picked = new List<BulletInstance>(loadout.Loaded);
        if (picked.Count == 0)
        {
            // Picked nothing: give the lightest bullet YOU defined, so the duel is never empty.
            var def = config.PickLightest(rng);
            if (def != null) picked.Add(BulletInstance.FromDefinition(config, def));
        }

        GameManager.Instance.Run.Loadout = picked;
        statusText.text = timeUp ? "Time's up!" : "Locked in!";
        StartCoroutine(Leave());
    }

    IEnumerator Leave()
    {
        yield return new WaitForSeconds(1.2f);
        GameManager.Instance.Run.Advance();
        SceneLoader.Instance.Load("Hub");
    }
}