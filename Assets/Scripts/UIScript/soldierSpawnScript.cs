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
        public string unitName;   // Added to track names for the deployment status panel text
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

    [Header("Army Capacity Settings")]
    public int totalMaxUnits = 12;
    public TextMeshProUGUI unitLimitText; // RESTORED: Displays "0 / 12" capacity limits

    [Header("Deployment Visual UI Elements")]
    public Slider queueSlider;             // RESTORED: Progression fill slider bar
    public TextMeshProUGUI statusText;    // RESTORED: Displays active string state (e.g., "Training Soldier...")

    [Header("UI Spawn Buttons")]
    public Button soldier1Button;
    public Button soldier2Button;
    public Button rangedButton;
    public Button armoredButton;

    private List<QueuedUnit> trainingQueue = new List<QueuedUnit>();
    private float trainingTimer = 0f;

    private int soldier1WaitingCount = 0;
    private int soldier2WaitingCount = 0;
    private int rangedWaitingCount = 0;
    private int armoredWaitingCount = 0;

    private moneyExpScript walletScript;

    void Start()
    {
        walletScript = Object.FindAnyObjectByType<moneyExpScript>();

        if (walletScript == null)
        {
            Debug.LogError("soldierSpawnScript could not find a moneyExpScript in the scene!");
        }

        UpdateQueueCounterUI();
        UpdateUnitLimitUI();
        ResetDeploymentStatusUI();
    }

    void Update()
    {
        ManageButtonInteractability();
        UpdateUnitLimitUI(); // Keep current active army limits accurate in real-time

        // --- PROCESSING THE RECRUITMENT TRAINING QUEUE ---
        if (trainingQueue.Count > 0)
        {
            trainingTimer += Time.deltaTime;

            // 1. Get the base training parameters
            float baseTrainingTime = trainingQueue[0].trainingTime;
            float dynamicReduction = 0f;

            // 2. Process upgrade multipliers if applicable
            if (UpgradeManager.Instance != null)
            {
                dynamicReduction = UpgradeManager.Instance.deploymentUpgradeLevel * UpgradeManager.Instance.deploymentTimeReductionPerLevel;
            }

            // 3. Compute final targeted timeline requirements
            float finalTargetTrainingTime = Mathf.Max(0.2f, baseTrainingTime - dynamicReduction);

            // 4. RESTORED: Update deployment status text and fill bar ratio
            if (statusText != null)
            {
                statusText.text = "Preparing Unit...";
            }

            if (queueSlider != null)
            {
                queueSlider.value = trainingTimer / finalTargetTrainingTime;
            }

            // 5. Trigger deployment spawn upon timer completion
            if (trainingTimer >= finalTargetTrainingTime)
            {
                Spawn(trainingQueue[0].prefab);
                DecrementWaitingCount(trainingQueue[0].unitTypeIndex);
                trainingQueue.RemoveAt(0);
                trainingTimer = 0f;
                
                UpdateQueueCounterUI();

                // Reset bar parameters if queue is emptied completely
                if (trainingQueue.Count == 0)
                {
                    ResetDeploymentStatusUI();
                }
            }
        }
        else
        {
            // Fallback clear state if nothing resides in our line array balances
            ResetDeploymentStatusUI();
        }
    }

    private void ManageButtonInteractability()
    {
        int totalActiveUnitsOnField = GetActiveUnitCount();
        int unitsCurrentlyInTraining = trainingQueue.Count;
        bool isArmyCapacityFull = (totalActiveUnitsOnField + unitsCurrentlyInTraining) >= totalMaxUnits;

        if (isArmyCapacityFull)
        {
            SetAllButtonsState(false);
            return;
        }

        int currentGold = walletScript != null ? walletScript.GetCurrentMoney() : 0;

        if (soldier1Button != null) soldier1Button.interactable = currentGold >= soldier1Cost;
        if (soldier2Button != null) soldier2Button.interactable = currentGold >= soldier2Cost;
        if (rangedButton != null) rangedButton.interactable = currentGold >= rangedCost;
        if (armoredButton != null) armoredButton.interactable = currentGold >= armoredCost;
    }

    private void SetAllButtonsState(bool targetState)
    {
        if (soldier1Button != null) soldier1Button.interactable = targetState;
        if (soldier2Button != null) soldier2Button.interactable = targetState;
        if (rangedButton != null) rangedButton.interactable = targetState;
        if (armoredButton != null) armoredButton.interactable = targetState;
    }

    public void OnClickQueueSoldier1() { TryQueueUnit(soldierType1Prefab, soldier1Time, soldier1Cost, 1, "Soldier"); }
    public void OnClickQueueSoldier2() { TryQueueUnit(soldierType2Prefab, soldier2Time, soldier2Cost, 2, "Assassin"); }
    public void OnClickQueueRanged()   { TryQueueUnit(rangedSoldierPrefab, rangedTime, rangedCost, 3, "Ranged"); }
    public void OnClickQueueArmored()  { TryQueueUnit(armoredSoldierPrefab, armoredTime, armoredCost, 4, "Armored"); }

    private void TryQueueUnit(GameObject prefab, float trainingTime, int unitCost, int typeIndex, string unitName)
    {
        if (walletScript == null || prefab == null) return;

        int totalUnitsPending = GetActiveUnitCount() + trainingQueue.Count;
        if (totalUnitsPending >= totalMaxUnits)
        {
            Debug.LogWarning("Cannot queue unit: Maximum combat cap reached!");
            return;
        }

        if (walletScript.GetCurrentMoney() >= unitCost)
        {
            walletScript.DeductMoney(unitCost);

            QueuedUnit newRequest;
            newRequest.prefab = prefab;
            newRequest.trainingTime = trainingTime;
            newRequest.unitTypeIndex = typeIndex;
            newRequest.unitName = unitName;

            trainingQueue.Add(newRequest);

            IncrementWaitingCount(typeIndex);
            UpdateQueueCounterUI();
        }
        else
        {
            Debug.LogWarning("Insufficient gold balance to hire this unit type.");
        }
    }

    private void IncrementWaitingCount(int unitTypeIndex)
    {
        if (unitTypeIndex == 1) soldier1WaitingCount++;
        else if (unitTypeIndex == 2) soldier2WaitingCount++;
        else if (unitTypeIndex == 3) rangedWaitingCount++;
        else if (unitTypeIndex == 4) armoredWaitingCount++;
    }

    private int GetActiveUnitCount()
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

    // RESTORED: Keeps your structural total text numbers (like 0/12) operating dynamically 
    private void UpdateUnitLimitUI()
    {
        if (unitLimitText != null)
        {
            unitLimitText.text = "Unit Limit: " + GetActiveUnitCount() + " / " + totalMaxUnits;
        }
    }

    // RESTORED: Clears status trackers back to empty layouts cleanly
    private void ResetDeploymentStatusUI()
    {
        if (statusText != null)
        {
            statusText.text = "Empty...";
        }
        if (queueSlider != null)
        {
            queueSlider.value = 0f;
        }
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