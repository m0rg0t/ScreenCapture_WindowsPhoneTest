using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Media;
using VideoWatermark1.Helpers;
using VideoWatermark1.Resources;

namespace VideoWatermark1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            // Пример кода для локализации ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private MediaCapture _mediaCapture;

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            var screenCapture = Windows.Media.Capture.ScreenCapture.GetForCurrentView();

            var mcis = new Windows.Media.Capture.MediaCaptureInitializationSettings();
            mcis.VideoSource = screenCapture.VideoSource;
            try
            {
                mcis.AudioSource = screenCapture.AudioSource;
            }
            catch (Exception ex)
            {
            }
            mcis.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video;
                //Windows.Media.Capture.StreamingCaptureMode.AudioAndVideo;

            // Initialize the MediaCapture with the initialization settings.
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync(mcis);

            // Set the MediaCapture to a variable in App.xaml.cs to handle suspension.
            (App.Current as App).MediaCapture = _mediaCapture;

            // Hook up events for the Failed, RecordingLimitationExceeded, and SourceSuspensionChanged events
            _mediaCapture.Failed += new Windows.Media.Capture.MediaCaptureFailedEventHandler(RecordingFailed);
            _mediaCapture.RecordLimitationExceeded +=
                new Windows.Media.Capture.RecordLimitationExceededEventHandler(RecordingReachedLimit);
            screenCapture.SourceSuspensionChanged +=
                new Windows.Foundation.TypedEventHandler<ScreenCapture, SourceSuspensionChangedEventArgs>(SourceSuspensionChanged);
        }

        private void RecordingFailed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
        {
            //throw new NotImplementedException();
        }

        private void RecordingReachedLimit(MediaCapture sender)
        {
            //throw new NotImplementedException();
        }

        private async void StartRecording()
        {

            try
            {
                // Get instance of the ScreenCapture object
                var screenCapture = Windows.Media.Capture.ScreenCapture.GetForCurrentView();

                // Set the MediaCaptureInitializationSettings to use the ScreenCapture as the
                // audio and video source.
                var mcis = new Windows.Media.Capture.MediaCaptureInitializationSettings();
                mcis.VideoSource = screenCapture.VideoSource;
                try
                {
                    mcis.AudioSource = screenCapture.AudioSource;
                }
                catch (Exception ex)
                {
                    
                }
                mcis.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Video;
                    //Windows.Media.Capture.StreamingCaptureMode.AudioAndVideo;

                // Initialize the MediaCapture with the initialization settings.
                _mediaCapture = new MediaCapture();
                //_mediaCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                
                await _mediaCapture.InitializeAsync(mcis);
                _mediaCapture.SetRecordRotation(VideoRotation.Clockwise270Degrees);

                // Set the MediaCapture to a variable in App.xaml.cs to handle suspension.
                (App.Current as App).MediaCapture = _mediaCapture;

                // Hook up events for the Failed, RecordingLimitationExceeded, and SourceSuspensionChanged events
                _mediaCapture.Failed += new Windows.Media.Capture.MediaCaptureFailedEventHandler(RecordingFailed);
                _mediaCapture.RecordLimitationExceeded +=
                    new Windows.Media.Capture.RecordLimitationExceededEventHandler(RecordingReachedLimit);
                screenCapture.SourceSuspensionChanged +=
                    new Windows.Foundation.TypedEventHandler<ScreenCapture, SourceSuspensionChangedEventArgs>(SourceSuspensionChanged);

                // Create a file to record to.             
                //StorageFile sampleFile = await folder.CreateFileAsync("sample.txt", CreationCollisionOption.ReplaceExisting);
                //var videoFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("recording.mp4", CreationCollisionOption.ReplaceExisting);
                StorageFolder folder = KnownFolders.VideosLibrary;
                string currentFileName = DateTime.Now.ToString("yy-MM-dd__hh-mm-ss");
                StorageFile videoFile = await folder.CreateFileAsync(currentFileName+".mp4", CreationCollisionOption.ReplaceExisting);

                /*StorageFile testFile = await folder.CreateFileAsync("test.mp4", CreationCollisionOption.ReplaceExisting);
                byte[] testArray = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
                await FileIO.WriteBytesAsync(testFile, testArray);
                testFile = await folder.GetFileAsync("test.mp4");
                var testArray2 = ReadFile(testFile);
                Debug.WriteLine(testArray2);*/

                try
                {
                    StorageFile eyeFile = await folder.GetFileAsync("eyeLens.mp4");
                    IRandomAccessStreamWithContentType stream = await eyeFile.OpenReadAsync();
                    var items = await eyeFile.Properties.GetVideoPropertiesAsync();
                    //items.
                        //await eyeFile.Properties.GetVideoPropertiesAsync();
                    items.Publisher = "Hello world!";
                    items.SavePropertiesAsync();
                    Debug.WriteLine(items.ToString());
                    /*foreach (var item in items.
                    {
                        Debug.WriteLine(item.ToString());
                    }
                    var exifFile = ExifLibrary.ExifFile.Read(stream.AsStream());
                    Debug.WriteLine(exifFile.Properties.ToString());*/
                }
                catch { }

                // Create an encoding profile to use.                  
                var profile = Windows.Media.MediaProperties.MediaEncodingProfile.CreateMp4(Windows.Media.MediaProperties.VideoEncodingQuality.HD1080p);
                //profile.Container.Properties.Add("MakerNote", "test");
                // Start recording
                await _mediaCapture.StartRecordToStorageFileAsync(profile, videoFile);
                
                //var exifFile = ExifLibrary.ExifFile.Read(videoFile.Path);
                //Debug.WriteLine(exifFile.Properties.ToString());
                
                //_recordingStatus = RecordingStatus.recording;

                // Set a tracking variable for recording state in App.xaml.cs
                (App.Current as App).IsRecording = true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                //NotifyUser("StartRecord Exception: " + ex.Message, NotifyType.ErrorMessage);
            }
        }

        public async Task<byte[]> ReadFile(StorageFile file)
        {
            byte[] fileBytes = null;
            using (IRandomAccessStreamWithContentType stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (DataReader reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }

            return fileBytes;
        }

        //Code for initialization, capture completed, image availability events; also setting the source for the viewfinder.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            // Check to see if the camera is available on the phone.
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                 (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                // Initialize the camera, when available.
                if (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                {
                    // Use front-facing camera if available.
                    //cam = new Microsoft.Devices.PhotoCamera(CameraType.FrontFacing);
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                }
                else
                {
                    // Otherwise, use standard camera on back of phone.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                }

                // Event is fired when the PhotoCamera object has been initialized.
                cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);

                // Event is fired when the capture sequence is complete.
                cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);

                // Event is fired when the capture sequence is complete and an image is available.
                cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);

                // Event is fired when the capture sequence is complete and a thumbnail image is available.
                cam.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureThumbnailAvailable);

                //Set the VideoBrush source to the camera.
                viewfinderBrush.SetSource(cam);
            }
            else
            {
                // The camera is not supported on the phone.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Write message.
                    //txtDebug.Text = "A Camera is not available on this phone.";
                });

                // Disable UI.
                //ShutterButton.IsEnabled = false;
            }
        }

        private void cam_CaptureThumbnailAvailable(object sender, ContentReadyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void cam_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        private int savedCounter = 0;
        PhotoCamera cam;
        MediaLibrary library = new MediaLibrary();

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                // Dispose camera to minimize power consumption and to expedite shutdown.
                cam.Dispose();

                // Release memory, ensure garbage collection.
                cam.Initialized -= cam_Initialized;
                cam.CaptureCompleted -= cam_CaptureCompleted;
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
                cam.CaptureThumbnailAvailable -= cam_CaptureThumbnailAvailable;
            }
        }
        

        private void SourceSuspensionChanged(ScreenCapture sender, SourceSuspensionChangedEventArgs args)
        {
            /*NotifyUser("SourceSuspensionChanged Event. Args: IsAudioSuspended:" +
                args.IsAudioSuspended.ToString() +
                " IsVideoSuspended:" +
                args.IsVideoSuspended.ToString(),
                NotifyType.ErrorMessage);*/
        }

        private void ContentPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
        }

        private void StopRecording()
        {

            try
            {
                //Stop Screen Recorder                  
                var stop_action = _mediaCapture.StopRecordAsync();
                stop_action.Completed += CompletedStop;

                // Set a tracking variable for recording state in App.xaml.cs
                (App.Current as App).IsRecording = false;

            }
            catch (Exception ex)
            {
                //NotifyUser("StopRecord Exception: " + ex.Message, NotifyType.ErrorMessage);
            }
        }

        private void CompletedStop(Windows.Foundation.IAsyncAction asyncInfo, Windows.Foundation.AsyncStatus asyncStatus)
        {
            //throw new NotImplementedException();
        }

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StopRecordingProcess();
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ToggleRecord();
        }

        private void ToggleRecord()
        {
            if ((App.Current as App).IsRecording == false)
            {
                StartRecordingProcess();
            }
            else
            {
                StopRecordingProcess();
            }
        }

        /// <summary>
        /// Start recording process and hide buttons and so on
        /// </summary>
        private void StartRecordingProcess()
        {
            VibrateController testVibrateController = VibrateController.Default;
            testVibrateController.Stop();

            RecordButton.Visibility = Visibility.Collapsed;

            var totpHelper = new TotpHelper();
            TotpTextBlock.Text = totpHelper.GetTOTP();

            StartRecording();

            timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromSeconds(0.5); 
            timer.Start();

            testVibrateController.Start(TimeSpan.FromSeconds(0.2));

            (App.Current as App).IsRecording = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var totpHelper = new TotpHelper();
            TotpTextBlock.Text = totpHelper.GetTOTP();
            //throw new NotImplementedException();
        }

        private DispatcherTimer timer; 

        private void StopRecordingProcess()
        {
            try
            {
                if ((App.Current as App).IsRecording == true)
                {
                    VibrateController testVibrateController = VibrateController.Default;
                    testVibrateController.Stop();

                    timer.Stop();

                    RecordButton.Visibility = Visibility.Visible;
                    StopRecording();
                    testVibrateController.Start(TimeSpan.FromSeconds(2));
                    (App.Current as App).IsRecording = false;
                }
            }
            catch { }
        }

        // Пример кода для сборки локализованной панели ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Установка в качестве ApplicationBar страницы нового экземпляра ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Создание новой кнопки и установка текстового значения равным локализованной строке из AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Создание нового пункта меню с локализованной строкой из AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}