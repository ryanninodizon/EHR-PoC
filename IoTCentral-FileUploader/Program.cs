using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Client.Transport;
using System.Text;
using Newtonsoft.Json;

class Program
 {
     private static DeviceClient _deviceClient;
     private static Device _device;

     static async Task Main(string[] args)
     {
         string scopeId = "<sopeId from IoTCental>"; // "0ne00E783DC";
         string deviceId = "<deviceId from IoTCental>"; //"1zt5skhk30g";
         string deviceKey = "<deviceId from IoTCental>"; //"zxnRZwcnEhPepj973NZDaK7uUPDT2vrOHn0ATwZ3kWY=";
         string modelId = "<modelId from IoTCental>"; //"dtmi:IoTCentral:IotCentralFileUploadDevice;1";

         _device = new Device(scopeId, deviceId, deviceKey, modelId);
         _deviceClient = await _device.ProvisionDeviceClient();
         Console.WriteLine("Device provisioned successfully.");

         await _deviceClient.OpenAsync();

         await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnHandleDeviceProperties, null);
         try
         {
             _deviceClient.SetConnectionStatusChangesHandler(OnDeviceClientError);
             await _deviceClient.SetMethodHandlerAsync("uploadFile", UploadFileCommand, null);

             Console.WriteLine($"IoT Central successfully connected device.");
             
             Console.WriteLine("Enter the file path to upload:");
             string filePath = Console.ReadLine();

             var fileUploadResult = await UploadFile(filePath);
             var result = new
             {
                 CommandResponseStatusCode = fileUploadResult.StatusCode,
                 CommandResponseMessage = fileUploadResult.Message,
                 CommandResponseData = fileUploadResult.Filename
             };

             await _deviceClient.UpdateReportedPropertiesAsync(new TwinCollection { ["uploadFile"] = result });

             Console.WriteLine("File upload completed.");
         }
         catch (Exception ex)
         {
             Console.WriteLine($"IoT Central connection error: {ex.Message}");
         }
     }
     private static async Task OnHandleDeviceProperties(TwinCollection desiredProperties, object userContext)
     {
         try
         {
             var patchedProperties = new TwinCollection();

             foreach (var property in desiredProperties)
             {
                 var propertyPair = (KeyValuePair<string, object>)property;
                 if (propertyPair.Key == "$version") continue;

                 switch (propertyPair.Key)
                 {
                     case "SettingFilenameSuffix":
                         Console.WriteLine($"Updating setting: {propertyPair.Key} with value: {propertyPair.Value}");
                         patchedProperties[propertyPair.Key] = propertyPair.Value;
                         break;

                     default:
                         Console.WriteLine($"Received desired property change for unknown setting '{propertyPair.Key}'");
                         break;
                 }
             }

             if (patchedProperties.Count > 0)
             {
                 await _deviceClient.UpdateReportedPropertiesAsync(patchedProperties);
             }
         }
         catch (Exception ex)
         {
             Console.WriteLine($"Exception while handling desired properties: {ex.Message}");
         }
     }

     private static void OnDeviceClientError(ConnectionStatus status, ConnectionStatusChangeReason reason)
     {
         Console.WriteLine($"Device client connection error: {reason}");
     }

     private static async Task<MethodResponse> UploadFileCommand(MethodRequest methodRequest, object userContext)
     {
         Console.WriteLine("Received upload file command");

         string filePath;
         try
         {
             var payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(methodRequest.DataAsJson);
             filePath = payload["filePath"];
         }
         catch (Exception ex)
         {
             var errorMessage = $"Invalid payload: {ex.Message}";
             Console.WriteLine(errorMessage);
             return new MethodResponse(Encoding.UTF8.GetBytes(errorMessage), 400);
         }

         var fileUploadResult = await UploadFile(filePath);

         var result = new
         {
             CommandResponseStatusCode = fileUploadResult.StatusCode,
             CommandResponseMessage = fileUploadResult.Message,
             CommandResponseData = fileUploadResult.Filename
         };

         await _deviceClient.UpdateReportedPropertiesAsync(new TwinCollection { ["uploadFile"] = result });

         var jsonResponse = JsonConvert.SerializeObject(result);
         return new MethodResponse(Encoding.UTF8.GetBytes(jsonResponse), 200);
     }

     private static async Task<(int StatusCode, string Message, string Filename)> UploadFile(string filePath)
     {
         var result = (StatusCode: 202, Message: "", Filename: "");
         try
         {
             var fileStats = new FileInfo(filePath);
             if (!fileStats.Exists)
             {
                 throw new Exception("File does not exist.");
             }

             var blobFilename = $"{Path.GetFileNameWithoutExtension(filePath)}-{DateTime.UtcNow:yyyyMMdd-HHmmss}{Path.GetExtension(filePath)}";
             var fileStream = File.OpenRead(filePath);

             Console.WriteLine($"uploadContent - data length: {fileStats.Length}, blob filename: {blobFilename}");

             var fileUploadSasUriRequest = new FileUploadSasUriRequest
             {
                 BlobName = blobFilename
             };

             var fileUploadSasUriResponse = await _deviceClient.GetFileUploadSasUriAsync(fileUploadSasUriRequest);
             var uploadUri = fileUploadSasUriResponse.GetBlobUri();

             using (var httpClient = new HttpClient())
             {
                 var content = new StreamContent(fileStream);
                 content.Headers.Add("x-ms-blob-type", "BlockBlob");
                 var uploadResponse = await httpClient.PutAsync(uploadUri, content);

                 if (uploadResponse.IsSuccessStatusCode)
                 {
                     var notification = new FileUploadCompletionNotification
                     {
                         CorrelationId = fileUploadSasUriResponse.CorrelationId,
                         IsSuccess = true,
                         StatusCode = (int)uploadResponse.StatusCode,
                         StatusDescription = "File uploaded successfully"
                     };

                     await _deviceClient.CompleteFileUploadAsync(notification);

                     result.Message = $"deviceId: '{_device}' uploaded a file named '{blobFilename}' to the Azure storage container";
                     result.Filename = blobFilename;
                     Console.WriteLine(result.Message);
                 }
                 else
                 {
                     var errorContent = await uploadResponse.Content.ReadAsStringAsync();
                     throw new Exception($"Failed to upload file to blob storage. Status code: {uploadResponse.StatusCode}, Error: {errorContent}");
                 }
             }
         }
         catch (Exception ex)
         {
             result.StatusCode = 500;
             result.Message = $"Error during deviceClient.uploadToBlob: {ex.Message}";
             Console.WriteLine(result.Message);
         }

         return result;
     }
 }