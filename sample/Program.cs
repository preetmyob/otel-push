// <copyright file="Program.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace sample;

public abstract class Program
{
    private static string serviceName = "MyServiceName";
    private static string serviceVersion = "1.0.0";
    
    private static Tracer? _tracer;
    private static TracerProvider? _tracerProvider;

    public static void Main()
    {
        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(serviceName)
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
            .AddConsoleExporter()
            .Build();

        _tracer = _tracerProvider!.GetTracer(serviceName, serviceVersion);
        using var span = _tracer.StartActiveSpan("Outer scope");
   
        span.AddEvent("Starting");
        span.AddEvent("Important middle stuff");
        span.AddEvent("Finishing");
        try
        {
            using var innerSpan = _tracer.StartActiveSpan(name: "Inner scope");
            innerSpan.SetAttribute("Some data", 999);
            DoStuffOverThere();
        }
        catch (Exception ex)
        {
            span.SetAttribute("foo", 1);
            span.SetAttribute("bar", "Hello, World!");
            span.SetAttribute("baz", new int[] { 1, 2, 3 });
            span.SetStatus(Status.Ok);
            span.RecordException(ex);
        }
    }

    private static void DoStuffOverThere()
    {
        Tracer.CurrentSpan.AddEvent("Doing some micro stuff over there");
        
        throw new NotImplementedException("OI!!!!");
    }
}
