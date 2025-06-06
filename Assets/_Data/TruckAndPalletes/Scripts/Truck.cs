using _Data.PlayerController.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    [Header("Products")]
    [SerializeField] private List<GameObject> productsList = new List<GameObject>();

    [Header("Spots for products")]
    [SerializeField] private List<Transform> spotsList = new List<Transform>();
    public List<GameObject> productsBroughtList = new List<GameObject>();

    [Header("Variables")]
    [SerializeField] private int productsToBring = 6;
    [SerializeField] private GameDataSO gameDataSO;
    private float timeForTruckToArrive;
    [SerializeField] private float numberOfTimesTruckComes = 3;
    [SerializeField] private int truckCost = 1000;
    private float timer;
    private float remainingSeconds;
    private float totalSeconds;
    private bool truckIsComing;

    [Header("UI")]
    [SerializeField] private RectTransform truckIcon;
    [SerializeField] private Vector3 truckStartPos = new Vector3(-900f, 400f, 0f);
    [SerializeField] private Vector3 truckEndPos = new Vector3(-675f, 400f, 0f);
    
    [Header("Sound")]
    [SerializeField] private SoundManagerSO soundManagerSO;
    [SerializeField] private AudioCueSO truckArrivesCue;

    private void Start()
    {
        timeForTruckToArrive = (gameDataSO.levelDurationInMinutes * 60) / numberOfTimesTruckComes;

        ResetTruckTimer();
    }

    private void Update()
    {
        TruckTimer();
    }

    private void TruckTimer()
    {
        if (!truckIsComing) return;

        UpdateTruckPosition();

        if (remainingSeconds <= 0)
        {
            TruckArrives();
            truckIsComing = false;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer -= 1f;
            remainingSeconds--;

            int minutes = (int)remainingSeconds / 60;
            int seconds = (int)remainingSeconds % 60;
        }
    }

    private void UpdateTruckPosition()
    {
        float progress = 1f - remainingSeconds / totalSeconds;
        truckIcon.anchoredPosition = Vector3.Lerp(truckStartPos, truckEndPos, progress);
    }

    public void SkipTruckJourney()
    {
        remainingSeconds = 0;
        UpdateTruckPosition();
    }

    public void ResetTruckTimer()
    {
        totalSeconds = timeForTruckToArrive;
        remainingSeconds = timeForTruckToArrive;
        timer = 0f;
        truckIsComing = true;

        truckIcon.anchoredPosition = truckStartPos;
    }

    public void TruckArrives()
    {
        soundManagerSO.OnPlaySFX(truckArrivesCue, "Truck", 1f);

        SkipTruckJourney();

        for (int i = 0; i < productsToBring; i++)
        {
            int random = Random.Range(0, productsList.Count);
            for (int j = 0; j < productsBroughtList.Count; j++)
            {
                if (productsBroughtList[j] == null)
                {
                    GameObject newProduct;

                    if (productsList[random].GetComponent<ProductScript>().objectType == "Lantern")
                    {
                        Quaternion extraRotation = Quaternion.Euler(0, -90, 0);
                        Quaternion finalRotation = productsList[random].transform.rotation * extraRotation;

                        newProduct = Instantiate(
                            productsList[random],
                            spotsList[j].transform.position,
                            finalRotation
                            );
                    }
                    else
                    {
                        newProduct = Instantiate(
                            productsList[random],
                            spotsList[j].transform.position,
                            productsList[random].transform.rotation
                            );  
                    }

                    if (newProduct.GetComponent<ProductScript>().objectType == "Lantern")
                    {
                        Quaternion extraRotation = Quaternion.Euler(0, -90, 0);
                        Quaternion finalRotation = newProduct.transform.rotation * extraRotation;
                    }

                    newProduct.GetComponent<ProductScript>().origin = ProductScript.productOrigin.Truck;
                    newProduct.transform.parent = null;

                    productsBroughtList[j] = newProduct;
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerInventoryScript>())
            other.GetComponent<PlayerInventoryScript>().canDrop = false;

        if (other.GetComponent<PlayerController>())
            other.GetComponent<PlayerController>().storage = gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerInventoryScript>())
            other.GetComponent<PlayerInventoryScript>().canDrop = true;

        if (other.GetComponent<PlayerController>())
            other.GetComponent<PlayerController>().storage = null;
    }

    public int GetTruckCost()
    {
        return truckCost;
    }
}
