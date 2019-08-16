using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static Manager Instance;
    public static bool hasPlayed = false;

    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;
    [SerializeField] public GameObject txCounter;
    [SerializeField] private GameObject txScore;
    [SerializeField] private Image background;
    [SerializeField] private Transform eyes;
    [HideInInspector] public bool activeSnake;
    [HideInInspector] public int scoreCounter = 0;
    [SerializeField] private float speedStep;
    public float speed = 1f;

    private Queue<GameObject> bodyPartsInPool;
    public List<GameObject> spawnedbodyPart;
    private Queue<GameObject> consumablesInPool;
    [Header("Pools")]
    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private int cubesPoolAmount;
    [SerializeField] private GameObject spherePrefab;
    [SerializeField] private int spheresPoolAmount;

    private Vector3 spawn_position = new Vector3(0.0f, 0.52f, 0.0f);
    private Vector3 moveDirection = new Vector3(0,0,1);
    private float[] directions = new float[4] { 0, 180, 270, 90 };

    private bool[] wrongDirectionFlag = new bool[4];
    private bool flagMoveMade = false;

    [SerializeField] private float timer = 0.0f;

    private void Awake()
    {
        activeSnake = false;
        Instance = this;
        InstantiatePools();
    }

    void Start()
    {
        Instance = this;
        if (hasPlayed) StartGame();
    }
    void Update()
    {
        if (activeSnake)
        {
            timer += Time.deltaTime * speed;

            if (timer > 0.1)
            {
                spawn_position += moveDirection;

                Manager.Instance.spawn_cube(spawn_position, Direction());
                flagMoveMade = false;

                timer = 0.0f;
            }

            if (Input.GetKey(KeyCode.UpArrow) && !wrongDirectionFlag[1] && !flagMoveMade)
            {
                moveDirection = new Vector3(0, 0, 1f);
                wrongDirectionFlag[0] = true;
                wrongDirectionFlag[1] = false;
                wrongDirectionFlag[2] = false;
                wrongDirectionFlag[3] = false;
                flagMoveMade = true;
            }
            else if (Input.GetKey(KeyCode.DownArrow) && !wrongDirectionFlag[0] && !flagMoveMade)
            {
                moveDirection = new Vector3(0, 0, -1f);
                wrongDirectionFlag[0] = false;
                wrongDirectionFlag[1] = true;
                wrongDirectionFlag[2] = false;
                wrongDirectionFlag[3] = false;
                flagMoveMade = true;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !wrongDirectionFlag[3] && !flagMoveMade)
            {
                moveDirection = new Vector3(-1f, 0, 0);
                wrongDirectionFlag[0] = false;
                wrongDirectionFlag[1] = false;
                wrongDirectionFlag[2] = true;
                wrongDirectionFlag[3] = false;
                flagMoveMade = true;
            }
            else if (Input.GetKey(KeyCode.RightArrow) && !wrongDirectionFlag[2] && !flagMoveMade)
            {
                moveDirection = new Vector3(1f, 0, 0);
                wrongDirectionFlag[0] = false;
                wrongDirectionFlag[1] = false;
                wrongDirectionFlag[2] = false;
                wrongDirectionFlag[3] = true;
                flagMoveMade = true;
            }
        }
    }
    private float Direction()
    {
        for(int i = 0; i < 4; i++)
        {
            if (wrongDirectionFlag[i])
            {
                return directions[i];
            }
        }

        return 0;
    }
    public void StartGame()
    {
        startScreen.SetActive(false);
        txCounter.SetActive(true);

        coroutine = Counter(1, () => { txCounter.GetComponent<Text>().text = "2"; background.CrossFadeAlpha(0, 3.0f, false); }, new Action[2] {
            () => coroutine = Counter(1, () => { txCounter.GetComponent<Text>().text = "1"; }, new Action[2] {
                () => coroutine = Counter(1, () => SetupGame(), new Action[2]{
                    () => coroutine = Counter(1, () =>{  txScore.SetActive(true); } ),
                    () => StartCoroutine(coroutine)}),
                        () => StartCoroutine(coroutine)}),
                            () => StartCoroutine(coroutine)});
        StartCoroutine(coroutine);
    }
    private void SetupGame()
    {
        txCounter.SetActive(false);
        spawn_cube(spawn_position);

        Vector3 BallPosition = new Vector3(UnityEngine.Random.Range(-4, 4), 0, UnityEngine.Random.Range(-4, 4));
        BallPosition += BallPosition + new Vector3(0f, 0f, 0f);
        spawn_sphere(BallPosition);

        activeSnake = true;
        eyes.gameObject.SetActive(true);

    }
    public void SetCountText()
    {
        txScore.GetComponent<Text>().text = $"Score: {scoreCounter.ToString()}";
        if (scoreCounter % 5 == 0 && scoreCounter!=0)
        {
            Debug.Log("eftasa ta +5 mila!!");
        }
    }
    private void InstantiatePools()
    {
        bodyPartsInPool = new Queue<GameObject>();
        consumablesInPool = new Queue<GameObject>();
        spawnedbodyPart = new List<GameObject>();

        GameObject cubesPool = Instantiate(emptyPrefab, transform);
        cubesPool.name = "BodyPartsPool";

        for (int i = 0; i < cubesPoolAmount; i++)
        {
            GameObject objectInstantiated = Instantiate(cubePrefab, cubesPool.transform);
            objectInstantiated.SetActive(false);
            bodyPartsInPool.Enqueue(objectInstantiated);
        }

        GameObject spheresPool = Instantiate(emptyPrefab, transform);
        spheresPool.name = "ConsumablesPool";

        for (int i = 0; i < spheresPoolAmount; i++)
        {
            GameObject objectInstantiated = Instantiate(spherePrefab, spheresPool.transform);
            objectInstantiated.SetActive(false);
            consumablesInPool.Enqueue(objectInstantiated);
        }
    }
    public void spawn_cube(Vector3 spawn_position = default(Vector3), float rotation = 0)
    {
        GameObject cube = bodyPartsInPool.Dequeue();
        bodyPartsInPool.Enqueue(cube);
        cube.transform.position = spawn_position;
        cube.SetActive(true);

        if(!spawnedbodyPart.Contains(cube))
        {
            spawnedbodyPart.Add(cube);
        }

        eyes.SetParent(cube.transform);
        eyes.localPosition = Vector3.zero;
        eyes.eulerAngles = new Vector3(0, rotation, 0);
    }
    public void spawn_sphere(Vector3 spawn_position = default(Vector3), float rotation = 0)
    {
        GameObject sphere = consumablesInPool.Dequeue();
        consumablesInPool.Enqueue(sphere);
        sphere.transform.position = spawn_position;
        sphere.SetActive(true);
    }
    public void AddBodyPart()
    {
        GameObject newBodyPart = Instantiate(cubePrefab, transform.GetChild(0));
        newBodyPart.SetActive(false);
        bodyPartsInPool.Enqueue(newBodyPart);
    }

    private IEnumerator coroutine;
    private IEnumerator Counter(float time, Action action, Action[] onClose = null)
    {
        while(time > 0)
        {
            time -= Time.deltaTime;

            if(time <= 0)
            {
                action.Invoke();

                if (onClose == null)
                    CloseCoroutine();
                else
                    CloseCoroutine(onClose);
            }

            yield return null;
        }
    }
    private void CloseCoroutine(Action[] onClose = null)
    {
        StopCoroutine(coroutine);
        if (onClose != null && onClose.Length > 0)
        {
            foreach (Action action in onClose)
            {
                action.Invoke();
            }
        }
    }

    public void Crash()
    {
        endScreen.SetActive(true);

        background.CrossFadeAlpha(1, 3.0f, false);
        endScreen.transform.GetChild(0).GetComponent<Image>().DOFade(1, 3.0f);
        endScreen.transform.GetChild(1).GetComponent<Text>().DOFade(1, 3.0f);
        endScreen.transform.GetChild(2).GetComponent<Image>().DOFade(1, 3.0f);
        endScreen.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().DOFade(1, 3.0f);
        endScreen.transform.GetChild(3).GetComponent<Image>().DOFade(1, 3.0f);
        endScreen.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().DOFade(1, 3.0f);
    }

    public void Restart()
    {
        hasPlayed = true;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
