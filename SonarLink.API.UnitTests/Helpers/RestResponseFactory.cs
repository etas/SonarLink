// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using RestSharp;
using RestSharp.Serializers;
using System.Net;
using System.Linq;

namespace SonarLink.API.UnitTests
{

    /// <summary>
    /// Utilities which generate sample responses
    /// as exhibited by the SonarQube WEB API
    /// </summary>
    public static class RestResponseFactory
    {
        /// <summary>
        /// Creates a typical HTTP response object
        /// </summary>
        /// <param name="content">HTTP (body) content</param>
        /// <param name="code">HTTP status code</param>
        /// <returns>An HTTP response encapsulating the specified content</returns>
        public static IRestResponse Create(string content, HttpStatusCode code = HttpStatusCode.OK)
        {
            return new RestResponse
            {
                Content = content,
                StatusCode = code,
                ResponseStatus = ResponseStatus.Completed
            };
        }
    
        /// <summary>
        /// Creates a typical valid response for a request to "api/components/search"
        /// </summary>
        /// <returns>Response for "api/components/search"</returns>
        public static IRestResponse CreateGetProjects_ValidResponse()
        {
            var jsonObj = new
            {
                paging = new
                {
                    pageIndex = 1,
                    pageSize = 500,
                    total = 3
                },
    
                components = new[] {
    
                    new {
                        organization = "default-organization",
                        id = "006ceca4-947a-4b55-b40d-32bffc64a567",
                        key =  "ETAS:INCA:HWA:A1b:LIN",
                        name = "ETAS-INCA-HWA-A1b-LIN",
                        qualifier = "TRK",
                        project = "ETAS:INCA:HWA:A1b:LIN"
                    },
    
                    new {
                        organization = "default-organization",
                        id = "006ceca4-947a-4b55-b40d-32bffc64a567",
                        key=  "ETAS:INCA:HWA:A1b:UDS",
                        name = "ETAS-INCA-HWA-A1b-UDS",
                        qualifier = "TRK",
                        project = "ETAS:INCA:HWA:A1b:UDS"
                    },
    
                    new {
                        organization = "default-organization",
                        id = "006ceca4-947a-4b55-b40d-32bffc64a567",
                        key=  "ETAS:INCA:HWA:A1b:MT1To2",
                        name = "ETAS-INCA-HWA-A1b-MT1To2",
                        qualifier = "TRK",
                        project = "ETAS:INCA:HWA:A1b:MT1To2"
                    },
                }
            };
    
            return Create(new JsonSerializer().Serialize(jsonObj));
        }
    
        /// <summary>
        /// Creates a typical valid response for a request to "api/issues/search"
        /// </summary>
        /// <returns>Response for "api/issues/search"</returns>
        public static IRestResponse CreateGetIssues_ValidResponse(string type = null)
        {
            var issues = new[] {
    
                new {
                    key = "AWXee8AZMEQ_S4Ze-xSr",
                    rule = "other:QACPP.3030",
                    severity = "INFO",
                    component = "ETAS:INCA:HWA:A1b:LIN:HardwareAccess/HardwareAccess/asap1b/Template/LIN/LinMon/source/CDriverParameterDescription.cpp",
                    project = "ETAS:INCA:HWA:A1b:LIN",
                    line = 182,
                    hash = "4e3735e0f70fa219ca85d126dc98513b",
                    textRange = new
                    {
                        startLine = 182,
                        endLine = 182,
                        startOffset = 0,
                        endOffset = 86
                    },
                    flows = new string[] { },
                    status = "OPEN",
                    message = "QACPP[1:3030]  This expression casts between two pointer types.",
                    effort = "10min",
                    debt = "10min",
                    author = "",
                    tags = new string[] { "iso-c++", "qacpp", "security-problems" },
                    creationDate = "2018-09-15T20:25:55+0200",
                    updateDate = "2018-09-15T20:25:55+0200",
                    type = "VULNERABILITY",
                    organization = "default-organization"
                },
    
                new {
                    key = "AWXee8A4MEQ_S4Ze-xSt",
                    rule = "other:QACPP.2985",
                    severity = "MINOR",
                    component = "ETAS:INCA:HWA:A1b:LIN:HardwareAccess/HardwareAccess/asap1b/Template/LIN/LinMon/source/CSignalBuffer.cpp",
                    project = "ETAS:INCA:HWA:A1b:LIN",
                    line = 59,
                    hash = "b064f43a70ba8e45e23c4f82649729d5",
                    textRange = new
                    {
                        startLine = 59,
                        endLine = 59,
                        startOffset = 0,
                        endOffset = 81
                    },
                    flows = new string[] { },
                    status = "OPEN",
                    message = "QACPP[1:2985]  This operation is redundant. The value of the result is always that of the left-hand operand.",
                    effort = "10min",
                    debt = "10min",
                    author = "",
                    tags = new string[] { "iso-c++", "qacpp", "security-problems" },
                    creationDate = "2018-09-15T20:25:55+0200",
                    updateDate = "2018-09-15T20:25:55+0200",
                    type = "BUG",
                    organization = "default-organization"
                }
            };
    
            var filtered = issues;
    
            if (!string.IsNullOrEmpty(type))
            {
                filtered = issues.Where(issue => issue.type == type).ToArray();
            }
    
            var jsonObj = new
            {
                total = 2,
                p = 1,
                ps = 100,
    
                paging = new
                {
                    pageIndex = 1,
                    pageSize = 500,
                    total = filtered.Length
                },
    
                issues = filtered
            };
    
            return Create(new JsonSerializer().Serialize(jsonObj));
        }
    
       /// <summary>
       /// Creates a response for a request to "api/issues/search" for a project which does not exist
       /// </summary>
       /// <returns>Response for "api/issues/search" for a project which does not exist</returns>
       public static IRestResponse CreateGetIssues_ProjectDoesNotExistResponse()
        {
            var jsonObj = new
            {
                total = 0,
                p = 1,
                ps = 500,
    
                paging = new
                {
                    pageIndex = 1,
                    pageSize = 500,
                    total = 0
                },
    
                issues = new string[] { },
                components = new string[] { }
            };
    
            return Create(new JsonSerializer().Serialize(jsonObj));
        }
    
        /// <summary>
        /// Creates a response for a request to "api/issues/search" for a project which does not have any issues
        /// </summary>
        /// <returns>Response for "api/issues/search" for a project which does not have any issues</returns>
        public static IRestResponse CreateGetIssues_ProjectHasNoIssuesResponse()
        {
            var jsonObj = new
            {
                total = 0,
                p = 1,
                ps = 500,
    
                paging = new
                {
                    pageIndex = 1,
                    pageSize = 500,
                    total = 0
                },
    
                issues = new string[] { },
                components = new string[] { }
            };
    
            return Create(new JsonSerializer().Serialize(jsonObj));
        }
    }
}
