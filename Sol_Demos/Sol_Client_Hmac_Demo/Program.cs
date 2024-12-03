// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Hello, World!");

HttpClient client = new HttpClient();

var userObj = new UserDto();
userObj.FirstName = "Kishor";
userObj.LastName = "Naik";

// Serialize the user object to JSON
string jsonBody = JsonConvert.SerializeObject(userObj);

// Create the payload
byte[] payloadBytes = Encoding.UTF8.GetBytes(jsonBody);

// Generate Signature
GeneratedSignature signature = new GeneratedSignature();
string signatureValue = signature.Handle(payloadBytes, "MySecretKey1");

// Set up the request
var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5205/api/hmacdemo/create")
{
    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
};

// Add headers
request.Headers.Add("X-CLIENT-ID", "MyClientId");
request.Headers.Add("X-AUTH-SIGNATURE", signatureValue);

// Send the request
HttpResponseMessage response = await client.SendAsync(request);

// Handle the response
if (response.IsSuccessStatusCode)
{
    Console.WriteLine("Request sent successfully.");
}
else
{
    Console.WriteLine("Failed to send request.");
}

public class UserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class GeneratedSignature
{
    public string Handle(byte[] payload, string secret)
    {
        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
        {
            byte[] hash = hmac.ComputeHash(payload);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}