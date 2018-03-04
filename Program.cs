using System;
using System.Threading;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace mqtt_ex
{
    class Program
    {
        static void Main(string[] args)
        {
            const string MQTT_BROKER_ADDRESS = "172.18.142.65";
            {
                
                MqttClient client = new MqttClient((MQTT_BROKER_ADDRESS));

                // register to message received 
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived1;

                string clientId = Guid.NewGuid().ToString();
                client.Connect(clientId);

                // subscribe to the topic "/home/temperature" with QoS 2 
                Console.WriteLine("subscribe");
                client.Subscribe(new string[] { "/home/temperature" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            }

            MqttClient publisher = new MqttClient((MQTT_BROKER_ADDRESS));
            {
                Console.WriteLine("new client");
             
                
                string clientId2 = Guid.NewGuid().ToString();
                publisher.Connect(clientId2); 
                
                Console.WriteLine("publish");
                // publish a message on "/home/temperature" topic with QoS 2 
                for(int i=45; i  <=50; i++)
                    publisher.Publish("/home/temperature", Encoding.UTF8.GetBytes(Convert.ToString(i)), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true); 
            }

            {
                Thread.Sleep(10000);
                MqttClient client = new MqttClient((MQTT_BROKER_ADDRESS));

                // register to message received 
                client.MqttMsgPublishReceived += client_MqttMsgPublishReceived2;

                string clientId = Guid.NewGuid().ToString();
                client.Connect(clientId);


                // subscribe to the topic "/home/temperature" with QoS 2                 Console.WriteLine("subscribe");
                client.Subscribe(new string[] { "/home/temperature" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                client.Subscribe(new string[] { "/home/temperature" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                client.Subscribe(new string[] { "tags/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

            }

            
            publisher.Publish("/home/temperature", Encoding.UTF8.GetBytes(Convert.ToString(50)), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
            publisher.Publish("/home/temperature", Encoding.UTF8.GetBytes(Convert.ToString(50)), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);

            {
                Console.WriteLine("Speed test, publishing 10000 values ");
                Console.Out.Flush();
                DateTime startTime = DateTime.Now;
                for (int i = 0; i < 10000; i++)
                {
                    publisher.Publish($"tags/temperature{i}", Encoding.UTF8.GetBytes(Convert.ToString(i)), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
                }
                Console.Beep();
                Console.WriteLine($"DONE publishing: Elapsed time = {DateTime.Now - startTime}");
                Console.Out.Flush();
            }

            Console.ReadLine();


        }

                
        static void client_MqttMsgPublishReceived1(object sender, MqttMsgPublishEventArgs e) 
        {

            Console.WriteLine("1received: " + Encoding.UTF8.GetString(e.Message) + ", Dupe=" + e.DupFlag);
        // handle message received 
        }

        static int count = 0;
        static void client_MqttMsgPublishReceived2(object sender, MqttMsgPublishEventArgs e)
        {
            if (++count == 10000) { 
                Console.WriteLine("2received: " + Encoding.UTF8.GetString(e.Message) + " @10000 received");
                Console.Out.Flush();
            }
            // handle message received 
        }
    }
}
