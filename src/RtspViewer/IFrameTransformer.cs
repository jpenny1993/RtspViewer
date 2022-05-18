using System;
using System.Drawing;

namespace RtspViewer
{
    public interface IFrameTransformer : IDisposable
    {
        Bitmap TransformToBitmap(IDecodedVideoFrame decodedFrame);

        Bitmap UpdateFrameSize(int width, int height);
    }
}
