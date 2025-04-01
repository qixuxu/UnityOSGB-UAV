using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class DroneCameraClickCapture : MonoBehaviour
{
    public Button captureButton;
    public Camera downCamera;
    public Transform droneTransform;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    void Start()
    {
        captureButton.onClick.AddListener(CaptureAndSaveImage);

        // 提前创建 RenderTexture 和 Texture2D 对象，避免频繁创建和销毁
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    void CaptureAndSaveImage()
    {
        try
        {
            // 设置相机的目标渲染纹理并渲染
            downCamera.targetTexture = renderTexture;
            downCamera.Render();

            // 激活渲染纹理并读取像素
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenShot.Apply();

            // 释放渲染纹理
            downCamera.targetTexture = null;
            RenderTexture.active = null;

            // 将 Texture2D 转换为字节数组
            byte[] imageBytes = screenShot.EncodeToPNG();

            // 确保保存路径存在
            string savePath = @"F:\tupian";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            // 生成保存的文件名，使用当前时间作为文件名
            string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            string imageFullPath = Path.Combine(savePath, fileName + ".png");
            string textFullPath = Path.Combine(savePath, fileName + ".txt");

            // 保存图片到指定路径
            File.WriteAllBytes(imageFullPath, imageBytes);

            // 获取无人机的位置信息
            Vector3 dronePosition = droneTransform.position;
            string positionInfo = $"X: {dronePosition.x}, Y: {dronePosition.y}, Z: {dronePosition.z}";

            // 保存位置信息到文本文件
            File.WriteAllText(textFullPath, positionInfo);

            Debug.Log("图片已保存到: " + imageFullPath);
            Debug.Log("位置信息已保存到: " + textFullPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("保存图片或位置信息时发生错误: " + ex.Message);
        }
    }

    void OnDestroy()
    {
        // 销毁提前创建的资源
        if (renderTexture != null)
        {
            Destroy(renderTexture);
        }
        if (screenShot != null)
        {
            Destroy(screenShot);
        }
    }
}
