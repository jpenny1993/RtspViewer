using System.Drawing;

namespace RtspViewer
{
    public interface IFrameTransformer
    {
        Bitmap TransformToBitmap(IDecodedVideoFrame decodedFrame);

        byte[] TransformToBytes(IDecodedVideoFrame decodedFrame);

        void UpdateFrameSize(int width, int height);
    }
}
