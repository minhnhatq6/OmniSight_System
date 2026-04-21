using Emgu.CV;
using Emgu.CV.Dnn;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace OmniSight.Services
{
    public enum FaceStatus
    {
        Normal,         // Bình thường
        NoFace,         // Không thấy mặt
        MultipleFaces,  // Có nhiều hơn 1 mặt
        TooFar,         // Ngồi quá xa
        TooClose,       // Ngồi quá gần
        OutOfSafeZone,  // Ngồi lệch lề màn hình
        LookingUp,      // Ngửa mặt lên
        LookingDown,    // Cúi mặt xuống
        LookingLeft,    // Quay mặt trái
        LookingRight,   // Quay mặt phải
        Glancing        // Liếc mắt (đầu thẳng nhưng mắt nhìn chỗ khác)
    }

    public class FaceAiService : IDisposable
    {
        // =========================================================================
        // 🛠️ KHU VỰC CẤU HÌNH NGƯỠNG VI PHẠM (CHỈ CHỈNH SỬA TẠI ĐÂY)
        // =========================================================================

        // 1. Tư thế đầu (Head Pose)
        private const float YAW_HIGH = 1.35f; // Quay trái (yawRatio > 1.35)
        private const float YAW_LOW = 0.75f; // Quay phải (yawRatio < 0.75)
        private const float PITCH_UP = 1.00f; // Ngửa lên (pitchRatio < 1.00)
        private const float PITCH_DOWN = 1.65f; // Cúi xuống (CỦA BẠN LÀ 1.37 NHƯNG NÊN ĐỂ > 1.50 THEO ẢNH TEST)

        // 2. Liếc mắt (Eye Gaze) - Tọa độ con ngươi trong hốc mắt (0.0 -> 1.0)
        private const float GAZE_MIN_X = 0.40f; // Mắt liếc trái (Pupil X < 0.30)
        private const float GAZE_MAX_X = 0.60f; // Mắt liếc phải (Pupil X > 0.70)
        private const float GAZE_MIN_Y = 0.35f; // Mắt liếc lên (Pupil Y < 0.35)
        private const float GAZE_MAX_Y = 0.88f; // Mắt liếc xuống (Pupil Y > 0.88)

        // 3. Khoảng cách và Vùng an toàn
        private const float DIST_TOO_FAR = 0.04f; // Mặt nhỏ hơn 4% diện tích ảnh là quá xa
        private const float DIST_TOO_CLOSE = 0.55f; // Mặt lớn hơn 55% diện tích ảnh là quá gần
        private const float SAFE_ZONE_LIMIT = 0.30f; // Lệch quá 30% tâm màn hình là báo lỗi

        // =========================================================================

        private VideoCapture? _capture;
        private FaceDetectorYN? _faceDetector;
        private FaceRecognizerSF? _faceRecognizer;

        public void InitializeModels(string detectorPath, string recognizerPath)
        {
            if (!File.Exists(detectorPath) || !File.Exists(recognizerPath))
                throw new Exception("Không tìm thấy file Model AI!");

            _faceDetector = new FaceDetectorYN(detectorPath, "", new Size(320, 320));
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
            return frame.IsEmpty ? null : frame;
        }

        public void StopCamera()
        {
            if (_capture != null) { _capture.Release(); _capture.Dispose(); _capture = null; }
        }

        // --- HÀM LOGIC CHÍNH: PHÂN TÍCH HÀNH VI ---
        public FaceStatus AnalyzeFaceBehavior(Mat frame)
        {
            if (_faceDetector == null) return FaceStatus.Normal;
            _faceDetector.InputSize = new Size(frame.Width, frame.Height);

            using Mat faces = new Mat();
            _faceDetector.Detect(frame, faces);

            // 1. Kiểm tra số lượng mặt
            if (faces.IsEmpty || faces.Rows == 0) return FaceStatus.NoFace;
            if (faces.Rows > 1) return FaceStatus.MultipleFaces;

            // 2. Trích xuất dữ liệu 5 điểm (Mắt trái, Mắt phải, Mũi, 2 khóe miệng)
            float[] d = new float[15];
            Marshal.Copy(faces.Row(0).DataPointer, d, 0, 15);

            float x = d[0], y = d[1], w = d[2], h = d[3]; // Tọa độ mặt
            float rx = d[4], ry = d[5], lx = d[6], ly = d[7]; // Mắt phải, mắt trái
            float nx = d[8], ny = d[9]; // Mũi
            float mrx = d[10], mry = d[11], mlx = d[12], mly = d[13]; // Miệng

            // 3. KIỂM TRA KHOẢNG CÁCH & VÙNG AN TOÀN
            float areaRatio = (w * h) / (float)(frame.Width * frame.Height);
            if (areaRatio < DIST_TOO_FAR) return FaceStatus.TooFar;
            if (areaRatio > DIST_TOO_CLOSE) return FaceStatus.TooClose;

            float faceCenterX = x + w / 2;
            if (Math.Abs(faceCenterX - frame.Width / 2f) > frame.Width * SAFE_ZONE_LIMIT)
                return FaceStatus.OutOfSafeZone;

            // 4. KIỂM TRA TƯ THẾ ĐẦU (YAW - Quay ngang)
            float distNoseToRightEye = (float)Math.Sqrt(Math.Pow(nx - rx, 2) + Math.Pow(ny - ry, 2));
            float distNoseToLeftEye = (float)Math.Sqrt(Math.Pow(nx - lx, 2) + Math.Pow(ny - ly, 2));
            float yawRatio = distNoseToRightEye / (distNoseToLeftEye + 0.001f);

            if (yawRatio > YAW_HIGH) return FaceStatus.LookingLeft;
            if (yawRatio < YAW_LOW) return FaceStatus.LookingRight;

            // 5. KIỂM TRA CÚI/NGỬA (PITCH)
            float eyeCenterY = (ry + ly) / 2;
            float mouthCenterY = (mry + mly) / 2;
            float pitchRatio = (ny - eyeCenterY) / (mouthCenterY - ny + 0.001f);

            if (pitchRatio < PITCH_UP) return FaceStatus.LookingUp;
            if (pitchRatio > PITCH_DOWN) return FaceStatus.LookingDown;

            // 6. KIỂM TRA LIẾC MẮT (Dùng hằng số cấu hình ở trên)
            if (CheckIndividualGaze(frame, lx, ly, w)) return FaceStatus.Glancing;

            return FaceStatus.Normal;
        }

        private bool CheckIndividualGaze(Mat frame, float eyeX, float eyeY, float faceWidth)
        {
            try
            {
                // Cắt vùng mắt
                int ew = (int)(faceWidth * 0.22f), eh = (int)(faceWidth * 0.12f);
                int ex = (int)(eyeX - ew / 2f), ey = (int)(eyeY - eh / 2f);

                if (ex < 0 || ey < 0 || ex + ew > frame.Width || ey + eh > frame.Height) return false;

                using Mat eye = new Mat(frame, new Rectangle(ex, ey, ew, eh));
                using Mat gray = new Mat();
                CvInvoke.CvtColor(eye, gray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

                double minV = 0, maxV = 0; Point minL = new Point(), maxL = new Point();
                CvInvoke.MinMaxLoc(gray, ref minV, ref maxV, ref minL, ref maxL);

                float pupilX = (float)minL.X / ew;
                float pupilY = (float)minL.Y / eh;

                // So sánh với các hằng số ở đầu file
                return (pupilX < GAZE_MIN_X || pupilX > GAZE_MAX_X ||
                        pupilY < GAZE_MIN_Y || pupilY > GAZE_MAX_Y);
            }
            catch { return false; }
        }

        // --- HÀM FACE ID (TRÍCH XUẤT VECTOR) ---
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
    }
}