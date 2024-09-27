// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0

using OpenTelemetry.Exporter.OpenTelemetryProtocol.Implementation.ExportClient;

namespace OpenTelemetry.Exporter.OpenTelemetryProtocol.Tests;

internal class TestExportClient<T>(bool throwException = false) : IExportClient<T>
{
    public bool SendExportRequestCalled { get; private set; }

    public bool ShutdownCalled { get; private set; }

    public bool ThrowException { get; set; } = throwException;

    public Task<ExportClientResponse> SendExportRequestAsync(T request, DateTime deadlineUtc, CancellationToken cancellationToken = default)
    {
        if (this.ThrowException)
        {
            throw new Exception("Exception thrown from SendExportRequest");
        }

        this.SendExportRequestCalled = true;
        return Task.FromResult<ExportClientResponse>(new TestExportClientResponse(true, deadlineUtc, null));
    }

    public bool Shutdown(int timeoutMilliseconds)
    {
        this.ShutdownCalled = true;
        return true;
    }

    private class TestExportClientResponse : ExportClientResponse
    {
        public TestExportClientResponse(bool success, DateTime deadline, Exception? exception)
            : base(success, deadline, exception)
        {
        }
    }
}
