
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Erwine.Leonard.T.GDIPlus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Represents EXIF image property tag values.
    /// </summary>
    /// <remarks>Many of the definitions were obtained from the document at https://msdn.microsoft.com/en-us/library/ms534416.aspx.</remarks>
    public enum ExifPropertyTag : int
    {
        /// <summary>
        /// Unknown property type
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Version of the Global Positioning Systems (GPS) IFD, given as 2.0.0.0.
        /// </summary>
        /// <remarks>This tag is mandatory when the <see cref="GpsIFD" /> tag is present. When the version is 2.0.0.0, the tag value is 0x02000000.</remarks>
        [ExifPropertyType(ExifPropertyType.Byte, Count = 4)]
        [ExifPropertyDescription("GPS Version", Summary = "Version of the Global Positioning Systems (GPS) IFD, given as 2.0.0.0.",
            Remarks = "This tag is mandatory when the ExifPropertyType.GpsIFD tag is present.")]
        [ExifItemDisplayName(0, "Major")]
        [ExifItemDisplayName(1, "Minor")]
        [ExifItemDisplayName(2, "Build")]
        [ExifItemDisplayName(3, "Revision")]
        GpsVer = 0x0000,

        /// <summary>
        /// Specifies whether the latitude is north or south.
        /// </summary>
        /// <remarks>one character plus the NULL terminator.
        /// <para>N specifies north latitude, and S specifies south latitude.</para></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Latitude Reference",
            Summary = "Character string that specifies whether the latitude is north or south.",
            Remarks = "N specifies north latitude, and S specifies south latitude.")]
        [ExifValueTranslation('N', "North latitude")]
        [ExifValueTranslation('S', "South latitude")]
        GpsLatitudeRef = 0x0001,

        /// <summary>
        /// Latitude.
        /// </summary>
        /// <remarks>Latitude is expressed as three rational values giving the degrees, minutes, and seconds respectively.
        /// When degrees, minutes, and seconds are expressed, the format is dd/1, mm/1, ss/1.
        /// When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is dd/1, mmmm/100, 0/1.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 3)]
        [ExifPropertyDescription("Latitude", Summary = "Latitude.")]
        [ExifItemDisplayName(0, "Degrees")]
        [ExifItemDisplayName(1, "Minutes")]
        [ExifItemDisplayName(2, "Seconds")]
        GpsLatitude = 0x0002,

        /// <summary>
        /// Specifies whether the longitude is east or west longitude.
        /// </summary>
        /// <remarks>one character plus the NULL terminator.
        /// <para>E specifies east longitude, and W specifies west longitude.</para></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Longitude Reference",
            Summary = "Character that specifies whether the longitude is east or west longitude.",
                Remarks = "E specifies east longitude, and W specifies west longitude.")]
        GpsLongitudeRef = 0x0003,

        /// <summary>
        /// Longitude.
        /// </summary>
        /// <remarks>Longitude is expressed as three rational values giving the degrees, minutes, and seconds respectively.
        /// When degrees, minutes and seconds are expressed, the format is ddd/1, mm/1, ss/1.
        /// When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is ddd/1, mmmm/100, 0/1.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 3)]
        [ExifPropertyDescription("Longitude", Summary = "Longitude.")]
        [ExifItemDisplayName(0, "Degrees")]
        [ExifItemDisplayName(1, "Minutes")]
        [ExifItemDisplayName(2, "Seconds")]
        GpsLongitude = 0x0004,

        /// <summary>
        /// Reference altitude, in meters.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("Reference altitude", Summary = "Reference altitude, in meters.")]
        GpsAltitudeRef = 0x0005,

        /// <summary>
        /// Altitude, in meters, based on the reference altitude specified by <see cref="GpsAltitudeRef" />.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Altitude", 
            Summary = "Altitude, in meters, based on the reference altitude specified by ExifPropertyType.GpsAltitudeRef.")]
        GpsAltitude = 0x0006,

        /// <summary>
        /// Time as coordinated universal time (UTC).
        /// </summary>
        /// <remarks>The value is expressed as three rational numbers that give the hour, minute, and second.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 3)]
        [ExifPropertyDescription("Time", Summary = "Time as coordinated universal time (UTC).",
            Remarks = "The value is expressed as three rational numbers that give the hour, minute, and second.")]
        [ExifItemDisplayName(0, "Hours")]
        [ExifItemDisplayName(1, "Minutes")]
        [ExifItemDisplayName(2, "Seconds")]
        GpsGpsTime = 0x0007,

        /// <summary>
        /// Specifies the GPS satellites used for measurements.
        /// </summary>
        /// <remarks>This tag can be used to specify the ID number, angle of elevation, azimuth, SNR, and other information about each satellite.
        /// The format is not specified. If the GPS receiver is incapable of taking measurements, the value of the tag must be set to NULL.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("GPS Satellites",
            Summary = "NSpecifies the GPS satellites used for measurements.",
            Remarks = @"This tag can be used to specify the ID number, angle of elevation, azimuth, SNR, and other information about each satellite.
