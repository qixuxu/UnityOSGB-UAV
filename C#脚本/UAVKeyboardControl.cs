using UnityEngine;

public class UAVKeyboardControl : MonoBehaviour
{
    public float baseMoveSpeed = 5f; // 无人机基础移动速度
    public float baseRotateSpeed = 100f; // 无人机基础旋转速度
    public float minHeight = 5f; // 无人机维持的最小高度
    public float speedMultiplier = 1f; // 速度乘数，用于加速和减速
    public float rotationSpeedMultiplier = 1f; // 旋转速度乘数，用于加速和减速
    public float accelerationFactor = 1.2f; // 加速因子
    public float decelerationFactor = 0.8f; // 减速因子
    public float targetHeight = 10f; // 设定的目标高度
    public bool heightSettingEnabled = false; // 高度设定功能开关

    // 假设的 GPS 坐标转换函数，需要根据实际情况实现
    public static Vector3 GPSToUnity(Vector2 gps)
    {
        // 这里只是示例，实际需要根据具体的 GPS 坐标系和 Unity 世界坐标系进行转换
        return new Vector3(gps.x, 0, gps.y);
    }

    // 假设的 Unity 坐标转换为 GPS 函数，需要根据实际情况实现
    public static Vector2 UnityToGPS(Vector3 unity)
    {
        // 这里只是示例，实际需要根据具体的 GPS 坐标系和 Unity 世界坐标系进行转换
        return new Vector2(unity.x, unity.z);
    }

    void FixedUpdate()
    {
        // 确保无人机高度不低于最小高度
        MaintainHeight();

        // 处理移动控制
        HandleMovement();
        // 处理旋转控制
        HandleRotation();
        // 处理上下移动控制
        HandleVerticalMovement();
        // 处理加速减速控制
        HandleAccelerationAndDeceleration();
        // 处理高度设定开关
        HandleHeightSettingToggle();
        // 处理高度设定
        if (heightSettingEnabled)
        {
            HandleHeightSetting();
        }
    }

    /// <summary>
    /// 处理无人机的前后和左右平移移动
    /// </summary>
    private void HandleMovement()
    {
        // 获取水平和垂直输入轴的值
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float currentMoveSpeed = baseMoveSpeed * speedMultiplier;

        // 计算前后移动的向量，使用世界坐标系统
        // vertical 为正（W 键）时向前移动，为负（S 键）时向后移动
        Vector3 forwardMovement = transform.forward * vertical * currentMoveSpeed * Time.fixedDeltaTime;
        // 计算左右平移的向量，使用世界坐标系统
        // horizontal 为正（D 键）时向右移动，为负（A 键）时向左移动
        Vector3 horizontalMovement = transform.right * horizontal * currentMoveSpeed * Time.fixedDeltaTime;

        // 应用移动向量到受控对象，使用世界坐标系统
        transform.Translate(forwardMovement + horizontalMovement, Space.World);

        // 更新 GPS 坐标（这里只是示例，需要根据实际情况实现）
        Vector2 currentGPS = UnityToGPS(transform.position);
        // 可以在这里将更新后的 GPS 坐标发送给外部系统
    }

    /// <summary>
    /// 处理无人机的左右旋转
    /// </summary>
    private void HandleRotation()
    {
        float currentRotateSpeed = baseRotateSpeed * rotationSpeedMultiplier;

        // 检测 J 键输入，向左旋转，使用世界坐标系统
        if (Input.GetKey(KeyCode.J))
        {
            transform.Rotate(Vector3.up, -currentRotateSpeed * Time.fixedDeltaTime, Space.World);
        }
        // 检测 L 键输入，向右旋转，使用世界坐标系统
        if (Input.GetKey(KeyCode.L))
        {
            transform.Rotate(Vector3.up, currentRotateSpeed * Time.fixedDeltaTime, Space.World);
        }
    }

    /// <summary>
    /// 处理无人机的上下移动
    /// </summary>
    private void HandleVerticalMovement()
    {
        float currentMoveSpeed = baseMoveSpeed * speedMultiplier;

        // 检测键I，向上移动，使用世界坐标系统
        if (Input.GetKey(KeyCode.I))
        {
            transform.Translate(Vector3.up * currentMoveSpeed * Time.fixedDeltaTime, Space.World);
        }
        // 检测 K 键输入，向下移动，使用世界坐标系统
        if (Input.GetKey(KeyCode.K))
        {
            transform.Translate(Vector3.down * currentMoveSpeed * Time.fixedDeltaTime, Space.World);
        }
    }

    /// <summary>
    /// 处理无人机的加速和减速
    /// </summary>
    private void HandleAccelerationAndDeceleration()
    {
        // 检测 G 键输入，加速
        if (Input.GetKeyDown(KeyCode.G))
        {
            speedMultiplier *= accelerationFactor;
            rotationSpeedMultiplier *= accelerationFactor;
        }
        // 检测 H 键输入，减速
        if (Input.GetKeyDown(KeyCode.H))
        {
            speedMultiplier *= decelerationFactor;
            rotationSpeedMultiplier *= decelerationFactor;
        }
    }

    /// <summary>
    /// 维持无人机在地图上方的最小高度
    /// </summary>
    private void MaintainHeight()
    {
        if (transform.position.y < minHeight)
        {
            // 如果无人机高度低于最小高度，将其位置调整到最小高度
            Vector3 newPosition = new Vector3(transform.position.x, minHeight, transform.position.z);
            transform.position = newPosition;
        }
    }

    /// <summary>
    /// 处理高度设定开关
    /// </summary>
    private void HandleHeightSettingToggle()
    {
        if (Input.GetKeyDown(KeyCode.T)) // 这里用 T 键作为开关高度设定功能的按键，你可以根据需要修改
        {
            heightSettingEnabled = !heightSettingEnabled;
            Debug.Log($"高度设定功能已 {(heightSettingEnabled ? "开启" : "关闭")}");
        }
    }

    /// <summary>
    /// 处理无人机高度设定
    /// </summary>
    private void HandleHeightSetting()
    {
        float currentHeight = transform.position.y;
        float heightDifference = targetHeight - currentHeight;
        float currentMoveSpeed = baseMoveSpeed * speedMultiplier;

        if (Mathf.Abs(heightDifference) > 0.1f) // 为了避免微小误差导致的频繁移动
        {
            Vector3 heightMovement = Vector3.up * Mathf.Sign(heightDifference) * currentMoveSpeed * Time.fixedDeltaTime;
            transform.Translate(heightMovement, Space.World);
        }
    }
}
