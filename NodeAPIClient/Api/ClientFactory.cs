using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Protocol;
using Thrift.Transport;

namespace NodeAPIClient.Api
{
    public class CreateClientFailedException : InvalidOperationException
    {
        public CreateClientFailedException(string msg, Exception inner)
            : base(msg, inner)
        { }
    }

    public static class ClientFactory
    {
        public static NodeApi.API.Client CreatePublicAPIClient(string networkIp, int port, int timeout)
        {
            TTransport transport = new TSocket(networkIp, port);
            TBinaryProtocol tr = new TBinaryProtocol(transport);
            var client = new NodeApi.API.Client(tr);
            try
            {
                transport.Open();
            }
            catch(Thrift.TException x)
            {
                throw new CreateClientFailedException("Failed to create public API client", x);
            }
            return client;
        }


        public static NodeApiExec.APIEXEC.Client CreateExecutorAPIClient(string networkIp, int port, int timeout)
        {
            var socket = new TSocket(networkIp, port);
            TBinaryProtocol tr = new TBinaryProtocol(socket);
            var client = new NodeApiExec.APIEXEC.Client(tr);
            try
            {
                socket.Open();
            }
            catch (Exception x)
            {
                throw new CreateClientFailedException("Failed to create executor API client", x);
            }
            return client;
        }

        public static NodeApiDiag.API_DIAG.Client CreateDiagnosticAPIClient(string networkIp, int port, int timeout)
        {
            TTransport socket = new TSocket(networkIp, port);
            TBinaryProtocol tr = new TBinaryProtocol(socket);
            var client = new NodeApiDiag.API_DIAG.Client(tr);
            try
            {
                socket.Open();
            }
            catch (Exception x)
            {
                throw new CreateClientFailedException("Failed to create diagnostic API client", x);
            }
            return client;
        }
    }
}