The format is not specified. If the GPS receiver is incapable of taking measurements, the value of the tag must be set to NULL.")]
        GpsGpsSatellites = 0x0008,

        /// <summary>
        /// Specifies the status of the GPS receiver when the image is recorded.
        /// </summary>
        /// <remarks>one character plus the NULL terminator.
        /// <para>><list type="bullet">
        /// <item><term>A</term><description>means measurement is in progress.</description></item>
        /// <item><term>V</term><description>means the measurement is Interoperability.</description></item>
        /// </list></para></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Gps Status",
            Summary = "Specifies the status of the GPS receiver when the image is recorded.",
            Remarks = "A means measurement is in progress, and V means the measurement is Interoperability.")]
        [ExifValueTranslation('A', "Progress")]
        [ExifValueTranslation('V', "Interoperability")]
        GpsGpsStatus = 0x0009,

        /// <summary>
        /// Specifies the GPS measurement mode.
        /// </summary>
        /// <remarks><list type="number">
        /// <item><term>2</term><description>specifies 2-D measurement.</description></item>
        /// <item><term>3</term><description>specifies 3-D measurement.</description></item>
        /// </list></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Gps Measure Mode",
            Summary = "Specifies the GPS measurement mode.",
            Remarks = "2 specifies 2-D measurement, and 3 specifies 3-D measurement.")]
        [ExifValueTranslation('2', "2-D")]
        [ExifValueTranslation('3', "3-D")]
        GpsGpsMeasureMode = 0x000A,

        /// <summary>
        /// GPS DOP (data degree of precision).
        /// </summary>
        /// <remarks>An HDOP value is written during 2-D measurement, and a PDOP value is written during 3-D measurement.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("GPS DOP", Summary = "GPS DOP (data degree of precision).",
            Remarks = "An HDOP value is written during 2-D measurement, and a PDOP value is written during 3-D measurement.")]
        GpsGpsDop = 0x000B,

        /// <summary>
        /// Specifies the unit used to express the GPS receiver speed of movement.
        /// </summary>
        /// <remarks><list type="number">
        /// <item><term>K</term><description>represents kilometers per hour.</description></item>
        /// <item><term>M</term><description>represents miles per hour.</description></item>
        /// <item><term>N</term><description>represents knots.</description></item>
        /// </list></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Speed Reference",
            Summary = "Specifies the unit used to express the GPS receiver speed of movement.",
            Remarks = "K, M, and N represent kilometers per hour, miles per hour, and knots respectively.")]
        [ExifValueTranslation('K', "Kilometers per hour")]
        [ExifValueTranslation('M', "Miles per hour")]
        [ExifValueTranslation('N', "Knots")]
        GpsSpeedRef = 0x000C,

        /// <summary>
        /// Speed of the GPS receiver movement.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("GPS Receiver Speed", Summary = "Speed of the GPS receiver movement.")]
        GpsSpeed = 0x000D,

        /// <summary>
        /// Specifies the reference for giving the direction of GPS receiver movement.
        /// </summary>
        /// <remarks><list type="number">
        /// <item><term>T</term><description>specifies true direction.</description></item>
        /// <item><term>M</term><description>specifies magnetic direction.</description></item>
        /// </list></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("GPS Track Reference",
            Summary = "Specifies the reference for giving the direction of GPS receiver movement.",
            Remarks = "T specifies true direction, and M specifies magnetic direction.")]
        [ExifValueTranslation('T', "True direction")]
        [ExifValueTranslation('M', "Magnetic direction")]
        GpsTrackRef = 0x000E,

        /// <summary>
        /// Direction of GPS receiver movement.
        /// </summary>
        /// <remarks>The range of values is from 0.00 to 359.99.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("GPS Track", Summary = "Direction of GPS receiver movement.",
            Remarks = "The range of values is from 0.00 to 359.99.")]
        GpsTrack = 0x000F,

        /// <summary>
        /// Specifies the reference for the direction of the image when it is captured.
        /// </summary>
        /// <remarks><list type="number">
        /// <item><term>T</term><description>specifies true direction.</description></item>
        /// <item><term>M</term><description>specifies magnetic direction.</description></item>
        /// </list></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Image Direction Reference",
            Summary = "Specifies the reference for the direction of the image when it is captured.",
            Remarks = "T specifies true direction, and M specifies magnetic direction.")]
        [ExifValueTranslation('T', "True direction")]
        [ExifValueTranslation('M', "Magnetic direction")]
        GpsImgDirRef = 0x0010,

        /// <summary>
        /// Direction of the image when it was captured.
        /// </summary>
        /// <remarks>The range of values is from 0.00 to 359.99.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Image Direction", Summary = "Direction of the image when it was captured.",
            Remarks = "The range of values is from 0.00 to 359.99.")]
        GpsImgDir = 0x0011,

        /// <summary>
        /// Specifies geodetic survey data used by the GPS receiver.
        /// </summary>
        /// <remarks>If the survey data is restricted to Japan, the value of this tag is TOKYO or WGS-84.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Geodetic survey data",
            Summary = "Specifies geodetic survey data used by the GPS receiver.",
            Remarks = "If the survey data is restricted to Japan, the value of this tag is TOKYO or WGS-84.")]
        GpsMapDatum = 0x0012,

        /// <summary>
        /// Specifies whether the latitude of the destination point is north or south latitude.
        /// </summary>
        /// <remarks>N specifies north latitude, and S specifies south latitude.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Destination Latitude Reference",
            Summary = "Specifies whether the latitude of the destination point is north or south latitude.",
            Remarks = "N specifies north latitude, and S specifies south latitude.")]
        [ExifValueTranslation('N', "North latitude")]
        [ExifValueTranslation('S', "South latitude")]
        GpsDestLatRef = 0x0013,

        /// <summary>
        /// Latitude of the destination point.
        /// </summary>
        /// <remarks>The latitude is expressed as three rational values giving the degrees, minutes, and seconds respectively.
        /// When degrees, minutes, and seconds are expressed, the format is dd/1, mm/1, ss/1.
        /// When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is dd/1, mmmm/100, 0/1.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 3)]
        [ExifPropertyDescription("Destination Latitude", Summary = "Latitude of the destination point.",
            Remarks = @"The latitude is expressed as three rational values giving the degrees, minutes, and seconds respectively.
When degrees, minutes, and seconds are expressed, the format is dd/1, mm/1, ss/1.
When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is dd/1, mmmm/100, 0/1.")]
        GpsDestLat = 0x0014,

        /// <summary>
        /// Specifies whether the longitude of the destination point is east or west longitude.
        /// </summary>
        /// <remarks>E specifies east longitude, and W specifies west longitude.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Destination Longitude Reference", 
            Summary = "Specifies whether the longitude of the destination point is east or west longitude.",
            Remarks = "E specifies east longitude, and W specifies west longitude.")]
        [ExifValueTranslation('E', "East longitude")]
        [ExifValueTranslation('W', "West longitude")]
        GpsDestLongRef = 0x0015,

        /// <summary>
        /// Longitude of the destination point.
        /// </summary>
        /// <remarks>The longitude is expressed as three rational values giving the degrees, minutes, and seconds respectively.
        /// When degrees, minutes, and seconds are expressed, the format is ddd/1, mm/1, ss/1.
        /// When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is ddd/1, mmmm/100, 0/1.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 3)]
        [ExifPropertyDescription("Destination Longitude", Summary = "Longitude of the destination point.",
            Remarks = @"The longitude is expressed as three rational values giving the degrees, minutes, and seconds respectively.
When degrees, minutes, and seconds are expressed, the format is ddd/1, mm/1, ss/1.
When degrees and minutes are used and, for example, fractions of minutes are given up to two decimal places, the format is ddd/1, mmmm/100, 0/1.")]
        GpsDestLong = 0x0016,

        /// <summary>
        /// Specifies the reference used for giving the bearing to the destination point.
        /// </summary>
        /// <remarks>one character plus the NULL terminator.
        /// <para>T specifies true direction, and M specifies magnetic direction.</para></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Destination Bearing Reference", Summary = "Specifies the reference used for giving the bearing to the destination point.",
            Remarks = @"one character plus the NULL terminator.
T specifies true direction, and M specifies magnetic direction.")]
        GpsDestBearRef = 0x0017,

        /// <summary>
        /// Bearing to the destination point.
        /// </summary>
        /// <remarks>The range of values is from 0.00 to 359.99.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Destination Bearing", Summary = "Bearing to the destination point.",
            Remarks = "The range of values is from 0.00 to 359.99.")]
        GpsDestBear = 0x0018,

        /// <summary>
        /// Specifies the unit used to express the distance to the destination point.
        /// </summary>
        /// <remarks>one character plus the NULL terminator.
        /// <para>K, M, and N represent kilometers, miles, and knots respectively.</para></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 2, HasNullTerminator = true)]
        [ExifPropertyDescription("Destination Distance Reference", Summary = "Specifies the unit used to express the distance to the destination point.",
            Remarks = @"one character plus the NULL terminator.
K, M, and N represent kilometers, miles, and knots respectively.")]
        GpsDestDistRef = 0x0019,

        /// <summary>
        /// Distance to the destination point.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Destination Distance", Summary = "Distance to the destination point.")]
        GpsDestDist = 0x001A,

        /// <summary>
        /// Type of data in a subfile.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("New Subfile Type", Summary = "Type of data in a subfile.")]
        NewSubfileType = 0x00FE,

        /// <summary>
        /// Type of data in a subfile.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Subfile Type", Summary = "Type of data in a subfile.")]
        SubfileType = 0x00FF,

        /// <summary>
        /// Number of pixels per row.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Image Width", Summary = "Number of pixels per row.")]
        ImageWidth = 0x0100,

        /// <summary>
        /// Number of pixel rows.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Image Height", Summary = "Number of pixel rows.")]
        ImageHeight = 0x0101,

        /// <summary>
        /// Number of bits per color component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Bits Per Sample", Summary = "Number of bits per color component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        BitsPerSample = 0x0102,

        /// <summary>
        /// Compression scheme used for the image data.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Compression Scheme", Summary = "Compression scheme used for the image data.")]
        Compression = 0x0103,

        /// <summary>
        /// How pixel data will be interpreted.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Photometric Interpretation", Summary = "How pixel data will be interpreted.")]
        PhotometricInterp = 0x0106,

        /// <summary>
        /// Technique used to convert from gray pixels to black and white pixels.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("ThreshHolding Technique", Summary = "Technique used to convert from gray pixels to black and white pixels.")]
        ThreshHolding = 0x0107,

        /// <summary>
        /// Width of the dithering or halftoning matrix.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Cell Width", Summary = "Width of the dithering or halftoning matrix.")]
        CellWidth = 0x0108,

        /// <summary>
        /// Height of the dithering or halftoning matrix.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Cell Height", Summary = "Height of the dithering or halftoning matrix.")]
        CellHeight = 0x0109,

        /// <summary>
        /// Logical order of bits in a byte.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Fill Order", Summary = "Logical order of bits in a byte.")]
        FillOrder = 0x010A,

        /// <summary>
        /// Specifies the name of the document from which the image was scanned.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Document Name",
            Summary = "Specifies the name of the document from which the image was scanned.")]
        DocumentName = 0x010D,

        /// <summary>
        /// Specifies the title of the image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Image Title", Summary = "Specifies the title of the image.")]
        ImageDescription = 0x010E,

        /// <summary>
        /// Specifies the manufacturer of the equipment used to record the image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Equipment Make", Summary = "Specifies the manufacturer of the equipment used to record the image.")]
        EquipMake = 0x010F,

        /// <summary>
        /// Specifies the model name or model number of the equipment used to record the image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Equipment Model", Summary = "Specifies the model name or model number of the equipment used to record the image.")]
        EquipModel = 0x0110,

        /// <summary>
        /// For each strip, the byte offset of that strip.
        /// </summary>
        /// <remarks>Number of strips.
        /// <para>See <see cref="RowsPerStrip" /> and <see cref="StripBytesCount" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Strip Offsets", Summary = "For each strip, the byte offset of that strip.",
            Remarks = @"Number of strips.
See also ExifPropertyType.RowsPerStrip and ExifPropertyType.StripBytesCount.")]
        StripOffsets = 0x0111,

        /// <summary>
        /// Image orientation viewed in terms of rows and columns.
        /// </summary>
        /// <remarks><list type="number">
        ///     <item><term>1</term><description>The 0th row is at the top of the visual image, and the 0th column is the visual left side.</description></item>
        ///     <item><term>2</term><description>The 0th row is at the visual top of the image, and the 0th column is the visual right side.</description></item>
        ///     <item><term>3</term><description>The 0th row is at the visual bottom of the image, and the 0th column is the visual right side.</description></item>
        ///     <item><term>4</term><description>The 0th row is at the visual bottom of the image, and the 0th column is the visual left side.</description></item>
        ///     <item><term>5</term><description>The 0th row is the visual left side of the image, and the 0th column is the visual top.</description></item>
        ///     <item><term>6</term><description>The 0th row is the visual right side of the image, and the 0th column is the visual top.</description></item>
        ///     <item><term>7</term><description>The 0th row is the visual right side of the image, and the 0th column is the visual bottom.</description></item>
        ///     <item><term>8</term><description>The 0th row is the visual left side of the image, and the 0th column is the visual bottom.</description></item>
        /// </list></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Orientation", Summary = "Image orientation viewed in terms of rows and columns.",
            Remarks = @"1 - The 0th row is at the top of the visual image, and the 0th column is the visual left side.
