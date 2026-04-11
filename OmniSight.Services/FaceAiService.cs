using Emgu.CV;
using Emgu.CV.Dnn; // Bắt buộc phải có để dùng Backend và Target
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace OmniSight.Services
{
    public enum FaceStatus
    {
        Normal,
        NoFace,
        MultipleFaces,
        TooFar,         // Ngồi quá xa
        TooClose,       // Ngồi quá gần (dí sát mặt vào màn hình)
        OutOfSafeZone,  // Lệch khỏi vùng giữa màn hình
        LookingUp,      // Ngước nhìn lên
        LookingDown,    // Cúi gầm mặt
        LookingLeft,    // Liếc/Quay trái
        LookingRight, // Liếc/Quay phải
        Glancing // THÊM MỚI: Trạng thái Liếc Mắt        
    }
    public class FaceAiService : IDisposable
    {

        private VideoCapture? _capture;
        private FaceDetectorYN? _faceDetector;
        private FaceRecognizerSF? _faceRecognizer;

        public void InitializeModels(string detectorPath, string recognizerPath)
        {
            if (!File.Exists(detectorPath) || !File.Exists(recognizerPath))
                throw new Exception("Không tìm thấy file Model AI (YuNet / SFace)!");

            // Khởi tạo Detector
            _faceDetector = new FaceDetectorYN(detectorPath, "", new Size(320, 320));

            // Sửa lỗi: Truy cập trực tiếp vào Enum Backend và Target của Emgu.CV.Dnn
            _faceRecognizer = new FaceRecognizerSF(recognizerPath, "", Emgu.CV.Dnn.Backend.OpenCV, Emgu.CV.Dnn.Target.Cpu);
        }

        public bool StartCamera(int cameraIndex = 0)
        {
            _capture = new VideoCapture(cameraIndex);
            return _capture.IsOpened;
        }

        public Mat? GetFrame()
        {
            if (_capture == null || !_capture.IsOpened) return null;
            Mat frame = new Mat();
            _capture.Read(frame);
            if (frame.IsEmpty) return null;
            return frame;
        }

        public void StopCamera()
        {
            if (_capture != null)
            {
                _capture.Release(); // Giải phóng camera
                _capture.Dispose();
                _capture = null;
            }
        }

        public float[]? ExtractEmbedding(Mat frame)
        {
            if (_faceDetector == null || _faceRecognizer == null) return null;

            _faceDetector.InputSize = new Size(frame.Width, frame.Height);
            using Mat faces = new Mat();
            _faceDetector.Detect(frame, faces);

            if (faces.IsEmpty || faces.Rows < 1) return null;

            using Mat alignedFace = new Mat();
            _faceRecognizer.AlignCrop(frame, faces.Row(0), alignedFace);

            using Mat feature = new Mat();
            _faceRecognizer.Feature(alignedFace, feature);

            float[] embedding = new float[128];
            Marshal.Copy(feature.DataPointer, embedding, 0, 128);

            return embedding;
        }

        public void Dispose()
        {
            StopCamera();
            _faceDetector?.Dispose();
            _faceRecognizer?.Dispose();
        }
        public int GetFaceCount(Mat frame)
        {
            if (_faceDetector == null) return 0;

            // Đặt kích thước đầu vào khớp với kích thước của frame
            _faceDetector.InputSize = new Size(frame.Width, frame.Height);

            using Mat faces = new Mat();
            _faceDetector.Detect(frame, faces);

            if (faces.IsEmpty) return 0;
            return faces.Rows; // Số dòng tương đương với số khuôn mặt phát hiện được
        }
        public bool IsLookingAway(Mat frame)
        {
            if (_faceDetector == null) return false;
            _faceDetector.InputSize = new Size(frame.Width, frame.Height);

            using Mat faces = new Mat();
            _faceDetector.Detect(frame, faces);

            // Mất mặt hoàn toàn -> Tính là quay đi
            if (faces.IsEmpty || faces.Rows == 0) return true;

            float[] faceData = new float[15];
            Marshal.Copy(faces.Row(0).DataPointer, faceData, 0, 15);

            float x = faceData[0], y = faceData[1], w = faceData[2], h = faceData[3];
            float rx = faceData[4], ry = faceData[5]; // Mắt phải
            float lx = faceData[6], ly = faceData[7]; // Mắt trái
            float nx = faceData[8], ny = faceData[9]; // Mũi

            // 1. KIỂM TRA NGỒI LỆCH TÂM (Siết từ 45% xuống 25%)
            // Nếu thí sinh cố tình né qua một bên để chừa góc cho người khác nhìn màn hình
            float faceCenterX = x + w / 2;
            float faceCenterY = y + h / 2;
            float frameCenterX = frame.Width / 2;
            float frameCenterY = frame.Height / 2;

            if (Math.Abs(faceCenterX - frameCenterX) > frame.Width * 0.25f ||
                Math.Abs(faceCenterY - frameCenterY) > frame.Height * 0.25f)
            {
                return true;
            }

            // 2. KIỂM TRA QUAY TRÁI / PHẢI (Yaw)
            float distRightEye = (float)Math.Sqrt(Math.Pow(nx - rx, 2) + Math.Pow(ny - ry, 2));
            float distLeftEye = (float)Math.Sqrt(Math.Pow(nx - lx, 2) + Math.Pow(ny - ly, 2));

            if (distRightEye == 0 || distLeftEye == 0) return false;
            float ratio = distRightEye / distLeftEye;

            // Siết chặt: Lệch tỷ lệ > 1.3 hoặc < 0.75 là bị bắt (Trước đó là 1.6 và 0.6)
            // Nghĩa là chỉ cần ngoảnh mặt một góc nhỏ là dính vi phạm
            if (ratio > 1.3f || ratio < 0.75f)
            {
                return true;
            }

            // 3. KIỂM TRA CÚI MẶT / NGỬA MẶT (Pitch)
            // Dựa vào khoảng cách từ mũi đến đường nối 2 mắt so với chiều cao khuôn mặt
            float eyeCenterY = (ry + ly) / 2;
            float verticalRatio = (ny - eyeCenterY) / h;

            // Siết chặt: < 0.15 là cúi gầm mặt (nhìn điện thoại/tài liệu dưới bàn)
            // > 0.35 là ngửa mặt lên trời
            if (verticalRatio < 0.15f || verticalRatio > 0.35f)
            {
                return true;
            }

            return false; // Nhìn thẳng chuẩn chỉnh
        }
        public FaceStatus AnalyzeFaceBehavior(Mat frame)
        {
            if (_faceDetector == null) return FaceStatus.Normal;
            _faceDetector.InputSize = new Size(frame.Width, frame.Height);

            using Mat faces = new Mat();
            _faceDetector.Detect(frame, faces);

            if (faces.IsEmpty || faces.Rows == 0) return FaceStatus.NoFace;
            if (faces.Rows > 1) return FaceStatus.MultipleFaces;

            // Lấy dữ liệu 5 điểm (Mắt trái, Mắt phải, Mũi, 2 Khóe miệng)
            float[] faceData = new float[15];
            Marshal.Copy(faces.Row(0).DataPointer, faceData, 0, 15);

            float x = faceData[0], y = faceData[1], w = faceData[2], h = faceData[3];
            float rx = faceData[4], ry = faceData[5];   // Mắt phải (trên ảnh)
            float lx = faceData[6], ly = faceData[7];   // Mắt trái
            float nx = faceData[8], ny = faceData[9];   // Mũi
            float mrx = faceData[10], mry = faceData[11]; // Khóe miệng phải
            float mlx = faceData[12], mly = faceData[13]; // Khóe miệng trái

            // 1. TÍNH KHOẢNG CÁCH (Dựa vào tỷ lệ diện tích khuôn mặt / khung hình)
            float faceArea = w * h;
            float frameArea = frame.Width * frame.Height;
            float distanceRatio = faceArea / frameArea;

            if (distanceRatio < 0.05f) return FaceStatus.TooFar;   // Mặt quá nhỏ -> Ngồi quá xa
            if (distanceRatio > 0.50f) return FaceStatus.TooClose; // Mặt quá to -> Dí sát màn hình để đọc trộm

            // 2. VÙNG AN TOÀN (Mặt phải nằm ở trung tâm màn hình, không được nép góc)
            float faceCenterX = x + w / 2;
            float faceCenterY = y + h / 2;
            float frameCenterX = frame.Width / 2;
            float frameCenterY = frame.Height / 2;
            
            // Lệch tâm quá 25% chiều rộng/cao là bị văng ra khỏi "Vùng an toàn"
            if (Math.Abs(faceCenterX - frameCenterX) > frame.Width * 0.25f || 
                Math.Abs(faceCenterY - frameCenterY) > frame.Height * 0.25f)
            {
                return FaceStatus.OutOfSafeZone; 
            }

            // 3. TÍNH GÓC QUAY (HEAD POSE - YAW) - Liếc ngang
            float distNoseToRightEye = (float)Math.Sqrt(Math.Pow(nx - rx, 2) + Math.Pow(ny - ry, 2));
            float distNoseToLeftEye = (float)Math.Sqrt(Math.Pow(nx - lx, 2) + Math.Pow(ny - ly, 2));
            float yawRatio = distNoseToRightEye / (distNoseToLeftEye + 0.001f);

            if (yawRatio > 1.4f) return FaceStatus.LookingLeft;
            if (yawRatio < 0.7f) return FaceStatus.LookingRight;

            // 4. TÍNH GÓC CÚI/NGỬA (HEAD POSE - PITCH) - Nhìn lên/xuống
            float eyeCenterY = (ry + ly) / 2;
            float mouthCenterY = (mry + mly) / 2;
            
            float distNoseToEye = ny - eyeCenterY;       // Mũi cách mắt bao xa
            float distNoseToMouth = mouthCenterY - ny;   // Mũi cách miệng bao xa
            float pitchRatio = distNoseToEye / (distNoseToMouth + 0.001f);

            if (pitchRatio < 0.65f) return FaceStatus.LookingUp;   // Mũi dính sát vào mắt -> Đang ngửa đầu nhìn lên
            if (pitchRatio > 1.35f) return FaceStatus.LookingDown; // Mũi dính sát vào miệng -> Đang cúi gầm mặt
            // 5. ÁNH MẮT (EYE GAZE - Bắt liếc mắt)
            if (CheckEyeGaze(frame, lx, ly, w) || CheckEyeGaze(frame, rx, ry, w))
            {
                return FaceStatus.Glancing; // Đánh cờ Liếc mắt
            }

            return FaceStatus.Normal; // Trạng thái hoàn hảo
        }
        private bool CheckEyeGaze(Mat frame, float eyeX, float eyeY, float faceWidth)
        {
            try
            {
                // Ước lượng độ to của mắt dựa trên tỷ lệ khuôn mặt
                int eyeW = (int)(faceWidth * 0.22f);
                int eyeH = (int)(faceWidth * 0.12f);
                int eyeStartX = (int)(eyeX - eyeW / 2f);
                int eyeStartY = (int)(eyeY - eyeH / 2f);

                // Chặn lỗi văng viền ảnh
                if (eyeStartX < 0 || eyeStartY < 0 || eyeStartX + eyeW > frame.Width || eyeStartY + eyeH > frame.Height)
                    return false;

                Rectangle eyeRect = new Rectangle(eyeStartX, eyeStartY, eyeW, eyeH);
                using Mat eyeMat = new Mat(frame, eyeRect);
                using Mat grayEye = new Mat();

                // Chuyển sang ảnh xám để nhận diện mống mắt/con ngươi dễ hơn
                CvInvoke.CvtColor(eyeMat, grayEye, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

                // Tìm điểm tối nhất (Darkest point) -> Chính là con ngươi
                double minVal = 0, maxVal = 0;
                Point minLoc = new Point(), maxLoc = new Point();
                CvInvoke.MinMaxLoc(grayEye, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                // Tính tỷ lệ vị trí con ngươi trên chiều ngang mắt (0.0 -> 1.0)
                float pupilRatioX = (float)minLoc.X / eyeW;

                // Nếu con ngươi chạy sát về 2 khóe mắt (< 30% hoặc > 70%)
                if (pupilRatioX < 0.30f || pupilRatioX > 0.70f)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}