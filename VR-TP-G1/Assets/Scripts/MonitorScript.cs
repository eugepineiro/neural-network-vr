using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonitorScript : MonoBehaviour
{
    public List<Material> screenList;
    private int currentScreen;
    public float screenPeriod = 1.0f;
    private float nextScreenTime;

    // Start is called before the first frame update
    void Start()
    {
        currentScreen = 0;
        nextScreenTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextScreenTime){
            nextScreenTime += screenPeriod;
            currentScreen++;
            if (currentScreen >= screenList.Count)
                currentScreen = 0;
            transform.GetComponent<MeshRenderer>().material = screenList[currentScreen];
        }
    }
}
