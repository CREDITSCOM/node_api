using System;
using System.Collections.Generic;
using System.Text;

namespace NodeAPIClient.Services
{
    public class NodeInfoService
    {
        public Models.NodeInfo GetNodeInfo(string ip, ushort port, int timeout)
        {
            var client = Api.ClientFactory.CreateDiagnosticAPIClient(ip, port, timeout);
            if (client == null)
            {
                return null;
            }
            NodeApiDiag.NodeInfoRequest request = new NodeApiDiag.NodeInfoRequest()
            {
                BlackListContent = true,
                GrayListContent = true,
                Session = true,
                State = true
            };
            NodeApiDiag.NodeInfoRespone result = null;
            try
            {
                result = client.GetNodeInfo(request);
                if (result.Result.Code != 0)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return new Models.NodeInfo()
            {
                Id = result.Info.Id,
                Platform = (int)result.Info.Platform,
                Version = result.Info.Version,
                TopBlock = (UInt64) result.Info.Session.LastBlock,
                StartRound = (UInt64)result.Info.Session.StartRound,
                CurrentRound = (UInt64)result.Info.Session.CurRound,
                UptimeMs = (UInt64)result.Info.Session.UptimeMs,
                AveRoundMs = (UInt64)result.Info.Session.AveRoundMs,
                GrayList = result.Info.GrayListContent,
                BlackList = result.Info.BlackListContent
            };
        }

    }
}
