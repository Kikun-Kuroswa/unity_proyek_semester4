using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class soldierSpawnScript : MonoBehaviour
{
    private struct QueuedUnit
    {
        public GameObject prefab;
        public float trainingTime;
        public int unitTypeIndex; // 1 = Soldier, 2 = Assassin, 3 = Ranged, 4 = Armored
    }

    [Header("Soldier Prefabs")]
    public GameObject soldierType1Prefab;
    public GameObject soldierType2Prefab;
    public GameObject rangedSoldierPrefab;
    public GameObject armoredSoldierPrefab;

    [Header("Placement & Organization")]
    public Transform spawnLocation;
    public Transform hierarchyGroupParent;

    [Header("Unit Costs")]
    public int soldier1Cost = 10;
    public int soldier2Cost = 25;
    public int rangedCost = 40;
    public int armoredCost = 65;

    [Header("Queue Settings & Training Times")]
    public float soldier1Time = 1f;
    public float soldier2Time = 2f;
    public float rangedTime = 2.5f;
    public float armoredTime = 3f;
    
    [Header("Queue Counter UI Text Elements")]
    public TextMeshProUGUI soldier1QueueText;
    public TextMeshProUGUI soldier2QueueText;
    public TextMeshProUGUI rangedQueueText;
    public TextMeshProUGUI armoredQueueText;

    [Header("Progress Bar UI Elements")]
    [Tooltip("Drag your Canvas 'QueueBar' Slider here.")]
    public Slider queueProgressBar;

    [Tooltip("Drag your 'Deploy_txt' TextMeshPro component here (Shows Empty / Preparing).")]
    public TextMeshProUGUI deployStatusText;

    [Tooltip("Drag your standalone unit cap text object here (Shows Units: X / 12).")]
    public TextMeshProUGUI unitLimitText;

    [Header("Global Unit Caps")]
    public int maxPlayerUnits = 12;

    private Queue<QueuedUnit> trainingQueue = new Queue<QueuedUnit>();
    private float trainingTimer = 0f;
    private bool isTraining = false;

    private int soldier1WaitingCount = 0;
    private int soldier2WaitingCount = 0;
    private int rangedWaitingCount = 0;
    private int armoredWaitingCount = 0;

    private moneyExpScript economyManager;

    void Start()
    {
        economyManager = Object.FindAnyObjectByType<moneyExpScript>();
        
        // Keep progress bar visible but empty at start
        if (queueProgressBar != null)
        {
            queueProgressBar.gameObject.SetActive(true);
            queueProgressBar.value = 0f;
        }

        // Initialize both UI displays
        UpdateDeployStatusText();
        UpdateUnitLimitText();
        UpdateQueueCounterUI();
    }

    void Update()
    {
        HandleQueueProcessing();
        
        // Always track active battlefield units so the text updates when they die
        UpdateUnitLimitText();
    }

    private void HandleQueueProcessing()
    {
        if (trainingQueue.Count > 0)
        {
            if (!isTraining)
            {
                isTraining = true;
                trainingTimer = 0f;

                // Change status text directly to Preparing
                if (deployStatusText != null)
                {
                    deployStatusText.text = "Preparing...";
                }
            }

            QueuedUnit currentUnit = trainingQueue.Peek();
            trainingTimer += Time.deltaTime;

            if (queueProgressBar != null)
            {
                queueProgressBar.value = trainingTimer / currentUnit.trainingTime;
            }

            if (trainingTimer >= currentUnit.trainingTime)
            {
                if (GetCurrentPlayerUnitCount() < maxPlayerUnits)
                {
                    Spawn(currentUnit.prefab);
                }
                else
                {
                    Debug.LogWarning("Player hit the 12-unit maximum field limit! Spawn canceled.");
                }

                trainingQueue.Dequeue();
                DecrementWaitingCount(currentUnit.unitTypeIndex);
                UpdateQueueCounterUI();

                trainingTimer = 0f;
                isTraining = false;
                
                // Immediately calculate status for the next frame
                UpdateDeployStatusText();
            }
        }
        else
        {
            // Reset slider and check if status text needs to return to "Empty"
            if (isTraining || (queueProgressBar != null && queueProgressBar.value > 0f))
            {
                if (queueProgressBar != null)
                {
                    queueProgressBar.value = 0f;
                }
                UpdateDeployStatusText();
            }
            isTraining = false;
        }
    }

    /// <summary>
    /// Updates the deployment text under the bar to show Empty or Preparing.
    /// </summary>
    private void UpdateDeployStatusText()
    {
        if (deployStatusText == null) return;

        if (trainingQueue.Count > 0)
        {
            deployStatusText.text = "Preparing...";
        }
        else
        {
            deployStatusText.text = "Empty";
        }
    }

    /// <summary>
    /// Updates your brand new standalone field limit tracker object.
    /// </summary>
    private void UpdateUnitLimitText()
    {
        if (unitLimitText == null) return;

        int currentFieldCount = GetCurrentPlayerUnitCount();
        unitLimitText.text = "Units: " + currentFieldCount + " / " + maxPlayerUnits;
    }

    // --- BUTTON TRIGGER FUNCTIONS ---

    public void SpawnSoldierType1()
    {
        if (VerifyAndChargeFunds(soldier1Cost, "Soldier"))
        {
            QueuedUnit unit = new QueuedUnit { prefab = soldierType1Prefab, trainingTime = soldier1Time, unitTypeIndex = 1 };
            trainingQueue.Enqueue(unit);
            soldier1WaitingCount++;
            UpdateQueueCounterUI();
        }
    }

    public void SpawnSoldierType2()
    {
        if (VerifyAndChargeFunds(soldier2Cost, "Assassin"))
        {
            QueuedUnit unit = new QueuedUnit { prefab = soldierType2Prefab, trainingTime = soldier2Time, unitTypeIndex = 2 };
            trainingQueue.Enqueue(unit);
            soldier2WaitingCount++;
            UpdateQueueCounterUI();
        }
    }

    public void SpawnRangedUnit()
    {
        if (VerifyAndChargeFunds(rangedCost, "Ranged Unit"))
        {
            QueuedUnit unit = new QueuedUnit { prefab = rangedSoldierPrefab, trainingTime = rangedTime, unitTypeIndex = 3 };
            trainingQueue.Enqueue(unit);
            rangedWaitingCount++;
            UpdateQueueCounterUI();
        }
    }

    public void SpawnArmoredUnit()
    {
        if (VerifyAndChargeFunds(armoredCost, "Armored Unit"))
        {
            QueuedUnit unit = new QueuedUnit { prefab = armoredSoldierPrefab, trainingTime = armoredTime, unitTypeIndex = 4 };
            trainingQueue.Enqueue(unit);
            armoredWaitingCount++;
            UpdateQueueCounterUI();
        }
    }

    // --- UTILITY METHODS ---

    private bool VerifyAndChargeFunds(int cost, string unitName)
    {
        if (economyManager == null) return true;

        if (economyManager.GetCurrentMoney() >= cost)
        {
            economyManager.DeductMoney(cost);
            return true;
        }
        
        Debug.Log("Not enough money to buy " + unitName + "!");
        return false;
    }

    private int GetCurrentPlayerUnitCount()
    {
        return GameObject.FindGameObjectsWithTag("soldier").Length;
    }

    private void DecrementWaitingCount(int unitTypeIndex)
    {
        if (unitTypeIndex == 1) soldier1WaitingCount--;
        else if (unitTypeIndex == 2) soldier2WaitingCount--;
        else if (unitTypeIndex == 3) rangedWaitingCount--;
        else if (unitTypeIndex == 4) armoredWaitingCount--;
    }

    private void UpdateQueueCounterUI()
    {
        SetTextAndVisibility(soldier1QueueText, soldier1WaitingCount);
        SetTextAndVisibility(soldier2QueueText, soldier2WaitingCount);
        SetTextAndVisibility(rangedQueueText, rangedWaitingCount);
        SetTextAndVisibility(armoredQueueText, armoredWaitingCount);
    }

    private void SetTextAndVisibility(TextMeshProUGUI element, int count)
    {
        if (element != null)
        {
            if (count > 0)
            {
                element.text = count.ToString();
                element.gameObject.SetActive(true);
            }
            else
            {
                element.gameObject.SetActive(false);
            }
        }
    }

    private void Spawn(GameObject prefabToSpawn)
    {
        if (prefabToSpawn == null || spawnLocation == null) return;

        GameObject newSoldier = Instantiate(prefabToSpawn, spawnLocation.position, spawnLocation.rotation);
        if (hierarchyGroupParent != null)
        {
            newSoldier.transform.SetParent(hierarchyGroupParent);
        }
    }
}