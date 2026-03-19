using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

/// <summary>
/// 카티아의 PrismaticJoint 기능을 유니티로 구현
/// PrismaticJoint는 선과 선, 면과 면을 구속하여 직선 운동을 구현
/// 1단계
/// 2개의 지점 포인트(선을 구현할 수 있는 지표)와 2개의 지점 포인트를 비교하여 동일한 선 상에 있는지 판단 가능한 클래스
/// </summary>
public class LineComparison : MonoBehaviour
{
    public GameObject objA;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Vector3 p1 = new Vector3(2, 1, 1);
        Vector3 p2 = new Vector3(6, 1, 1);
        Vector3 p3 = new Vector3(7, 1, 1);
        Vector3 p4 = new Vector3(9, 1, 1);

        IsOnSameLine(p1, p2, p3, p4);

        // objA의 MeshFilter 컴포넌트 가져오기
        MeshFilter mf = objA.GetComponent<MeshFilter>();

        // 메시의 모든 버텍스 배열 가져오기 (중복 포함 24개)
        Vector3[] vector3s = mf.mesh.vertices;

        // 중복을 자동으로 제거하는 컬렉션 생성
        HashSet<Vector3> uniqueVertices = new HashSet<Vector3>();

        // 24개의 버텍스를 하나씩 추가 (중복은 자동 무시)
        foreach (Vector3 vertices in vector3s)
        {
            uniqueVertices.Add(vertices);
        }

        // 중복 제거된 고유 버텍스 8개 출력
        int count = 0;
        foreach (Vector3 vertices in uniqueVertices)
        {
            Debug.Log($"[{count}] {vertices}");
            count++;
        }

        // HashSet을 인덱스 접근 가능한 배열로 변환
        Vector3[] verts = uniqueVertices.ToArray();

        // 12개의 모서리를 2개의 꼭짓점 쌍으로 정의 [모서리 인덱스, 시작/끝점]
        Vector3[,] edges = new Vector3[12, 2]
        {
            // 앞면 4개
            { verts[0], verts[1] },
            { verts[0], verts[2] },
            { verts[1], verts[3] },
            { verts[2], verts[3] },
            // 뒷면 4개
            { verts[6], verts[7] },
            { verts[6], verts[4] },
            { verts[7], verts[5] },
            { verts[4], verts[5] },
            // 연결 4개
            { verts[0], verts[6] },
            { verts[1], verts[7] },
            { verts[2], verts[4] },
            { verts[3], verts[5] },
         };

        // 12개 모서리의 시작점과 끝점 출력
        for (int i = 0; i < 12; i++)
        {
            Debug.Log($"edge[{i}]: {edges[i, 0]} → {edges[i, 1]}");
        }

        // Update is called once per frame
        //void Update()
        //{

        //}
    }

    private static void IsOnSameLine(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        // 1. A Line - 2개의 포인트(선을 구현할 수 있는 지표) 구함
        Vector3 dirA = p1 - p2;
        // 2. B Line - 2개의 포인트(선을 구현할 수 있는 지표) 구함
        Vector3 dirB = p3 - p4;
        // 3. A - 1개의 포인트와 B - 1개의 포인트의 방향 구함
        Vector3 dirC = p1 - p3;

        // A Line 과 B Line의 방향을 비교
        // A Line과 B Line의 외적(Cross Product) 계산
        // 두 방향 벡터가 평행하면 수직인 벡터를 만들 수 없어 (0,0,0)이 나옴
        // (0,0,0)이면 두 선이 평행 or 일치, 아니면 다른 방향
        Vector3 crossStep1 = Vector3.Cross(dirA, dirB);
        // A Line과 A - 1개의 포인트와 B - 1개의 포인트의 방향을 비교
        Vector3 crossStep2 = Vector3.Cross(dirA, dirC);

        // A Line과 B Line이 평행일 때, A Line과 A - 1개의 포인트와 B - 1개의 포인트의 방향도 평행하면 같은 선상에 있음
        // magnitude: 벡터의 크기(길이)
        // Mathf.Epsilon: 부동소수점 오차 허용 범위 (약 0.000001)
        // 외적 결과의 크기가 사실상 0이면 평행으로 판단
        if (crossStep1.magnitude < Mathf.Epsilon && crossStep2.magnitude < Mathf.Epsilon)
        {
            Debug.Log("같은 선상!");
        }

        else
        {
            Debug.Log("다른선상!!!!");
        }
    }
}
