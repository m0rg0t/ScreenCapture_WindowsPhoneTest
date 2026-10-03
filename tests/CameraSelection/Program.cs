using System;
using Microsoft.Devices;
using VideoWatermark1;

internal static class Program
{
    private static int Main()
    {
        int failed = 0;
        failed += Run("primary only", true, false, CameraType.Primary);
        failed += Run("front only", false, true, CameraType.FrontFacing);
        failed += Run("both cameras prefer front", true, true, CameraType.FrontFacing);
        failed += Run("no cameras", false, false, null);
        Console.WriteLine("Results: " + (4 - failed) + " passed, " + failed + " failed");
        return failed == 0 ? 0 : 1;
    }

    private static int Run(string name, bool primary, bool front, CameraType? expected)
    {
        PhotoCamera.Reset(primary, front);
        try
        {
            var page = new MainPage();
            page.TestNavigateTo();
            Assert(PhotoCamera.Created.Count == (expected.HasValue ? 1 : 0), "camera allocation count");
            if (expected.HasValue)
            {
                var camera = PhotoCamera.Created[0];
                Assert(camera.Type == expected.Value, "selected camera type");
                Assert(object.ReferenceEquals(page.TestViewfinder.Source, camera), "viewfinder source");
                Assert(camera.SubscriptionCount == 4, "camera event subscriptions");
                page.TestNavigateFrom();
                Assert(camera.Disposed, "camera disposed on departure");
                Assert(camera.SubscriptionCount == 0, "event handlers removed on departure");
            }
            else
            {
                Assert(page.TestViewfinder.Source == null, "no viewfinder source");
                page.TestNavigateFrom();
            }
            Console.WriteLine("PASS " + name);
            return 0;
        }
        catch (Exception e)
        {
            Console.WriteLine("FAIL " + name + ": " + e.Message);
            return 1;
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}
