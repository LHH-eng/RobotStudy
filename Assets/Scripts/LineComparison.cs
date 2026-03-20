using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

/// <summary>
/// 카티아의 PrismaticJoint 기능을 유니티로 구현
/// PrismaticJoint는 선과 선, 면과 면을 구속하여 직선 운동을 구현
/// 1단계 - 선 비교
/// 두 GameObject의 메시에서 버텍스(Point)를 추출하고, 버텍스로 모서리를 구성하여 같은 선상에 있는 모서리 쌍을 탐지하는 클래스
/// 같은 선상 판단: 방향벡터 외적(Cross Product)이 (0,0,0)이면 같은 선상
/// </summary>
public class LineComparison : MonoBehaviour
{
    public GameObject objA;
    public GameObject objB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<Vector3[]> edgesA = ExtractEdges(objA);
        List<Vector3[]> edgesB = ExtractEdges(objB);

        CompareEdges(edgesA, edgesB);
    }

    /// <summary>
    /// 두 모서리 목록을 비교하여 같은 선상에 있는 모서리 쌍을 출력하는 메서드
    /// 같은 선상인 쌍이 없으면 "같은 선상인 모서리 없음" 출력
    /// </summary>
    /// <param name="edgesA">objA의 모서리 목록</param>
    /// <param name="edgesB">objB의 모서리 목록</param>
    private static void CompareEdges(List<Vector3[]> edgesA, List<Vector3[]> edgesB)
    {
        bool found = false;

        // objA의 모서리 12개를 하나씩 순회
        for (int i = 0; i < edgesA.Count; i++)
        {
            // objB의 모서리 12개를 하나씩 순회 (총 12 x 12 = 144번 비교)
            for (int j = 0; j < edgesB.Count; j++)
            {
                // A의 i번째 모서리(시작점, 끝점)와 B의 j번째 모서리(시작점, 끝점)가
                // 같은 선상에 있는지 비교
                if (IsOnSameLine(edgesA[i][0], edgesA[i][1], edgesB[j][0], edgesB[j][1]))
                {
                    Debug.Log($"같은 선상! A: {edgesA[i][0]} → {edgesA[i][1]} / B: {edgesB[j][0]} → {edgesB[j][1]}");
                    found = true;
                }

            }
        }

        if (!found)
        {
            Debug.Log("같은 선상인 모서리 없음");
        }
    }

    /// <summary>
    /// GameObject의 메시에서 모서리 목록을 추출하는 메서드
    /// 버텍스 24개 → 월드좌표 변환 → 중복제거 8개 → 모서리 12개 추출
    /// </summary>
    /// <param name="obj">모서리를 추출할 GameObject</param>
    /// <returns>모서리 목록 (각 모서리 = 시작점, 끝점 2개의 Vector3)</returns>
    List<Vector3[]> ExtractEdges(GameObject obj)
    {
        // objA의 MeshFilter 컴포넌트 가져오기
        MeshFilter mf = obj.GetComponent<MeshFilter>();

        // 메시의 모든 버텍스 배열 가져오기 - 로컬기준좌표 (중복 포함 24개)
        Vector3[] vector3s = mf.mesh.vertices;

        // 로컬기준 좌표를 월드좌표로 변경
        for (int i = 0; i < vector3s.Length; i++)
        {
            vector3s[i] = obj.transform.TransformPoint(vector3s[i]);
        }

        // 중복을 자동으로 제거하는 컬렉션 생성
        HashSet<Vector3> uniqueVertices = new HashSet<Vector3>();

        // 24개의 버텍스를 하나씩 추가 (중복은 자동 무시)
        foreach (Vector3 vertices in vector3s)
        {
            uniqueVertices.Add(vertices);
        }

        //Debug.Log($"총 꼭지점 수: {uniqueVertices.Count}");

        // 중복 제거된 고유 버텍스 8개 출력
        //int count = 0;
        //foreach (Vector3 vertices in uniqueVertices)
        //{
        //    Debug.Log($"point[{count}]: {vertices}");
        //    count++;
        //}

        // HashSet을 인덱스 접근 가능한 배열로 변환
        Vector3[] verts = uniqueVertices.ToArray();

        // 모서리를 동적으로 담을 리스트 생성 (각 모서리 = 시작점, 끝점 2개의 Vector3)
        List<Vector3[]> edges = new List<Vector3[]>();

        // 8개의 버텍스를 모든 쌍으로 비교 (중복 비교 방지: j = i+1 부터 시작)
        for (int i = 0; i < verts.Length; i++)
        {
            for (int j = i + 1; j < verts.Length; j++)
            {
                // x, y, z 중 같은 좌표 개수 카운트
                int sameCount = 0;
                if (verts[i].x == verts[j].x) sameCount++;
                if (verts[i].y == verts[j].y) sameCount++;
                if (verts[i].z == verts[j].z) sameCount++;

                // 2개의 좌표가 같으면 모서리로 연결된 점 → 리스트에 추가
                if (sameCount == 2)
                    edges.Add(new Vector3[] { verts[i], verts[j] });
            }
        }

        //Debug.Log($"총 모서리 수: {edges.Count}");

        //// 12개 모서리의 시작점과 끝점 출력
        //for (int i = 0; i < edges.Count; i++)
        //{
        //    Debug.Log($"edge[{i}]: {edges[i][0]} → {edges[i][1]}");
        //}

        return edges;
    }

    /// <summary>
    /// 두 선분이 같은 선상에 있는지 판단하는 메서드
    /// 외적(Cross Product)을 이용하여 방향 벡터가 평행한지 확인
    /// </summary>
    /// <param name="p1">A 선분의 시작점</param>
    /// <param name="p2">A 선분의 끝점</param>
    /// <param name="p3">B 선분의 시작점</param>
    /// <param name="p4">B 선분의 끝점</param>
    private static bool IsOnSameLine(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
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
            // Debug.Log($"같은 선상! A: {p1} → {p2} / B: {p3} → {p4}");
            return true;
        }

        else
        {
            // Debug.Log("다른선상!!!!");
            return false;
        }
    }
}
