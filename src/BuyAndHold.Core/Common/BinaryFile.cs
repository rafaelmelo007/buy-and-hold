namespace BuyAndHold.Core.Common;
public record BinaryFile
{
    public string? MimeType { get; set; }
    public string? FileName { get; set; }
    public byte[]? FileContent { get; set; }
}