2 - The 0th row is at the visual top of the image, and the 0th column is the visual right side.
3 - The 0th row is at the visual bottom of the image, and the 0th column is the visual right side.
4 - The 0th row is at the visual bottom of the image, and the 0th column is the visual left side.
5 - The 0th row is the visual left side of the image, and the 0th column is the visual top.
6 - The 0th row is the visual right side of the image, and the 0th column is the visual top.
7 - The 0th row is the visual right side of the image, and the 0th column is the visual bottom.
8 - The 0th row is the visual left side of the image, and the 0th column is the visual bottom.")]
        [ExifValueTranslation((ushort)1, "The 0th row is at the top of the visual image, and the 0th column is the visual left side.")]
        [ExifValueTranslation((ushort)2, "The 0th row is at the visual top of the image, and the 0th column is the visual right side.")]
        [ExifValueTranslation((ushort)3, "The 0th row is at the visual bottom of the image, and the 0th column is the visual right side.")]
        [ExifValueTranslation((ushort)4, "The 0th row is at the visual bottom of the image, and the 0th column is the visual left side.")]
        [ExifValueTranslation((ushort)5, "The 0th row is the visual left side of the image, and the 0th column is the visual top.")]
        [ExifValueTranslation((ushort)6, "The 0th row is the visual right side of the image, and the 0th column is the visual top.")]
        [ExifValueTranslation((ushort)7, "The 0th row is the visual right side of the image, and the 0th column is the visual bottom.")]
        [ExifValueTranslation((ushort)8, "The 0th row is the visual left side of the image, and the 0th column is the visual bottom.")]
        Orientation = 0x0112,

        /// <summary>
        /// Number of color components per pixel.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Samples Per Pixel", Summary = "Number of color components per pixel.")]
        SamplesPerPixel = 0x0115,

        /// <summary>
        /// Number of rows per strip.
        /// </summary>
        /// <remarks>See <see cref="StripBytesCount" /> and <see cref="StripOffsets" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Rows Per Strip", Summary = "Number of rows per strip.",
            Remarks = "See also ExifPropertyType.StripBytesCount and ExifPropertyType.StripOffsets.")]
        RowsPerStrip = 0x0116,

        /// <summary>
        /// For each strip, the total number of bytes in that strip.
        /// </summary>
        /// <remarks>Number of strips.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Strip Bytes Count", Summary = "For each strip, the total number of bytes in that strip.",
            Remarks = "Number of strips.")]
        StripBytesCount = 0x0117,

        /// <summary>
        /// For each color component, the minimum value assigned to that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Minimum Sample Value", Summary = "For each color component, the minimum value assigned to that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        MinSampleValue = 0x0118,

        /// <summary>
        /// For each color component, the maximum value assigned to that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Maximum Sample Value", Summary = "For each color component, the maximum value assigned to that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        MaxSampleValue = 0x0119,

        /// <summary>
        /// Number of pixels per unit in the image width (x) direction.
        /// </summary>
        /// <remarks>The unit is specified by <see cref="ResolutionUnit" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("X-Resolution", Summary = "Number of pixels per unit in the image width (x) direction.",
            Remarks = "The unit is specified by ExifPropertyType.ResolutionUnit.")]
        XResolution = 0x011A,

        /// <summary>
        /// Number of pixels per unit in the image height (y) direction.
        /// </summary>
        /// <remarks>The unit is specified by <see cref="ResolutionUnit" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Y-Resolution", Summary = "Number of pixels per unit in the image height (y) direction.",
            Remarks = "The unit is specified by ExifPropertyType.ResolutionUnit.")]
        YResolution = 0x011B,

        /// <summary>
        /// Whether pixel components are recorded in chunky or planar format.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Planar Config", Summary = "Whether pixel components are recorded in chunky or planar format.")]
        PlanarConfig = 0x011C,

        /// <summary>
        /// Specifies the name of the page from which the image was scanned.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII)]
        [ExifPropertyDescription("Page Name", Summary = "Specifies the name of the page from which the image was scanned.",
            Remarks = "Length of the string including the NULL terminator.")]
        PageName = 0x011D,

        /// <summary>
        /// Offset from the left side of the page to the left side of the image.
        /// </summary>
        /// <remarks>The unit of measure is specified by <see cref="ResolutionUnit" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("X-Position", Summary = "Offset from the left side of the page to the left side of the image.",
            Remarks = "The unit of measure is specified by ExifPropertyType.ResolutionUnit.")]
        XPosition = 0x011E,

        /// <summary>
        /// Offset from the top of the page to the top of the image.
        /// </summary>
        /// <remarks>The unit of measure is specified by <see cref="ResolutionUnit" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Y-Position", Summary = "Offset from the top of the page to the top of the image.",
            Remarks = "The unit of measure is specified by ExifPropertyType.ResolutionUnit.")]
        YPosition = 0x011F,

        /// <summary>
        /// For each string of contiguous unused bytes, the byte offset of that string.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Free Offset", Summary = "For each string of contiguous unused bytes, the byte offset of that string.")]
        FreeOffset = 0x0120,

        /// <summary>
        /// For each string of contiguous unused bytes, the number of bytes in that string.
        /// </summary>
        /// <remarks>Number of strings of contiguous unused bytes.</remarks>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Free Byte Counts", Summary = "For each string of contiguous unused bytes, the number of bytes in that string.",
            Remarks = "Number of strings of contiguous unused bytes.")]
        FreeByteCounts = 0x0121,

        /// <summary>
        /// Precision of the number specified by <see cref="GrayResponseCurve" />.
        /// </summary>
        /// <remarks>1 specifies tenths, 2 specifies hundredths, 3 specifies thousandths, and so on.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Gray Response Unit", Summary = "Precision of the number specified by ExifPropertyType.GrayResponseCurve.",
            Remarks = "1 specifies tenths, 2 specifies hundredths, 3 specifies thousandths, and so on.")]
        GrayResponseUnit = 0x0122,

        /// <summary>
        /// For each possible pixel value in a grayscale image, the optical density of that pixel value.
        /// </summary>
        /// <remarks>Number of possible pixel values.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Gray Response Curve", Summary = "For each possible pixel value in a grayscale image, the optical density of that pixel value.",
            Remarks = "Number of possible pixel values.")]
        GrayResponseCurve = 0x0123,

        /// <summary>
        /// Set of flags that relate to T4 encoding.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("T4 Encoding Options", Summary = "Set of flags that relate to T4 encoding.")]
        T4Option = 0x0124,

        /// <summary>
        /// Set of flags that relate to T6 encoding.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("T6 Encoding Options", Summary = "Set of flags that relate to T6 encoding.")]
        T6Option = 0x0125,

        /// <summary>
        /// Unit of measure for the horizontal resolution and the vertical resolution.
        /// </summary>
        /// <remarks>2 - inch 3 - centimeter.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Resolution Unit", Summary = "Unit of measure for the horizontal resolution and the vertical resolution.",
            Remarks = "2 - inch 3 - centimeter.")]
        ResolutionUnit = 0x0128,

        /// <summary>
        /// Page number of the page from which the image was scanned.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Page Number", Summary = "Page number of the page from which the image was scanned.")]
        PageNumber = 0x0129,

        /// <summary>
        /// Tables that specify transfer functions for the image.
        /// </summary>
        /// <remarks>Total number of 16-bit words required for the tables.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Transfer Function", Summary = "Tables that specify transfer functions for the image.",
            Remarks = "Total number of 16-bit words required for the tables.")]
        TransferFunction = 0x012D,

        /// <summary>
        /// Specifies the name and version of the software or firmware of the device used to generate the image.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Software Used", Summary = "Specifies the name and version of the software or firmware of the device used to generate the image.",
            Remarks = "Length of the string including the NULL terminator.")]
        SoftwareUsed = 0x0131,

        /// <summary>
        /// Date and time the image was created.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 20)]
        [ExifPropertyDescription("Date/Time", Summary = "Date and time the image was created.")]
        DateTime = 0x0132,

        /// <summary>
        /// Specifies the name of the person who created the image.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Artist", Summary = "Specifies the name of the person who created the image.",
            Remarks = "Length of the string including the NULL terminator.")]
        Artist = 0x013B,

        /// <summary>
        /// Specifies the computer and/or operating system used to create the image.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII)]
        [ExifPropertyDescription("Host Computer", Summary = "Specifies the computer and/or operating system used to create the image.",
            Remarks = "Length of the string including the NULL terminator.")]
        HostComputer = 0x013C,

        /// <summary>
        /// Type of prediction scheme that was applied to the image data before the encoding scheme was applied.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Predictor", Summary = "Type of prediction scheme that was applied to the image data before the encoding scheme was applied.")]
        Predictor = 0x013D,

        /// <summary>
        /// Chromaticity of the white point of the image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 2)]
        [ExifPropertyDescription("White-Point", Summary = "Chromaticity of the white point of the image.")]
        WhitePoint = 0x013E,

        /// <summary>
        /// For each of the three primary colors in the image, the chromaticity of that color.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 6)]
        [ExifPropertyDescription("Primary Chromaticities", Summary = "For each of the three primary colors in the image, the chromaticity of that color.")]
        PrimaryChromaticities = 0x013F,

        /// <summary>
        /// Color palette (lookup table) for a palette-indexed image.
        /// </summary>
        /// <remarks>Number of 16-bit words required for the palette.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Color Map", Summary = "Color palette (lookup table) for a palette-indexed image.",
            Remarks = "Number of 16-bit words required for the palette.")]
        ColorMap = 0x0140,

        /// <summary>
        /// Information used by the halftone function
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short, Count = 2)]
        [ExifPropertyDescription("Halftone Hints", Summary = "Information used by the halftone function")]
        HalftoneHints = 0x0141,

        /// <summary>
        /// Number of pixel columns in each tile.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Tile Width", Summary = "Number of pixel columns in each tile.")]
        TileWidth = 0x0142,

        /// <summary>
        /// Number of pixel rows in each tile.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Tile Length", Summary = "Number of pixel rows in each tile.")]
        TileLength = 0x0143,

        /// <summary>
        /// For each tile, the byte offset of that tile.
        /// </summary>
        /// <remarks>Number of tiles.</remarks>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Tile Offset", Summary = "For each tile, the byte offset of that tile.",
            Remarks = "Number of tiles.")]
        TileOffset = 0x0144,

        /// <summary>
        /// For each tile, the number of bytes in that tile.
        /// </summary>
        /// <remarks>Number of tiles.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Tile Byte Counts", Summary = "For each tile, the number of bytes in that tile.",
            Remarks = "Number of tiles.")]
        TileByteCounts = 0x0145,

        /// <summary>
        /// Set of inks used in a separated image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Ink Set", Summary = "Set of inks used in a separated image.")]
        InkSet = 0x014C,

        /// <summary>
        /// Sequence of concatenated, null-terminated, character strings that specify the names of the inks used in a separated image.
        /// </summary>
        /// <remarks>Total length of the sequence of strings including the NULL terminators.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true, IsRepeating = true)]
        [ExifPropertyDescription("Ink Names", Summary = "Sequence of concatenated, null-terminated, character strings that specify the names of the inks used in a separated image.",
            Remarks = "Total length of the sequence of strings including the NULL terminators.")]
        InkNames = 0x014D,

        /// <summary>
        /// Number of inks.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Number of Inks", Summary = "Number of inks.")]
        NumberOfInks = 0x014E,

        /// <summary>
        /// Color component values that correspond to a 0 percent dot and a 100 percent dot.
        /// </summary>
        /// <remarks>2 or 2×<see cref="SamplesPerPixel" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Byte, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Dot Range", Summary = "Color component values that correspond to a 0 percent dot and a 100 percent dot.",
            Remarks = "2 or 2×ExifPropertyType.SamplesPerPixel.")]
        DotRange = 0x0150,

        /// <summary>
        /// Null-terminated character string that describes the intended printing environment.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Target Printer", Summary = "Null-terminated character string that describes the intended printing environment.",
            Remarks = "Length of the string including the NULL terminator.")]
        TargetPrinter = 0x0151,

        /// <summary>
        /// Number of extra color components.
        /// </summary>
        /// <remarks>For example, one extra component might hold an alpha value.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Extra Samples", Summary = "Number of extra color components.",
            Remarks = "For example, one extra component might hold an alpha value.")]
        ExtraSamples = 0x0152,

        /// <summary>
        /// For each color component, the numerical format (unsigned, signed, floating point) of that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Sample Format", Summary = "For each color component, the numerical format (unsigned, signed, floating point) of that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        SampleFormat = 0x0153,

        /// <summary>
        /// For each color component, the minimum value of that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.BestMatching)]
        [ExifPropertyDescription("S-Min Sample Value", Summary = "For each color component, the minimum value of that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        SMinSampleValue = 0x0154,

        /// <summary>
        /// For each color component, the maximum value of that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.BestMatching)]
        [ExifPropertyDescription("S-Max Sample Value", Summary = "For each color component, the maximum value of that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        SMaxSampleValue = 0x0155,

        /// <summary>
        /// Table of values that extends the range of the transfer function.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short, Count = 6)]
        [ExifPropertyDescription("Transfer Range", Summary = "Table of values that extends the range of the transfer function.")]
        TransferRange = 0x0156,

        /// <summary>
        /// JPEG compression process.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("JPEG Process", Summary = "JPEG compression process.")]
        JPEGProc = 0x0200,

        /// <summary>
        /// Offset to the start of a JPEG bitstream.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("JPEG Inter Format", Summary = "Offset to the start of a JPEG bitstream.")]
        JPEGInterFormat = 0x0201,

        /// <summary>
        /// Length, in bytes, of the JPEG bitstream.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("", Summary = "Length, in bytes, of the JPEG bitstream.")]
        JPEGInterLength = 0x0202,

        /// <summary>
        /// Length of the restart interval.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("JPEG Restart Interval", Summary = "Length of the restart interval.")]
        JPEGRestartInterval = 0x0203,

        /// <summary>
        /// For each color component, a lossless predictor-selection value for that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("JPEG Lossless Predictors", Summary = "For each color component, a lossless predictor-selection value for that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        JPEGLosslessPredictors = 0x0205,

        /// <summary>
        /// For each color component, a point transformation value for that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("JPEG Point Transforms", Summary = "For each color component, a point transformation value for that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        JPEGPointTransforms = 0x0206,

        /// <summary>
        /// For each color component, the offset to the quantization table for that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("JPEG Q-Tables", Summary = "For each color component, the offset to the quantization table for that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        JPEGQTables = 0x0207,

        /// <summary>
        /// For each color component, the offset to the DC Huffman table (or lossless Huffman table) for that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("JPEG DC-Tables", Summary = "For each color component, the offset to the DC Huffman table (or lossless Huffman table) for that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        JPEGDCTables = 0x0208,

        /// <summary>
        /// For each color component, the offset to the AC Huffman table for that component.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel.
        /// <para>See <see cref="SamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("JPEG AC-Tables", Summary = "For each color component, the offset to the AC Huffman table for that component.",
            Remarks = @"Number of samples (components) per pixel.
See also ExifPropertyType.SamplesPerPixel.")]
        JPEGACTables = 0x0209,

        /// <summary>
        /// Coefficients for transformation from RGB to YCbCr image data.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 3)]
        [ExifPropertyDescription("YCbCr Coefficients", Summary = "Coefficients for transformation from RGB to YCbCr image data.")]
        YCbCrCoefficients = 0x0211,

        /// <summary>
        /// Sampling ratio of chrominance components in relation to the luminance component.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short, Count = 2)]
        [ExifPropertyDescription("YCbCr Subsampling", Summary = "Sampling ratio of chrominance components in relation to the luminance component.")]
        YCbCrSubsampling = 0x0212,

        /// <summary>
        /// Position of chrominance components in relation to the luminance component.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("YCbCr Positioning", Summary = "Position of chrominance components in relation to the luminance component.")]
        YCbCrPositioning = 0x0213,

        /// <summary>
        /// Reference black point value and reference white point value.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 6)]
        [ExifPropertyDescription("REF Black/White", Summary = "Reference black point value and reference white point value.")]
        REFBlackWhite = 0x0214,

        /// <summary>
        /// Gamma value attached to the image.
        /// </summary>
        /// <remarks>The gamma value is stored as a rational number (pair of long) with a numerator of 100000. For example, a gamma value of 2.2 is stored as the pair (100000, 45455).</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Gamma", Summary = "Gamma value attached to the image.",
            Remarks = "The gamma value is stored as a rational number (pair of long) with a numerator of 100000. For example, a gamma value of 2.2 is stored as the pair (100000, 45455).")]
        Gamma = 0x0301,

        /// <summary>
        /// Null-terminated character string that identifies an ICC profile.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("ICC Profile Descriptor", Summary = "Null-terminated character string that identifies an ICC profile.",
            Remarks = "Length of the string including the NULL terminator.")]
        ICCProfileDescriptor = 0x0302,

        /// <summary>
        /// Saturation intent, which is suitable for charts and graphs, preserves saturation at the expense of hue and lightness.
        /// </summary>
        /// <remarks><list type="number">
        ///     <item><term>0</term><description>perceptual</description></item>
        ///     <item><term>1</term><description>relative colorimetric</description></item>
        ///     <item><term>2</term><description>saturation</description></item>
        ///     <item><term>3</term><description>absolute colorimetric.</description></item>
        /// </list>
        /// <para>Absolute colorimetric intent is suitable for proofs (previews of images destined for a different display device) that require preservation of 
        /// absolute colorimetry.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("SRGB Rendering Intent", Summary = "Saturation intent, which is suitable for charts and graphs, preserves saturation at the expense of hue and lightness.",
            Remarks = @"0 - perceptual 1 - relative colorimetric 2 - saturation 3 - absolute colorimetric.
