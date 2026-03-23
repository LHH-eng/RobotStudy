using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

/// <summary>
/// 카티아의 PrismaticJoint 기능을 유니티로 구현
/// PrismaticJoint는 선과 선, 면과 면을 구속하여 직선 운동을 구현
/// 
/// [선 & 선 비교]
/// 두 GameObject의 메시에서 버텍스(Point)를 추출하고, 버텍스로 모서리를 구성하여
/// 같은 선상에 있는 모서리 쌍을 탐지하는 기능
/// 같은 선상 판단: 방향벡터 외적(Cross Product)이 (0,0,0)이면 같은 선상
/// 
/// [면 & 면 비교]
/// 오브젝트 내부의 평행한 모서리 쌍을 추출하고, 두 오브젝트의 평행 쌍을 비교하여
/// 같은 평면에 있는지 탐지하는 기능
/// 같은 평면 판단: 평행한 모서리 쌍의 두 모서리가 모두 같은 선상에 있으면 같은 평면
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

        List<(Vector3[], Vector3[])> parallelPairsA = ExtractParallelPairs(edgesA);
        List<(Vector3[], Vector3[])> parallelPairsB = ExtractParallelPairs(edgesB);

        ComparePlanes(parallelPairsA, parallelPairsB);

        //Debug.Log($"A 평행쌍 수: {parallelPairsA.Count}");
        //Debug.Log($"B 평행쌍 수: {parallelPairsB.Count}");

        //for (int i = 0; i < parallelPairsA.Count; i++)
        //{
        //    Debug.Log($"A쌍[{i}]: {parallelPairsA[i].Item1[0]}→{parallelPairsA[i].Item1[1]} / {parallelPairsA[i].Item2[0]}→{parallelPairsA[i].Item2[1]}");
        //}

        //for (int i = 0; i < parallelPairsB.Count; i++)
        //{
        //    Debug.Log($"B쌍[{i}]: {parallelPairsB[i].Item1[0]}→{parallelPairsB[i].Item1[1]} / {parallelPairsB[i].Item2[0]}→{parallelPairsB[i].Item2[1]}");
        //}



    }

    //----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// [선 & 선] - 2단계
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
            Debug.Log("다른 선!!!");
        }
    }

    //----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// [선 & 선] - 1단계
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

    //----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// [선 & 선] - 2단계_보조메서드
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

    //----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// [면 & 면] - 1단계_보조메서드
    /// 두 선분이 평행인지 판단하는 메서드 (일치는 제외)
    /// 외적(Cross Product)을 이용하여 방향 벡터가 평행한지 확인
    /// </summary>
    /// <param name="p1">A 선분의 시작점</param>
    /// <param name="p2">A 선분의 끝점</param>
    /// <param name="p3">B 선분의 시작점</param>
    /// <param name="p4">B 선분의 끝점</param>
    private static bool IsParallel(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        // 1. A Line의 방향 벡터 구함
        Vector3 dirA = p1 - p2;
        // 2. B Line의 방향 벡터 구함
        Vector3 dirB = p3 - p4;
        // 3. A의 시작점 → B의 시작점 방향 벡터 구함
        Vector3 dirC = p1 - p3;

        // A Line과 B Line의 외적(Cross Product) 계산
        // 두 방향 벡터가 평행하면 외적 결과가 (0,0,0)이 나옴
        // (0,0,0)이면 두 선이 평행 or 일치, 아니면 교차 or 꼬인 관계
        Vector3 crossStep1 = Vector3.Cross(dirA, dirB);
        // A Line과 dirC의 외적 계산
        // (0,0,0)이면 두 선이 일치, 아니면 평행 (다른 위치에 있음)
        Vector3 crossStep2 = Vector3.Cross(dirA, dirC);

        // crossStep1 = 0 : 두 선의 방향이 평행
        // crossStep2 > Epsilon : 두 선이 일치하지 않음 (다른 위치에 있는 평행선)
        // magnitude: 벡터의 크기(길이)
        // Mathf.Epsilon: 부동소수점 오차 허용 범위 (약 0.000001)
        if (crossStep1.magnitude < Mathf.Epsilon && crossStep2.magnitude > Mathf.Epsilon)
        {
            // Debug.Log($"평행! A: {p1} → {p2} / B: {p3} → {p4}");
            return true;
        }

        else
        {
            // Debug.Log("평행 아님!!!!");
            return false;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// [면 & 면] - 1단계
    /// 모서리 목록에서 같은 면에 속한 평행한 모서리 쌍을 추출하는 메서드
    /// 같은 선상(일치)은 제외하고 평행한 쌍만 추출
    /// 
    /// 같은 면 판단 조건:
    /// 두 모서리의 4개 점이 x, y, z 중 하나의 축값이 모두 동일해야 같은 면
    /// ex) x=5.50 면: 4개 점의 x값이 모두 5.50
    /// </summary>
    /// <param name="edges">모서리 목록</param>
    /// <returns>같은 면에 속한 평행한 모서리 쌍 목록 (각 쌍 = 모서리1, 모서리2 튜플)</returns>
    List<(Vector3[], Vector3[])> ExtractParallelPairs(List<Vector3[]> edges)
    {
        // 평행한 모서리 쌍을 담을 튜플 리스트 생성
        List<(Vector3[], Vector3[])> parallelPairs = new List<(Vector3[], Vector3[])>();

        // 모서리 목록을 모든 쌍으로 비교 (중복 비교 방지: j = i+1 부터 시작)
        for (int i = 0; i < edges.Count; i++)
        {
            for (int j = i + 1; j < edges.Count; j++)
            {
                // i번째 모서리와 j번째 모서리가 평행하고,
                // 두 모서리가 같은 면에 있는지 확인 (x, y, z 중 하나의 축값이 4개 점 모두 동일해야 함)
                if (IsParallel(edges[i][0], edges[i][1], edges[j][0], edges[j][1]) && 
                    (edges[i][0].x == edges[i][1].x && edges[i][1].x == edges[j][0].x && edges[j][0].x == edges[j][1].x ||
                    edges[i][0].y == edges[i][1].y && edges[i][1].y == edges[j][0].y && edges[j][0].y == edges[j][1].y ||
                    edges[i][0].z == edges[i][1].z && edges[i][1].z == edges[j][0].z && edges[j][0].z == edges[j][1].z))
                {
                    parallelPairs.Add((edges[i], edges[j]));
                }
            }
        }

        return parallelPairs;
    }

    //----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// [면 & 면] - 2단계
    /// 두 오브젝트의 평행한 모서리 쌍을 비교하여 같은 평면인지 판단하는 메서드
    /// 
    /// 같은 평면 판단 조건 (두 가지 경우):
    /// 경우1: A쌍의 모서리1 == B쌍의 모서리1 (같은 선상) AND A쌍의 모서리2 == B쌍의 모서리2 (같은 선상)
    /// 경우2: A쌍의 모서리1 == B쌍의 모서리2 (같은 선상) AND A쌍의 모서리2 == B쌍의 모서리1 (같은 선상)
    /// → 순서에 상관없이 두 쌍의 모서리가 모두 같은 선상이면 같은 평면
    /// 중복 출력 방지: HashSet으로 이미 찾은 면의 축값을 기록하여 중복 무시
    /// </summary>
    /// <param name="parallelPairsA">objA의 평행한 모서리 쌍 목록</param>
    /// <param name="parallelPairsB">objB의 평행한 모서리 쌍 목록</param>
    private static void ComparePlanes(List<(Vector3[], Vector3[])> parallelPairsA, List<(Vector3[], Vector3[])> parallelPairsB)
    {
        bool found = false;

        // 이미 찾은 면을 기록하는 HashSet (축이름, 축값) - 중복 출력 방지
        HashSet<(string, float)> foundPlanes = new HashSet<(string, float)>();

        // A의 평행한 모서리 쌍을 하나씩 순회
        for (int i = 0; i < parallelPairsA.Count; i++)
        {
            // B의 평행한 모서리 쌍을 하나씩 순회
            for (int j = 0; j < parallelPairsB.Count; j++)
            {
                // 두 가지 경우로 같은 평면 판단:
                // 경우1: A모서리1 == B모서리1 (같은 선상) AND A모서리2 == B모서리2 (같은 선상)
                // 경우2: A모서리1 == B모서리2 (같은 선상) AND A모서리2 == B모서리1 (같은 선상) ← 순서가 반대인 경우
                if ((IsOnSameLine(parallelPairsA[i].Item1[0], parallelPairsA[i].Item1[1], parallelPairsB[j].Item1[0], parallelPairsB[j].Item1[1]) && 
                     IsOnSameLine(parallelPairsA[i].Item2[0], parallelPairsA[i].Item2[1], parallelPairsB[j].Item2[0], parallelPairsB[j].Item2[1])) ||
                     (IsOnSameLine(parallelPairsA[i].Item1[0], parallelPairsA[i].Item1[1], parallelPairsB[j].Item2[0], parallelPairsB[j].Item2[1]) &&
                     IsOnSameLine(parallelPairsA[i].Item2[0], parallelPairsA[i].Item2[1], parallelPairsB[j].Item1[0], parallelPairsB[j].Item1[1])))
                {
                    found = true;

                    // 일치하는 평면의 축값 추출 (어떤 축의 면인지 확인)
                    (string, float) planeAxis = GetPlaneAxis(parallelPairsA[i]);

                    // 이미 찾은 면이 아닐 때만 출력 (중복 방지)
                    if (!foundPlanes.Contains(planeAxis))
                    {
                        // 새로운 면 → HashSet에 추가 후 출력
                        foundPlanes.Add(planeAxis);
                        Debug.Log($"일치하는 평면! 축: {planeAxis.Item1} = {planeAxis.Item2}");
                    }

                }
            }
        }

        if (!found)
        {
            Debug.Log("떨어진 평면!!!!");
        }
    }

    //----------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// [면 & 면] - 2단계_보조 메서드
    /// 평행한 모서리 쌍을 받아서 어떤 축의 면인지 반환하는 메서드
    /// 4개 점의 축값이 모두 동일한 축을 찾아 (축이름, 축값) 튜플로 반환
    /// ex) x=5.50 면 → ("x", 5.50f)
    /// </summary>
    /// <param name="pair">평행한 모서리 쌍 (모서리1, 모서리2 튜플)</param>
    /// <returns>(축이름, 축값) 튜플 / 해당 없으면 ("none", 0f)</returns>
    private static (string, float) GetPlaneAxis((Vector3[], Vector3[]) pair)
    {
        Vector3 p = pair.Item1[0];  // 대표 점 하나 (축값 추출용)

        // 4개 점의 x값이 모두 같으면 x축 수직면 (YZ평면)
        if (pair.Item1[0].x == pair.Item1[1].x && pair.Item1[1].x == pair.Item2[0].x && pair.Item2[0].x == pair.Item2[1].x)
            return ("x", p.x);
        // 4개 점의 y값이 모두 같으면 y축 수직면 (XZ평면)
        if (pair.Item1[0].y == pair.Item1[1].y && pair.Item1[1].y == pair.Item2[0].y && pair.Item2[0].y == pair.Item2[1].y)
            return ("y", p.y);
        // 4개 점의 z값이 모두 같으면 z축 수직면 (XY평면)
        if (pair.Item1[0].z == pair.Item1[1].z && pair.Item1[1].z == pair.Item2[0].z && pair.Item2[0].z == pair.Item2[1].z)
            return ("z", p.z);

        // x, y, z 모두 해당 없을 때 기본값 반환
        return ("none", 0f);
    }
}
