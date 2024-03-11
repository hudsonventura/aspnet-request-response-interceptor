using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestResponseInterceptor;

public class InterceptorOptions
{
    /// <summary>
    /// If you are using docker container logs, leave it enabled. It is going to agrupate whole request and reponse line and will write to logs in the end
    /// </summary>
    public bool WriteRequestAndResponseTogetherInTheEnd { get; set; } = true;

    /// <summary>
    /// If you are using docker container logs, leave it enabled. It will be easier to search by 'traceId'
    /// </summary>
    public bool WriteTraceIDBeforEachLine { get; set; } = true;


    /// <summary>
    /// Able to log the get requests. Default is false.
    /// </summary>
    public bool LogGetRequest { get; set; } = false;

}
