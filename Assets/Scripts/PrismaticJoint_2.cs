using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카티아의 PrismaticJoint 기능을 유니티로 구현
/// PrismaticJoint는 선과 선, 면과 면을 구속하여 직선 운동을 구현
/// 선 구속  → 두 선이 같은 직선 위
/// 면 구속  → 두 면이 같은 평면 위
/// 완전구속 → 선 + 면 동시 구속 → 구속된 축 방향으로만 이동
/// </summary>
public class PrismaticJoint_2 : MonoBehaviour
{
    // =============================================
    // Joint 클래스 - 조인트 속성 정의
    // =============================================
    [System.Serializable]
    public class Joint
    {
        public string name;            // 식별용 이름
        public GameObject systemPart;  // 대상 오브젝트
        public float minLimit;         // 이동 최소 범위
        public float maxLimit;         // 이동 최대 범위
    }

    // =============================================
    // Line 클래스 - 2개의 포인트로 선 정의
    // =============================================
    public class Line
    {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 Direction => (p2 - p1).normalized;
        public float Length => Vector3.Distance(p1, p2);
    }

    // =============================================
    // Plane 클래스 - 3개의 포인트로 면 정의
    // =============================================
    public class Plane
    {
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;

        public Vector3 Normal
        {
            get
            {
                Vector3 v1 = p2 - p1;
                Vector3 v2 = p3 - p1;
                return Vector3.Cross(v1, v2).normalized;
            }
        }

        public Vector3 Center => (p1 + p2 + p3) / 3f;
    }

    // =============================================
    // VertexFinder 클래스 - 버텍스 및 모서리 추출
    // =============================================
    public class VertexFinder
    {
        public List<Vector3> objectVertices = new List<Vector3>();
        public List<Line> objectLines = new List<Line>();

        /// <summary>
        /// 오브젝트의 버텍스와 모서리 선 추출
        /// </summary>
        public void FindVertices(GameObject target)
        {
            objectVertices.Clear();
            objectLines.Clear();

            MeshFilter mf = target.GetComponent<MeshFilter>();
            if (mf == null) { Debug.LogWarning($"[{target.name}] MeshFilter 없음"); return; }

            Mesh mesh = mf.sharedMesh;
            Vector3[] localVerts = mesh.vertices;

            // 로컬 기준 중복 제거 → 현재 월드 좌표로 변환
            HashSet<Vector3> uniqueVerts = new HashSet<Vector3>();
            foreach (Vector3 v in localVerts)
            {
                if (uniqueVerts.Add(v))
                    objectVertices.Add(target.transform.TransformPoint(v));
            }

            FindEdges(target, mesh);
        }

        /// <summary>
        /// 축 방향 모서리 선 12개 추출 (대각선 제거)
        /// </summary>
        private void FindEdges(GameObject target, Mesh mesh)
        {
            Vector3[] verts = mesh.vertices;
            int[] triangles = mesh.triangles;
            HashSet<string> uniqueEdges = new HashSet<string>();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int[,] edgeIndices = new int[,]
                {
                    { triangles[i],     triangles[i + 1] },
                    { triangles[i + 1], triangles[i + 2] },
                    { triangles[i + 2], triangles[i]     }
                };

                for (int e = 0; e < 3; e++)
                {
                    Vector3 worldA = target.transform.TransformPoint(verts[edgeIndices[e, 0]]);
                    Vector3 worldB = target.transform.TransformPoint(verts[edgeIndices[e, 1]]);
                    Vector3 dir = (worldB - worldA).normalized;

                    // 축 방향만 허용 (대각선 제거)
                    bool isAxisAligned =
                        (Mathf.Abs(Mathf.Abs(dir.x) - 1f) < 0.001f && Mathf.Abs(dir.y) < 0.001f && Mathf.Abs(dir.z) < 0.001f) ||
                        (Mathf.Abs(dir.x) < 0.001f && Mathf.Abs(Mathf.Abs(dir.y) - 1f) < 0.001f && Mathf.Abs(dir.z) < 0.001f) ||
                        (Mathf.Abs(dir.x) < 0.001f && Mathf.Abs(dir.y) < 0.001f && Mathf.Abs(Mathf.Abs(dir.z) - 1f) < 0.001f);

                    if (!isAxisAligned) continue;

                    bool aFirst = worldA.x < worldB.x ||
                        (worldA.x == worldB.x && worldA.y < worldB.y) ||
                        (worldA.x == worldB.x && worldA.y == worldB.y && worldA.z <= worldB.z);

                    Vector3 kA = aFirst ? worldA : worldB;
                    Vector3 kB = aFirst ? worldB : worldA;
                    string key = $"{kA.x:F3},{kA.y:F3},{kA.z:F3}_{kB.x:F3},{kB.y:F3},{kB.z:F3}";

                    if (uniqueEdges.Add(key))
                        objectLines.Add(new Line { p1 = worldA, p2 = worldB });
                }
            }
        }

