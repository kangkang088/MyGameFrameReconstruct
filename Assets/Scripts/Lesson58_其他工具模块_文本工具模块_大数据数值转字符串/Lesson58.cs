using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson58 : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        print(TextUtil.GetStrOfTheBigDataAndNumber(1200000000));
        print(TextUtil.GetStrOfTheBigDataAndNumber(8545051));
        print(TextUtil.GetStrOfTheBigDataAndNumber(2515));
        print(TextUtil.GetStrOfTheBigDataAndNumber(1));

        Vector3 src = new Vector3(1,5,0);
        Vector3 target = new Vector3(0,19,1);
        print(MathUtil.GetObjDistanceXZ(src,target));
        if(MathUtil.CheckObjDistanceXZ(src,target,5))
        {
            print("Lower 5.");
        }

        Vector3 pos = Vector3.zero;
        Vector3 forward = Vector3.forward;
        Vector3 targetPos = new Vector3(1,0,1);
        if(MathUtil.AnalysisPosIsInTheSectorRangeXZ(pos,forward,targetPos,2,60))
        {
            print("In");
        }
        else
        {
            print("Out");
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}