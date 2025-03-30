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
    public Camera camera; // ��һ����ͷ
    // ������ʱ�䣬��λΪ�룬���� Inspector ��������е���
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

        // ��ǰ���� RenderTexture �� Texture2D ���󣬱���Ƶ������������
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // ���ӵ����紮�ڣ���������������ַΪ 127.0.0.1���˿�Ϊ 8888��
        try
        {
            client = new TcpClient("127.0.0.1", 8888);
            stream = client.GetStream();
        }
        catch (Exception ex)
        {
            Debug.LogError("���ӵ����紮��ʱ��������: " + ex.Message);
        }
    }

    void StartCapture()
    {
        if (!isCapturing)
        {
            isCapturing = true;
            // �������߳̽���ѭ������
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
            // �߳�����ָ���ļ��ʱ�䣬��λΪ���룬����Ҫ���� 1000
            Thread.Sleep((int)(captureInterval * 1000));
        }
    }

    void CaptureAndSendImage()
    {
        try
        {
            // ���������Ŀ����Ⱦ������Ⱦ
            camera.targetTexture = renderTexture;
            camera.Render();

            // ������Ⱦ������ȡ����
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenShot.Apply();

            // �ͷ���Ⱦ����
            camera.targetTexture = null;
            RenderTexture.active = null;

            // �� Texture2D ת��Ϊ�ֽ�����
            byte[] imageBytes = screenShot.EncodeToPNG();

            // ͨ�����紮�ڷ���ͼƬ����
            if (stream != null && stream.CanWrite)
            {
                stream.Write(imageBytes, 0, imageBytes.Length);
            }

            Debug.Log("ͼƬ��ͨ�����紮�ڷ���");
        }
        catch (Exception ex)
        {
            Debug.LogError("���㲢����ͼƬʱ��������: " + ex.Message);
        }
    }

    void OnDestroy()
    {
        // ������ǰ��������Դ
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
