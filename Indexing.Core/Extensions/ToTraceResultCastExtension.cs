using System;
using System.Collections.Generic;
using System.Text;
using Web3Tracer.Models;
using Web3Tracer.Tracers.Geth.Models;

namespace Web3Tracer.Extensions
{
    static class ToTraceResultCastExtension
    {

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