        /// <summary>
        /// X축 방향 선 목록 전체 반환
        /// </summary>
        public List<Line> GetAllXLines()
        {
            List<Line> xLines = new List<Line>();
            foreach (Line l in objectLines)
            {
                bool isXAxis = Mathf.Abs(Mathf.Abs(l.Direction.x) - 1f) < 0.001f;
                if (isXAxis) xLines.Add(l);
            }
            return xLines;
        }

        /// <summary>
        /// Y값이 같은 버텍스 그룹으로 면 목록 생성
        /// </summary>
        public List<Plane> GetAllFacePlanes()
        {
            Dictionary<float, List<Vector3>> yGroups = new Dictionary<float, List<Vector3>>();
            foreach (Vector3 v in objectVertices)
            {
                float roundedY = Mathf.Round(v.y * 1000f) / 1000f;
                if (!yGroups.ContainsKey(roundedY))
                    yGroups[roundedY] = new List<Vector3>();
                yGroups[roundedY].Add(v);
            }

            List<Plane> planes = new List<Plane>();
            foreach (var group in yGroups.Values)
            {
                if (group.Count < 3) continue;
                planes.Add(new Plane { p1 = group[0], p2 = group[1], p3 = group[2] });
            }
            return planes;
        }
    }

    // =============================================
    // 인스펙터 노출 변수
    // =============================================
    public Joint[] joints;   // joints[0] = 고정, joints[1] = 이동
    public float speed = 2f; // 이동 속도

    // 고정 오브젝트 데이터 (Start에서 1회 캐싱)
    private List<Line> xLinesA = new List<Line>();
    private List<Plane> planesA = new List<Plane>();

    // 이동 오브젝트 데이터 (Update에서 매 프레임 갱신)
    private List<Line> xLinesB = new List<Line>();
    private List<Plane> planesB = new List<Plane>();

    // 구속 상태
    private bool isLineLocked = false;
    private bool isPlaneLocked = false;
    private Vector3 slideAxis = Vector3.zero;

    // 이동 오브젝트 참조
    private GameObject movingObject;

    // =============================================
    // Start - 고정 오브젝트 데이터 한 번만 추출
    // =============================================
    private void Start()
    {
        if (joints.Length < 2) { Debug.LogError("Joint 2개 이상 필요!"); return; }
        if (joints[0].systemPart == null || joints[1].systemPart == null)
        {
            Debug.LogError("systemPart가 비어있습니다!");
            return;
        }

        movingObject = joints[1].systemPart;

        // 고정 오브젝트 데이터 캐싱 (위치 안 바뀌므로 1회만)
        VertexFinder finderA = new VertexFinder();
        finderA.FindVertices(joints[0].systemPart);
        xLinesA = finderA.GetAllXLines();
        planesA = finderA.GetAllFacePlanes();

        Debug.Log($"=== [{joints[0].name}] 고정 오브젝트 데이터 추출 완료 ===");
        Debug.Log($"  X축 선 수: {xLinesA.Count}개 / 면 수: {planesA.Count}개");
    }

    // =============================================
    // Update - 이동 오브젝트 갱신 + 구속 확인 + 키 이동
    // =============================================
    private void Update()
    {
        if (movingObject == null) return;

        // 이동 오브젝트는 위치가 바뀌므로 매 프레임 갱신
        VertexFinder finderB = new VertexFinder();
        finderB.FindVertices(movingObject);
        xLinesB = finderB.GetAllXLines();
        planesB = finderB.GetAllFacePlanes();

        // 구속 상태 확인
        CheckConstraints();

        // 완전 구속 상태에서만 키 입력 이동
        if (isLineLocked && isPlaneLocked)
        {
            float input = 0f;
            if (Input.GetKey(KeyCode.RightArrow)) input = 1f;
            if (Input.GetKey(KeyCode.LeftArrow)) input = -1f;

            if (input != 0f)
            {
                Vector3 newPos = movingObject.transform.position
                    + slideAxis * input * speed * Time.deltaTime;

                // minLimit ~ maxLimit 범위 제한
                float projected = Vector3.Dot(
                    newPos - joints[0].systemPart.transform.position,
                    slideAxis);
                projected = Mathf.Clamp(projected, joints[1].minLimit, joints[1].maxLimit);

                movingObject.transform.position =
                    joints[0].systemPart.transform.position + slideAxis * projected;
            }
        }
    }

    // =============================================
    // 구속 상태 확인 - 상태 변경 시에만 로그 출력
    // =============================================
    private void CheckConstraints()
    {
        // ----- 선 구속 확인 -----
        // A의 모든 X선 vs B의 모든 X선 전체 조합 비교
        bool newLineLocked = false;
        foreach (Line la in xLinesA)
        {
            foreach (Line lb in xLinesB)
            {
                if (IsCollinear(la, lb))
                {
                    newLineLocked = true;
                    slideAxis = la.Direction; // 구속된 선 방향 = 이동 허용 축
                    break;
                }
            }
            if (newLineLocked) break;
        }

        // ----- 면 구속 확인 -----
        // A의 모든 면 vs B의 모든 면 전체 조합 비교
        bool newPlaneLocked = false;
        foreach (Plane pA in planesA)
        {
            foreach (Plane pB in planesB)
            {
                if (IsCoplanar(pA, pB))
                {
                    newPlaneLocked = true;
                    break;
                }
            }
            if (newPlaneLocked) break;
        }

        // 완전 구속 이전 상태 저장
        bool wasFullyLocked = isLineLocked && isPlaneLocked;

        // ----- 상태 변경 시에만 로그 출력 (매 프레임 스팸 방지) -----
        if (newLineLocked != isLineLocked)
        {
            isLineLocked = newLineLocked;
            Debug.Log(isLineLocked
                ? "✅ 선 구속 완료! 두 선이 같은 직선 위에 있습니다."
                : "❌ 선 구속 해제.");
        }

        if (newPlaneLocked != isPlaneLocked)
        {
            isPlaneLocked = newPlaneLocked;
            Debug.Log(isPlaneLocked
                ? "✅ 면 구속 완료! 두 면이 같은 평면 위에 있습니다."
                : "❌ 면 구속 해제.");
        }

        // 완전 구속 상태 변경 시에만 출력
        bool isFullyLocked = isLineLocked && isPlaneLocked;
        if (isFullyLocked != wasFullyLocked)
        {
            if (isFullyLocked)
                Debug.Log($"🔒 완전 구속 완료! [{slideAxis}] 방향으로만 이동 가능. ← → 키로 이동하세요.");
            else
                Debug.Log("🔓 완전 구속 해제.");
        }
    }

    // =============================================
    // 두 Line이 같은 직선 위에 있는지 확인
    // 조건1: 방향벡터 평행 (외적 ≈ 0)
    // 조건2: 연결벡터도 같은 직선 위 (외적 ≈ 0)
    // =============================================
    private bool IsCollinear(Line lineA, Line lineB)
    {
        // 방향벡터 평행 확인
        Vector3 crossDir = Vector3.Cross(lineA.Direction, lineB.Direction);
        if (crossDir.magnitude >= 0.01f) return false;

        // 동일 직선 확인
        Vector3 connectVec = (lineB.p1 - lineA.p1).normalized;
        Vector3 crossPos = Vector3.Cross(connectVec, lineA.Direction);
        return crossPos.magnitude < 0.01f;
    }

    // =============================================
    // 두 Plane이 같은 평면 위에 있는지 확인
    // 조건1: 법선벡터 평행 (외적 ≈ 0)
    // 조건2: 두 면이 같은 평면 위 (내적 ≈ 0)
    // =============================================
    private bool IsCoplanar(Plane planeA, Plane planeB)
    {
        // 법선벡터 평행 확인
        Vector3 crossNormal = Vector3.Cross(planeA.Normal, planeB.Normal);
        if (crossNormal.magnitude >= 0.01f) return false;

        // 동일 평면 확인
        Vector3 connectVec = planeB.p1 - planeA.p1;
        float dot = Mathf.Abs(Vector3.Dot(connectVec, planeA.Normal));
        return dot < 0.01f;
    }
}