using UnityEngine;

public class UAVKeyboardControl : MonoBehaviour
{
    public Transform controlledObject; // 可选择的受控对象，可在Unity编辑器中指定为无人机
    public float baseMoveSpeed = 5f; // 无人机基础移动速度
    public float rotateSpeed = 100f; // 无人机旋转速度
    public float minHeight = 5f; // 无人机维持的最小高度
    public float speedMultiplier = 1f; // 速度乘数，用于加速和减速
    public float accelerationFactor = 1.2f; // 加速因子
    public float decelerationFactor = 0.8f; // 减速因子
    public float targetHeight = 10f; // 设定的目标高度
    public bool heightSettingEnabled = false; // 高度设定功能开关

    void FixedUpdate()
    {
        if (controlledObject == null)
        {
            Debug.LogError("受控对象未指定，请在Unity编辑器中指定一个Transform对象。");
            return;
        }

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
        Vector3 forwardMovement = controlledObject.forward * vertical * currentMoveSpeed * Time.fixedDeltaTime;
        // 计算左右平移的向量，使用世界坐标系统
        // horizontal 为正（D 键）时向右移动，为负（A 键）时向左移动
        Vector3 horizontalMovement = controlledObject.right * horizontal * currentMoveSpeed * Time.fixedDeltaTime;

        // 应用移动向量到受控对象，使用世界坐标系统
        controlledObject.Translate(forwardMovement + horizontalMovement, Space.World);
    }

    /// <summary>
    /// 处理无人机的左右旋转
    /// </summary>
    private void HandleRotation()
    {
        // 检测 J 键输入，向左旋转，使用世界坐标系统
        if (Input.GetKey(KeyCode.J))
        {
            controlledObject.Rotate(Vector3.up, -rotateSpeed * Time.fixedDeltaTime, Space.World);
        }
        // 检测 L 键输入，向右旋转，使用世界坐标系统
        if (Input.GetKey(KeyCode.L))
        {
            controlledObject.Rotate(Vector3.up, rotateSpeed * Time.fixedDeltaTime, Space.World);
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
            controlledObject.Translate(Vector3.up * currentMoveSpeed * Time.fixedDeltaTime, Space.World);
        }
        // 检测 K 键输入，向下移动，使用世界坐标系统
        if (Input.GetKey(KeyCode.K))
        {
            controlledObject.Translate(Vector3.down * currentMoveSpeed * Time.fixedDeltaTime, Space.World);
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
        }
        // 检测 H 键输入，减速
        if (Input.GetKeyDown(KeyCode.H))
        {
            speedMultiplier *= decelerationFactor;
        }
    }

    /// <summary>
    /// 维持无人机在地图上方的最小高度
    /// </summary>
    private void MaintainHeight()
    {
        if (controlledObject.position.y < minHeight)
        {
            // 如果无人机高度低于最小高度，将其位置调整到最小高度
            Vector3 newPosition = new Vector3(controlledObject.position.x, minHeight, controlledObject.position.z);
            controlledObject.position = newPosition;
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
        float currentHeight = controlledObject.position.y;
        float heightDifference = targetHeight - currentHeight;
        float currentMoveSpeed = baseMoveSpeed * speedMultiplier;

        if (Mathf.Abs(heightDifference) > 0.1f) // 为了避免微小误差导致的频繁移动
        {
            Vector3 heightMovement = Vector3.up * Mathf.Sign(heightDifference) * currentMoveSpeed * Time.fixedDeltaTime;
            controlledObject.Translate(heightMovement, Space.World);
        }
    }
}