﻿using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

String serverLoc = "C:\\Program Files\\mosquitto\\mosquitto.exe";
if (File.Exists(serverLoc))
{
    Process.Start(serverLoc);
}

MqttClient client = new MqttClient("192.168.1.213");


client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;




string clientId = Guid.NewGuid().ToString();
client.Connect(clientId);

// subscribe to the topic "razer" with QoS 2
client.Subscribe(new string[] { "razer" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

client.ConnectionClosed += client_MqttConnectionClosed;

void client_MqttConnectionClosed(object sender, EventArgs e)
{
    while (!client.IsConnected)
    {
        client.Disconnect();
        Thread.Sleep(1000);
        client.Connect(clientId);
        client.Subscribe(new string[] { "razer" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
    }
}

static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
{

    string message = System.Text.Encoding.UTF8.GetString(e.Message);
    string[] messageSplit = message.Split(' ');


    string myip = GetLocalIPAddress();
    int id = int.Parse(messageSplit[1]);
    string theip = "0.0.0.0";

    if (id == 1) { theip = "192.168.1.85"; } //blade 14
    if (id == 2) { theip = "192.168.1.69"; } //blade 14
    if (id == 3) { theip = "192.168.1.150"; } //blade 17
    if (id == 4) { theip = "192.168.1.232"; } //blade 17
    if (id == 5) { theip = "192.168.1.198"; } //blade 15
    if (id == 6) { theip = "192.168.1.13"; } //blade 15
    if (id == 7) { theip = "192.168.1.11"; } //blade 15
    if (id == 8) { theip = "192.168.1.71"; } //blade 18
    if (id == 9) { theip = "192.168.1.64"; } //blade 16
    if (id == 10) { theip = "192.168.1.94"; } //quartz book 
    if (id == 11) { theip = "192.168.1.140"; } //mercury blade 14
    if (id == 12) { theip = "192.168.1.224"; } //blade with raptor
    if (id == 13) { theip = "192.168.1.56"; } //blade with hypersense


    if (!myip.Equals(theip) && !theip.Equals("0.0.0.0"))
    {
        return;
    }

    if (messageSplit[0].ToLower().Equals("restart"))
    {
        Process.Start("shutdown", "/r /t 0");

    }
    
    if (messageSplit[0].ToLower().Equals("shutdown")){

        Process.Start("shutdown", "/s /t 0");
    }

    if (messageSplit[0].ToLower().Equals("lock"))
    {

        Process.Start("Rundll32.exe", "user32.dll,LockWorkStation");
    }

    if (messageSplit[0].ToLower().Equals("killchrome"))
    {

        Process[] chromeInstances = Process.GetProcessesByName("chrome");


        if (chromeInstances.Length > 0) //if chrome is open
        {
            for(int i = 0; i < chromeInstances.Length; i++)
            {
                Process chromeInstance = chromeInstances[i];
                chromeInstance.Kill();
            }
        }
    }

}

static string GetLocalIPAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            return ip.ToString();
        }
    }
    throw new Exception("No network adapters with an IPv4 address in the system!");
}