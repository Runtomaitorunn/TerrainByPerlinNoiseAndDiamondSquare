using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class methodChange : MonoBehaviour
{
    public GameObject PerlinUI;
    public GameObject PerlinObj;
    public GameObject DiamondUI;
    public GameObject DiamondObj;

    // Start is called before the first frame update
    void Start()
    {
        DiamondUI.SetActive(true);
        DiamondObj.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            PerlinUI.SetActive(false);
            PerlinObj.SetActive(false);
            DiamondUI.SetActive(true);
            DiamondObj.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            DiamondUI.SetActive(false);
            DiamondObj.SetActive(false);
            PerlinUI.SetActive(true);
            PerlinObj.SetActive(true);
        }
    }

}