Absolute colorimetric intent is suitable for proofs (previews of images destined for a different display device) that require preservation of absolute colorimetry.")]
        [ExifValueTranslation(0, "perceptual.")]
        [ExifValueTranslation(1, "relative colorimetric.")]
        [ExifValueTranslation(2, "saturation.")]
        [ExifValueTranslation(3, "absolute colorimetric.")]
        SRGBRenderingIntent = 0x0303,

        /// <summary>
        /// Specifies the title of the image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Image Title", Summary = "Specifies the title of the image.")]
        ImageTitle = 0x0320,

        /// <summary>
        /// Units in which to display horizontal resolution.
        /// </summary>
        /// <remarks><list type="number">
        ///     <item><term>1</term><description>pixels per inch</description></item>
        ///     <item><term>2</term><description>pixels per centimeter.</description></item>
        /// </list></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Resolution X-Unit", Summary = "Units in which to display horizontal resolution.",
            Remarks = "1 - pixels per inch 2 - pixels per centimeter.")]
        [ExifValueTranslation((ushort)1, "pixels per inch.")]
        [ExifValueTranslation((ushort)2, "pixels per centimeter.")]
        ResolutionXUnit = 0x5001,

        /// <summary>
        /// Units in which to display vertical resolution.
        /// </summary>
        /// <remarks><list type="number">
        ///     <item><term>1</term><description>pixels per inch</description></item>
        ///     <item><term>2</term><description>pixels per centimeter.</description></item>
        /// </list></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Resolution Y-Unit", Summary = "Units in which to display vertical resolution.",
            Remarks = "1 - pixels per inch 2 - pixels per centimeter.")]
        [ExifValueTranslation((ushort)1, "pixels per inch.")]
        [ExifValueTranslation((ushort)2, "pixels per centimeter.")]
        ResolutionYUnit = 0x5002,

        /// <summary>
        /// Units in which to display the image width.
        /// </summary>
        /// <remarks>1 - inches 2 - centimeters 3 - points 4 - picas 5 - columns.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Resolution X-Length Unit", Summary = "Units in which to display the image width.",
            Remarks = "1 - inches 2 - centimeters 3 - points 4 - picas 5 - columns.")]
        ResolutionXLengthUnit = 0x5003,

        /// <summary>
        /// Units in which to display the image height.
        /// </summary>
        /// <remarks><list type="number">
        ///     <item><term>1</term><description>inches</description></item>
        ///     <item><term>2</term><description>centimeters</description></item>
        ///     <item><term>3</term><description>points</description></item>
        ///     <item><term>4</term><description>picas</description></item>
        ///     <item><term>5</term><description>columns</description></item>
        /// </list></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Resolution Y-Length Unit", Summary = "Units in which to display the image height.",
            Remarks = "1 - inches 2 - centimeters 3 - points 4 - picas 5 - columns.")]
        [ExifValueTranslation((ushort)1, "inches")]
        [ExifValueTranslation((ushort)2, "centimeters")]
        [ExifValueTranslation((ushort)3, "points")]
        [ExifValueTranslation((ushort)4, "picas")]
        [ExifValueTranslation((ushort)5, "columns")]
        ResolutionYLengthUnit = 0x5004,

        /// <summary>
        /// Sequence of one-byte Boolean values that specify printing options.
        /// </summary>
        /// <remarks>Number of flags.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0)]
        [ExifPropertyDescription("Print Flags", Summary = "Sequence of one-byte Boolean values that specify printing options.",
            Remarks = "Number of flags.")]
        PrintFlags = 0x5005,

        /// <summary>
        /// Print flags version.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Print Flags Version", Summary = "Print flags version.")]
        PrintFlagsVersion = 0x5006,

        /// <summary>
        /// Print flags center crop marks.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("Print Flags Crop", Summary = "Print flags center crop marks.")]
        PrintFlagsCrop = 0x5007,

        /// <summary>
        /// Print flags bleed width.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Print Flags Bleed Width", Summary = "Print flags bleed width.")]
        PrintFlagsBleedWidth = 0x5008,

        /// <summary>
        /// Print flags bleed width scale.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Print Flags Bleed Width Scale", Summary = "Print flags bleed width scale.")]
        PrintFlagsBleedWidthScale = 0x5009,

        /// <summary>
        /// Ink's screen frequency, in lines per inch.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Halftone LPI", Summary = "Ink's screen frequency, in lines per inch.")]
        HalftoneLPI = 0x500A,

        /// <summary>
        /// Units for the screen frequency.
        /// </summary>
        /// <remarks>1 - lines per inch 2 - lines per centimeter.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Halftone LPI Unit", Summary = "Units for the screen frequency.",
            Remarks = "1 - lines per inch 2 - lines per centimeter.")]
        HalftoneLPIUnit = 0x500B,

        /// <summary>
        /// Angle for screen.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Halftone Degree", Summary = "Angle for screen.")]
        HalftoneDegree = 0x500C,

        /// <summary>
        /// Shape of the halftone dots.
        /// </summary>
        /// <remarks>0 - round 1 - ellipse 2 - line 3 - square 4 - cross 6 - diamond.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Halftone Shape", Summary = "Shape of the halftone dots.",
            Remarks = "0 - round 1 - ellipse 2 - line 3 - square 4 - cross 6 - diamond.")]
        HalftoneShape = 0x500D,

        /// <summary>
        /// Miscellaneous halftone information.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Halftone Misc", Summary = "Miscellaneous halftone information.")]
        HalftoneMisc = 0x500E,

        /// <summary>
        /// Boolean value that specifies whether to use the printer's default screens.
        /// </summary>
        /// <remarks>1 - use printer's default screens 2 - other.</remarks>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("Halftone Screen", Summary = "Boolean value that specifies whether to use the printer's default screens.",
            Remarks = "1 - use printer's default screens 2 - other.")]
        HalftoneScreen = 0x500F,

        /// <summary>
        /// Private tag used by the Adobe Photoshop format.
        /// </summary>
        /// <remarks>Not for public use.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("JPEG Quality", Summary = "Private tag used by the Adobe Photoshop format.",
            Remarks = "Not for public use.")]
        JPEGQuality = 0x5010,

        /// <summary>
        /// Block of information about grids and guides.
        /// </summary>
        /// <remarks>Any.</remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("Grid Size", Summary = "Block of information about grids and guides.",
            Remarks = "Any.")]
        GridSize = 0x5011,

        /// <summary>
        /// Format of the thumbnail image.
        /// </summary>
        /// <remarks>0 - raw RGB 1 - JPEG.</remarks>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Format", Summary = "Format of the thumbnail image.",
            Remarks = "0 - raw RGB 1 - JPEG.")]
        ThumbnailFormat = 0x5012,

        /// <summary>
        /// Width, in pixels, of the thumbnail image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Width", Summary = "Width, in pixels, of the thumbnail image.")]
        ThumbnailWidth = 0x5013,

        /// <summary>
        /// Height, in pixels, of the thumbnail image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Height", Summary = "Height, in pixels, of the thumbnail image.")]
        ThumbnailHeight = 0x5014,

        /// <summary>
        /// bits per pixel (BPP) for the thumbnail image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Color Depth", Summary = "bits per pixel (BPP) for the thumbnail image.")]
        ThumbnailColorDepth = 0x5015,

        /// <summary>
        /// Number of color planes for the thumbnail image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Planes", Summary = "Number of color planes for the thumbnail image.")]
        ThumbnailPlanes = 0x5016,

        /// <summary>
        /// Byte offset between rows of pixel data.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Raw Bytes", Summary = "Byte offset between rows of pixel data.")]
        ThumbnailRawBytes = 0x5017,

        /// <summary>
        /// Total size, in bytes, of the thumbnail image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Size", Summary = "Total size, in bytes, of the thumbnail image.")]
        ThumbnailSize = 0x5018,

        /// <summary>
        /// Compressed size, in bytes, of the thumbnail image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Compressed Size", Summary = "Compressed size, in bytes, of the thumbnail image.")]
        ThumbnailCompressedSize = 0x5019,

        /// <summary>
        /// Table of values that specify color transfer functions.
        /// </summary>
        /// <remarks>Any.</remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("Color Transfer Function", Summary = "Table of values that specify color transfer functions.",
            Remarks = "Any.")]
        ColorTransferFunction = 0x501A,

        /// <summary>
        /// Raw thumbnail bits in JPEG or RGB format.
        /// </summary>
        /// <remarks>Count Depends on <see cref="ThumbnailFormat" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Byte, Count = 0)]
        [ExifPropertyDescription("Thumbnail Data", Summary = "Raw thumbnail bits in JPEG or RGB format.",
            Remarks = @"Variable.
Depends on ExifPropertyType.ThumbnailFormat.")]
        ThumbnailData = 0x501B,

        /// <summary>
        /// Number of pixels per row in the thumbnail image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Image Width", Summary = "Number of pixels per row in the thumbnail image.")]
        ThumbnailImageWidth = 0x5020,

        /// <summary>
        /// Number of pixel rows in the thumbnail image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Image Height", Summary = "Number of pixel rows in the thumbnail image.")]
        ThumbnailImageHeight = 0x5021,

        /// <summary>
        /// Number of bits per color component in the thumbnail image.
        /// </summary>
        /// <remarks>Number of samples (components) per pixel in the thumbnail image.
        /// <para>See <see cref="ThumbnailSamplesPerPixel" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Bits Per Sample", Summary = "Number of bits per color component in the thumbnail image.",
            Remarks = @"Number of samples (components) per pixel in the thumbnail image.
See also ExifPropertyType.ThumbnailSamplesPerPixel.")]
        ThumbnailBitsPerSample = 0x5022,

        /// <summary>
        /// Compression scheme used for thumbnail image data.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Compression", Summary = "Compression scheme used for thumbnail image data.")]
        ThumbnailCompression = 0x5023,

        /// <summary>
        /// How thumbnail pixel data will be interpreted.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Photometric Interp", Summary = "How thumbnail pixel data will be interpreted.")]
        ThumbnailPhotometricInterp = 0x5024,

        /// <summary>
        /// Specifies the title of the image.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Thumbnail Image Description", Summary = "Specifies the title of the image.",
            Remarks = "Length of the string including the NULL terminator.")]
        ThumbnailImageDescription = 0x5025,

        /// <summary>
        /// Specifies the manufacturer of the equipment used to record the thumbnail image.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Thumbnail Equipment Make", Summary = "Specifies the manufacturer of the equipment used to record the thumbnail image.",
            Remarks = "Length of the string including the NULL terminator.")]
        ThumbnailEquipMake = 0x5026,

        /// <summary>
        /// Specifies the model name or model number of the equipment used to record the thumbnail image.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Thumbnail Equipment Model", Summary = "Specifies the model name or model number of the equipment used to record the thumbnail image.",
            Remarks = "Length of the string including the NULL terminator.")]
        ThumbnailEquipModel = 0x5027,

        /// <summary>
        /// For each strip in the thumbnail image, the byte offset of that strip.
        /// </summary>
        /// <remarks>Number of strips.
        /// <para>See <see cref="ThumbnailRowsPerStrip" /> and <see cref="ThumbnailStripBytesCount" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("", Summary = "For each strip in the thumbnail image, the byte offset of that strip.",
            Remarks = @"Number of strips.
