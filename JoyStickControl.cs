using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStickControl : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private Image bgImg;    // 조이스틱 범위 이미지
    private Image stickImg; // 조이스틱
    public Vector3 inputVector;
    //public Vector3 playerVector;

    public GameObject player;   //플레이어 오브젝트 추가
    Rigidbody playerMove;
    public float moveSpeed;
    private bool moveCheck = false;

    void Start()
    {
        playerMove = player.GetComponent<Rigidbody>();
        bgImg = GetComponent<Image>();  // Image 컴퍼넌트 받아온다
        stickImg = transform.GetChild(0).GetComponent<Image>(); // 0번째 자식의 Image 컴퍼넌트 받아온다
    }

    void Update()
    {
        //playerVector = inputVector;
        if (moveCheck == true)
        {
            player.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            player.transform.rotation = Quaternion.Euler(0f, Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg, 0);
        }
    }

    public virtual void OnDrag(PointerEventData ped)    // 드래그 했을때
    {
        Vector2 pos;


        // 순서대로 rect, screenpoint, cam, out localpoint
        // screenpoint가 rect의 local스페이스를 기준으로 어느 정도의 좌표에 있는지를 localpoint안에 담아준다
        // 그리고 screenpoint가 rect의 내부에 있으면 true를 반환
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImg.rectTransform,ped.position,ped.pressEventCamera,out pos))
        {
            pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2, pos.y * 2, 0);

            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;    // magnitude: 벡터의 길이를 반환 , normalized 정규화하여 1로 변환

            // 조이스틱 일정범위 안에서 움직이기
            stickImg.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x /3), inputVector.y * (bgImg.rectTransform.sizeDelta.y /3));

            // 플레이어 움직임
            //바로 조이스틱의 벡터이다. 하지만 이 벡터는 바로 쓸수 없다.
            //쓰기 위해선 아크탄젠트를 통해 변환하고, 나온 라디안 값을 디그리로 변환해주고 나온 디그리 값으로 플레이어의 y축 회전에 쓰면된다.
        }
    }

    public virtual void OnPointerDown(PointerEventData ped) // 눌렀을때
    {
        moveCheck = true;
        OnDrag(ped);    // 드래그 함수 호출
    }

    public virtual void OnPointerUp(PointerEventData ped)   // 땟을때-+

    {
        moveCheck = false;
        inputVector = Vector3.zero;
        stickImg.rectTransform.anchoredPosition = Vector3.zero;
    }
}
