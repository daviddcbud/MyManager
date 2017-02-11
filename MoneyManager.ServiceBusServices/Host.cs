using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MoneyManager.ServiceBusServices
{
    public static class Host
    {
        public static ServiceHost OpenTCPHost()
        {


            var sh = new ServiceHost(typeof(MyService));

            var port = System.Configuration.ConfigurationManager.AppSettings["port"];
            var uri = "net.tcp://localhost:" + port;


            var key = System.Configuration.ConfigurationManager.AppSettings["key"];
            var tcpbinding = new NetTcpBinding();
            tcpbinding.MaxBufferSize = int.MaxValue;
            tcpbinding.MaxReceivedMessageSize = int.MaxValue;
            tcpbinding.MaxBufferPoolSize = int.MaxValue;

            tcpbinding.SendTimeout = TimeSpan.FromHours(5);
            tcpbinding.ReceiveTimeout = TimeSpan.FromHours(72);
            tcpbinding.ReliableSession.InactivityTimeout = TimeSpan.FromHours(72);
            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxArrayLength = int.MaxValue;
            readerQuotas.MaxBytesPerRead = int.MaxValue;
            readerQuotas.MaxDepth = int.MaxValue;
            readerQuotas.MaxNameTableCharCount = int.MaxValue;
            readerQuotas.MaxStringContentLength = int.MaxValue;

            tcpbinding.ReaderQuotas = readerQuotas;
            sh.AddServiceEndpoint(
               typeof(IMyService), tcpbinding,
               uri);



            sh.AddServiceEndpoint(
               typeof(IMyService), new NetTcpRelayBinding(),
               ServiceBusEnvironment.CreateServiceUri("sb", "davidmmanager", "relay"))
                .Behaviors.Add(new TransportClientEndpointBehavior
                {
                    TokenProvider = SharedAccessSignatureTokenProvider.CreateSharedAccessSignatureTokenProvider("RootManageSharedAccessKey", key)

                });



            foreach (ServiceDebugBehavior behavior in sh.Description.Behaviors.OfType<ServiceDebugBehavior>())
            {
                (behavior).IncludeExceptionDetailInFaults = true;
            }

            sh.Open();
            return sh;

        }
    }

}
