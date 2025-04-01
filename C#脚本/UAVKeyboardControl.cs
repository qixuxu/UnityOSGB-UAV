using UnityEngine;

public class UAVKeyboardControl : MonoBehaviour
{
    public float baseMoveSpeed = 5f; // ���˻������ƶ��ٶ�
    public float baseRotateSpeed = 100f; // ���˻�������ת�ٶ�
    public float minHeight = 5f; // ���˻�ά�ֵ���С�߶�
    public float speedMultiplier = 1f; // �ٶȳ��������ڼ��ٺͼ���
    public float rotationSpeedMultiplier = 1f; // ��ת�ٶȳ��������ڼ��ٺͼ���
    public float accelerationFactor = 1.2f; // ��������
    public float decelerationFactor = 0.8f; // ��������
    public float targetHeight = 10f; // �趨��Ŀ��߶�
    public bool heightSettingEnabled = false; // �߶��趨���ܿ���

    // ����� GPS ����ת����������Ҫ����ʵ�����ʵ��
    public static Vector3 GPSToUnity(Vector2 gps)
    {
        // ����ֻ��ʾ����ʵ����Ҫ���ݾ���� GPS ����ϵ�� Unity ��������ϵ����ת��
        return new Vector3(gps.x, 0, gps.y);
    }

    // ����� Unity ����ת��Ϊ GPS ��������Ҫ����ʵ�����ʵ��
    public static Vector2 UnityToGPS(Vector3 unity)
    {
        // ����ֻ��ʾ����ʵ����Ҫ���ݾ���� GPS ����ϵ�� Unity ��������ϵ����ת��
        return new Vector2(unity.x, unity.z);
    }

    void FixedUpdate()
    {
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
        Vector3 forwardMovement = transform.forward * vertical * currentMoveSpeed * Time.fixedDeltaTime;
        // ��������ƽ�Ƶ�������ʹ����������ϵͳ
        // horizontal Ϊ����D ����ʱ�����ƶ���Ϊ����A ����ʱ�����ƶ�
        Vector3 horizontalMovement = transform.right * horizontal * currentMoveSpeed * Time.fixedDeltaTime;

        // Ӧ���ƶ��������ܿض���ʹ����������ϵͳ
        transform.Translate(forwardMovement + horizontalMovement, Space.World);

        // ���� GPS ���꣨����ֻ��ʾ������Ҫ����ʵ�����ʵ�֣�
        Vector2 currentGPS = UnityToGPS(transform.position);
        // ���������ｫ���º�� GPS ���귢�͸��ⲿϵͳ
    }

    /// <summary>
    /// �������˻���������ת
    /// </summary>
    private void HandleRotation()
    {
        float currentRotateSpeed = baseRotateSpeed * rotationSpeedMultiplier;

        // ��� J �����룬������ת��ʹ����������ϵͳ
        if (Input.GetKey(KeyCode.J))
        {
            transform.Rotate(Vector3.up, -currentRotateSpeed * Time.fixedDeltaTime, Space.World);
        }
        // ��� L �����룬������ת��ʹ����������ϵͳ
        if (Input.GetKey(KeyCode.L))
        {
            transform.Rotate(Vector3.up, currentRotateSpeed * Time.fixedDeltaTime, Space.World);
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
            transform.Translate(Vector3.up * currentMoveSpeed * Time.fixedDeltaTime, Space.World);
        }
        // ��� K �����룬�����ƶ���ʹ����������ϵͳ
        if (Input.GetKey(KeyCode.K))
        {
            transform.Translate(Vector3.down * currentMoveSpeed * Time.fixedDeltaTime, Space.World);
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
            rotationSpeedMultiplier *= accelerationFactor;
        }
        // ��� H �����룬����
        if (Input.GetKeyDown(KeyCode.H))
        {
            speedMultiplier *= decelerationFactor;
            rotationSpeedMultiplier *= decelerationFactor;
        }
    }

    /// <summary>
    /// ά�����˻��ڵ�ͼ�Ϸ�����С�߶�
    /// </summary>
    private void MaintainHeight()
    {
        if (transform.position.y < minHeight)
        {
            // ������˻��߶ȵ�����С�߶ȣ�����λ�õ�������С�߶�
            Vector3 newPosition = new Vector3(transform.position.x, minHeight, transform.position.z);
            transform.position = newPosition;
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
        float currentHeight = transform.position.y;
        float heightDifference = targetHeight - currentHeight;
        float currentMoveSpeed = baseMoveSpeed * speedMultiplier;

        if (Mathf.Abs(heightDifference) > 0.1f) // Ϊ�˱���΢С���µ�Ƶ���ƶ�
        {
            Vector3 heightMovement = Vector3.up * Mathf.Sign(heightDifference) * currentMoveSpeed * Time.fixedDeltaTime;
            transform.Translate(heightMovement, Space.World);
        }
    }
}
