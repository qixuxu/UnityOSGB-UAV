using UnityEngine;

public class UAVKeyboardControl : MonoBehaviour
{
    public Transform controlledObject; // ��ѡ����ܿض��󣬿���Unity�༭����ָ��Ϊ���˻�
    public float baseMoveSpeed = 5f; // ���˻������ƶ��ٶ�
    public float rotateSpeed = 100f; // ���˻���ת�ٶ�
    public float minHeight = 5f; // ���˻�ά�ֵ���С�߶�
    public float speedMultiplier = 1f; // �ٶȳ��������ڼ��ٺͼ���
    public float accelerationFactor = 1.2f; // ��������
    public float decelerationFactor = 0.8f; // ��������
    public float targetHeight = 10f; // �趨��Ŀ��߶�
    public bool heightSettingEnabled = false; // �߶��趨���ܿ���

    void FixedUpdate()
    {
        if (controlledObject == null)
        {
            Debug.LogError("�ܿض���δָ��������Unity�༭����ָ��һ��Transform����");
            return;
        }

        // ȷ�����˻��߶Ȳ�������С�߶�
        MaintainHeight();

        // �����ƶ�����
        HandleMovement();
        // ������ת����
        HandleRotation();
        // ���������ƶ�����
        HandleVerticalMovement();
        // ������ټ��ٿ���
        HandleAccelerationAndDeceleration();
        // ����߶��趨����
        HandleHeightSettingToggle();
        // ����߶��趨
        if (heightSettingEnabled)
        {
            HandleHeightSetting();
        }
    }

    /// <summary>
    /// �������˻���ǰ�������ƽ���ƶ�
    /// </summary>
    private void HandleMovement()
    {
        // ��ȡˮƽ�ʹ�ֱ�������ֵ
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float currentMoveSpeed = baseMoveSpeed * speedMultiplier;

        // ����ǰ���ƶ���������ʹ����������ϵͳ
        // vertical Ϊ����W ����ʱ��ǰ�ƶ���Ϊ����S ����ʱ����ƶ�
        Vector3 forwardMovement = controlledObject.forward * vertical * currentMoveSpeed * Time.fixedDeltaTime;
        // ��������ƽ�Ƶ�������ʹ����������ϵͳ
        // horizontal Ϊ����D ����ʱ�����ƶ���Ϊ����A ����ʱ�����ƶ�
        Vector3 horizontalMovement = controlledObject.right * horizontal * currentMoveSpeed * Time.fixedDeltaTime;

        // Ӧ���ƶ��������ܿض���ʹ����������ϵͳ
        controlledObject.Translate(forwardMovement + horizontalMovement, Space.World);
    }

    /// <summary>
    /// �������˻���������ת
    /// </summary>
    private void HandleRotation()
    {
        // ��� J �����룬������ת��ʹ����������ϵͳ
        if (Input.GetKey(KeyCode.J))
        {
            controlledObject.Rotate(Vector3.up, -rotateSpeed * Time.fixedDeltaTime, Space.World);
        }
        // ��� L �����룬������ת��ʹ����������ϵͳ
        if (Input.GetKey(KeyCode.L))
        {
            controlledObject.Rotate(Vector3.up, rotateSpeed * Time.fixedDeltaTime, Space.World);
        }
    }

    /// <summary>
    /// �������˻��������ƶ�
    /// </summary>
    private void HandleVerticalMovement()
    {
        float currentMoveSpeed = baseMoveSpeed * speedMultiplier;

        // ����I�������ƶ���ʹ����������ϵͳ
        if (Input.GetKey(KeyCode.I))
        {
            controlledObject.Translate(Vector3.up * currentMoveSpeed * Time.fixedDeltaTime, Space.World);
        }
        // ��� K �����룬�����ƶ���ʹ����������ϵͳ
        if (Input.GetKey(KeyCode.K))
        {
            controlledObject.Translate(Vector3.down * currentMoveSpeed * Time.fixedDeltaTime, Space.World);
        }
    }

    /// <summary>
    /// �������˻��ļ��ٺͼ���
    /// </summary>
    private void HandleAccelerationAndDeceleration()
    {
        // ��� G �����룬����
        if (Input.GetKeyDown(KeyCode.G))
        {
            speedMultiplier *= accelerationFactor;
        }
        // ��� H �����룬����
        if (Input.GetKeyDown(KeyCode.H))
        {
            speedMultiplier *= decelerationFactor;
        }
    }

    /// <summary>
    /// ά�����˻��ڵ�ͼ�Ϸ�����С�߶�
    /// </summary>
    private void MaintainHeight()
    {
        if (controlledObject.position.y < minHeight)
        {
            // ������˻��߶ȵ�����С�߶ȣ�����λ�õ�������С�߶�
            Vector3 newPosition = new Vector3(controlledObject.position.x, minHeight, controlledObject.position.z);
            controlledObject.position = newPosition;
        }
    }

    /// <summary>
    /// ����߶��趨����
    /// </summary>
    private void HandleHeightSettingToggle()
    {
        if (Input.GetKeyDown(KeyCode.T)) // ������ T ����Ϊ���ظ߶��趨���ܵİ���������Ը�����Ҫ�޸�
        {
            heightSettingEnabled = !heightSettingEnabled;
            Debug.Log($"�߶��趨������ {(heightSettingEnabled ? "����" : "�ر�")}");
        }
    }

    /// <summary>
    /// �������˻��߶��趨
    /// </summary>
    private void HandleHeightSetting()
    {
        float currentHeight = controlledObject.position.y;
        float heightDifference = targetHeight - currentHeight;
        float currentMoveSpeed = baseMoveSpeed * speedMultiplier;

        if (Mathf.Abs(heightDifference) > 0.1f) // Ϊ�˱���΢С���µ�Ƶ���ƶ�
        {
            Vector3 heightMovement = Vector3.up * Mathf.Sign(heightDifference) * currentMoveSpeed * Time.fixedDeltaTime;
            controlledObject.Translate(heightMovement, Space.World);
        }
    }
}