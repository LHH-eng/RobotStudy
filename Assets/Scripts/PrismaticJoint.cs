//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PrismaticJoint : MonoBehaviour
//{
//    [System.Serializable]
//    public class Joint
//    {
//        public GameObject systemPart;  // 움직일 오브젝트
//        public string inputAxis;       // 식별용 이름
//        public float minLimit;         // 이동 최소 범위
//        public float maxLimit;         // 이동 최대 범위
//    }

//    public Joint[] joints;  // 조인트 배열 (0번 = 고정, 1번~ = 이동)
//    public float speed = 1f;  // 이동 속도

//    void Update()
//    {
//        // 키보드 입력 감지 (-1 ~ 1)
//        float input = 0f;
//        if (Keyboard.current.rightArrowKey.isPressed) input = 1f;
//        if (Keyboard.current.leftArrowKey.isPressed) input = -1f;

//        // joints[0]은 고정 파트(Conveyor)이므로 1번부터 순회
//        for (int i = 1; i < joints.Length; i++)
//        {
//            Transform part = joints[i].systemPart.transform;

//            // 현재 localPosition 가져오기
//            Vector3 pos = part.localPosition;

//            // X축으로만 이동 (속도 * 입력 * 프레임보정)
//            pos.x += input * speed * Time.deltaTime;

//            // 이동 범위 제한
//            pos.x = Mathf.Clamp(pos.x, joints[i].minLimit, joints[i].maxLimit);

//            // Y, Z는 고정 (변하지 않도록)
//            pos.y = part.localPosition.y;
//            pos.z = part.localPosition.z;

//            // 계산된 위치 적용
//            part.localPosition = pos;
//        }
//    }
//}
