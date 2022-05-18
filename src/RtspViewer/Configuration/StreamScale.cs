namespace RtspViewer.Configuration
{
    public enum StreamScale
    {
        /// <summary>
        /// Original stream size.
        /// </summary>
        None = 0,

        /// <summary>
        /// Scale down to available space.
        /// Set Image.Stretch to Uniform.
        /// </summary>
        AdjustToFit = 1,

        /// <summary>
        /// Scales image up to cover all available space.
        /// Set Image.Stretch to UniformToFill.
        /// </summary>
        Overscan = 2,

        /// <summary>
        /// Resizes image to fit all available space but, skews the image.
        /// Set Image.Stretch to Fill.
        /// </summary>
        Stretch = 3,

        /// <summary>
        /// Scale down the image to whatever the user asks for.
        /// </summary>
        Manual = 4
    }
}
