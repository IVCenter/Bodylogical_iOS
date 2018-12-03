using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.XR.WSA.WebCam;

public class VideoCaptureHandler : MonoBehaviour
{
    VideoCapture m_VideoCapture = null;
    float m_stopRecordingTimer = float.MaxValue;
    private bool recording;

    PhotoCapture photoCaptureObject = null;
    public Texture2D targetTexture;
    //Resolution cameraResolution;
    // Use this for initialization
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !recording)
        {
            StartVideoCapture();
            recording = true;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
            recording = false;
        }
    }



    #region photo
    public void CapturePhoto()
    {
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width + res.height).First();
        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, onPhotoModeStarted);
    }

    void onPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);
        }
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }
#endregion

    #region Video
    //PUBLIC
    public void StartRecording()
    {
        StartVideoCapture();
        recording = true;
    }
    public void StopRecording()
    {
        m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
        recording = false;
    }


    void StartVideoCapture()
    {
        // New Stuff
        VideoCapture.CreateAsync(true, OnVideoCaptureCreated);
        
    }

    void OnVideoCaptureCreated( VideoCapture videoCapture)
    {
        if (videoCapture != null)
        {
            m_VideoCapture = videoCapture;

            Resolution cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            float cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();

            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 1.0f;
            cameraParameters.frameRate = cameraFramerate;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            m_VideoCapture.StartVideoModeAsync(cameraParameters,
                                                VideoCapture.AudioState.ApplicationAndMicAudio,
                                                OnStartedVideoCaptureMode);
        }
        else
        {
            Debug.LogError("Failed to create VideoCapture Instance!");
        }
    }


    void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Started Video Capture Mode!");
            //string timeStamp = Time.time.ToString().Replace(".", "").Replace(":", "");
            //string filename = string.Format("TestVideo_{0}.mp4", timeStamp);
            //string filepath = System.IO.Path.Combine(Application.persistentDataPath, filename);

#if WINDOWS_UWP
            //filepath = System.IO.Path.Combine(Windows.Storage.KnownFolders.CameraRoll.Path, filename);

            //static async void SaveData(byte[] data)
            //{
            //    Windows.Storage.StorageFolder storageFolder = Windows.Storage.KnownFolders.CameraRoll;
            //    Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync("anchors.dat", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            //}
#endif
            //filepath = filepath.Replace("/", @"\");

            string filename = string.Format("HoloLensRecording_{0}.mp4", Time.time);
            string filepath = System.IO.Path.Combine(Application.persistentDataPath, filename);

            m_VideoCapture.StartRecordingAsync(filepath, OnStartedRecordingVideo);
            Debug.Log("Saving to: " + filepath);
        }
    }

    void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Video Capture Mode!");
        m_VideoCapture.Dispose();
        m_VideoCapture = null;
    }

    void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Recording Video!");
        //m_stopRecordingTimer = Time.time + MaxRecordingTime;
    }

    void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Recording Video!");
        m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
    }


#endregion

}