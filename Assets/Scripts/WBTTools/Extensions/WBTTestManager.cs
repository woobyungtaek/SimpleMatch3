using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WBTTestManager : MonoBehaviour
{
    private WBTTestClass mTestClass;

    // Start is called before the first frame update
    void Start()
    {
        mTestClass = new WBTTestClass();
        mTestClass.CommonLog();

        WBTGenericTestClass<float> testGenericClass = new WBTGenericTestClass<float>();
        testGenericClass.GenericTestMethod();
        //testGenericClass.GenericTestMethodInt();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            mTestClass.IncreaseCount();
        }
    }
}
