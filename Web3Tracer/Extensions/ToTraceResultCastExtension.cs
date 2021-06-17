﻿using System;
using System.Collections.Generic;
using System.Text;
using Web3Tracer.Models;
using Web3Tracer.Tracers.Geth.Models;
using Web3Tracer.Tracers.Parity.Models;

namespace Web3Tracer.Extensions
{
    static class ToTraceResultCastExtension
    {
        public static TraceResult ToTraceResult(this ParityTrace trace)
        {
            return new TraceResult
            {
                From = trace.Action?.From,
                Error = trace.Error,
                CallType = trace.Action?.CallType,
                Gas = trace.Action?.GasUsed,
                GasUsed = trace.Result?.GasUsed,
                Input = trace.Action?.Input,
                To = trace.Action?.To,
                Value = trace.Action?.Value,
                Output = trace.Result?.Output
            };
        }

        public static TraceResult ToTraceResult(this GethCall call)
        {
            return new TraceResult
            {
                From = call.From,
                Error = call.Error,
                CallType = call.Type,
                Gas = call.Gas,
                GasUsed = call.GasUsed,
                Input = call.Input,
                To = call.To,
                Value = call.Value,
                Output = call.Output,
                Time = call.Time
            };
        }
    }
}
