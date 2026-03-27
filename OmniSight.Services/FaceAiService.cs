using Emgu.CV;
using Emgu.CV.Dnn; // Bắt buộc phải có để dùng Backend và Target
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace OmniSight.Services
{
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
    }
}