See also ExifPropertyType.ThumbnailRowsPerStrip and ExifPropertyType.ThumbnailStripBytesCount.")]
        ThumbnailStripOffsets = 0x5028,

        /// <summary>
        /// Thumbnail image orientation in terms of rows and columns.
        /// </summary>
        /// <remarks>See <see cref="Orientation" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Orientation", Summary = "Thumbnail image orientation in terms of rows and columns.",
            Remarks = "See also ExifPropertyType.Orientation.")]
        ThumbnailOrientation = 0x5029,

        /// <summary>
        /// Number of color components per pixel in the thumbnail image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Samples Per Pixel", Summary = "Number of color components per pixel in the thumbnail image.")]
        ThumbnailSamplesPerPixel = 0x502A,

        /// <summary>
        /// Number of rows per strip in the thumbnail image.
        /// </summary>
        /// <remarks>See <see cref="ThumbnailStripBytesCount" /> and <see cref="ThumbnailStripOffsets" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Rows Per Strip", Summary = "Number of rows per strip in the thumbnail image.",
            Remarks = "See also ExifPropertyType.ThumbnailStripBytesCount and ExifPropertyType.ThumbnailStripOffsets.")]
        ThumbnailRowsPerStrip = 0x502B,

        /// <summary>
        /// For each thumbnail image strip, the total number of bytes in that strip.
        /// </summary>
        /// <remarks>Number of strips in the thumbnail image.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Thumbnail Strip Bytes Count", Summary = "For each thumbnail image strip, the total number of bytes in that strip.",
            Remarks = "Number of strips in the thumbnail image.")]
        ThumbnailStripBytesCount = 0x502C,

        /// <summary>
        /// Thumbnail resolution in the width direction.
        /// </summary>
        /// <remarks>The resolution unit is given in <see cref="ThumbnailResolutionUnit" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("Thumbnail X-Resolutio", Summary = "Thumbnail resolution in the width direction.",
            Remarks = "The resolution unit is given in ExifPropertyType.ThumbnailResolutionUnit.")]
        ThumbnailResolutionX = 0x502D,

        /// <summary>
        /// Thumbnail resolution in the height direction.
        /// </summary>
        /// <remarks>The resolution unit is given in <see cref="ThumbnailResolutionUnit" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("Thumbnail Y-Resolution", Summary = "Thumbnail resolution in the height direction.",
            Remarks = "The resolution unit is given in ExifPropertyType.ThumbnailResolutionUnit.")]
        ThumbnailResolutionY = 0x502E,

        /// <summary>
        /// Whether pixel components in the thumbnail image are recorded in chunky or planar format.
        /// </summary>
        /// <remarks>See <see cref="PlanarConfig" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Planar Config", Summary = "Whether pixel components in the thumbnail image are recorded in chunky or planar format.",
            Remarks = "See also ExifPropertyType.PlanarConfig.")]
        ThumbnailPlanarConfig = 0x502F,

        /// <summary>
        /// Unit of measure for the horizontal resolution and the vertical resolution of the thumbnail image.
        /// </summary>
        /// <remarks>See <see cref="ResolutionUnit" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Resolution Unit", Summary = "Unit of measure for the horizontal resolution and the vertical resolution of the thumbnail image.",
            Remarks = "See also ExifPropertyType.ResolutionUnit.")]
        ThumbnailResolutionUnit = 0x5030,

        /// <summary>
        /// Tables that specify transfer functions for the thumbnail image.
        /// </summary>
        /// <remarks>Total number of 16-bit words required for the tables.
        /// <para>See <see cref="TransferFunction" />.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail Transfer Function", Summary = "Tables that specify transfer functions for the thumbnail image.",
            Remarks = @"Total number of 16-bit words required for the tables.
