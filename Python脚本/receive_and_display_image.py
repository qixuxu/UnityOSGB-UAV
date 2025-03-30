import socket
import tkinter as tk
from PIL import Image, ImageTk
import io

# 创建 Tkinter 窗口
root = tk.Tk()
root.title("图片显示窗口")

# 创建一个 Label 用于显示图片
image_label = tk.Label(root)
image_label.pack()

def receive_and_display_image():
    try:
        # 创建 TCP 套接字
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.bind(('0.0.0.0', 12345))  # 监听所有可用的网络接口
            s.listen(1)
            print("等待连接...")
            conn, addr = s.accept()
            with conn:
                print(f"已连接: {addr}")
                image_data = b''
                while True:
                    data = conn.recv(4096)
                    if not data:
                        break
                    image_data += data

        # 将接收到的数据转换为图片
        image = Image.open(io.BytesIO(image_data))
        photo = ImageTk.PhotoImage(image)
        image_label.config(image=photo)
        image_label.image = photo
    except Exception as e:
        print(f"接收或显示图片时出错: {e}")
    # 每隔 1 秒检查一次连接
    root.after(1000, receive_and_display_image)

# 开始接收和显示图片
receive_and_display_image()

# 运行 Tkinter 主循环
root.mainloop()