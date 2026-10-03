// TEST ONLY: in-memory platform surface for compiling the entire production page.
// Recording/storage/authentication operations throw rather than access real resources.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace System.Windows
{
    public class RoutedEventArgs : EventArgs { }
    public enum Visibility { Visible, Collapsed }
}
namespace System.Windows.Controls
{
    public class Button { public System.Windows.Visibility Visibility { get; set; } }
    public class TextBlock { public string Text { get; set; } }
}
namespace System.Windows.Input { public class GestureEventArgs : EventArgs { } }
namespace System.Windows.Navigation
{
    public class NavigationEventArgs : EventArgs { }
    public class NavigatingCancelEventArgs : EventArgs { }
}
namespace System.Windows.Threading
{
    public class Dispatcher { public void BeginInvoke(Action action) { action(); } }
    public class DispatcherTimer
    {
        public event EventHandler Tick;
        public TimeSpan Interval { get; set; }
        public void Start() { throw new NotSupportedException("Timer stub"); }
        public void Stop() { throw new NotSupportedException("Timer stub"); }
    }
}
namespace Microsoft.Phone.Controls
{
    public class PhoneApplicationPage
    {
        public System.Windows.Threading.Dispatcher Dispatcher = new System.Windows.Threading.Dispatcher();
        protected virtual void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) { }
        protected virtual void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e) { }
    }
}
namespace Microsoft.Phone.Shell { internal class NamespaceMarker { } }
namespace Microsoft.Xna.Framework.Media { public class MediaLibrary { } }
namespace Microsoft.Devices
{
    public enum CameraType { Primary, FrontFacing }
    public class CameraOperationCompletedEventArgs : EventArgs { }
    public class ContentReadyEventArgs : EventArgs { }
    public sealed class PhotoCamera : IDisposable
    {
        public static readonly List<PhotoCamera> Created = new List<PhotoCamera>();
        private static bool primarySupported;
        private static bool frontSupported;
        public CameraType Type { get; private set; }
        public bool Disposed { get; private set; }
        public event EventHandler<CameraOperationCompletedEventArgs> Initialized;
        public event EventHandler<CameraOperationCompletedEventArgs> CaptureCompleted;
        public event EventHandler<ContentReadyEventArgs> CaptureImageAvailable;
        public event EventHandler<ContentReadyEventArgs> CaptureThumbnailAvailable;
        public int SubscriptionCount
        {
            get
            {
                return Count(Initialized) + Count(CaptureCompleted) + Count(CaptureImageAvailable) + Count(CaptureThumbnailAvailable);
            }
        }
        private static int Count(Delegate value) { return value == null ? 0 : value.GetInvocationList().Length; }
        public static void Reset(bool primary, bool front)
        {
            Created.Clear();
            primarySupported = primary;
            frontSupported = front;
        }
        public static bool IsCameraTypeSupported(CameraType type)
        {
            return type == CameraType.Primary ? primarySupported : frontSupported;
        }
        public PhotoCamera(CameraType type)
        {
            if (!IsCameraTypeSupported(type)) throw new InvalidOperationException("Unsupported camera: " + type);
            Type = type;
            Created.Add(this);
        }
        public void Dispose() { Disposed = true; }
    }
    public class VibrateController
    {
        public static VibrateController Default { get { throw new NotSupportedException("Vibration stub"); } }
        public void Start(TimeSpan duration) { throw new NotSupportedException(); }
        public void Stop() { throw new NotSupportedException(); }
    }
}
namespace Windows.Foundation
{
    public delegate void TypedEventHandler<TSender, TResult>(TSender sender, TResult args);
    public enum AsyncStatus { Completed }
    public delegate void AsyncActionCompletedHandler(IAsyncAction action, AsyncStatus status);
    public interface IAsyncAction { AsyncActionCompletedHandler Completed { get; set; } }
}
namespace Windows.Media.Capture
{
    public enum StreamingCaptureMode { Video }
    public enum VideoRotation { Clockwise270Degrees }
    public class SourceSuspensionChangedEventArgs : EventArgs { }
    public class MediaCaptureFailedEventArgs : EventArgs { }
    public delegate void MediaCaptureFailedEventHandler(MediaCapture sender, MediaCaptureFailedEventArgs args);
    public delegate void RecordLimitationExceededEventHandler(MediaCapture sender);
    public class MediaCaptureInitializationSettings
    {
        public object VideoSource { get; set; }
        public object AudioSource { get; set; }
        public StreamingCaptureMode StreamingCaptureMode { get; set; }
    }
    public class ScreenCapture
    {
        public static ScreenCapture GetForCurrentView() { throw new NotSupportedException("Screen capture stub"); }
        public object VideoSource { get; set; }
        public object AudioSource { get; set; }
        public event Windows.Foundation.TypedEventHandler<ScreenCapture, SourceSuspensionChangedEventArgs> SourceSuspensionChanged;
    }
    public class MediaCapture
    {
        public MediaCapture() { throw new NotSupportedException("Media capture stub"); }
        public event MediaCaptureFailedEventHandler Failed;
        public event RecordLimitationExceededEventHandler RecordLimitationExceeded;
        public Task InitializeAsync(MediaCaptureInitializationSettings settings) { throw new NotSupportedException(); }
        public void SetRecordRotation(VideoRotation rotation) { throw new NotSupportedException(); }
        public Task StartRecordToStorageFileAsync(Windows.Media.MediaProperties.MediaEncodingProfile profile, Windows.Storage.StorageFile file) { throw new NotSupportedException(); }
        public Windows.Foundation.IAsyncAction StopRecordAsync() { throw new NotSupportedException(); }
    }
}
namespace Windows.Media.MediaProperties
{
    public enum VideoEncodingQuality { HD1080p }
    public class MediaEncodingProfile
    {
        public static MediaEncodingProfile CreateMp4(VideoEncodingQuality quality) { throw new NotSupportedException(); }
    }
}
namespace Windows.Storage
{
    public enum CreationCollisionOption { ReplaceExisting }
    public class StorageFolder
    {
        public Task<StorageFile> CreateFileAsync(string name, CreationCollisionOption option) { throw new NotSupportedException("Storage stub"); }
        public Task<StorageFile> GetFileAsync(string name) { throw new NotSupportedException("Storage stub"); }
    }
    public static class KnownFolders
    {
        public static StorageFolder VideosLibrary { get { throw new NotSupportedException("Storage stub"); } }
    }
    public class StorageFile
    {
        public Task<Streams.IRandomAccessStreamWithContentType> OpenReadAsync() { throw new NotSupportedException(); }
        public FileProperties Properties { get { throw new NotSupportedException(); } }
    }
    public class FileProperties
    {
        public Task<VideoProperties> GetVideoPropertiesAsync() { throw new NotSupportedException(); }
    }
    public class VideoProperties
    {
        public string Publisher { get; set; }
        public Task SavePropertiesAsync() { throw new NotSupportedException(); }
    }
}
namespace Windows.Storage.Streams
{
    public interface IRandomAccessStreamWithContentType : IDisposable { ulong Size { get; } }
    public class DataReader : IDisposable
    {
        public DataReader(IRandomAccessStreamWithContentType stream) { throw new NotSupportedException(); }
        public Task<uint> LoadAsync(uint count) { throw new NotSupportedException(); }
        public void ReadBytes(byte[] bytes) { throw new NotSupportedException(); }
        public void Dispose() { }
    }
}
namespace VideoWatermark1.Helpers
{
    public class TotpHelper { public string GetTOTP() { throw new NotSupportedException("Authentication stub"); } }
}
namespace VideoWatermark1.Resources { internal class NamespaceMarker { } }
namespace VideoWatermark1
{
    public class App
    {
        public static App Current { get { throw new NotSupportedException("Application stub"); } }
        public Windows.Media.Capture.MediaCapture MediaCapture { get; set; }
        public bool IsRecording { get; set; }
    }
    public class TestVideoBrush
    {
        public Microsoft.Devices.PhotoCamera Source { get; private set; }
        public void SetSource(Microsoft.Devices.PhotoCamera camera) { Source = camera; }
    }
    public partial class MainPage
    {
        private readonly TestVideoBrush viewfinderBrush = new TestVideoBrush();
        private readonly System.Windows.Controls.Button RecordButton = new System.Windows.Controls.Button();
        private readonly System.Windows.Controls.TextBlock TotpTextBlock = new System.Windows.Controls.TextBlock();
        private void InitializeComponent() { }
        public TestVideoBrush TestViewfinder { get { return viewfinderBrush; } }
        public void TestNavigateTo() { OnNavigatedTo(new System.Windows.Navigation.NavigationEventArgs()); }
        public void TestNavigateFrom() { OnNavigatingFrom(new System.Windows.Navigation.NavigatingCancelEventArgs()); }
    }
}