See also ExifPropertyType.TransferFunction.")]
        ThumbnailTransferFunction = 0x5031,

        /// <summary>
        /// Specifies the name and version of the software or firmware of the device used to generate the thumbnail image.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Thumbnail Software Used", Summary = "Specifies the name and version of the software or firmware of the device used to generate the thumbnail image.",
            Remarks = "Length of the string including the NULL terminator.")]
        ThumbnailSoftwareUsed = 0x5032,

        /// <summary>
        /// Date and time the thumbnail image was created.
        /// </summary>
        /// <remarks>See <see cref="DateTime" />.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 20)]
        [ExifPropertyDescription("Thumbnail Date Time", Summary = "Date and time the thumbnail image was created.",
            Remarks = "See also ExifPropertyType.DateTime.")]
        ThumbnailDateTime = 0x5033,

        /// <summary>
        /// Specifies the name of the person who created the thumbnail image.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Thumbnail Artist", Summary = "Specifies the name of the person who created the thumbnail image.",
            Remarks = "Length of the string including the NULL terminator.")]
        ThumbnailArtist = 0x5034,

        /// <summary>
        /// Chromaticity of the white point of the thumbnail image.
        /// </summary>
        /// <remarks>See <see cref="WhitePoint" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 2)]
        [ExifPropertyDescription("Thumbnail White Point", Summary = "Chromaticity of the white point of the thumbnail image.",
            Remarks = "See also ExifPropertyType.WhitePoint.")]
        ThumbnailWhitePoint = 0x5035,

        /// <summary>
        /// For each of the three primary colors in the thumbnail image, the chromaticity of that color.
        /// </summary>
        /// <remarks>See <see cref="PrimaryChromaticities" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 6)]
        [ExifPropertyDescription("Thumbnail Primary Chromaticities", Summary = "For each of the three primary colors in the thumbnail image, the chromaticity of that color.",
            Remarks = "See also ExifPropertyType.PrimaryChromaticities.")]
        ThumbnailPrimaryChromaticities = 0x5036,

        /// <summary>
        /// Coefficients for transformation from RGB to YCbCr data for the thumbnail image.
        /// </summary>
        /// <remarks>See <see cref="YCbCrCoefficients" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 3)]
        [ExifPropertyDescription("Thumbnail YCbCr Coefficients", Summary = "Coefficients for transformation from RGB to YCbCr data for the thumbnail image.",
            Remarks = "See also ExifPropertyType.YCbCrCoefficients.")]
        ThumbnailYCbCrCoefficients = 0x5037,

        /// <summary>
        /// Sampling ratio of chrominance components in relation to the luminance component for the thumbnail image.
        /// </summary>
        /// <remarks>See <see cref="YCbCrSubsampling" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, Count = 2)]
        [ExifPropertyDescription("Thumbnail YCbCr Subsampling", Summary = "Sampling ratio of chrominance components in relation to the luminance component for the thumbnail image.",
            Remarks = "See also ExifPropertyType.YCbCrSubsampling.")]
        ThumbnailYCbCrSubsampling = 0x5038,

        /// <summary>
        /// Position of chrominance components in relation to the luminance component for the thumbnail image.
        /// </summary>
        /// <remarks>See <see cref="YCbCrPositioning" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Thumbnail YCbCr Positioning", Summary = "Position of chrominance components in relation to the luminance component for the thumbnail image.",
            Remarks = "See also ExifPropertyType.YCbCrPositioning.")]
        ThumbnailYCbCrPositioning = 0x5039,

        /// <summary>
        /// Reference black point value and reference white point value for the thumbnail image.
        /// </summary>
        /// <remarks>See <see cref="REFBlackWhite" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational, Count = 6)]
        [ExifPropertyDescription("Thumbnail Ref Black/White", Summary = "Reference black point value and reference white point value for the thumbnail image.",
            Remarks = "See also ExifPropertyType.REFBlackWhite.")]
        ThumbnailRefBlackWhite = 0x503A,

        /// <summary>
        /// Contains copyright information for the thumbnail image.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Thumbnail Copyright", Summary = "Contains copyright information for the thumbnail image.",
            Remarks = "Length of the string including the NULL terminator.")]
        ThumbnailCopyRight = 0x503B,

        /// <summary>
        /// Luminance table.
        /// </summary>
        /// <remarks>The luminance table and the chrominance table are used to control JPEG quality.
        /// A valid luminance or chrominance table has 64 entries of type <see cref="short" />.
        /// If an image has either a luminance table or a chrominance table, then it must have both tables.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, Count = 64)]
        [ExifPropertyDescription("Luminance Table", Summary = "Luminance table.",
            Remarks = @"The luminance table and the chrominance table are used to control JPEG quality.
A valid luminance or chrominance table has 64 entries of type ExifPropertyType.TypeShort.
If an image has either a luminance table or a chrominance table, then it must have both tables.")]
        LuminanceTable = 0x5090,

        /// <summary>
        /// Chrominance table.
        /// </summary>
        /// <remarks>The luminance table and the chrominance table are used to control JPEG quality.
        /// A valid luminance or chrominance table has 64 entries of type <see cref="short" />.
        /// If an image has either a luminance table or a chrominance table, then it must have both tables.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, Count = 64)]
        [ExifPropertyDescription("Chrominance Table", Summary = "Chrominance table.",
            Remarks = @"The luminance table and the chrominance table are used to control JPEG quality.
A valid luminance or chrominance table has 64 entries of type ExifPropertyType.TypeShort.
If an image has either a luminance table or a chrominance table, then it must have both tables.")]
        ChrominanceTable = 0x5091,

        /// <summary>
        /// Time delay, in hundredths of a second, between two frames in an animated GIF image.
        /// </summary>
        /// <remarks>Number of frames in the image.</remarks>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Frame Delay", Summary = "Time delay, in hundredths of a second, between two frames in an animated GIF image.",
            Remarks = "Number of frames in the image.")]
        FrameDelay = 0x5100,

        /// <summary>
        /// For an animated GIF image, the number of times to display the animation.
        /// </summary>
        /// <remarks>A value of 0 specifies that the animation should be displayed infinitely.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Loop Count", Summary = "For an animated GIF image, the number of times to display the animation.",
            Remarks = "A value of 0 specifies that the animation should be displayed infinitely.")]
        LoopCount = 0x5101,

        /// <summary>
        /// Color palette for an indexed bitmap in a GIF image.
        /// </summary>
        /// <remarks>3 x number of palette entries.</remarks>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("Global Palette", Summary = "Color palette for an indexed bitmap in a GIF image.",
            Remarks = "3 x number of palette entries.")]
        GlobalPalette = 0x5102,

        /// <summary>
        /// Index of the background color in the palette of a GIF image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("Index Background", Summary = "Index of the background color in the palette of a GIF image.")]
        IndexBackground = 0x5103,

        /// <summary>
        /// Index of the transparent color in the palette of a GIF image.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("Index Transparent", Summary = "Index of the transparent color in the palette of a GIF image.")]
        IndexTransparent = 0x5104,

        /// <summary>
        /// Unit for <see cref="PixelPerUnitX" /> and <see cref="PixelPerUnitY" />.
        /// </summary>
        /// <remarks>0 - unknown.</remarks>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("Pixel Unit", Summary = "Unit for ExifPropertyType.PixelPerUnitX and ExifPropertyType.PixelPerUnitY.",
            Remarks = "0 - unknown.")]
        PixelUnit = 0x5110,

        /// <summary>
        /// Pixels per unit in the x direction.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Pixel Per Unit X", Summary = "Pixels per unit in the x direction.")]
        PixelPerUnitX = 0x5111,

        /// <summary>
        /// Pixels per unit in the y direction.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Pixel Per Unit Y", Summary = "Pixels per unit in the y direction.")]
        PixelPerUnitY = 0x5112,

        /// <summary>
        /// Palette histogram.
        /// </summary>
        /// <remarks>Length of the histogram.</remarks>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("Palette Histogram", Summary = "Palette histogram.",
            Remarks = "Length of the histogram.")]
        PaletteHistogram = 0x5113,

        /// <summary>
        /// Null-terminated character string that contains copyright information.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Copyright", Summary = "Null-terminated character string that contains copyright information.",
            Remarks = "Length of the string including the NULL terminator.")]
        Copyright = 0x8298,

        /// <summary>
        /// Exposure time, measured in seconds.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Exposure Time", Summary = "Exposure time, measured in seconds.")]
        ExifExposureTime = 0x829A,

        /// <summary>
        /// F number.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("F-Number", Summary = "F number.")]
        ExifFNumber = 0x829D,

        /// <summary>
        /// Private tag used by GDI+.
        /// </summary>
        /// <remarks>Not for public use. GDI+ uses this tag to locate Exif-specific information.</remarks>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("GDI+ Private Tag", Summary = "Private tag used by GDI+.",
            Remarks = "Not for public use. GDI+ uses this tag to locate Exif-specific information.")]
        ExifIFD = 0x8769,

        /// <summary>
        /// ICC profile embedded in the image.
        /// </summary>
        /// <remarks>Length of the profile.</remarks>
        [ExifPropertyType(ExifPropertyType.Byte)]
        [ExifPropertyDescription("ICC Profile", Summary = "ICC profile embedded in the image.",
            Remarks = "Length of the profile.")]
        ICCProfile = 0x8773,

        /// <summary>
        /// Class of the program used by the camera to set exposure when the picture is taken.
        /// </summary>
        /// <remarks>Default is 0
        /// <list type="number">
        ///     <item><term>0</term><description>not defined</description></item>
        ///     <item><term>1</term><description>manual</description></item>
        ///     <item><term>2</term><description>normal program</description></item>
        ///     <item><term>3</term><description>aperture priority</description></item>
        ///     <item><term>4</term><description>shutter priority</description></item>
        ///     <item><term>5</term><description>creative program (biased toward depth of field)</description></item>
        ///     <item><term>6</term><description>action program (biased toward fast shutter speed)</description></item>
        ///     <item><term>7</term><description>portrait mode (for close-up photos with the background out of focus)</description></item>
        ///     <item><term>8</term><description>landscape mode (for landscape photos with the background in focus)</description></item>
        /// </list>
        /// 9 to 255 - reserved.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Exposure Prog", Summary = "Class of the program used by the camera to set exposure when the picture is taken.",
            Remarks = @"Default is 0
0 - not defined
1 - manual
2 - normal program
3 - aperture priority
4 - shutter priority
5 - creative program (biased toward depth of field)
6 - action program (biased toward fast shutter speed)
7 - portrait mode (for close-up photos with the background out of focus)
8 - landscape mode (for landscape photos with the background in focus)
9 to 255 - reserved.")]
        ExifExposureProg = 0x8822,

        /// <summary>
        /// Specifies the spectral sensitivity of each channel of the camera used.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.
        /// <para>The string is compatible with the standard developed by the ASTM Technical Committee.</para></remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Spectral Sense", Summary = "Specifies the spectral sensitivity of each channel of the camera used.",
            Remarks = @"Length of the string including the NULL terminator.
The string is compatible with the standard developed by the ASTM Technical Committee.")]
        ExifSpectralSense = 0x8824,

        /// <summary>
        /// Offset to a block of GPS property items.
        /// </summary>
        /// <remarks>Property items whose tags have the prefix Gps are stored in the GPS block.
        /// The GPS property items are defined in the EXIF specification. GDI+ uses this tag to locate GPS information, but GDI+ does not expose this tag for public use.</remarks>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Gps IFD", Summary = "Offset to a block of GPS property items.",
            Remarks = @"Property items whose tags have the prefix ExifPropertyType.Gps are stored in the GPS block.
The GPS property items are defined in the EXIF specification. GDI+ uses this tag to locate GPS information, but GDI+ does not expose this tag for public use.")]
        GpsIFD = 0x8825,

        /// <summary>
        /// ISO speed and ISO latitude of the camera or input device as specified in ISO 12232.
        /// </summary>
        /// <remarks>Any.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("ISO Speed", Summary = "ISO speed and ISO latitude of the camera or input device as specified in ISO 12232.",
            Remarks = "Any.")]
        ExifISOSpeed = 0x8827,

        /// <summary>
        /// Optoelectronic conversion function (OECF) specified in ISO 14524.
        /// </summary>
        /// <remarks>Any.
        /// <para>The OECF is the relationship between the camera optical input and the image values.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("OECF", Summary = "Optoelectronic conversion function (OECF) specified in ISO 14524.",
            Remarks = @"Any.
