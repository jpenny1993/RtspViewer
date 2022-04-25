using System.Drawing;

namespace RtspViewer
{
    public interface IFrameTransformer
    {
        Bitmap TransformToBitmap(IDecodedVideoFrame decodedFrame);

        Bitmap UpdateFrameSize(int width, int height);
    }
}
