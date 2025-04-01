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

        // ��ǰ���� RenderTexture �� Texture2D ���󣬱���Ƶ������������
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    }

    void CaptureAndSaveImage()
    {
        try
        {
            // ���������Ŀ����Ⱦ������Ⱦ
            downCamera.targetTexture = renderTexture;
            downCamera.Render();

            // ������Ⱦ������ȡ����
            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenShot.Apply();

            // �ͷ���Ⱦ����
            downCamera.targetTexture = null;
            RenderTexture.active = null;

            // �� Texture2D ת��Ϊ�ֽ�����
            byte[] imageBytes = screenShot.EncodeToPNG();

            // ȷ������·������
            string savePath = @"F:\tupian";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            // ���ɱ�����ļ�����ʹ�õ�ǰʱ����Ϊ�ļ���
            string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            string imageFullPath = Path.Combine(savePath, fileName + ".png");
            string textFullPath = Path.Combine(savePath, fileName + ".txt");

            // ����ͼƬ��ָ��·��
            File.WriteAllBytes(imageFullPath, imageBytes);

            // ��ȡ���˻���λ����Ϣ
            Vector3 dronePosition = droneTransform.position;
            string positionInfo = $"X: {dronePosition.x}, Y: {dronePosition.y}, Z: {dronePosition.z}";

            // ����λ����Ϣ���ı��ļ�
            File.WriteAllText(textFullPath, positionInfo);

            Debug.Log("ͼƬ�ѱ��浽: " + imageFullPath);
            Debug.Log("λ����Ϣ�ѱ��浽: " + textFullPath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("����ͼƬ��λ����Ϣʱ��������: " + ex.Message);
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
    }
}