The OECF is the relationship between the camera optical input and the image values.")]
        ExifOECF = 0x8828,

        /// <summary>
        /// Version of the EXIF standard supported.
        /// </summary>
        /// <remarks>Default	0210.
        /// <para>Nonexistence of this field is taken to mean nonconformance to the standard.
        /// Conformance to the standard is indicated by recording 0210 as a 4-byte ASCII string. Because the type is undefined, there is no NULL terminator.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Any, Count = 4)]
        [ExifPropertyDescription("EXIF Version", Summary = "Version of the EXIF standard supported.",
            Remarks = @"Default	0210.
Nonexistence of this field is taken to mean nonconformance to the standard.
Conformance to the standard is indicated by recording 0210 as a 4-byte ASCII string. Because the type is ExifPropertyType.TypeUndefined, there is no NULL terminator.")]
        ExifVer = 0x9000,

        /// <summary>
        /// Date and time when the original image data was generated.
        /// </summary>
        /// <remarks>For a DSC, the date and time when the picture was taken.
        /// The format is YYYY:MM:DD HH:MM:SS with time shown in 24-hour format and the date and time separated by one blank character (0x2000).
        /// The character string length is 20 bytes including the NULL terminator. When the field is empty, it is treated as unknown.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 20)]
        [ExifPropertyDescription("Date/Time of Original", Summary = "Date and time when the original image data was generated.",
            Remarks = @"For a DSC, the date and time when the picture was taken.
The format is YYYY:MM:DD HH:MM:SS with time shown in 24-hour format and the date and time separated by one blank character (0x2000).
The character string length is 20 bytes including the NULL terminator. When the field is empty, it is treated as unknown.")]
        ExifDTOrig = 0x9003,

        /// <summary>
        /// Date and time when the image was stored as digital data.
        /// </summary>
        /// <remarks>If, for example, an image was captured by DSC and at the same time the file was recorded, then DateTimeOriginal and DateTimeDigitized will have the same contents.
        /// The format is YYYY:MM:DD HH:MM:SS with time shown in 24-hour format and the date and time separated by one blank character (0x2000).
        /// The character string length is 20 bytes including the NULL terminator. When the field is empty, it is treated as unknown.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 20)]
        [ExifPropertyDescription("Date/Time Digitized", Summary = "Date and time when the image was stored as digital data.",
            Remarks = @"If, for example, an image was captured by DSC and at the same time the file was recorded, then DateTimeOriginal and DateTimeDigitized will have the same contents.
The format is YYYY:MM:DD HH:MM:SS with time shown in 24-hour format and the date and time separated by one blank character (0x2000).
The character string length is 20 bytes including the NULL terminator. When the field is empty, it is treated as unknown.")]
        ExifDTDigitized = 0x9004,

        /// <summary>
        /// Information specific to compressed data.
        /// </summary>
        /// <remarks>Default 4 5 6 0 (if RGB uncompressed) 1 2 3 0 (other cases) 0 - does not exist 1 - Y 2 - Cb 3 - Cr 4 - R 5 - G 6 - B Other - reserved.
        /// <para>The channels of each component are arranged in order from the first component to the fourth.
        /// For uncompressed data, the data arrangement is given in the <see cref="PhotometricInterp" /> tag.
        /// However, because <see cref="PhotometricInterp" /> can only express the order of Y, Cb, and Cr, this tag is provided for cases when compressed data uses components
        /// other than Y, Cb, and Cr and to support other sequences.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Any, Count = 4)]
        [ExifPropertyDescription("Compression Config", Summary = "Information specific to compressed data.",
            Remarks = @"Default	4 5 6 0 (if RGB uncompressed) 1 2 3 0 (other cases) 0 - does not exist 1 - Y 2 - Cb 3 - Cr 4 - R 5 - G 6 - B Other - reserved.
The channels of each component are arranged in order from the first component to the fourth.
For uncompressed data, the data arrangement is given in the ExifPropertyType.PhotometricInterp tag.
However, because ExifPropertyType.PhotometricInterp can only express the order of Y, Cb, and Cr, this tag is provided for cases when compressed data uses components
other than Y, Cb, and Cr and to support other sequences.")]
        ExifCompConfig = 0x9101,

        /// <summary>
        /// Information specific to compressed data.
        /// </summary>
        /// <remarks>The compression mode used for a compressed image is indicated in unit BPP.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Compression BPP", Summary = "Information specific to compressed data.",
            Remarks = "The compression mode used for a compressed image is indicated in unit BPP.")]
        ExifCompBPP = 0x9102,

        /// <summary>
        /// Shutter speed.
        /// </summary>
        /// <remarks>The unit is the Additive System of Photographic Exposure (APEX) value.</remarks>
        [ExifPropertyType(ExifPropertyType.SRational)]
        [ExifPropertyDescription("Shutter Speed", Summary = "Shutter speed.",
            Remarks = "The unit is the Additive System of Photographic Exposure (APEX) value.")]
        ExifShutterSpeed = 0x9201,

        /// <summary>
        /// Lens aperture.
        /// </summary>
        /// <remarks>The unit is the APEX value.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Aperture", Summary = "Lens aperture.",
            Remarks = "The unit is the APEX value.")]
        ExifAperture = 0x9202,

        /// <summary>
        /// Brightness value.
        /// </summary>
        /// <remarks>The unit is the APEX value. Ordinarily it is given in the range of -99.99 to 99.99.</remarks>
        [ExifPropertyType(ExifPropertyType.SRational)]
        [ExifPropertyDescription("Brightness", Summary = "Brightness value.",
            Remarks = "The unit is the APEX value. Ordinarily it is given in the range of -99.99 to 99.99.")]
        ExifBrightness = 0x9203,

        /// <summary>
        /// Exposure bias.
        /// </summary>
        /// <remarks>The unit is the APEX value. Ordinarily it is given in the range -99.99 to 99.99.</remarks>
        [ExifPropertyType(ExifPropertyType.SRational)]
        [ExifPropertyDescription("Exposure Bias", Summary = "Exposure bias.",
            Remarks = "The unit is the APEX value. Ordinarily it is given in the range -99.99 to 99.99.")]
        ExifExposureBias = 0x9204,

        /// <summary>
        /// Smallest F number of the lens.
        /// </summary>
        /// <remarks>The unit is the APEX value. Ordinarily it is given in the range of 00.00 to 99.99, but it is not limited to this range.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Max Aperture", Summary = "Smallest F number of the lens.",
            Remarks = "The unit is the APEX value. Ordinarily it is given in the range of 00.00 to 99.99, but it is not limited to this range.")]
        ExifMaxAperture = 0x9205,

        /// <summary>
        /// Distance to the subject, measured in meters.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Subject Distance", Summary = "Distance to the subject, measured in meters.")]
        ExifSubjectDist = 0x9206,

        /// <summary>
        /// Metering mode.
        /// </summary>
        /// <remarks>Default	0 0 - unknown 1 - Average 2 - CenterWeightedAverage 3 - Spot 4 - MultiSpot 5 - Pattern 6 - Partial 7 to 254 - reserved 255 - other.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Metering Mode", Summary = "Metering mode.",
            Remarks = "Default	0 0 - unknown 1 - Average 2 - CenterWeightedAverage 3 - Spot 4 - MultiSpot 5 - Pattern 6 - Partial 7 to 254 - reserved 255 - other.")]
        ExifMeteringMode = 0x9207,

        /// <summary>
        /// Type of light source.
        /// </summary>
        /// <remarks>Default	0 0 - unknown 1 - Daylight 2 - Flourescent 3 - Tungsten 17 - Standard Light A 18 - Standard Light B 19 - Standard Light C 20 - D55 21 - D65 22 - D75 23 to 254 - reserved 255 - other.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Light Source", Summary = "Type of light source.",
            Remarks = "Default	0 0 - unknown 1 - Daylight 2 - Flourescent 3 - Tungsten 17 - Standard Light A 18 - Standard Light B 19 - Standard Light C 20 - D55 21 - D65 22 - D75 23 to 254 - reserved 255 - other.")]
        ExifLightSource = 0x9208,

        /// <summary>
        /// Flash status.
        /// </summary>
        /// <remarks>Values for bit 0 that indicate whether the flash fired:<list type="table">
        ///     <item><term>0b</term><description>flash did not fire</description></item>
        ///     <item><term>1b</term><description>flash fired</description></item>
        /// </list>
        /// Values for bits 1 and 2 that indicate the status of returned light:<list type="table">
        ///     <item><term>00b</term><description>no strobe return detection function</description></item>
        ///     <item><term>01b</term><description>reserved</description></item>
        ///     <item><term>10b</term><description>strobe return light not detected</description></item>
        ///     <item><term>11b</term><description>strobe return light detected</description></item>
        /// </list>
        /// Resulting flash tag values:<list type="table">
        ///     <item><term>0x0000</term><description>flash did not fire</description></item>
        ///     <item><term>0x0001</term><description>flash fired</description></item>
        ///     <item><term>0x0005</term><description>strobe return light not detected.</description></item>
        /// </list>
        /// <para>This tag is recorded when an image is taken using a strobe light (flash). Bit 0 indicates the flash firing status, and bits 1 and 2 indicate the flash return status.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Flash status", Summary = "Flash status.",
            Remarks = @"Values for bit 0 that indicate whether the flash fired:
0b - flash did not fire
1b - flash fired
Values for bits 1 and 2 that indicate the status of returned light:
00b - no strobe return detection function
01b - reserved
10b - strobe return light not detected
11b - strobe return light detected
Resulting flash tag values:
0x0000 - flash did not fire
0x0001 - flash fired
0x0005 - strobe return light not detected.
This tag is recorded when an image is taken using a strobe light (flash). Bit 0 indicates the flash firing status, and bits 1 and 2 indicate the flash return status.")]
        ExifFlash = 0x9209,

        /// <summary>
        /// Actual focal length, in millimeters, of the lens.
        /// </summary>
        /// <remarks>Conversion is not made to the focal length of a 35 millimeter film camera.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("", Summary = "Actual focal length, in millimeters, of the lens.",
            Remarks = "Conversion is not made to the focal length of a 35 millimeter film camera.")]
        ExifFocalLength = 0x920A,

        /// <summary>
        /// Note tag.
        /// </summary>
        /// <remarks>Any.
        /// <para>A tag used by manufacturers of EXIF writers to record information. The contents are up to the manufacturer.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("Maker Note", Summary = "Note tag.",
            Remarks = @"Any.
A tag used by manufacturers of EXIF writers to record information. The contents are up to the manufacturer.")]
        ExifMakerNote = 0x927C,

        /// <summary>
        /// Comment tag.
        /// </summary>
        /// <remarks>Any The character code used in the <see cref="ExifUserComment" /> tag is identified based on an ID code in a fixed 8-byte area at the start of the tag data area.
        /// The unused portion of the area is padded with null characters (0). ID codes are assigned by means of registration. Because the type is not ASCII, it is not necessary to use a NULL terminator.
        /// <para>A tag used by EXIF users to write keywords or comments about the image besides those in <see cref="ImageDescription" />
        /// and without the character-code limitations of the <see cref="ImageDescription" /> tag.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("User Comment", Summary = "Comment tag.",
            Remarks = @"Any The character code used in the ExifPropertyType.ExifUserComment tag is identified based on an ID code in a fixed 8-byte area at the start of the tag data area.
