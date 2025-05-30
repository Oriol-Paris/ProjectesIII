using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerBase : MonoBehaviour
{
    public PlayerData playerData; // Reference to the ScriptableObject containing player data

    public enum ActionEnum { MOVE, SHOOT,ESPECIALSHOOT, HEAL, MELEE, RECOVERY, SPEED_UP, MAX_HP_INCREASE, NOTHING };
    public enum ActionType { ACTIVE, PASSIVE, SINGLE_USE };

    [System.Serializable]
    public struct Action
    {
        public Action(ActionType type, ActionEnum action, KeyCode key, int cost, BulletStyle style = null)
        {
            m_action = action;
            m_key = key;
            m_style = style;
            m_cost = cost;
        }

        public static Action nothing { get { return new Action(ActionType.ACTIVE, ActionEnum.NOTHING, KeyCode.None, 0); } }

        public ActionEnum m_action { get; private set; }
        public KeyCode m_key { get; private set; }
        public BulletStyle m_style { get; private set; }

        public int m_cost { get; private set; }

        public void ChangeKey(KeyCode newKey) { m_key = newKey; }
    }

    #region VARIABLES

    [SerializeField] private cameraManager _camera;
    private float cameraPostProcesLength = 0.5f;
    private float cameraPostProcesIntensity = 0.3f;
    public BulletStyle activeStyle { get; private set; }

    public float health = 5;
    public float maxHealth;
    public float actionPoints;
    public float maxActionPoints;
    public float range;
    public float exp = 0;

    public bool godMode = false;
    
    public PlayerActionManager turnsDone;

    public Action activeAction { get; private set; }
    public List<Action> availableActions = new List<Action>();

    private float hitFeedBackTime = 0.3f;
    [SerializeField] TimeSecuence isExecuting;
    private bool isInAction;
    private bool isAlive;
    public bool victory;
    public bool defeat;
    [SerializeField] AudioClip[] damageClips;
    [SerializeField] GameObject bloodSplash;
    [SerializeField] GameObject aro;
    [SerializeField] private UIBarManager healthManager;

    #endregion

    private void Awake()
    {
        // Load saved data before Start() is called
        PlayerData loadedData = PlayerData.LoadFromDisk();
        if (loadedData != null)
        {
            // Copy the loaded data to our scriptable object
            playerData.health = loadedData.health;
            playerData.maxHealth = loadedData.maxHealth;
            playerData.actionPoints = loadedData.actionPoints;
            playerData.maxActionPoints = loadedData.maxActionPoints;
            playerData.maxTime = loadedData.maxTime;
            playerData.lastLevel = loadedData.lastLevel;
            playerData.levelCompleted = loadedData.levelCompleted;
            playerData.timesHealed = loadedData.timesHealed;
            playerData.timesIncreasedMaxHP = loadedData.timesIncreasedMaxHP;
            playerData.moveRange = loadedData.moveRange;
            playerData.exp = loadedData.exp;
            playerData.isAlive = loadedData.isAlive;
            playerData.victory = loadedData.victory;
            playerData.healAmount = loadedData.healAmount;

            // Clear and copy actions
            playerData.availableActions.Clear();
            foreach (var action in loadedData.availableActions)
            {
                playerData.availableActions.Add(action);
            }
        }
        else
        {
            // If no saved data exists, this might be the first time playing
            playerData.SaveOriginalPlayerIfNotExists();
        }
    }

    void Start()
    {
        if (playerData.health <= 0)
        {
            playerData.health = playerData.maxHealth;
        }
        LoadPlayerData();

        if (availableActions.Count > 0)
        {
            activeAction = availableActions[0];
        }

        isInAction = false;
        turnsDone = GetComponent<PlayerActionManager>();
        
        

        // Only save level start state if this is a new level
        if (playerData.lastLevel != SceneManager.GetActiveScene().name)
        {
            playerData.lastLevel = SceneManager.GetActiveScene().name;
            playerData.SavePlayerAtLevelStart();
        }

        this.GetComponent<ControlLiniarRender>().ChangeLineColor(activeAction);
    }




    private void LoadPlayerData()
    {
        // Load health, range, and other properties from the ScriptableObject
        maxHealth = playerData.maxHealth;
        health = playerData.health;
        actionPoints = playerData.actionPoints;
        maxActionPoints = playerData.maxActionPoints;
        exp = playerData.exp;
        isAlive = playerData.isAlive;
        victory = playerData.victory;

        // Clear existing actions before loading
        availableActions.Clear();

        // Load available actions from playerData and populate availableActions list
        foreach (var actionData in playerData.availableActions)
        {
            if (actionData.action == ActionEnum.SHOOT)
            {
                // Only get the bullet style if it hasn't been set yet
                if (actionData.style == null)
                {
                    actionData.style = FindAnyObjectByType<BulletCollection>().GetBullet(actionData.bulletType);
                }
            }

            availableActions.Add(new Action(
                actionData.actionType,
                actionData.action,
                actionData.key,
                actionData.cost,
                actionData.style
            ));
        }

        range = playerData.moveRange;
    }


    void Update()
    {
        if (godMode)
        {
            aro.SetActive(true);
        }
        else
        {
            aro.SetActive(false);
        }

        if (!victory && isAlive && !defeat)
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                godMode = !godMode;

            }
            if (!isExecuting.isExecuting)
            {
                foreach (Action action in availableActions)
                {
                    if (Input.GetKeyDown(action.m_key))
                    {
                        activeAction = action;

                        if (action.m_style != null)
                            activeStyle = action.m_style;
                    }
                }
            }

            range = activeAction.m_style != null ? activeAction.m_style.range : playerData.moveRange;

            if (activeAction.m_action == ActionEnum.HEAL)
            {
                Heal(playerData.healAmount); // Use healAmount from playerData
            }
        }
        else
        {
            activeAction = Action.nothing;
            //Debug.Log("Doing nothing");
        }

        // Check for death condition
        if (health <= 0 || playerData.health <= 0)
        {
            if (isAlive)
            {
                isAlive = false;
                StartCoroutine(DeathCoroutine());
            }
        }
        //Debug.LogWarning((float)_camera.colorPostProces.intensity);

        //if (health <= 2 && (float)_camera.colorPostProces.intensity < cameraPostProcesIntensity / 2)
        //{
        //    Debug.LogWarning("FadeIN");
        //    StartCoroutine(_camera.FadeInVignette(cameraPostProcesIntensity, cameraPostProcesLength, Color.red));
        //}

        
        //if (health > 2 && (float)_camera.colorPostProces.intensity > cameraPostProcesIntensity / 2)
        //{
        //    Debug.LogWarning("FadeOUT");
        //    StartCoroutine(_camera.Flash(cameraPostProcesIntensity, cameraPostProcesLength, Color.red));
        //}
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyMovement>() != null)
        {
            collision.gameObject.GetComponent<EnemyMovement>().Attack();
            this.GetComponent<Animator>().SetTrigger("hit");

            if (health > 0 || playerData.health > 0)
            {
                Damage(1, collision.gameObject);
            }
            else if(health<=0||playerData.health<=0) 
            {
                isAlive = false;
                Debug.Log("YOU DIED");
                StartCoroutine(DeathCoroutine());
            }
        }
    }

    IEnumerator DeathCoroutine()
    {
        this.GetComponent<Animator>().SetBool("isDead", true);
        activeAction = Action.nothing;
        isAlive = false;

        yield return new WaitForSeconds(1);
    }

    IEnumerator whitecolor()
    {
        float elapsed = 0;
        while (elapsed < hitFeedBackTime)
        {
            yield return null;
            GetComponent<SpriteRenderer>().color = new Color(1, GetComponent<SpriteRenderer>().color.b + Time.deltaTime, GetComponent<SpriteRenderer>().color.b + Time.deltaTime);


            elapsed += Time.deltaTime;
        }
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void SaveCurrentState()
    {
        playerData.Save();
    }

    public void ResetToLevelStart()
    {
        playerData.ResetToLevelStart();
        LoadPlayerData();
    }

    public void ResetToOriginal()
    {
        playerData.ResetToOriginal();
        LoadPlayerData();
    }

    private void OnApplicationQuit()
    {
        SaveCurrentState();
    }

    private void OnDestroy()
    {
        SaveCurrentState();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveCurrentState();
    }


    #region GETTERS

    public float GetRange() { return range; }
    public bool GetIsAlive() { return isAlive; }
    public Action GetAction() { return activeAction; }
    public bool GetInAction() { return isInAction; }

    public List<Action> GetAvailableActions() { return availableActions; }

    #endregion

    #region SETTERS

    public void Heal(int amount)
    {
        actionPoints -= activeAction.m_cost;
        playerData.actionPoints -= activeAction.m_cost;
        actionPoints = Mathf.Min(actionPoints, maxActionPoints);
        playerData.actionPoints = Mathf.Min(playerData.actionPoints, maxActionPoints);
        health += amount;
        playerData.health += amount;

        health = Mathf.Min(health, maxHealth);
        playerData.health = Mathf.Min(playerData.health, maxHealth);
        healthManager.UpdateHealthbar();
        activeAction = Action.nothing;
    }

    public void Rest()
    {
        actionPoints += 3;
        playerData.actionPoints += 3;

        actionPoints = Mathf.Min(actionPoints, maxActionPoints);
        playerData.actionPoints = Mathf.Min(playerData.actionPoints, maxActionPoints);

        activeAction = Action.nothing;
        turnsDone.ResetFlags(); // End the turn after resting

        SaveCurrentState();

    }

    public void Damage(int val, GameObject hitObject) 
    {
        if (!godMode)
        {


          
            health -= val;
            playerData.health -= val;

            GetComponent<SpriteRenderer>().color = Color.red;
            StartCoroutine(whitecolor());

            SoundEffectsManager.instance.PlaySoundFXClip(damageClips, transform, 1f);
            StartCoroutine(_camera.Flash(1f, 0.8f, Color.red));
            StartCoroutine(_camera.Shake(0.3f, 0.8f));
            Instantiate(bloodSplash, this.transform.position, hitObject.transform.rotation);

            // Check for death condition immediately after taking damage
            if (health <= 0 || playerData.health <= 0)
            {
                if (isAlive)
                {
                    isAlive = false;
                    StartCoroutine(DeathCoroutine());
                }
            }
            if (health <= 2/* && (float)_camera.colorPostProces.intensity <= cameraPostProcesIntensity / 2*/)
            {

                StartCoroutine(_camera.FadeInVignette(cameraPostProcesIntensity, cameraPostProcesLength, Color.red));
            }

            FindAnyObjectByType<UIBarManager>().UpdateHealthbar();
            SaveCurrentState();
        }
    }

    public void InstantHeal(int amount = 1)
    {
        playerData.health += amount;

        if(playerData.health > playerData.maxHealth)
        {
            playerData.health = playerData.maxHealth;
        }

        health = playerData.health;

        playerData.timesHealed++;
    }

    public void InstantMaxHPIncrease(int amount = 1)
    {
        playerData.maxHealth += amount;
        maxHealth += amount;
        playerData.health += amount;
        health += amount;

        playerData.timesIncreasedMaxHP++;

        SaveCurrentState();
    }

    public void ResetActiveAction() { activeAction = availableActions[0]; }
    public void SetRange(float newRange) { range = newRange; }
    public void SetInAction(bool newVal) { isInAction = newVal; }
    public void AddNewAction(Action action) { availableActions.Add(action); playerData.Save(); }
    public void DeleteAction(int index) { availableActions.RemoveAt(index); playerData.Save(); }

    #endregion

    
}