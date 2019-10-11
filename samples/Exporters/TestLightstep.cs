﻿// <auto-generated/>

namespace Samples
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using OpenTelemetry.Exporter.LightStep;
    using OpenTelemetry.Trace;
    using OpenTelemetry.Trace.Export;

    internal class TestLightstep
    {
        internal static object Run(string accessToken)
        {
            var exporter = new LightStepTraceExporter(
                new LightStepTraceExporterOptions
                {
                    AccessToken = accessToken,
                    ServiceName = "tracing-to-lightstep-service",
                });

            var tracerFactory = new TracerFactory(new BatchingSpanProcessor(exporter));
            var tracer = tracerFactory.GetTracer(string.Empty);
            
            using (tracer.WithSpan(tracer.StartSpan("Main")))
            {
                tracer.CurrentSpan.SetAttribute("custom-attribute", 55);
                Console.WriteLine("About to do a busy work");
                for (int i = 0; i < 10; i++)
                {
                    DoWork(i, tracer);
                }
            }
            Thread.Sleep(10000);
            // 5. Gracefully shutdown the exporter so it'll flush queued traces to LightStep.
            exporter.ShutdownAsync(CancellationToken.None).GetAwaiter().GetResult();
            return null;
        }
        
        private static void DoWork(int i, ITracer tracer)
        {
            using (tracer.WithSpan(tracer.StartSpan("DoWork")))
            {
                // Simulate some work.
                var span = tracer.CurrentSpan;

                try
                {
                    Console.WriteLine("Doing busy work");
                    Thread.Sleep(1000);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Set status upon error
                    span.Status = Status.Internal.WithDescription(e.ToString());
                }

                // Annotate our span to capture metadata about our operation
                var attributes = new Dictionary<string, object>();
                attributes.Add("use", "demo");
                span.AddEvent("Invoking DoWork", attributes);
            }
        }
    }
}
