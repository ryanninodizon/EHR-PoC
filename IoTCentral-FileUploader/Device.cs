using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;

public class Device
{
    private readonly string _scopeId;
    private readonly string _deviceId;
    private readonly string _deviceKey;
    private readonly string _modelId;

    public Device( string scopeId, string deviceId, string deviceKey, string modelId)
    {
        _scopeId = scopeId;
        _deviceId = deviceId;
        _deviceKey = deviceKey;
        _modelId = modelId;
    }

    public async Task<DeviceClient> ProvisionDeviceClient()
    {
        var security = new SecurityProviderSymmetricKey(_deviceId, _deviceKey, null);
        var transport = new ProvisioningTransportHandlerMqtt();
        var provisioningClient = ProvisioningDeviceClient.Create("global.azure-devices-provisioning.net", _scopeId, security, transport);

        Console.WriteLine("Provisioning started...");
        var result = await provisioningClient.RegisterAsync();
        Console.WriteLine($"Provisioning result: {result.Status}");

        if (result.Status != ProvisioningRegistrationStatusType.Assigned)
        {
            throw new Exception("Provisioning failed");
        }

        var authMethod = new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId, _deviceKey);
        var deviceClient = DeviceClient.Create(result.AssignedHub, authMethod, TransportType.Mqtt);
        return deviceClient;
    }
}