using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    PlayerInputActions inputActions;
    Rigidbody rigid;
    Animator animator;

    /// <summary>
    /// �̵� ����(1 : ����, -1 : ����, 0 : ����)
    /// </summary>
    float moveDirection = 0.0f;

    /// <summary>
    /// �̵� �ӵ�
    /// </summary>
    public float moveSpeed = 5.0f;

    /// <summary>
    /// ȸ������(1 : ��ȸ��, -1 : ��ȸ��, 0 : ����)
    /// </summary>
    float rotateDirection = 0.0f;

    /// <summary>
    /// ȸ�� �ӵ�
    /// </summary>
    public float rotateSpeed = 180.0f;

    /// <summary>
    /// ȹ���� ������ ����
    /// </summary>
    public int paperCount = 0;

    public int hitted = 0;


    /// <summary>
    /// ���� ������ŭ ���̸� ����ٰ� �˸��� ��������Ʈ
    /// </summary>
    public Action onGateOpen;

    /// <summary>
    /// �ִϸ����Ϳ� �ؽð�
    /// </summary>
    readonly int IsMoveHash = Animator.StringToHash("IsMove");
    readonly int UseHash = Animator.StringToHash("Use");
    readonly int DieHash = Animator.StringToHash("Die");

    /// <summary>
    /// ������
    /// </summary>
    public float jumpPower = 6.0f;

    /// <summary>
    /// ���߿� �� �ִ��� �ƴ��� ��Ÿ���� ������Ƽ
    /// </summary>
    bool InAir
    {
        get => GroundCount < 1;
    }

    /// <summary>
    /// �����ϰ� �ִ� "Groud" �±� ������Ʈ�� ����Ȯ�� �� ������ ������Ƽ
    /// </summary>
    int GroundCount
    {
        get => groundCount;
        set
        {
            if (groundCount < 0)    // 0���ϸ� 0���� ����
            {
                groundCount = 0;
            }
            groundCount = value;
            if (groundCount < 0)    // ������ ���� 0���ϸ� 0
            {
                groundCount = 0;
            }
        }
    }

    /// <summary>
    /// �����ϰ� �ִ� "Groud" �±� ������Ʈ�� ����
    /// </summary>
    int groundCount = 0;

    /// <summary>
    /// ���� �� Ÿ��
    /// </summary>
    public float jumpCoolTime = 5.0f;

    /// <summary>
    /// �����ִ� ��Ÿ��
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
    /// ������ �������� Ȯ���ϴ� ������Ƽ(�������� �ƴϰ� ��Ÿ���� �� ������.)
    /// </summary>
    bool IsJumpAvailable => !InAir && (JumpCoolRemains < 0.0f) && isAlive;

    /// <summary>
    /// �÷��̾��� ���� ����
    /// </summary>
    bool isAlive = true;

    /// <summary>
    /// �÷��̾��� ����� �˸��� ��������Ʈ
    /// </summary>
    public Action onDie;


    private void Awake()
    {
        //inputActions = new PlayerInputActions();
        inputActions = new();
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //ItemUseChecker checker = GetComponentInChildren<ItemUseChecker>();
        //checker.onItemUse += (interacable) => interacable.Use();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMoveInput;
        inputActions.Player.Move.canceled += OnMoveInput;
        inputActions.Player.Jump.performed += OnJumpInput;
        inputActions.Player.Use.performed += OnUseInput;
    }

    private void OnDisable()
    {
        inputActions.Player.Use.performed -= OnUseInput;
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

    private void OnUseInput(InputAction.CallbackContext context)
    {
        //animator.SetTrigger(UseHash);
    }

    private void FixedUpdate()
    {
        Move();
        Rotate();
    }

    void Update()
    {
        JumpCoolRemains -= Time.deltaTime;
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
    /// �̵� �Է� ó���� �Լ�
    /// </summary>
    /// <param name="input">�Էµ� ����</param>
    /// <param name="isMove">�̵� ���̸� true, �̵� ���� �ƴϸ� false</param>
    void SetInput(Vector2 input, bool isMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;

        animator.SetBool(IsMoveHash, isMove);
    }

    /// <summary>
    /// ���� �̵� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void Move()
    {
        rigid.MovePosition(rigid.position + Time.fixedDeltaTime * moveSpeed * moveDirection * transform.forward);
    }

    /// <summary>
    /// ���� ȸ�� ó�� �Լ�(FixedUpdate���� ���)
    /// </summary>
    void Rotate()
    {
        // �̹� fixedUpdate���� �߰��� ȸ���� ȸ��(delta)
        Quaternion rotate = Quaternion.AngleAxis(Time.fixedDeltaTime * rotateSpeed * rotateDirection, transform.up);

        // ���� ȸ������ rotate��ŭ �߰��� ȸ��
        rigid.MoveRotation(rigid.rotation * rotate);
    }

    /// <summary>
    /// ���� ���� ó���� �ϴ� �Լ�
    /// </summary>
    void Jump()
    {
        if (IsJumpAvailable) // ������ ������ ���� ����
        {
            rigid.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);  // �������� jumpPower��ŭ ���� ���ϱ�
            JumpCoolRemains = jumpCoolTime;
        }
    }

    /// <summary>
    /// ��� ó���� �Լ�
    /// </summary>
    public void Die()
    {
        if (isAlive)
        {
            Debug.Log("�׾���");

            // �״� �ִϸ��̼��� ���´�.
            animator.SetTrigger(DieHash);

            // �� �̻� ������ �ȵǾ�� �Ѵ�.
            inputActions.Player.Disable();

            // �������� ������.(�ڷ� �Ѿ�鼭 Y������ ������ �Դ´�.)
            rigid.constraints = RigidbodyConstraints.None;  // ���� ����� ���� �����ϱ�

            Transform head = transform.GetChild(0);
            rigid.AddForceAtPosition(-transform.forward, head.position, ForceMode.Impulse);
            rigid.AddTorque(transform.up * 1.5f, ForceMode.Impulse);

            // �׾��ٰ� ��ȣ������(onDie ��������Ʈ ����)
            onDie?.Invoke();

            isAlive = false;
        }
    }

    public void GateOpen()
    {
        if (paperCount > 4)
        {
            onGateOpen?.Invoke();   // ���� ���ȴٰ� �˸�
        }
    }
}
