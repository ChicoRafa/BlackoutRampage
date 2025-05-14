using _Data.PlayerController.Scripts;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private int timeForTruckToArrive = 90;
    private float timer;
    private int remainingSeconds;
    private bool truckIsComing;

    private void Start()
    {
        ResetTruckTimer();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            TruckArrives();

        if (truckIsComing)
            TruckTimer();
    }

    private void TruckTimer()
    {
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

            int minutes = remainingSeconds / 60;
            int seconds = remainingSeconds % 60;

            //Debug.Log(string.Format("{0:00}:{1:00}", minutes, seconds));
        }
    }

    public void ResetTruckTimer()
    {
        remainingSeconds = timeForTruckToArrive;
        timer = 0f;
        truckIsComing = true;
    }

    private void TruckArrives()
    {
        Debug.Log("Track has arrived");

        for (int i = 0; i < productsToBring; i++)
        {
            int random = Random.Range(0, productsList.Count);
            //GameObject newProduct = Instantiate(productsList[random]);
            //newProduct.GetComponent<ProductScript>().origin = ProductScript.productOrigin.Truck;
            //newProduct.transform.parent = null;

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
                    //newProduct.transform.position = spotsList[j].transform.position;
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
}