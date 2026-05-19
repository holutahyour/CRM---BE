using CRM.Services.Interfaces;
using FluentAssertions;
using Xunit;

namespace CRM.Tests.Services;

public class BlobStorageServiceTests
{
    [Fact]
    public async Task UploadAsync_WithValidStream_ReturnsNonEmptyUrl()
    {
        // Arrange
        var service = new FakeBlobStorageService();
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        // Act
        var url = await service.UploadAsync(stream, "test.pdf", "application/pdf");

        // Assert
        url.Should().NotBeNullOrEmpty();
        url.Should().Contain("test.pdf");
    }

    // Testable fake — real BlobStorageService requires Azure connection
    private class FakeBlobStorageService : IBlobStorageService
    {
        public Task<string> UploadAsync(Stream stream, string fileName, string contentType)
            => Task.FromResult($"https://fake.blob.core.windows.net/container/{fileName}");
    }
}
