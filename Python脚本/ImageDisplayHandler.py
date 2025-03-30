import time
import os
import socket
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler

class ImageDisplayHandler(FileSystemEventHandler):
    def __init__(self):
        self.current_image_path = None
        self.current_image_time = 0
        self.folder_path = r"F:\tupian"
        self.server_address = ('192.168.0.147', 12345)  # 替换为接收端电脑的 IP 地址

    def show_image(self, image_path):
        max_attempts = 3
        wait_time = 1  # 每次尝试前等待 1 秒

        for attempt in range(max_attempts):
            try:
                time.sleep(wait_time)  # 等待一段时间
                # 获取图片的创建时间
                image_time = os.path.getctime(image_path)
                if image_time <= self.current_image_time:
                    return  # 如果新图片的创建时间不晚于当前图片，不进行更新
                print(f"已识别到新图片: {image_path}")  # 输出识别到新图片的文字
                self.current_image_path = image_path
                self.current_image_time = image_time
                self.delete_old_images()  # 调用删除旧图片的方法
                self.send_image(image_path)  # 发送图片到接收端
                return  # 成功处理则退出循环
            except PermissionError:
                if attempt == max_attempts - 1:
                    print(f"Error opening image: {image_path} (尝试次数过多)")
            except Exception as e:
                print(f"Error opening image: {e}")
                return

    def send_image(self, image_path):
        try:
            with open(image_path, 'rb') as file:
                image_data = file.read()

            # 创建 TCP 套接字
            with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
                s.connect(self.server_address)
                s.sendall(image_data)
            print(f"图片 {image_path} 已发送到接收端")
        except Exception as e:
            print(f"发送图片时出错: {e}")

    def delete_old_images(self):
        # 遍历文件夹中的所有文件
        for filename in os.listdir(self.folder_path):
            file_path = os.path.join(self.folder_path, filename)
            if os.path.isfile(file_path):
                file_extension = os.path.splitext(file_path)[1].lower()
                if file_extension in ['.png', '.jpg', '.jpeg']:
                    file_time = os.path.getctime(file_path)
                    if file_time < self.current_image_time:
                        try:
                            os.remove(file_path)
                            print(f"已删除旧图片: {file_path}")
                        except Exception as e:
                            print(f"删除旧图片 {file_path} 时出错: {e}")

    def on_created(self, event):
        if not event.is_directory:
            file_extension = os.path.splitext(event.src_path)[1].lower()
            if file_extension in ['.png', '.jpg', '.jpeg']:
                self.show_image(event.src_path)


if __name__ == "__main__":
    event_handler = ImageDisplayHandler()
    observer = Observer()
    path = r"F:\tupian"
    observer.schedule(event_handler, path=path, recursive=False)
    observer.start()

    try:
        print("正在监控文件夹中的新图片...")
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        observer.stop()
    observer.join()
    