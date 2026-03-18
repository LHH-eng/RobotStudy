using System.Drawing;
using UnityEngine;

/// <summary>
/// 카티아의 PrismaticJoint 기능을 유니티로 구현
/// PrismaticJoint는 선과 선, 면과 면을 구속하여 직선 운동을 구현
/// 1단계
/// 2개의 지점 포인트(선을 구현할 수 있는 지표)와 2개의 지점 포인트를 비교하여 동일한 선 상에 있는지 판단 가능한 클래스
/// </summary>
public class LineComparison : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float x;
        float y;
        float z;

        Vector3 point = new Vector3(x, y, z);


        // 1. p1.x / p2.x = T -> T값 확인 후 2단계로 

        // 2. p1.y / p2.y = T -> true -> 3단계로 
        //                    -> false -> 다른선
        // 3. p1.y / p2.y = T -> true
        //                    -> false -> 다른선

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
