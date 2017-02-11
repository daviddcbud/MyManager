using Microsoft.ServiceBus;
using MoneyManager.ServiceBusServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{

    public interface IServiceChannel : IMyService, IClientChannel
    {

    }
    public static class ServiceBusUtils
        {
            public static IServiceChannel CreateChannel( )
            {
                var binding = new NetTcpRelayBinding();
                 

                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReaderQuotas.MaxDepth = int.MaxValue;
                binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
                binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
                binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
                binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
                binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
                //binding.ReliableSession.InactivityTimeout =
                binding.SendTimeout = TimeSpan.FromMinutes(5);
                var key = System.Configuration.ConfigurationManager.AppSettings["key"];





                var cf = new ChannelFactory<IServiceChannel>(
        binding,
        new EndpointAddress(ServiceBusEnvironment.CreateServiceUri("sb", "davidmmanager", "relay")));

                cf.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior
                {
                    TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider("RootManageSharedAccessKey",
                key)
                });


                return cf.CreateChannel();
            }


             
        }
    }

