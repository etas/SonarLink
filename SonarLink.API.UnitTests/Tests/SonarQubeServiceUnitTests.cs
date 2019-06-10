// (C) Copyright 2018 ETAS GmbH (http://www.etas.com/)

using Moq;
using NUnit.Framework;
using RestSharp;
using SonarLink.API.Models;
using SonarLink.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SonarLink.API.UnitTests
{

    public class SonarQubeServiceUnitTests
    {
        ConnectionInformation Connection = null;
        
        /// <summary>
        /// Mock Rest client
        /// </summary>
        private Mock<IRestClient> MockClient = null;
    
        [SetUp]
        public void SetUp()
        {
            Connection = new ConnectionInformation("sonarqube.com:9000", "login", "password");
    
            MockClient = new Mock<IRestClient>();
            SetupResponse("api/authentication/validate", RestResponseFactory.Create("{\"valid\": true}"));
        }
    
        [TearDown]
        public void TearDown()
        {
        }
    
        void SetupResponse(string resource, IRestResponse response)
        {
            MockClient
            .Setup(x => x.ExecuteTaskAsync(It.Is<IRestRequest>(m => m.Resource == resource)))
            .Returns(Task.FromResult(response));
        }
    
        /// <summary>
        /// Assert that: true is returned if authentication is successful.
        /// </summary>
        [Test]
        public void Connect_AuthenticationSuccess()
        {
            var service = new SonarQubeService(MockClient.Object);
            Assert.That(service.ConnectAsync(Connection).Result, Is.True);
            Assert.That(service.IsConnected, Is.True);
        }
    
        /// <summary>
        /// Assert that: false is returned if authentication fails.
        /// </summary>
        [Test]
        public void Connect_AuthenticationFailure()
        {
            SetupResponse("api/authentication/validate", RestResponseFactory.Create("{\"valid\": false}"));
            var service = new SonarQubeService(MockClient.Object);
            Assert.That(service.ConnectAsync(Connection).Result, Is.False);
            Assert.That(service.IsConnected, Is.False);
        }
    
        /// <summary>
        /// Assert that: authentication is successful and given a valid response, SonarQube project details
        /// are parsed successfully.
        /// </summary>
        [Test]
        public void GetAllProjects_Success()
        {
            SetupResponse("api/components/search", RestResponseFactory.CreateGetProjects_ValidResponse());
            var service = new SonarQubeService(MockClient.Object);
    
            Assert.That(service.ConnectAsync(Connection).Result, Is.True);
    
            var expected = new List<SonarQubeProject>()
            {
                new SonarQubeProject {Key="ETAS:INCA:HWA:A1b:LIN", Name="ETAS-INCA-HWA-A1b-LIN"},
                new SonarQubeProject {Key="ETAS:INCA:HWA:A1b:UDS", Name="ETAS-INCA-HWA-A1b-UDS"},
                new SonarQubeProject {Key="ETAS:INCA:HWA:A1b:MT1To2", Name="ETAS-INCA-HWA-A1b-MT1To2"}
            };
    
            var actual = service.GetAllProjectsAsync().Result;
    
            Assert.That(actual.Count, Is.EqualTo(expected.Count));
    
            for(var i = 0; i < actual.Count; ++i)
            {
                Assert.That(actual[i].Key, Is.EqualTo(expected[i].Key));
                Assert.That(actual[i].Name, Is.EqualTo(expected[i].Name));
            }
        }
    
        /// <summary>
        /// Mocks a valid response for an issues request for a specific issue type.
        /// </summary>
        /// <param name="type">Issue type.</param>
        private void MockGetIssuesResponseForType(string type)
        {
            MockClient.
                Setup(x => x.ExecuteTaskAsync(It.Is<IRestRequest>(request => 
                    (request.Resource == "api/issues/search") && request.Parameters.Any(param => param.Name == "types" && param.Value.ToString() == type)
                ))).
            Returns(Task.FromResult(RestResponseFactory.CreateGetIssues_ValidResponse(type)));
        }
    
        /// <summary>
        /// Assert that: authentication is successful and given a valid response, SonarQube issue details
        /// are parsed successfully.
        /// </summary>
        [Test]
        public void GetProjectIssues_Success()
        {
            MockGetIssuesResponseForType("BUG");
            MockGetIssuesResponseForType("VULNERABILITY");
            MockGetIssuesResponseForType("CODE_SMELL");
    
            var service = new SonarQubeService(MockClient.Object);
    
            Assert.That(service.ConnectAsync(Connection).Result, Is.True);
    
            var expected = new List<SonarQubeIssue>()
            {
                new SonarQubeIssue {Rule = "other:QACPP.3030",
                                    Severity = "INFO",
                                    Component = "ETAS:INCA:HWA:A1b:LIN:HardwareAccess/HardwareAccess/asap1b/Template/LIN/LinMon/source/CDriverParameterDescription.cpp",
                                    Line = 182,
                                    Message = "QACPP[1:3030]  This expression casts between two pointer types.",
                                    Type = "VULNERABILITY",},
    
                new SonarQubeIssue {Rule = "other:QACPP.2985",
                                    Severity = "MINOR",
                                    Component = "ETAS:INCA:HWA:A1b:LIN:HardwareAccess/HardwareAccess/asap1b/Template/LIN/LinMon/source/CSignalBuffer.cpp",
                                    Line = 59,
                                    Message = "QACPP[1:2985]  This operation is redundant. The value of the result is always that of the left-hand operand.",
                                    Type = "BUG",}
            };
    
            var actual = service.GetProjectIssuesAsync("ETAS:INCA:HWA:A1b:LIN").Result;
    
            Assert.That(actual.Count, Is.EqualTo(expected.Count));
    
            for (var i = 0; i < actual.Count; ++i)
            {
                Assert.That(actual[i].Rule, Is.EqualTo(expected[i].Rule));
                Assert.That(actual[i].Severity, Is.EqualTo(expected[i].Severity));
                Assert.That(actual[i].Component, Is.EqualTo(expected[i].Component));
                Assert.That(actual[i].Line, Is.EqualTo(expected[i].Line));
                Assert.That(actual[i].Message, Is.EqualTo(expected[i].Message));
                Assert.That(actual[i].Type, Is.EqualTo(expected[i].Type));
            }
        }
    
        /// <summary>
        /// Assert that: authentication is successful and if project does not exist, the returned
        /// list of SonarQube issues is empty.
        /// </summary>
        [Test]
        public void GetProjectIssues_ProjectDoesNotExist()
        {
            SetupResponse("api/issues/search", RestResponseFactory.CreateGetIssues_ProjectDoesNotExistResponse());
            var service = new SonarQubeService(MockClient.Object);
    
            Assert.That(service.ConnectAsync(Connection).Result, Is.True);
    
            var issues = service.GetProjectIssuesAsync("ProjectKey").Result;
            Assert.That(issues.Count, Is.EqualTo(0));
        }
    
        /// <summary>
        /// Assert that: authentication is successful and if project has no issues, the returned
        /// list of SonarQube issues is also empty.
        /// </summary>
        [Test]
        public void GetProjectIssues_ProjectHasNoIssues()
        {
            SetupResponse("api/issues/search", RestResponseFactory.CreateGetIssues_ProjectHasNoIssuesResponse());
            var service = new SonarQubeService(MockClient.Object);
    
            Assert.That(service.ConnectAsync(Connection).Result, Is.True);
    
            var issues = service.GetProjectIssuesAsync("ProjectKey").Result;
            Assert.That(issues.Count, Is.EqualTo(0));
        }
    
        /// <summary>
        /// Assert that: an exception is thrown when attempting to make a request that requires
        /// client authentication.
        /// </summary>
        [Test]
        public void ServiceRequestThrowsIfNotConnected()
        {
            SetupResponse("api/components/search", RestResponseFactory.CreateGetProjects_ValidResponse());
            SetupResponse("api/issues/search", RestResponseFactory.CreateGetIssues_ValidResponse());
            var service = new SonarQubeService(MockClient.Object);
    
            var exception = Assert.Throws<InvalidOperationException>(async () => await service.GetAllProjectsAsync());
            Assert.That(exception.Message, Is.EqualTo("This operation expects the service to be connected."));
    
            exception = Assert.Throws<InvalidOperationException>(async () => await service.GetProjectIssuesAsync("ProjectKey"));
            Assert.That(exception.Message, Is.EqualTo("This operation expects the service to be connected."));
        }
    
        /// <summary>
        /// Assert that: If Sonar fails to enumerate all issues due to the 10K issue,
        ///              at least 10K issues should be made visible
        /// </summary>
        [Test]
        public void GetProjectIssues10K_Success()
        {
                MockClient.
                    Setup(x => x.ExecuteTaskAsync(It.Is<IRestRequest>(request => (request.Resource == "api/issues/search") && request.Parameters.Any(param => param.Name == "types" && param.Value.ToString() == "BUG")))).
                    Returns((IRestRequest request) =>
                    {
                        var p = request.Parameters.First(param => param.Name == "p").Value;
                        var ps = request.Parameters.First(param => param.Name == "ps").Value;
    
                        var issue = "{\"key\": \"AWDVIu_vf44sODN6PmH7\", \"rule\": \"cxx:UndocumentedApi\", \"severity\": \"INFO\", \"component\": " +
                                    "\"ETAS:INCA:HWA:A1b:LIN/file.cpp\", \"project\": \"ETAS:INCA:HWA:A1b:LIN\", \"line\": 1, \"hash\": \"a133b69778aeed2e9f10f652bd6cffed\", " +
                                    "\"textRange\": { \"startLine\": 1, \"endLine\": 1, \"startOffset\": 0, \"endOffset\": 1 }, " +
                                    "\"flows\": [], \"status\": \"OPEN\", \"message\": \"Undocumented API: Matches\", \"effort\": \"5min\", " +
                                    "\"debt\": \"5min\", \"author\": \"\", \"tags\": [\"convention\"], \"creationDate\": \"2018-01-08T10:37:27+0100\", \"updateDate\": \"2018-01-08T10:37:27+0100\", " +
                                    "\"type\": \"BUG\", \"organization\": \"default-organization\" }";
    
                        var response = $"{{\"total\": 1, \"p\": {p}, \"ps\": {ps}, \"paging\": {{ \"pageIndex\": {p}, \"pageSize\": {ps}, \"total\": 1 }}, \"issues\": [{issue}]}}";
    
                        return Task.FromResult(RestResponseFactory.Create(response, HttpStatusCode.OK));
                    });
    
                MockClient.
                Setup(x => x.ExecuteTaskAsync(It.Is<IRestRequest>(request => (request.Resource == "api/issues/search") && request.Parameters.Any(param => param.Name == "types" && param.Value.ToString() == "CODE_SMELL")))).
                Returns((IRestRequest request) =>
                {
                    var p = request.Parameters.First(param => param.Name == "p").Value;
                    var ps = request.Parameters.First(param => param.Name == "ps").Value;
    
                    var response = $"{{\"total\": 0, \"p\": {p}, \"ps\": {ps}, \"paging\": {{ \"pageIndex\": {p}, \"pageSize\": {ps}, \"total\": 0 }}, \"issues\": []}}";
    
                    return Task.FromResult(RestResponseFactory.Create(response, HttpStatusCode.OK));
                });
    
                // Simulate 10k issue
                MockClient.
                Setup(x => x.ExecuteTaskAsync(It.Is<IRestRequest>(request => (request.Resource == "api/issues/search") && request.Parameters.Any(param => param.Name == "types" && param.Value.ToString() == "VULNERABILITY")))).
                Returns((IRestRequest request) => {
                    var p = request.Parameters.First(param => param.Name == "p").Value;
                    var ps = request.Parameters.First(param => param.Name == "ps").Value;
    
                    var issue = "{\"key\": \"AWDVIu_vf44sODN6PmH7\", \"rule\": \"cxx:UndocumentedApi\", \"severity\": \"INFO\", \"component\": " +
                                "\"ETAS:INCA:HWA:A1b:LIN/file.cpp\", \"project\": \"ETAS:INCA:HWA:A1b:LIN\", \"line\": 1, \"hash\": \"a133b69778aeed2e9f10f652bd6cffed\", " +
                                "\"textRange\": { \"startLine\": 1, \"endLine\": 1, \"startOffset\": 0, \"endOffset\": 1 }, " +
                                "\"flows\": [], \"status\": \"OPEN\", \"message\": \"Undocumented API: Matches\", \"effort\": \"5min\", " +
                                "\"debt\": \"5min\", \"author\": \"\", \"tags\": [\"convention\"], \"creationDate\": \"2018-01-08T10:37:27+0100\", \"updateDate\": \"2018-01-08T10:37:27+0100\", " +
                                "\"type\": \"VULNERABILITY\", \"organization\": \"default-organization\" }";
    
                    var issues = string.Join(",", Enumerable.Repeat(issue, int.Parse(ps.ToString())));
    
                    var response = $"{{\"total\": 24866, \"p\": {p}, \"ps\": {ps}, \"paging\": {{ \"pageIndex\": {p}, \"pageSize\": {ps}, \"total\": 24866 }}, \"issues\": [{issues}]}}";
    
                    return Task.FromResult(RestResponseFactory.Create(response, HttpStatusCode.OK));
                });
    
            var service = new SonarQubeService(MockClient.Object);
    
            Assert.That(service.ConnectAsync(Connection).Result, Is.True);
    
            var actual = service.GetProjectIssuesAsync("ETAS:INCA:HWA:A1b:LIN").Result;
    
            // 1 - BUG, 10000 - VULNERABILITY, 0 - CODE_SMELL
            Assert.That(actual.Count, Is.EqualTo(1 + 10000));
         }
    }
}
