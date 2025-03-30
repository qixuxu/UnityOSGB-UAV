using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Threading;
using System;
using System.Net.Sockets;

public class DroneCameraClickCapture : MonoBehaviour
{
    public Button startCaptureButton;
    public Button stopCaptureButton;
    public Camera camera; // 单一摄像头
    // 拍摄间隔时间，单位为秒，可在 Inspector 面板或代码中调节
    public float captureInterval = 5f;
    private bool isCapturing = false;
    private RenderTexture renderTexture;
    private Texture2D screenShot;
    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        startCaptureButton.onClick.AddListener(StartCapture);
        stopCaptureButton.onClick.AddListener(StopCapture);

        // 提前创建 RenderTexture 和 Texture2D 对象，避免频繁创建和销毁
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // 连接到网络串口（这里假设服务器地址为 127.0.0.1，端口为 8888）
        try
        {
            client = new TcpClient("127.0.0.1", 8888);
            stream = client.GetStream();
        }
        catch (Exception ex)
        {
            Debug.LogError("连接到网络串口时发生错误: " + ex.Message);
        }
    }

    void StartCapture()
    {
        if (!isCapturing)
        {
            isCapturing = true;
            // 开启新线程进行循环拍摄
            Thread captureThread = new Thread(CaptureLoop);
            captureThread.Start();
        }
    }

    void StopCapture()
    {
        isCapturing = false;
    }

    void CaptureLoop()
    {
        while (isCapturing)
        {
            CaptureAndSendImage();
            // 线程休眠指定的间隔时间，单位为毫秒，所以要乘以 1000
            Thread.Sleep((int)(captureInterval * 1000));
        }
    }

    void CaptureAndSendImage()
    {
        try
        {
            // 设置相机的目标渲染纹理并渲染
            camera.targetTexture = renderTexture;
            camera.Render();

            // 激活渲染纹理并读取像素
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenShot.Apply();

            // 释放渲染纹理
            camera.targetTexture = null;
            RenderTexture.active = null;

            // 将 Texture2D 转换为字节数组
            byte[] imageBytes = screenShot.EncodeToPNG();

            // 通过网络串口发送图片数据
            if (stream != null && stream.CanWrite)
            {
                stream.Write(imageBytes, 0, imageBytes.Length);
            }

            Debug.Log("图片已通过网络串口发送");
        }
        catch (Exception ex)
        {
            Debug.LogError("拍摄并发送图片时发生错误: " + ex.Message);
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
        if (stream != null)
        {
            stream.Close();
        }
        if (client != null)
        {
            client.Close();
        }
    }
}
