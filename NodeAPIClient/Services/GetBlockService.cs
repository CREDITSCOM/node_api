using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Protocol;
using Thrift.Transport;

namespace NodeAPI.Services
{
    public class GetBlockService
    {
        public string RemoteNodeIp { get; set; }

        public UInt16 RemoteNodePort { get; set; }

        public Models.Block GetBlock(UInt64 sequence)
        {
            var bytes = AcquireBlock(sequence);
            if(bytes == null)
            {
                return null;
            }
            return Models.Block.Parse(bytes);
        }

        byte[] AcquireBlock(UInt64 sequence)
        {
            var cl = getClient();
            NodeApiExec.PoolGetResult result;
            try
            {
                result = cl.PoolGet((long)sequence);
                if(result.Status.Code != 0)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return result.Pool;            
        }

        NodeApiExec.APIEXEC.Client client = null;

        NodeApiExec.APIEXEC.Client getClient()
        {
            if(client == null)
            {
                TTransport transport = new TSocket(RemoteNodeIp, RemoteNodePort, 60000);
                TProtocol protocol = new TBinaryProtocol(transport);
                client = new NodeApiExec.APIEXEC.Client(protocol);
                transport.Open();
            }
            return client;
        }


    }
}