The unused portion of the area is padded with null characters (0). ID codes are assigned by means of registration. Because the type is not ASCII, it is not necessary to use a NULL terminator.
A tag used by EXIF users to write keywords or comments about the image besides those in ExifPropertyType.ImageDescription and without the character-code limitations of the ExifPropertyType.ImageDescription tag.")]
        ExifUserComment = 0x9286,

        /// <summary>
        /// Specifies a fraction of a second for the <see cref="DateTime" /> tag.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Date/Time Seconds Fraction", Summary = "Specifies a fraction of a second for the ExifPropertyType.DateTime tag.",
            Remarks = "Length of the string including the NULL terminator.")]
        ExifDTSubsec = 0x9290,

        /// <summary>
        /// Specifies a fraction of a second for the <see cref="ExifDTOrig" /> tag.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Original Date/Time Seconds Fraction", Summary = "Specifies a fraction of a second for the ExifPropertyType.ExifDTOrig tag.",
            Remarks = "Length of the string including the NULL terminator.")]
        ExifDTOrigSS = 0x9291,

        /// <summary>
        /// Specifies a fraction of a second for the <see cref="ExifDTDigitized" /> tag.
        /// </summary>
        /// <remarks>Length of the string including the NULL terminator.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 0, HasNullTerminator = true)]
        [ExifPropertyDescription("Date/Time Digitized Seconds Fraction", Summary = "Specifies a fraction of a second for the ExifPropertyType.ExifDTDigitized tag.",
            Remarks = "Length of the string including the NULL terminator.")]
        ExifDTDigSS = 0x9292,

        /// <summary>
        /// FlashPix format version supported by an FPXR file.
        /// </summary>
        /// <remarks>Default	0100 0100 - FlashPix format version 1.0 Other - reserved.
        /// <para>If the FPXR function supports FlashPix format version 1.0, this is indicated similarly to <see cref="ExifVer" /> by recording 0100 as a 4-byte ASCII string.
        /// Because the type is undefined, there is no NULL terminator.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Any, Count = 4)]
        [ExifPropertyDescription("FlashPix format version", Summary = "FlashPix format version supported by an FPXR file.",
            Remarks = @"Default	0100 0100 - FlashPix format version 1.0 Other - reserved.
If the FPXR function supports FlashPix format version 1.0, this is indicated similarly to ExifPropertyType.ExifVer by recording 0100 as a 4-byte ASCII string.
Because the type is ExifPropertyType.TypeUndefined, there is no NULL terminator.")]
        ExifFPXVer = 0xA000,

        /// <summary>
        /// Color space specifier.
        /// </summary>
        /// <remarks>0x1 - sRGB 0xFFFF - uncalibrated Other - reserved.
        /// <para>Normally sRGB (=1) is used to define the color space based on the PC monitor conditions and environment.
        /// If a color space other than sRGB is used, Uncalibrated (=0xFFFF) is set. Image data recorded as Uncalibrated can be treated as sRGB when it is converted to FlashPix.</para></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Color space specifier", Summary = "Color space specifier.",
            Remarks = @"0x1 - sRGB 0xFFFF - uncalibrated Other - reserved.
Normally sRGB (=1) is used to define the color space based on the PC monitor conditions and environment.
If a color space other than sRGB is used, Uncalibrated (=0xFFFF) is set.Image data recorded as Uncalibrated can be treated as sRGB when it is converted to FlashPix.")]
        ExifColorSpace = 0xA001,

        /// <summary>
        /// Information specific to compressed data.
        /// </summary>
        /// <remarks>When a compressed file is recorded, the valid width of the meaningful image must be recorded in this tag, whether or not there is padding data or a restart marker.
        /// This tag should not exist in an uncompressed file.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Pix X-Dim", Summary = "Information specific to compressed data.",
            Remarks = @"When a compressed file is recorded, the valid width of the meaningful image must be recorded in this tag, whether or not there is padding data or a restart marker.
This tag should not exist in an uncompressed file.")]
        ExifPixXDim = 0xA002,

        /// <summary>
        /// Information specific to compressed data.
        /// </summary>
        /// <remarks>When a compressed file is recorded, the valid height of the meaningful image must be recorded in this tag whether or not there is padding data or a restart marker.
        /// This tag should not exist in an uncompressed file.
        /// Because data padding is unnecessary in the vertical direction, the number of lines recorded in this valid image height tag will be the same as that recorded in the SOF.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, IsPrimary = true)]
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Pix Y-Dim", Summary = "Information specific to compressed data.",
            Remarks = @"When a compressed file is recorded, the valid height of the meaningful image must be recorded in this tag whether or not there is padding data or a restart marker.
This tag should not exist in an uncompressed file. Because data padding is unnecessary in the vertical direction, the number of lines recorded in this valid image height tag will be the same as that recorded in the SOF.")]
        ExifPixYDim = 0xA003,

        /// <summary>
        /// The name of an audio file related to the image data.
        /// </summary>
        /// <remarks>The only relational information recorded is the EXIF audio file name and extension (an ASCII string that consists of 8 characters plus a period (.), plus 3 characters).
        /// The path is not recorded. When you use this tag, audio files must be recorded in conformance with the EXIF audio format.
        /// Writers can also store audio data within APP2 as FlashPix extension stream data.</remarks>
        [ExifPropertyType(ExifPropertyType.ASCII, Count = 13)]
        [ExifPropertyDescription("Related Wav", Summary = "The name of an audio file related to the image data.",
            Remarks = @"The only relational information recorded is the EXIF audio file name and extension (an ASCII string that consists of 8 characters plus a period (.), plus 3 characters).
The path is not recorded. When you use this tag, audio files must be recorded in conformance with the EXIF audio format. Writers can also store audio data within APP2 as FlashPix extension stream data.")]
        ExifRelatedWav = 0xA004,

        /// <summary>
        /// Offset to a block of property items that contain interoperability information.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Long)]
        [ExifPropertyDescription("Interop", Summary = "Offset to a block of property items that contain interoperability information.")]
        ExifInterop = 0xA005,

        /// <summary>
        /// Strobe energy, in Beam Candle Power Seconds (BCPS), at the time the image was captured.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Flash Energy", Summary = "Strobe energy, in Beam Candle Power Seconds (BCPS), at the time the image was captured.")]
        ExifFlashEnergy = 0xA20B,

        /// <summary>
        /// Camera or input device spatial frequency table and SFR values in the image width, image height, and diagonal direction, as specified in ISO 12233.
        /// </summary>
        /// <remarks>Any.</remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("Spatial FR ", Summary = "Camera or input device spatial frequency table and SFR values in the image width, image height, and diagonal direction, as specified in ISO 12233.",
            Remarks = "Any.")]
        ExifSpatialFR = 0xA20C,

        /// <summary>
        /// Number of pixels in the image width (x) direction per unit on the camera focal plane.
        /// </summary>
        /// <remarks>The unit is specified in <see cref="ExifFocalResUnit" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Focal X-Res", Summary = "Number of pixels in the image width (x) direction per unit on the camera focal plane.",
            Remarks = "The unit is specified in ExifPropertyType.ExifFocalResUnit.")]
        ExifFocalXRes = 0xA20E,

        /// <summary>
        /// Number of pixels in the image height (y) direction per unit on the camera focal plane.
        /// </summary>
        /// <remarks>The unit is specified in <see cref="ExifFocalResUnit" />.</remarks>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Focal Y-Res", Summary = "Number of pixels in the image height (y) direction per unit on the camera focal plane.",
            Remarks = "The unit is specified in ExifPropertyType.ExifFocalResUnit.")]
        ExifFocalYRes = 0xA20F,

        /// <summary>
        /// Unit of measure for <see cref="ExifFocalXRes" /> and <see cref="ExifFocalYRes" />.
        /// </summary>
        /// <remarks><list type="number">
        ///     <item><term>2</term><description>inch</description></item>
        ///     <item><term>3</term><description>centimeter</description></item>
        /// </list></remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Focal Res Unit", Summary = "Unit of measure for ExifPropertyType.ExifFocalXRes and ExifPropertyType.ExifFocalYRes.",
            Remarks = @"2 - inch
3 - centimeter.")]
        ExifFocalResUnit = 0xA210,

        /// <summary>
        /// Location of the main subject in the scene.
        /// </summary>
        /// <remarks>The value of this tag represents the pixel at the center of the main subject relative to the left edge.
        /// The first value indicates the column number, and the second value indicates the row number.</remarks>
        [ExifPropertyType(ExifPropertyType.Short, Count = 2)]
        [ExifPropertyDescription("Subject Location", Summary = "Location of the main subject in the scene.",
            Remarks = "The value of this tag represents the pixel at the center of the main subject relative to the left edge. The first value indicates the column number, and the second value indicates the row number.")]
        ExifSubjectLoc = 0xA214,

        /// <summary>
        /// Exposure index selected on the camera or input device at the time the image was captured.
        /// </summary>
        [ExifPropertyType(ExifPropertyType.Rational)]
        [ExifPropertyDescription("Exposure Index", Summary = "Exposure index selected on the camera or input device at the time the image was captured.")]
        ExifExposureIndex = 0xA215,

        /// <summary>
        /// Image sensor type on the camera or input device.
        /// </summary>
        /// <remarks><list type="number">
        ///     <item><term>1</term><description>not defined</description></item>
        ///     <item><term>2</term><description>one-chip color area sensor</description></item>
        ///     <item><term>3</term><description>two-chip color area sensor</description></item>
        ///     <item><term>4</term><description>three-chip color area sensor</description></item>
        ///     <item><term>5</term><description>color sequential area sensor</description></item>
        ///     <item><term>7</term><description>trilinear sensor</description></item>
        ///     <item><term>8</term><description>color sequential linear sensor</description></item>
        /// </list>
        /// Other - reserved.</remarks>
        [ExifPropertyType(ExifPropertyType.Short)]
        [ExifPropertyDescription("Sensing Method", Summary = "Image sensor type on the camera or input device.",
            Remarks = @"1 - not defined
2 - one-chip color area sensor
3 - two-chip color area sensor
4 - three-chip color area sensor
5 - color sequential area sensor
7 - trilinear sensor
8 - color sequential linear sensor Other - reserved.")]
        ExifSensingMethod = 0xA217,

        /// <summary>
        /// The image source.
        /// </summary>
        /// <remarks>If a DSC recorded the image, the value of this tag is 3.</remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("File Source", Summary = "The image source.",
            Remarks = "If a DSC recorded the image, the value of this tag is 3.")]
        ExifFileSource = 0xA300,

        /// <summary>
        /// The type of scene.
        /// </summary>
        /// <remarks>If a DSC recorded the image, the value of this tag must be set to 1, indicating that the image was directly photographed.</remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("Scene Type", Summary = "The type of scene.",
            Remarks = "If a DSC recorded the image, the value of this tag must be set to 1, indicating that the image was directly photographed.")]
        ExifSceneType = 0xA301,

        /// <summary>
        /// The color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used.
        /// </summary>
        /// <remarks>It does not apply to all sensing methods.</remarks>
        [ExifPropertyType(ExifPropertyType.Any)]
        [ExifPropertyDescription("CFA Pattern", Summary = "The color filter array (CFA) geometric pattern of the image sensor when a one-chip color area sensor is used.",
            Remarks = "It does not apply to all sensing methods.")]
        ExifCfaPattern = 0xA302,
    }
}
