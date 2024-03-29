using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    PlayerInputActions inputActions;
    Rigidbody rigid;
    Animator animator;
    public TextMeshProUGUI text;

    /// <summary>
    /// 이동 방향(1 : 전진, -1 : 후진, 0 : 정지)
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// 이동 속도
    /// </summary>
    public float moveSpeed = 3.0f;

    public float runSpeed = 6.0f;

    /// <summary>
    /// 회전방향(1 : 우회전, -1 : 좌회전, 0 : 정지)
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// 회전 속도
    /// </summary>
    public float rotateSpeed = 180.0f;

    /// <summary>
    /// 획득한 아이템 갯수(5가되면 클리어조건 달성)
    /// </summary>
    public int paperCount = 0;

    /// <summary>
    /// 맞은 횟수(3이되면 사망)
    /// </summary>
    public int hitted = 0;


    /// <summary>
    /// 문이 열릴만큼 종이를 얻었다고 알리는 델리게이트
    /// </summary>
    public Action onGateOpen;

    /// <summary>
    /// 애니메이터용 해시값
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int DieHash = Animator.StringToHash("Die");

    /// <summary>
    /// 점프력
    /// </summary>
    public float jumpPower = 6.0f;

    /// <summary>
    /// 공중에 떠 있는지 아닌지 나타내는 프로퍼티
    /// </summary>
    bool InAir
    {
        get => GroundCount < 1;
    }

    /// <summary>
    /// 접촉하고 있는 "Groud" 태그 오브젝트의 개수확인 및 설정용 프로퍼티
    /// </summary>
    int GroundCount
    {
        get => groundCount;
        set
        {
            if (groundCount < 0)    // 0이하면 0에서 설정
            {
                groundCount = 0;
            }
            groundCount = value;
            if (groundCount < 0)    // 설정한 값이 0이하면 0
            {
                groundCount = 0;
            }
        }
    }

    /// <summary>
    /// 접촉하고 있는 "Groud" 태그 오브젝트의 개수
    /// </summary>
    int groundCount = 0;

    /// <summary>
    /// 점프 쿨 타임
    /// </summary>
    public float jumpCoolTime = 5.0f;

    /// <summary>
    /// 남아있는 쿨타임
    /// </summary>
    float jumpCoolRemains = -1.0f;

    float JumpCoolRemains
    {
        get => jumpCoolRemains;
        set
        {
            jumpCoolRemains = value;
            onJumpCoolTimeChange?.Invoke(jumpCoolRemains / jumpCoolTime);
        }
    }

    public Action<float> onJumpCoolTimeChange;

    /// <summary>
    /// 점프가 가능한지 확인하는 프로퍼티(점프중이 아니고 쿨타임이 다 지났다.)
    /// </summary>
    bool IsJumpAvailable => !InAir && (JumpCoolRemains < 0.0f) && isAlive;

    bool isSprintAvailable = true;

    /// <summary>
    /// 플레이어의 생존 여부
    /// </summary>
    bool isAlive = true;

    /// <summary>
    /// 플레이어의 사망을 알리는 델리게이트
    /// </summary>
    public Action onDie;


    private void Awake()
    {
        //inputActions = new PlayerInputActions();
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        text = GetComponent<TextMeshProUGUI>();

        //ItemUseChecker checker = GetComponentInChildren<ItemUseChecker>();
        //checker.onItemUse += (interacable) => interacable.Use();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Jump.performed += OnJumpInput;
        inputActions.Player.Sprint.performed += OnSprintInput;
    }

    private void OnDisable()
    {
        inputActions.Player.Sprint.performed -= OnSprintInput;
        inputActions.Player.Jump.performed -= OnJumpInput;
        inputActions.Player.Move.canceled -= OnMoveInput;
        inputActions.Player.Move.performed -= OnMoveInput;
        inputActions.Player.Disable();
    }

    private void OnMoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), !context.canceled);
    }

    private void OnJumpInput(InputAction.CallbackContext _)
    {
        Jump();
    }

    private void OnSprintInput(InputAction.CallbackContext _)
    {
        if(isSprintAvailable == true)
        {
            StartCoroutine(SprintDuration());
        }
    }

    private void Start()
    {
        SetText();
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    void Update()
    {
        JumpCoolRemains -= Time.deltaTime;
        SetText();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GroundCount++;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GroundCount--;
        }
    }

    /// <summary>
    /// 달리고 기다린 뒤 속도 복구하는 코루틴
    /// </summary>
    IEnumerator SprintDuration()
    {
        isSprintAvailable = false;
        moveSpeed = runSpeed;
        yield return new WaitForSeconds(3.5f);
        moveSpeed = 3.0f;
        yield return new WaitForSeconds(4.5f);
        isSprintAvailable = true;
    }

    public void SetText()
    {
        if(text != null)
        {
            text.text = $"Paper {paperCount} / 5\nLife : {3-hitted}";
        }
    }

    /// <summary>
    /// 이동 입력 처리용 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    /// <param name="isMove">이동 중이면 true, 이동 중이 아니면 false</param>
    void SetInput(Vector2 input, bool isMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;

        animator.SetBool(IsMoveHash, isMove);
    }

    /// <summary>
    /// 실제 이동 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Move()
    {
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveSpeed * moveDirection * transform.forward);
    }

    /// <summary>
    /// 실제 회전 처리 함수(FixedUpdate에서 사용)
    /// </summary>
    void Rotate()
    {
        // 이번 fixedUpdate에서 추가로 회전할 회전(delta)
        Quaternion rotate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);

        // 현재 회전에서 rotate만큼 추가로 회전
        rigid.MoveRotation(rigid.rotation * rotate);
    }

    /// <summary>
    /// 실제 점프 처리를 하는 함수
    /// </summary>
    void Jump()
    {
        if (IsJumpAvailable) // 점프가 가능할 때만 점프
        {
            rigid.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);  // 위쪽으로 jumpPower만큼 힘을 더하기
            JumpCoolRemains = jumpCoolTime;
        }
    }

    /// <summary>
    /// 사망 처리용 함수
    /// </summary>
    public void Die()
    {
        if (isAlive)
        {
            Debug.Log("죽었음");

            // 죽는 애니메이션이 나온다.
            animator.SetTrigger(DieHash);

            // 더 이상 조종이 안되어야 한다.
            inputActions.Player.Disable();

            // 데굴데굴 구른다.(뒤로 넘어가면서 Y축으로 스핀을 먹는다.)
            rigid.constraints = RigidbodyConstraints.None;  // 물리 잠금을 전부 해제하기

            Transform head = transform.GetChild(0);
            rigid.AddForceAtPosition(-transform.forward, head.position, ForceMode.Impulse);
            rigid.AddTorque(transform.up * 1.5f, ForceMode.Impulse);

            // 죽었다고 신호보내기(onDie 델리게이트 실행)
            onDie?.Invoke();

            isAlive = false;
        }
    }

    public void GateOpen()
    {
        if (paperCount > 4)
        {
            onGateOpen?.Invoke();   // 문이 열렸다고 알림
        }
    }
}
