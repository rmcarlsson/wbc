using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Grpcproto;
using System.IO;
using Google.Protobuf;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;

namespace WpfApplication1
{
 
    public class GrainBrainStatus
    {
        public List<GFCalc.Domain.MashProfileStep> RemainingMashStepList;
        public int RemainingBoilTime;
        public BrewStep State;
        public int Temperature;
        public int Progress;

        public GrainBrainStatus()
        {
            RemainingMashStepList = new List<GFCalc.Domain.MashProfileStep>();
        }
    }
    public class GrainbrainNetDiscovery
    {


        private static IPAddress ipAddr;
        private static GrainBrainStatus status = new GrainBrainStatus();


        public static GrainBrainStatus GetGrainBrainStatus()
        {
            return status;
        }

        private static void updateStatus()
        {

            string addr = String.Format("{0}:50051", ipAddr);
            Channel channel = new Channel(addr, ChannelCredentials.Insecure);

            McServer.McServerClient client = new McServer.McServerClient(channel);

            Empty statusReq = new Empty();
            BrewStatusReply rep = client.GetStatus(statusReq);
            channel.ShutdownAsync().Wait();

            status.RemainingMashStepList.Clear();

            foreach (MashProfileStep statusMp in rep.RemainingMashSteps)
            {
                var mps = new GFCalc.Domain.MashProfileStep();
                mps.Temperature = statusMp.Temperature;
                mps.Time = statusMp.Time;

                status.RemainingMashStepList.Add(mps);
            }

            var ol = status.RemainingMashStepList.OrderBy(x => x.Temperature).ToList();
            status.RemainingMashStepList.Clear();

            foreach (GFCalc.Domain.MashProfileStep ms in ol)
            {
                status.RemainingMashStepList.Add(ms);
            }

            status.Temperature = (int)(Math.Round(rep.MashTemperature));
            status.RemainingBoilTime = rep.RemainingBoilTime;
            status.State = rep.CurrentBrewStep;
            status.Progress = rep.Progress;
        }

        public static bool GetGrainBrainAddress(out IPAddress aIpAddress)
        {
            aIpAddress = ipAddr;
            return true;
        }

        public static bool ScanGrainBrains()
        {
            bool ret = false;

            var Client = new UdpClient();
            var ServerEp = new IPEndPoint(IPAddress.Any, 8888);

            var raw = new byte[1024];
            NetworkDiscoveryRequest request = new NetworkDiscoveryRequest();
            request.Name = "NETWORK-DISCOVERY-MSG";

            var ms = new MemoryStream();
            request.WriteTo(ms);
            Client.EnableBroadcast = true;
            Client.Send(ms.GetBuffer(), (int)ms.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

            var rawr = new byte[15000];
            var AsyncResponseData = Client.BeginReceive(null, null);
            int timeToWait = 1000;
            AsyncResponseData.AsyncWaitHandle.WaitOne(timeToWait);
            if (AsyncResponseData.IsCompleted)
            {
                try
                {
                    byte[] receivedData = Client.EndReceive(AsyncResponseData, ref ServerEp);
                    //Receive(ref ServerEp);
                    var msr = new MemoryStream(receivedData);
                    NetworkDiscoveryReply reply = new NetworkDiscoveryReply();
                    reply.MergeDelimitedFrom(msr);
                    Console.Out.WriteLine("Grainbrain is found at {0}", ServerEp.Address);

                    ipAddr = ServerEp.Address;
                    ret = true;
                    // EndReceive worked and we have received data and remote endpoint
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Caught exception {0}", ex);
                    // EndReceive failed and we ended up here
                }
            }
            else
            {
                // The operation wasn't completed before the timeout and we're off the hook
            }


            Client.Close();

            if (ret)
                updateStatus();

            return ret;
        }

    }
}
